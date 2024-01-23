
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Device.I2c;
using System.Device.Spi;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;
using System.Xml.Serialization;

using Iot.Device.Board;
using Iot.Device.Graphics;
using Iot.Device.Graphics.SkiaSharpAdapter;
using Iot.Device.Pcx857x;
using Iot.Device.Ssd1351;
using Iot.Device.Tca954x;

using RaspberryPiDevices;

using UnitsNet;
using UnitsNet.Units;

namespace RaspberryPiDevices;

//https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/async-scenarios#recognize-cpu-bound-and-io-bound-work
public class RPiHardware : IDisposable
{
    [Flags]
    public enum Pcf8574I2CSlaveSwitch : byte
    {
        None = 0b0000,
        A0 = 0b0001,
        A1 = 0b0010,
        A2 = 0b0100,
    }

    public static class DeviceTypes
    {
        public const int None = 0b0000_0000;
        public const int WaterFlow = 0b0000_0001;
        public const int PHProbe = 0b0000_0010;
        public const int LED4DigitDisplay = 0b0000_0100;
        public const int LED8DigitDisplay = 0b0000_1000;
        public const int TemperatureHumidity = 0b0001_0000;
        public const int BarometricPressure = 0b0010_0000;
        public const int Relay12V = 0b0100_0000;
    }

    internal static Dictionary<Guid, int> DeviceRegistry;

    internal const byte LowestAddress = 0x03;
    internal const byte HighestAddress = 0x77;

    internal const char SpaceChar = ' ';
    internal const char LightShadeChar = '░';
    internal const char FullBlockChar = '█';
    internal static readonly char[] FullBlockSpacePattern1 = new char[]
    {
        FullBlockChar, SpaceChar, FullBlockChar, SpaceChar, FullBlockChar, SpaceChar,
        FullBlockChar, SpaceChar, FullBlockChar, SpaceChar, FullBlockChar, SpaceChar,
        FullBlockChar, SpaceChar, FullBlockChar, SpaceChar, FullBlockChar, SpaceChar,
        FullBlockChar, SpaceChar, FullBlockChar, SpaceChar, FullBlockChar, SpaceChar,
        FullBlockChar, SpaceChar, FullBlockChar, SpaceChar, FullBlockChar, SpaceChar,
        FullBlockChar, SpaceChar, FullBlockChar, SpaceChar, FullBlockChar
    };
    internal static readonly string FullBlockSpacePatternString1 = new string(FullBlockSpacePattern1);
    internal static readonly char[] FullBlockSpacePattern2 = new char[]
    {
        SpaceChar, FullBlockChar, SpaceChar, FullBlockChar, SpaceChar,
        FullBlockChar, SpaceChar, FullBlockChar, SpaceChar, FullBlockChar, SpaceChar,
        FullBlockChar, SpaceChar, FullBlockChar, SpaceChar, FullBlockChar, SpaceChar,
        FullBlockChar, SpaceChar, FullBlockChar, SpaceChar, FullBlockChar, SpaceChar,
        FullBlockChar, SpaceChar, FullBlockChar, SpaceChar, FullBlockChar, SpaceChar,
        FullBlockChar, SpaceChar, FullBlockChar, SpaceChar, FullBlockChar
    };
    internal static readonly string FullBlockSpacePatternString2 = new string(FullBlockSpacePattern2);

    internal static readonly char[] FullBlockLightShadePattern1 = new char[]
    {
        FullBlockChar, LightShadeChar, FullBlockChar, LightShadeChar, FullBlockChar, LightShadeChar,
        FullBlockChar, LightShadeChar, FullBlockChar, LightShadeChar, FullBlockChar, LightShadeChar,
        FullBlockChar, LightShadeChar, FullBlockChar, LightShadeChar, FullBlockChar, LightShadeChar,
        FullBlockChar, LightShadeChar, FullBlockChar, LightShadeChar, FullBlockChar, LightShadeChar,
        FullBlockChar, LightShadeChar, FullBlockChar, LightShadeChar, FullBlockChar, LightShadeChar,
        FullBlockChar, LightShadeChar, FullBlockChar, LightShadeChar, FullBlockChar
    };
    internal static readonly string FullBlockLightShadePatternString1 = new string(FullBlockLightShadePattern1);
    internal static readonly char[] FullBlockLightShadePattern2 = new char[]
    {
        LightShadeChar, FullBlockChar, LightShadeChar, FullBlockChar, LightShadeChar,
        FullBlockChar, LightShadeChar, FullBlockChar, LightShadeChar, FullBlockChar, LightShadeChar,
        FullBlockChar, LightShadeChar, FullBlockChar, LightShadeChar, FullBlockChar, LightShadeChar,
        FullBlockChar, LightShadeChar, FullBlockChar, LightShadeChar, FullBlockChar, LightShadeChar,
        FullBlockChar, LightShadeChar, FullBlockChar, LightShadeChar, FullBlockChar, LightShadeChar,
        FullBlockChar, LightShadeChar, FullBlockChar, LightShadeChar, FullBlockChar
    };
    internal static readonly string FullBlockLightShadePatternString2 = new string(FullBlockLightShadePattern2);

    public static readonly Guid Pcf8574Id = Guid.Parse("F43EE6DB-EA14-4F97-863E-000000000001");
    public static readonly Guid Tca9548AId = Guid.Parse("F43EE6DB-EA14-4F97-863E-000000000002");
    public static readonly Guid WaterFlowSensorId1 = Guid.Parse("F43EE6DB-EA14-4F97-863E-100000000001");
    public static readonly Guid PHProbeSensorId1 = Guid.Parse("F43EE6DB-EA14-4F97-863E-200000000001");
    public static readonly Guid PHProbeSensorId2 = Guid.Parse("F43EE6DB-EA14-4F97-863E-200000000002");
    public static readonly Guid TemperatureHumiditySensorId1 = Guid.Parse("F43EE6DB-EA14-4F97-863E-300000000001");
    public static readonly Guid BarometricPressureSensorId1 = Guid.Parse("F43EE6DB-EA14-4F97-863E-400000000001");
    //public static readonly Guid LED8DigitDisplayId1             = Guid.Parse("F43EE6DB-EA14-4F97-863E-500000000001");
    public static readonly Guid LEDDigitDisplayId1 = Guid.Parse("F43EE6DB-EA14-4F97-863E-500000000001");
    public static readonly Guid LEDDigitDisplayId2 = Guid.Parse("F43EE6DB-EA14-4F97-863E-500000000002");
    public static readonly Guid LEDDigitDisplayId3 = Guid.Parse("F43EE6DB-EA14-4F97-863E-500000000003");
    public static readonly Guid LEDDigitDisplayId4 = Guid.Parse("F43EE6DB-EA14-4F97-863E-500000000004");
    //public static readonly Guid Relay12VId1                     = Guid.Parse("F43EE6DB-EA14-4F97-863E-700000000001");

    public static readonly TimeSpan Timeout = new TimeSpan(0, 0, 1);

    public static readonly Range<double> PHRange = new Range<double>(0.0, 14.0);
    public static readonly Range<double> TemperatureRange = new Range<double>(-200.0, 200.0);
    public static readonly Range<double> HumidityRange = new Range<double>(0.0, 100.0);
    public static readonly Range<double> PressureRange = new Range<double>(0.0, 1000.0);
    public static readonly Range<double> FlowRateRange = new Range<double>(0.0, 10.0);

    private readonly SensorRecords _records;

    private const int I2cBusIndex = 1;

    //public const byte BarometricPressureSensor_Address = 0x27;

    public const byte Pcf8574A_BaseAddress = 0x27;
    internal static byte Pcf8574A_SlaveAddress(Pcf8574I2CSlaveSwitch slaveSwitch)
    {
        return (byte)(Pcf8574A_BaseAddress - slaveSwitch);
    }

    public const byte TemperatureHumiditySensor_Address = 0x38;

    public const byte Tca9548A_Address = 0x70;
    public const byte Ssd1306Address = 0x3C;

    public const int Ssd1306Width = 128;
    public const int Ssd1306Height = 32;

    public const int TemperatureHumiditySensor_Channel = 2;

    public const byte PHProbeSensor1_Address = 0x48;
    public const byte PHProbeSensor2_Address = 0x49;
    public const byte WaterFlowSensor_Address = 0x4A;

    public const int BarometricPressure_PinDio = 23;
    public const int BarometricPressure_PinClk = 24;

    public const int LED1_Channel = 4;
    public const int LED2_Channel = 5;
    public const int LED3_Channel = 6;
    public const int LED4_Channel = 7;

    public const int LED1_PinClk = 27;
    public const int LED1_PinDio = 22;
    public const int LED2_PinClk = 5;
    public const int LED2_PinDio = 6;
    public const int LED3_PinClk = 13;
    public const int LED3_PinDio = 26;
    public const int LED4_PinClk = 20;
    public const int LED4_PinDio = 21;

    internal readonly byte[] PCF8574A_Addresses = new byte[] { 0x27, 0x28, 0x29, 0x30, 0x31, 0x32, 0x33, 0x34 };

    public static class PCF8574A_RelaySwitch
    {
        public const byte AllOn = 0b0000_0000;
        public const byte AllOff = 0b1111_1111;

        public const byte Relay1On = 0b0111_1111;
        public const byte Relay2On = 0b1011_1111;
        public const byte Relay3On = 0b1101_1111;
        public const byte Relay4On = 0b1110_1111;
        public const byte Relay5On = 0b1111_0111;
        public const byte Relay6On = 0b1111_1011;
        public const byte Relay7On = 0b1111_1101;
        public const byte Relay8On = 0b1111_1110;

        public const byte Relay1Off = 0b1000_0000;
        public const byte Relay2Off = 0b0100_0000;
        public const byte Relay3Off = 0b0010_0000;
        public const byte Relay4Off = 0b0001_0000;
        public const byte Relay5Off = 0b0000_1000;
        public const byte Relay6Off = 0b0000_0100;
        public const byte Relay7Off = 0b0000_0010;
        public const byte Relay8Off = 0b0000_0001;
    }

    internal readonly byte[] PCF8574A_Write_Bytes = new byte[]
    {
        0b11111111,
        0b01111111,
        0b10111111,
        0b11011111,
        0b11101111,
        0b11110111,
        0b11111011,
        0b11111101,
        0b11111110
    };

    internal readonly byte[] Tca9548A_Addresses = new byte[] { 0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77 };

    static RPiHardware()
    {
        DeviceRegistry = new Dictionary<Guid, int>();

        DeviceRegistry.Add(WaterFlowSensorId1, DeviceTypes.WaterFlow);
        DeviceRegistry.Add(PHProbeSensorId1, DeviceTypes.PHProbe);
        DeviceRegistry.Add(PHProbeSensorId2, DeviceTypes.PHProbe);
        DeviceRegistry.Add(TemperatureHumiditySensorId1, DeviceTypes.TemperatureHumidity);
        DeviceRegistry.Add(BarometricPressureSensorId1, DeviceTypes.BarometricPressure);

    }

    private bool disposedValue;

    private static volatile bool keepRunning;

    //internal readonly RPiSettings _settings;
    internal readonly RaspberryPiBoard _raspberryPiBoard;
    internal readonly I2cBus _i2cBus;

    public readonly Dictionary<Guid, I2cDevice> I2cDevices;
    public readonly List<IDisposable> Disposables;

    internal readonly GpioController _gpioController;

    internal readonly I2cDevice _pCF8574A_i2cDevice;
    internal readonly Pcf8574 _pcf8574;
    internal readonly I2cDevice _tca9548A_i2cDevice;
    internal readonly Tca9548A _tca9548a;
    internal readonly Ssd1351 _ssd1351;

    internal readonly BarometricPressureSensor BarometricPressureSensor;
    internal readonly WaterFlowSensor WaterFlowSensor;
    internal readonly PHProbeSensor _pHProbeSensor1;
    internal readonly PHProbeSensor _pHProbeSensor2;
    internal readonly TemperatureHumiditySensor TemperatureHumiditySensor;

    internal readonly MultiplexerChannel TemperatureHumiditySensorChannel = MultiplexerChannel.Channel0;
    internal readonly Tca9548AChannelBus temperatureHumiditySensorChannel;

    public int GpioInterruptPinNumber
    {
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        get
        {
            return 4;
        }
    }

    private readonly OLEDDisplay _oLEDDisplay;

    private readonly int _FontSize = 12;
    private readonly string _FontFamilyName = "Cascadia Code";

    //private RPiSensors _sensors;

    //private RPiDisplayData _displayData;

    //private RPiPhSensorValues[] _phSensorValues;


    private const int Ph1PageId = 0;
    private const int Ph2PageId = 1;
    private const int WaterFlowPageId = 2;
    private const int TemperatureHumidityPageId = 3;
    private const int BarometricPressurePageId = 4;
    private const int PageMaxIds = 5;
    private byte relaySwitchValue;


    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    public RPiHardware(in Pcf8574I2CSlaveSwitch slaveSwitch = Pcf8574I2CSlaveSwitch.None)
    {
        _raspberryPiBoard = new RaspberryPiBoard()
        {

        };

        _gpioController = _raspberryPiBoard.CreateGpioController();

        _i2cBus = _raspberryPiBoard.CreateOrGetI2cBus(I2cBusIndex);

        _ssd1351 = new(SpiDevice.Create(new SpiConnectionSettings(0, 0)
        {
            Mode = SpiMode.Mode3,
            DataBitLength = 8,
            ClockFrequency = 12_000_000 /* 12MHz */
        }), 23, 24);

        _ssd1351.ResetDisplayAsync().Wait();

        InitDisplay(_ssd1351);

        _oLEDDisplay = OLEDDisplay.Create_2_42in_OLED_Ssd1351(5);
        _oLEDDisplay.PageChangedEvent += OnPageChanged;

        _oLEDDisplay.SetText(Ph1PageId, "Ph1PageId");
        _oLEDDisplay.SetText(Ph2PageId, "Ph2PageId");
        _oLEDDisplay.SetText(WaterFlowPageId, "WaterFlowPageId");
        _oLEDDisplay.SetText(TemperatureHumidityPageId, "TemperatureHumidityPageId");
        _oLEDDisplay.SetText(BarometricPressurePageId, "BarometricPressurePageId");

        Disposables = new List<IDisposable>();
        I2cDevices = new Dictionary<Guid, I2cDevice>();

        _pHProbeSensor1 = new PHProbeSensor(_i2cBus.CreateDevice(PHProbeSensor1_Address));
        //Console.WriteLine("PHProbeSensor1");
        _pHProbeSensor2 = new PHProbeSensor(_i2cBus.CreateDevice(PHProbeSensor2_Address));
        //Console.WriteLine("PHProbeSensor2");
        WaterFlowSensor = new WaterFlowSensor(_i2cBus.CreateDevice(WaterFlowSensor_Address), _gpioController);
        WaterFlowSensor.PulseEvent += WaterFlowSensor_PulseEvent;
        //Console.WriteLine("WaterFlowSensor");

        BarometricPressureSensor = new BarometricPressureSensor(_raspberryPiBoard, _gpioController, BarometricPressure_PinDio, BarometricPressure_PinClk);
        //Console.WriteLine("BarometricPressureSensor");

        byte pcf8574_Address = Pcf8574A_SlaveAddress(slaveSwitch);
        //Console.WriteLine($"PCF8574 I2C Slave Address: 0x{pcf8574_Address:X2}");
        _pCF8574A_i2cDevice = _raspberryPiBoard.CreateI2cDevice(new I2cConnectionSettings(I2cBusIndex, pcf8574_Address));
        _pcf8574 = new Pcf8574(_pCF8574A_i2cDevice, GpioInterruptPinNumber, _gpioController, false);

        relaySwitchValue = PCF8574A_RelaySwitch.AllOff;

        _tca9548A_i2cDevice = _raspberryPiBoard.CreateI2cDevice(new I2cConnectionSettings(I2cBusIndex, Tca9548A_Address));
        //I2cDevices.Add(Tca9548AId, _tca9548A_i2cDevice);
        _tca9548a = new Tca9548A(_tca9548A_i2cDevice, _i2cBus, false);
        //Console.WriteLine("Tca9548A");

        temperatureHumiditySensorChannel = (Tca9548AChannelBus)CreateI2cBusFromChannel(MultiplexerChannel.Channel0);
        //Console.WriteLine("TemperatureHumiditySensorChannel");

        TemperatureHumiditySensor = new TemperatureHumiditySensor(temperatureHumiditySensorChannel.CreateDevice(TemperatureHumiditySensor_Address));
        //Console.WriteLine("TemperatureHumiditySensor");

        _records = new SensorRecords(5);
        _records.AddDevice(PHProbeSensorId1);
        _records.AddDevice(PHProbeSensorId2);
        _records.AddDevice(WaterFlowSensorId1);
        _records.AddDevice(TemperatureHumiditySensorId1);
        _records.AddDevice(BarometricPressureSensorId1);

        Disposables.Add(_oLEDDisplay);
        Disposables.Add(_ssd1351);
        Disposables.Add(_pHProbeSensor1);
        Disposables.Add(_pHProbeSensor2);
        Disposables.Add(WaterFlowSensor);
        Disposables.Add(_pcf8574);
        Disposables.Add(_tca9548a);
        Disposables.Add(TemperatureHumiditySensor);
    }

    private void OnPageChanged(object? sender, PageChangedEventArgs e)
    {
        switch (e.Index)
        {
            case Ph1PageId:
            {
                //Task.Run(() => {
                _oLEDDisplay.DisplayRedText(_ssd1351, Ph1PageId);
                //}).Wait();
                break;
            }
            case Ph2PageId:
            {
                //Task.Run(() => {
                _oLEDDisplay.DisplayGreenText(_ssd1351, Ph2PageId);
                //}).Wait();
                break;
            }
            case WaterFlowPageId:
            {
                //Task.Run(() => {
                _oLEDDisplay.DisplayBlueText(_ssd1351, WaterFlowPageId);
                //}).Wait();
                break;
            }
            case TemperatureHumidityPageId:
            {
                //Task.Run(() => {
                _oLEDDisplay.DisplayWhiteText(_ssd1351, TemperatureHumidityPageId);
                //}).Wait();
                break;
            }
            case BarometricPressurePageId:
            {
                //Task.Run(() => {
                _oLEDDisplay.DisplayText(_ssd1351, BarometricPressurePageId, Ssd1351Color.Convert(Color.Purple));
                //}).Wait();
                break;
            }
        }

        _oLEDDisplay.Wait();
    }

    #region Dctor
    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    ~RPiHardware()
    {
        Dispose(disposing: false);
    }

    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    protected virtual void Dispose(bool disposing)
    {    
        //lock (this)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    keepRunning = false;

                    SetRelaySwitch(PCF8574A_RelaySwitch.AllOff);

                    //ExportSettings(Settings);

                    _ssd1351.SetDisplayOff();
                    _ssd1351.Dispose();

                    //_ssd1306.ClearScreen();
                    //Utilities.DelayMicroseconds(1);
                    //_ssd1306.SendCommand(new SetDisplayOff());

                    _pCF8574A_i2cDevice.WriteByte(0b11111111);
                    Utilities.DelayMicroseconds(10);

                    foreach (IDisposable disposable in Disposables)
                    {
                        try
                        {
                            disposable.Dispose();
                            Utilities.DelayMicroseconds(10);
                        }
                        catch { }
                    }

                    //foreach (KeyValuePair<Guid, I2cDevice> i2cDevice in I2cDevices)
                    //{
                    //    try
                    //    {
                    //        i2cDevice.Value.Dispose();
                    //    }
                    //    catch { }
                    //}

                    //_gpioController.ClosePin(GpioInterruptPinNumber);

                    //try
                    //{
                    //    _gpioController.Dispose();
                    //}
                    //catch { }

                    try
                    {
                        _raspberryPiBoard.Dispose();
                        Utilities.DelayMicroseconds(10);
                    }
                    catch { }

                }

                // unmanaged
                disposedValue = true;
            }
        }
    }

    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public bool IsDisposed
    {
        get
        {
            return disposedValue;
        }
    }
    #endregion


    private static void InitDisplay(Ssd1351 device)
    {
        device.Unlock();  // Unlock OLED driver IC MCU interface from entering command
        device.MakeAccessible(); // Command A2,B1,B3,BB,BE,C1 accessible if in unlock state
        device.SetDisplayOff(); // Turn on sleep mode
        device.SetDisplayClockDivideRatioOscillatorFrequency(0x01, 0x0F); // 7:4 = Oscillator Frequency, 3:0 = CLK Div Ratio (A[3:0]+1 = 1..16)
        device.SetMultiplexRatio(); // Use all 128 common lines by default....
        device.SetSegmentReMapColorDepth(ColorDepth.ColourDepth65K, CommonSplit.OddEven, Seg0Common.Column127); // 0x74 Color Depth = 64K, Enable COM Split Odd Even, Scan from COM[N-1] to COM0. Where N is the Multiplex ratio., Color sequence is normal: B -> G -> R
        device.SetColumnAddress(); // Columns = 0 -> 127
        device.SetRowAddress(); // Rows = 0 -> 127
        device.SetDisplayStartLine(); // set startline to to 0
        device.SetDisplayOffset(0); // Set vertical scroll by Row to 0-127.
        device.SetGpio(GpioMode.Disabled, GpioMode.Disabled); // Set all GPIO to Input disabled
        device.SetVDDSource(); // Enable internal VDD regulator
        device.SetPreChargePeriods(2, 3); // Phase 1 period of 5 DCLKS,  Phase 2 period of 3 DCLKS
        device.SetNormalDisplay(); // Reset to Normal Display
        device.SetVcomhDeselectLevel(); // 0.82 x VCC
        device.SetContrastABC(0xC8, 0x80, 0xC8); // Contrast A = 200, B = 128, C = 200
        device.SetMasterContrast(); // No Change = 15
        device.SetVSL(); // External VSL
        device.Set3rdPreChargePeriod(0x01); // Set Second Pre-charge Period = 1 DCLKS
        device.SetDisplayOn(); //--turn on oled panel
        device.ClearScreen();

        //device.SetDisplayEnhancement(true);
    }

    //private void DrawBitmap()
    //{
    //    _ssd1306.DrawBitmap(_ssd1306BitmapImage);
    //}

    //private void DisplayFullBlocks(in int startX = 0, in int startY = 0)
    //{
    //    _ssd1306BitmapImage.Clear(Color.Black);

    //    StringBuilder sb = new StringBuilder();

    //    sb.AppendLine(FullBlockLightShadePatternString1);
    //    sb.AppendLine(FullBlockLightShadePatternString2);
    //    sb.AppendLine(FullBlockLightShadePatternString1);
    //    sb.AppendLine(FullBlockLightShadePatternString2);
    //    sb.AppendLine(FullBlockLightShadePatternString1);

    //    Ssd1306Graphics.DrawText(sb.ToString(), _ssd1306Font, _ssd1306FontSize, Color.White, new Point(startX, startY));

    //    DrawBitmap();
    //}

    //private void DisplayText(in string text)
    //{
    //    _ssd1306BitmapImage.Clear(Color.Black);

    //    Ssd1306Graphics.DrawText(text, _ssd1306Font, _ssd1306FontSize, Color.White, Point.Empty);

    //    DrawBitmap();
    //}

    //private void DisplayText(in string text, Color color, in int startX = 0, in int startY = 0)
    //{
    //    _bitmapImage.Clear(Color.Black);

    //    IGraphics graphics = _bitmapImage.GetDrawingApi();

    //    graphics.DrawText(text, _FontFamilyName, _FontSize, Ssd1351Color.Convert(color), new Point(startX, startY));

    //    //DrawBitmap();
    //}

    //private void DisplayClock(in int startX = 0, in int startY = 0)
    //{
    //    //_ssd1306BitmapImage.Clear(Color.Black);

    //    //Ssd1306Graphics.DrawText(DateTime.Now.ToString("HH:mm:ss"), _ssd1306Font, _ssd1306FontSize, Color.White, new Point(startX, startY));

    //    //DrawBitmap();
    //}

    //private static void DisplayImage(GraphicDisplay ssd1306, BitmapImage ssd1306BitmapImage)
    //{
    //    //Console.WriteLine("Display Image");
    //    ssd1306.DrawBitmap(ssd1306BitmapImage);
    //}

    //private static void DisplayClock(GraphicDisplay ssd1306, BitmapImage ssd1306BitmapImage)
    //{
    //    //Console.WriteLine("Display clock");

    //    int fontSize = 25;
    //    string font = "DejaVu Sans";

    //    ssd1306BitmapImage.Clear(Color.Black);

    //    IGraphics g = ssd1306BitmapImage.GetDrawingApi();

    //    g.DrawText(DateTime.Now.ToString("HH:mm:ss"), font, fontSize, Color.White, new Point(0, 0));

    //    ssd1306.DrawBitmap(ssd1306BitmapImage);
    //}

    //public I2cDevice CreateI2cDevice(Tca9548AChannelBus bus, in byte address)
    //{
    //    try
    //    {
    //        return bus.CreateDevice(address);
    //    }
    //    catch
    //    {
    //        throw new Exception();
    //    }
    //}

    public I2cBus CreateI2cBusFromChannel(in MultiplexerChannel channel)
    {
        //Console.WriteLine($"channel {Enum.GetName(channel)}");

        _tca9548a.SelectChannel(channel);

        if (_tca9548a.TryGetSelectedChannel(out MultiplexerChannel selectedChannel))
        {
            return _tca9548a.GetChannel(selectedChannel);
        }

        return _tca9548a.GetChannel(channel);
        //throw new Exception("CreateI2cBusFromChannel Failed.");
    }

    
    public void OpenWaterValve()
    {
        relaySwitchValue &= PCF8574A_RelaySwitch.Relay1On;
        SetRelaySwitch(relaySwitchValue);
    }

    public void CloseWaterValve()
    {
        relaySwitchValue ^= PCF8574A_RelaySwitch.Relay1Off;
        SetRelaySwitch(relaySwitchValue);
    }

    public void SetRelaySwitch(params byte[] RelayValues)
    {
        byte relayValue = (byte)(PCF8574A_RelaySwitch.AllOn ^ RelayValues.BinaryOp_OR());

        _pcf8574.WriteByte(relayValue);
    }


    private void CancellationTokenCallback()
    {
        keepRunning = false;
    }

    private async Task<SensorRecord> GetTemperatureHumiditySensorRecordAsync<TSensor>(int userId) where TSensor : SensorRecord
    {


        return await Task.FromResult(new TemperatureHumiditySensorRecord()).ConfigureAwait(false);
    }

    private void PhSensor_Event(object? sender, WaterFlowPulseEventArgs args)
    {
        WaterFlowSensorRecord record = new WaterFlowSensorRecord(args.FlowRate, args.TotalLitres);

        //Ssd1306OLEDDisplay[2].Text = $"Flow: {record.FlowRate.LitersPerMinute:N4}\nTotal: {record.TotalLitres.Liters:N6}";

        _records.Add(WaterFlowSensorId1, record);
    }

    private void WaterFlowSensor_PulseEvent(object? sender, WaterFlowPulseEventArgs args)
    {
        WaterFlowSensorRecord record = new WaterFlowSensorRecord(args.FlowRate, args.TotalLitres);


        //PHSensor(WaterFlowSensorId1, record);
    }

    //private static async Task<IEnumerable<SensorRecordBase>> GetUsersAsync(IEnumerable<int> userIds)
    //{
    //    List<Task<SensorRecordBase>> getUserTasks = new List<Task<SensorRecordBase>>();

    //    foreach (int userId in userIds)
    //    {
    //        getUserTasks.Add(GetTemperatureHumiditySensorRecordAsync(userId));
    //    }

    //    return await Task.WhenAll(getUserTasks).ConfigureAwait(false);
    //}

    //private static async Task<User[]> GetUsersAsyncByLINQ(IEnumerable<int> userIds)
    //{
    //    var getUserTasks = userIds.Select(id => GetUserAsync(id)).ToArray();
    //    return await Task.WhenAll(getUserTasks);
    //}

    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    //public async Task RunAsync(CancellationToken cancellationToken)
    //{
    //    cancellationToken.Register(CancellationTokenCallback);

    //    IEnumerable<SensorRecord> sensorRecords = new




    //    return await Task.WhenAll(getUserTasks);
    //}

    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    public async Task Run(CancellationToken cancellationToken)
    {
        cancellationToken.Register(CancellationTokenCallback);

        keepRunning = true;

        TimeSpan delay = new TimeSpan(0, 0, 0, 0, 0, 100);

        TimeSpan switchDisplayTime = new TimeSpan(days: 0, hours: 0, minutes: 0, seconds: 0, milliseconds: 500, microseconds: 0);


        //const int sampleSize = 5;//(int)Math.Round((_pHProbeSensor.DataFrequency * (((double)delay.Seconds)+(((double)delay.Milliseconds)/1000.0)+(((double)delay.Microseconds)/1000000.0))));

        //int Index = 0;

        //double displayPhSensor1_PH = 0.0;
        //double displayPhSensor1_Temperature = 0.0;
        //double displayPhSensor2_PH = 0.0;
        //double displayPhSensor2_Temperature = 0.0;
        //double displayWaterFlowSensor1_AverageFlow = 0.0;
        //double displayWaterFlowSensor1_AverageTotal = 0.0;
        //double displayTemperatureHumidity1_AverageHumidity = 0.0;
        //double displayTemperatureHumidity1_AverageTemperature = 0.0;
        //double displayBarometricPressure1_AveragePressure = 0.0;

        //double PhSensor1_AveragePH = 0.0;
        //double PhSensor1_AverageTemperature = 0.0;
        //double PhSensor2_AveragePH = 0.0;
        //double PhSensor2_AverageTemperature = 0.0;
        //double WaterFlowSensor1_AverageFlow = 0.0;
        //double WaterFlowSensor1_AverageTotal = 0.0;
        //double TemperatureHumidity1_AverageHumidity = 0.0;
        //double TemperatureHumidity1_AverageTemperature = 0.0;
        //double BarometricPressure1_AveragePressure = 0.0;

        Console.WriteLine("Running");

        //_sensors = new RPiSensors();

        Stopwatch sw = new Stopwatch();
        sw.Reset();
        sw.Start();

        //List<Task> sensorTasks = new List<Task>(5);

        //Task pHProbeSensor1Task;
        //Task pHProbeSensor2Task;
        //Task waterFlowSensor1Task;
        //Task temperatureHumidity1Task;
        //Task barometricPressure1Task;



        //Task OLEDDisplayTask = Task.Run(async () =>
        //{
        //    while (await switchDisplayTimer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
        //    {
        //        DisplayText(Ssd1306OLEDDisplay[displayIndex].Text, 0, 0);

        //        displayIndex = (++displayIndex) % 5;
        //    }
        //});

        OpenWaterValve();

        while ((!Console.KeyAvailable) || (!cancellationToken.IsCancellationRequested) || (keepRunning))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                keepRunning = false;
                break;
            }

            //ProgressPrinter.Instance.Report(Index++);

            //Index %= 100;

            //PhSensor1_AveragePH = 0.0;
            //PhSensor2_AveragePH = 0.0;
            //PhSensor1_AverageTemperature = 0.0;
            //PhSensor2_AverageTemperature = 0.0;
            //WaterFlowSensor1_AverageFlow = 0.0;
            //WaterFlowSensor1_AverageTotal = 0.0;
            //TemperatureHumidity1_AverageHumidity = 0.0;
            //TemperatureHumidity1_AverageTemperature = 0.0;
            //BarometricPressure1_AveragePressure = 0.0;

            //for (int i = 0; i < sampleSize; i++)
            //{
            //    Console.Write($"\r{DateTime.Now.ToLongTimeString()}");


            //    pHProbeSensor1Task = Task.Factory.StartNew(() =>
            //    {
            //        _sensors.Ph1 = _pHProbeSensor1.GetValues();

            //        _records.Add(PHProbeSensorId1, new PhSensorRecord()
            //        {
            //            Vcc = _sensors.Ph1.Vcc,
            //            Ph = _sensors.Ph1.Ph,
            //            Temperature = _sensors.Ph1.Temperature
            //        });

            //        Ssd1306OLEDDisplay[0].Text = $"Ph 1: {displayPhSensor1_PH:N3}\nTemperature 1: {displayPhSensor1_Temperature:N5}";
            //    });

            //    pHProbeSensor2Task = Task.Factory.StartNew(() =>
            //    {
            //        _sensors.Ph2 = _pHProbeSensor2.GetValues();
            //        _records.Add(PHProbeSensorId2, new PhSensorRecord()
            //        {
            //            Vcc = _sensors.Ph2.Vcc,
            //            Ph = _sensors.Ph2.Ph,
            //            Temperature = _sensors.Ph2.Temperature
            //        });

            //        Ssd1306OLEDDisplay[1].Text = $"Ph 2: {displayPhSensor2_PH:N3}\nTemperature 2: {displayPhSensor2_Temperature:N5}";
            //    });

            //    waterFlowSensor1Task = Task.Factory.StartNew(() =>
            //    {
            //        WaterFlowSensor.Run();
            //        _records.Add(WaterFlowSensorId1, new WaterFlowSensorRecord()
            //        {
            //            FlowRate = WaterFlowSensor.FlowRate,
            //            TotalLitres = WaterFlowSensor.TotalLitres
            //        });
            //        Ssd1306OLEDDisplay[2].Text = $"Flow: {FlowRate.LitersPerMinute:N4}\nTotal: {TotalLitres.Liters:N6}";
            //    });

            //    temperatureHumidity1Task = Task.Factory.StartNew(() =>
            //    {
            //        _records.Add(TemperatureHumiditySensorId1, new TemperatureHumiditySensorRecord()
            //        {
            //            Temperature = TemperatureHumiditySensor.Temperature,
            //            Humidity = TemperatureHumiditySensor.Humidity
            //        });
            //        Ssd1306OLEDDisplay[3].Text = $"Temperature: {Math.Round(TemperatureHumidity.DegreesFahrenheit, 2):N5}\nHumidity: {Math.Round(Humidity.Percent, 4):N4}%";
            //    });

            //    barometricPressure1Task = Task.Factory.StartNew(() =>
            //    {
            //        _records.Add(BarometricPressureSensorId1, new BarometricPressureSensorRecord()
            //        {
            //            Pressure = BarometricPressureSensor.Pressure
            //        });
            //        Ssd1306OLEDDisplay[4].Text = $"Pressure: {Pressure.Kilopascals:N3}";
            //    });

            //    sensorTasks.Clear();
            //    sensorTasks.Add(pHProbeSensor1Task);
            //    sensorTasks.Add(pHProbeSensor2Task);
            //    sensorTasks.Add(waterFlowSensor1Task);
            //    sensorTasks.Add(temperatureHumidity1Task);
            //    sensorTasks.Add(barometricPressure1Task);

            //    //Task.WaitAll(sensorTasks.ToArray(), 1000);
            //    RunTasksAsync(sensorTasks, cancellationToken).Wait();

            //    //Task.WaitAll(sensorTasks.ToArray(), 1000);
            //    //RunTasks(sensorTasks, cancellationToken).Wait(Timeout);

            //    PhSensor1_AveragePH += PHRange.Limit(_sensors.Ph1.Ph);
            //    displayPhSensor1_PH = Math.Round(PhSensor1_AveragePH / sampleSize, 2);

            //    PhSensor1_AverageTemperature += TemperatureRange.Limit(_sensors.Ph1.Temperature.DegreesFahrenheit);
            //    displayPhSensor1_Temperature = Math.Round(PhSensor1_AverageTemperature / sampleSize, 2);

            //    PhSensor2_AveragePH += PHRange.Limit(_sensors.Ph2.Ph);
            //    displayPhSensor2_PH = Math.Round(PhSensor2_AveragePH / sampleSize, 2);

            //    PhSensor2_AverageTemperature += TemperatureRange.Limit(_sensors.Ph2.Temperature.DegreesFahrenheit);
            //    displayPhSensor2_Temperature = Math.Round(PhSensor2_AverageTemperature / sampleSize, 2);

            //    if (WaterFlowRateHasChanged)
            //    {
            //        WaterFlowSensor1_AverageFlow += MaxMin(_sensors.WaterFlow1.FlowRate.LitersPerMinute, 0.0, 100.0);
            //        displayWaterFlowSensor1_AverageFlow = Math.Round(WaterFlowSensor1_AverageFlow / sampleSize, 2);

            //        WaterFlowSensor1_AverageTotal += MaxMin(_sensors.WaterFlow1.TotalLitres.Liters, 0.0, 100.0);
            //        displayWaterFlowSensor1_AverageTotal = Math.Round(WaterFlowSensor1_AverageTotal / sampleSize, 2);

            //        WaterFlowRateHasChanged = false;
            //    }

            //    TemperatureHumidity1_AverageHumidity += HumidityRange.Limit(_sensors.TemperatureHumidity1.Humidity.Percent);
            //    displayTemperatureHumidity1_AverageHumidity = Math.Round(TemperatureHumidity1_AverageHumidity / sampleSize, 2);

            //    TemperatureHumidity1_AverageTemperature += TemperatureRange.Limit(_sensors.TemperatureHumidity1.Temperature.DegreesFahrenheit);
            //    displayTemperatureHumidity1_AverageTemperature = Math.Round(TemperatureHumidity1_AverageTemperature / sampleSize, 2);

            //    BarometricPressure1_AveragePressure += _sensors.BarometricPressure1.Pressure.Kilopascals;
            //    displayBarometricPressure1_AveragePressure = Math.Round(BarometricPressure1_AveragePressure / sampleSize, 2);
            //}

            //if (sw.ElapsedMilliseconds > switchDisplayTime.TotalMilliseconds)
            //{
            //    _ssd1306Text.Clear();

            //    switch (switchDisplay)
            //    {
            //        case 0:
            //        {
            //            _ssd1306Text.AppendLine($"Ph 1: {displayPhSensor1_PH:N3}");
            //            _ssd1306Text.AppendLine($"Temperature 1: {displayPhSensor1_Temperature:N5}");
            //            break;
            //        }
            //        case 1:
            //        {
            //            _ssd1306Text.AppendLine($"Ph 2: {displayPhSensor2_PH:N3}");
            //            _ssd1306Text.AppendLine($"Temperature 2: {displayPhSensor2_Temperature:N5}");
            //            break;
            //        }
            //        case 2:
            //        {
            //            _ssd1306Text.AppendLine($"Flow: {FlowRate.LitersPerMinute:N4}");
            //            _ssd1306Text.AppendLine($"Total: {TotalLitres.Liters:N6}");
            //            break;
            //        }
            //        case 3:
            //        {
            //            _ssd1306Text.AppendLine($"Temperature: {Math.Round(TemperatureHumidity.DegreesFahrenheit, 2):N5}");
            //            _ssd1306Text.AppendLine($"Humidity: {Math.Round(Humidity.Percent, 4):N4}%");
            //            break;
            //        }
            //        case 4:
            //        {
            //            _ssd1306Text.AppendLine($"Pressure: {Pressure.Kilopascals:N3}");
            //            break;
            //        }
            //    }

            //    DisplayText(_ssd1306Text.ToString(), 0, 0);

            //    switchDisplay = (++switchDisplay) % switchDisplayMax;

            //    sw.Restart();
            //}

            ProgressPrinter.Instance.Report();

            //Task.Delay(100).Wait();
        }
                
        CloseWaterValve();

        keepRunning = false;

        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        static double MaxMin(in double value, in double low, in double high)
        {
            return Math.Max(low, Math.Min(value, high));
        }

        ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        //static async Task RunTasks(IEnumerable<Task> tasks, CancellationToken cancellationToken)
        //{
        //    await foreach (Task task in ToAsync(tasks, cancellationToken))
        //    {
        //        task.Wait(cancellationToken);
        //    }
        //}



        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        static async Task RunTasksAsync(IEnumerable<Task> tasks, CancellationToken cancellationToken)
        {
            foreach (Task task in tasks)
            {
                try
                {
                    await task.WaitAsync(WaitAsyncDelay, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }

        ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        //static async IAsyncEnumerable<Task> ToAsync(IEnumerable<Task> tasks, [EnumeratorCancellation] CancellationToken cancellationToken)
        //{
        //    foreach (Task item in tasks)
        //    {
        //        await item.WaitAsync(cancellationToken);
        //        yield return item;
        //    }



        //    //using IEnumerator<Task> enumerator = tasks.GetEnumerator();

        //    //while (enumerator.MoveNext())
        //    //{
        //    //    await Task.Run(enumerator.MoveNext);
        //    //    yield return enumerator.Current;
        //    //}
        //    //while (await Task.Run(enumerator.MoveNext).ConfigureAwait(false))
        //    //{
        //    //    Console.WriteLine("await Task.Run");
        //    //    yield return enumerator.Current;
        //    //}
        //}

        ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        //static async IAsyncEnumerable<Task> Execute(IAsyncEnumerable<Task> data)
        //{
        //    await foreach (Task item in data)
        //    {
        //        yield return item;
        //    }
        //}

        //return Task.CompletedTask;
    }

    private static readonly TimeSpan WaitAsyncDelay = new TimeSpan(0, 0, 0, 1, 0, 0);

    //public Tca9548A Setup(I2cDevice i2cDevice, I2cBus i2cBus)
    //{
    //    Tca9548A tca9548a = new Tca9548A(i2cDevice, i2cBus);

    //    foreach (Tca9548AChannelBus channelBuses in tca9548a)
    //    {
    //        Console.WriteLine(channelBuses.QueryComponentInformation());

    //        Console.WriteLine("PerformBusScan");

    //        List<int> deviceAddress = channelBuses.PerformBusScan(0x08, 0x6F);

    //        foreach (int device in deviceAddress)
    //        {
    //            Console.WriteLine($"0x{device:X2}");
    //        }

    //    }
    //    return tca9548a;
    //}

    public override string? ToString()
    {
        return _raspberryPiBoard.QueryComponentInformation().ToString();
    }

    internal const string settingsFilePath = "/home/trmcnealy/DeviceSettings.xml";

    //internal static RPiSettings ImportSettings()
    //{
    //    return new RPiSettings();
    //    //if (File.Exists(settingsFilePath))
    //    //{
    //    //    using (FileStream _inputStream = new FileStream(settingsFilePath, FileMode.Open))
    //    //    {
    //    //        using (BufferedStream _inputBuffered = new BufferedStream(_inputStream))
    //    //        {
    //    //            XmlSerializer serializer = new XmlSerializer(typeof(DeviceSettings));

    //    //            using (XmlTextReader reader = new XmlTextReader(_inputBuffered))
    //    //            {
    //    //                return serializer.Deserialize(reader) as DeviceSettings ?? throw new Exception("DeviceSettings Deserialize is null");
    //    //            }
    //    //        }
    //    //    }
    //    //}
    //    //else
    //    //{
    //    //    //ExportSettings(new DeviceSettings());
    //    //    //return ImportSettings();
    //    //}
    //}

    //internal static void ExportSettings(RPiSettings deviceSettings)
    //{
    //    using (FileStream _inputStream = new FileStream(settingsFilePath, FileMode.Create))
    //    {
    //        using (BufferedStream _inputBuffered = new BufferedStream(_inputStream))
    //        {
    //            XmlSerializer serializer = new XmlSerializer(typeof(RPiSettings));

    //            using (XmlTextWriter writer = new XmlTextWriter(_inputBuffered, null))
    //            {
    //                serializer.Serialize(writer, deviceSettings);
    //            }
    //        }
    //    }
    //}

    private static bool IsRaspi4()
    {
        if (File.Exists("/proc/device-tree/model"))
        {
            string model = File.ReadAllText("/proc/device-tree/model", Encoding.ASCII);

            if (model.Contains("Raspberry Pi 4") || model.Contains("Raspberry Pi Compute Module 4"))
            {
                return true;
            }
        }

        return false;
    }

    //private static List<byte> BusScan(Tca9548AChannelBus bus)
    //{
    //    List<byte> results = bus.PerformBusScan(LowestAddress, HighestAddress);

    //    if (results.Count == 0)
    //    {
    //        Console.WriteLine($"PerformBusScan Failed.");
    //    }
    //    else
    //    {
    //        foreach (byte result in results)
    //        {
    //            Console.WriteLine($"PerformBusScan: 0x{result:X2}");
    //        }

    //    }

    //    return results;
    //}

    private static List<byte> BusScan<TBus>(TBus bus, in byte lowest = LowestAddress, in byte highest = HighestAddress) where TBus : I2cBus
    {
        List<byte> ret = new List<byte>((highest - lowest) + 1);

        for (byte addr = lowest; addr <= highest; addr++)
        {
            try
            {
                using (I2cDevice device = bus.CreateDevice(addr))
                {
                    device.ReadByte();
                    ret.Add(addr);
                    //Console.WriteLine($"Device found @ 0x{addr:X2} on Channel {bus.QueryComponentInformation()}");
                }
            }
            catch
            {
                //throw new Exception();
            }
            finally
            {
                bus.RemoveDevice(addr);
            }
        }

        return ret;
    }
}
