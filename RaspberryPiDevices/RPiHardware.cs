
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Device.I2c;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Xml;
using System.Xml.Serialization;

using Iot.Device.Board;
using Iot.Device.Graphics;
using Iot.Device.Graphics.SkiaSharpAdapter;
using Iot.Device.Pcx857x;
using Iot.Device.Ssd13xx;
using Iot.Device.Ssd13xx.Commands;
using Iot.Device.Tca954x;

using RaspberryPiDevices;

using UnitsNet;
using UnitsNet.Units;


namespace RaspberryPiDevices;

public class RPiHardware : IDisposable
{
    public static readonly Acceleration Gravity = new Acceleration(32.17405, AccelerationUnit.FootPerSecondSquared);

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

    internal readonly byte[] PCF8574A_Write_Bytes = new byte[]
    {
        0b11111110,
        0b11111101,
        0b11111011,
        0b11110111,
        0b11101111,
        0b11011111,
        0b10111111,
        0b01111111,
        0b11111111
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

    internal readonly RPiSettings _settings;

    internal readonly RaspberryPiBoard _raspberryPiBoard;

    internal readonly I2cBus _i2cBus;


    public readonly Dictionary<Guid, I2cDevice> I2cDevices;
    public readonly List<IDisposable> Disposables;

    internal readonly GpioController _gpioController;

    internal readonly I2cDevice _pCF8574A_i2cDevice;
    internal readonly Pcf8574 _pcf8574;
    internal readonly I2cDevice _tca9548A_i2cDevice;
    internal readonly Tca9548A _tca9548a;
    internal readonly Ssd1306 _ssd1306;

    internal readonly BarometricPressureSensor BarometricPressureSensor;
    internal readonly WaterFlowSensor WaterFlowSensor;
    internal readonly PHProbeSensor _pHProbeSensor1;
    internal readonly PHProbeSensor _pHProbeSensor2;
    internal readonly TemperatureHumiditySensor TemperatureHumiditySensor;

    internal readonly MultiplexerChannel TemperatureHumiditySensorChannel = MultiplexerChannel.Channel0;
    internal readonly Tca9548AChannelBus temperatureHumiditySensorChannel;

    public int GpioInterruptPinNumber
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return 4;
        }
    }

    public double Ph1
    {
        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get; private set;
    }
    public Temperature PhTemperature1
    {
        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get; private set;
    }

    public double Ph2
    {
        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get; private set;
    }
    public Temperature PhTemperature2
    {
        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get; private set;
    }

    public Temperature TemperatureHumidity
    {
        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get; private set;
    }
    public RelativeHumidity Humidity
    {
        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get; private set;
    }

    public Pressure Pressure
    {
        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get; private set;
    }

    public VolumeFlow FlowRate
    {
        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get; private set;
    }
    public Volume TotalLitres
    {
        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get; private set;
    }

    private bool WaterFlowRateHasChanged;

    public IGraphics Ssd1306Graphics
    {
        get
        {
            return _ssd1306BitmapImage.GetDrawingApi();
        }
    }

    private readonly StringBuilder _ssd1306Text;

    private readonly BitmapImage _ssd1306BitmapImage;// = BitmapImage.CreateBitmap(Ssd1306Width, Ssd1306Height, PixelFormat.Format32bppArgb);

    private readonly int _ssd1306FontSize = 12;
    private readonly string _ssd1306Font = "Cascadia Code";

    private RPiSensors _sensors;

    //private RPiDisplayData _displayData;

    //private RPiPhSensorValues[] _phSensorValues;

    //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public RPiHardware(Pcf8574I2CSlaveSwitch slaveSwitch = Pcf8574I2CSlaveSwitch.None)
    {
        _sensors = new RPiSensors();
        _settings = new RPiSettings();

        //_displayData = new RPiDisplayData();

        //_phSensorValues = new RPiPhSensorValues[2] { new RPiPhSensorValues(), new RPiPhSensorValues() };

        _raspberryPiBoard = new RaspberryPiBoard()
        {

        };

        _gpioController = _raspberryPiBoard.CreateGpioController();

        _i2cBus = _raspberryPiBoard.CreateOrGetI2cBus(I2cBusIndex);

        //(List<byte> FoundDevices, byte LowestAddress, byte HighestAddress) busScan = _i2cBus.PerformBusScan(ProgressPrinter.Instance, LowestAddress, HighestAddress);
        //Console.WriteLine(busScan.ToUserReadableTable());

        Disposables = new List<IDisposable>();
        I2cDevices = new Dictionary<Guid, I2cDevice>();

        _ssd1306 = new Ssd1306(_raspberryPiBoard.CreateI2cDevice(new I2cConnectionSettings(I2cBusIndex, Ssd1306Address)), Ssd1306Width, Ssd1306Height);

        _ssd1306Text = new StringBuilder();
        _ssd1306BitmapImage = _ssd1306.GetBackBufferCompatibleImage();


        if (_ssd1306BitmapImage.Width != Ssd1306Width || _ssd1306BitmapImage.Height != Ssd1306Height)
        {
            throw new Exception($"{_ssd1306BitmapImage.Width} != {Ssd1306Width} || {_ssd1306BitmapImage.Height} != {Ssd1306Height}");
        }

        _ssd1306.ClearScreen();

        //DisplayFullBlocks();

        //for (int x = 0; x < _ssd1306BitmapImage.Width; x++)
        //{
        //    for (int y = 0; y < _ssd1306BitmapImage.Height; y++)
        //    {
        //        _ssd1306BitmapImage[x, y] = Color.White;
        //    }
        //}

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

        _tca9548A_i2cDevice = _raspberryPiBoard.CreateI2cDevice(new I2cConnectionSettings(I2cBusIndex, Tca9548A_Address));
        //I2cDevices.Add(Tca9548AId, _tca9548A_i2cDevice);
        _tca9548a = new Tca9548A(_tca9548A_i2cDevice, _i2cBus, false);
        //Console.WriteLine("Tca9548A");

        temperatureHumiditySensorChannel = (Tca9548AChannelBus)CreateI2cBusFromChannel(MultiplexerChannel.Channel0);
        //Console.WriteLine("TemperatureHumiditySensorChannel");


        TemperatureHumiditySensor = new TemperatureHumiditySensor(temperatureHumiditySensorChannel.CreateDevice(TemperatureHumiditySensor_Address));
        //Console.WriteLine("TemperatureHumiditySensor");

        //Disposables.Add(_ssd1306BitmapImage);
        Disposables.Add(_ssd1306);
        Disposables.Add(_pHProbeSensor1);
        Disposables.Add(_pHProbeSensor2);
        Disposables.Add(WaterFlowSensor);
        Disposables.Add(_pcf8574);
        Disposables.Add(_tca9548a);
        Disposables.Add(TemperatureHumiditySensor);
    }



    #region Dctor
    //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    ~RPiHardware()
    {
        Dispose(disposing: false);
    }

    //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected virtual void Dispose(bool disposing)
    {
        lock (this)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    keepRunning = false;


                    //ExportSettings(Settings);

                    _ssd1306.ClearScreen();
                    Utilities.DelayMicroseconds(1);
                    _ssd1306.SendCommand(new SetDisplayOff());

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

                    _ssd1306BitmapImage.Dispose();
                }

                // unmanaged
                disposedValue = true;
            }
        }
    }

    //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
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

    private void DrawBitmap()
    {
        _ssd1306.DrawBitmap(_ssd1306BitmapImage);
    }

    private void DisplayFullBlocks(int startX = 0, int startY = 0)
    {
        _ssd1306BitmapImage.Clear(Color.Black);

        StringBuilder sb = new StringBuilder();

        sb.AppendLine(FullBlockLightShadePatternString1);
        sb.AppendLine(FullBlockLightShadePatternString2);
        sb.AppendLine(FullBlockLightShadePatternString1);
        sb.AppendLine(FullBlockLightShadePatternString2);
        sb.AppendLine(FullBlockLightShadePatternString1);

        Ssd1306Graphics.DrawText(sb.ToString(), _ssd1306Font, _ssd1306FontSize, Color.White, new Point(startX, startY));

        DrawBitmap();
    }

    private void DisplayText(string text, int startX = 0, int startY = 0)
    {
        _ssd1306BitmapImage.Clear(Color.Black);

        Ssd1306Graphics.DrawText(text, _ssd1306Font, _ssd1306FontSize, Color.White, new Point(startX, startY));

        DrawBitmap();
    }

    private void DisplayClock(int startX = 0, int startY = 0)
    {
        _ssd1306BitmapImage.Clear(Color.Black);

        Ssd1306Graphics.DrawText(DateTime.Now.ToString("HH:mm:ss"), _ssd1306Font, _ssd1306FontSize, Color.White, new Point(startX, startY));

        DrawBitmap();
    }

    private static void DisplayImage(GraphicDisplay ssd1306, BitmapImage ssd1306BitmapImage)
    {
        //Console.WriteLine("Display Image");
        ssd1306.DrawBitmap(ssd1306BitmapImage);
    }
    private static void DisplayClock(GraphicDisplay ssd1306, BitmapImage ssd1306BitmapImage)
    {
        //Console.WriteLine("Display clock");

        int fontSize = 25;
        string font = "DejaVu Sans";

        ssd1306BitmapImage.Clear(Color.Black);

        IGraphics g = ssd1306BitmapImage.GetDrawingApi();

        g.DrawText(DateTime.Now.ToString("HH:mm:ss"), font, fontSize, Color.White, new Point(0, 0));

        ssd1306.DrawBitmap(ssd1306BitmapImage);
    }

    public I2cDevice CreateI2cDevice(Tca9548AChannelBus bus, byte address)
    {
        try
        {
            return bus.CreateDevice(address);
        }
        catch
        {
            throw new Exception();
        }
    }

    public I2cBus CreateI2cBusFromChannel(MultiplexerChannel channel)
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

    private void CancellationTokenCallback()
    {
        keepRunning = false;
    }


    //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Run(CancellationToken cancellationToken)
    {
        cancellationToken.Register(CancellationTokenCallback);

        keepRunning = true;

        TimeSpan delay = new TimeSpan(0, 0, 0, 0, 0, 100);

        TimeSpan switchDisplayTime = new TimeSpan(days: 0, hours: 0, minutes: 0, seconds: 0, milliseconds: 500, microseconds: 0);
        int switchDisplay = 0;
        const int switchDisplayMax = 5;

        const int sampleSize = 10;//(int)Math.Round((_pHProbeSensor.DataFrequency * (((double)delay.Seconds)+(((double)delay.Milliseconds)/1000.0)+(((double)delay.Microseconds)/1000000.0))));


        //int Index = 0;

        double displayPhSensor1_PH = 0.0;
        double displayPhSensor1_Temperature = 0.0;
        double displayPhSensor2_PH = 0.0;
        double displayPhSensor2_Temperature = 0.0;
        double displayWaterFlowSensor1_AverageFlow = 0.0;
        double displayWaterFlowSensor1_AverageTotal = 0.0;
        double displayTemperatureHumidity1_AverageHumidity = 0.0;
        double displayTemperatureHumidity1_AverageTemperature = 0.0;
        double displayBarometricPressure1_AveragePressure = 0.0;

        double PhSensor1_AveragePH = 0.0;
        double PhSensor1_AverageTemperature = 0.0;
        double PhSensor2_AveragePH = 0.0;
        double PhSensor2_AverageTemperature = 0.0;
        double WaterFlowSensor1_AverageFlow = 0.0;
        double WaterFlowSensor1_AverageTotal = 0.0;
        double TemperatureHumidity1_AverageHumidity = 0.0;
        double TemperatureHumidity1_AverageTemperature = 0.0;
        double BarometricPressure1_AveragePressure = 0.0;

        Console.WriteLine("Running");

        _sensors = new RPiSensors();


        Stopwatch sw = new Stopwatch();
        sw.Reset();
        sw.Start();

        List<Task> sensorTasks = new List<Task>(5);

        while ((!Console.KeyAvailable) || (!cancellationToken.IsCancellationRequested) || (keepRunning))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                keepRunning = false;
                break;
            }

            //ProgressPrinter.Instance.Report(((float)Index));

            //Index = ((++Index) % 100);

            PhSensor1_AveragePH = 0.0;
            PhSensor2_AveragePH = 0.0;
            PhSensor1_AverageTemperature = 0.0;
            PhSensor2_AverageTemperature = 0.0;
            WaterFlowSensor1_AverageFlow = 0.0;
            WaterFlowSensor1_AverageTotal = 0.0;
            TemperatureHumidity1_AverageHumidity = 0.0;
            TemperatureHumidity1_AverageTemperature = 0.0;
            BarometricPressure1_AveragePressure = 0.0;

            for (int i = 0; i < sampleSize; i++)
            {
                Console.Write($"{DateTime.Now.ToLongTimeString()}");

                //Task.Run(async () => await SampleTasksAsync());
                //Utilities.Delay(delay);

                //Task.Run(() =>
                //{
                //    _sensors.Ph1 = _pHProbeSensor1.GetValues();
                //    _sensors.Ph2 = _pHProbeSensor2.GetValues();

                //    WaterFlowSensor.Run();

                //    _sensors.TemperatureHumidity1.TemperatureHumidity = TemperatureHumiditySensor.Temperature;
                //    _sensors.TemperatureHumidity1.Humidity = TemperatureHumiditySensor.Humidity;

                //    _sensors.BarometricPressure1.Pressure = (BarometricPressureSensor.Weight * Gravity) / Area.FromSquareInches(0.03937);
                //}).Wait();

                Task pHProbeSensor1Task = Task.Factory.StartNew(() =>
                {
                    Console.WriteLine(nameof(pHProbeSensor1Task));
                    _sensors.Ph1 = _pHProbeSensor1.GetValues();
                });

                Task pHProbeSensor2Task = Task.Factory.StartNew(() =>
                {
                    Console.WriteLine(nameof(pHProbeSensor2Task));
                    _sensors.Ph2 = _pHProbeSensor2.GetValues();
                });

                Task waterFlowSensor1Task = Task.Factory.StartNew(() =>
                {
                    Console.WriteLine(nameof(waterFlowSensor1Task));
                    WaterFlowSensor.Run();
                });

                Task temperatureHumidity1Task = Task.Factory.StartNew(() =>
                {
                    Console.WriteLine(nameof(temperatureHumidity1Task));
                    _sensors.TemperatureHumidity1.Temperature = TemperatureHumiditySensor.Temperature;
                    _sensors.TemperatureHumidity1.Humidity = TemperatureHumiditySensor.Humidity;
                });

                Task barometricPressure1Task = Task.Factory.StartNew(() =>
                {
                    Console.WriteLine(nameof(barometricPressure1Task));
                    _sensors.BarometricPressure1.Pressure = (BarometricPressureSensor.Weight * Gravity) / Area.FromSquareInches(0.03937);
                });

                sensorTasks.Clear();
                sensorTasks.Add(pHProbeSensor1Task);
                sensorTasks.Add(pHProbeSensor2Task);
                sensorTasks.Add(waterFlowSensor1Task);
                sensorTasks.Add(temperatureHumidity1Task);
                sensorTasks.Add(barometricPressure1Task);

                //Task.WaitAll(sensorTasks.ToArray(), 1000);
                RunTasksAsync(sensorTasks, cancellationToken).Wait();

                //Task.WaitAll(sensorTasks.ToArray(), 1000);
                //RunTasks(sensorTasks, cancellationToken).Wait(Timeout);

                PhSensor1_AveragePH += MaxMin(_sensors.Ph1.Ph, 0.0, 14.0);
                displayPhSensor1_PH = Math.Round(PhSensor1_AveragePH / sampleSize, 2);

                PhSensor1_AverageTemperature += MaxMin((_sensors.Ph1.Temperature.DegreesFahrenheit), -200, 200.0);
                displayPhSensor1_Temperature = Math.Round(PhSensor1_AverageTemperature / sampleSize, 2);

                PhSensor2_AveragePH += MaxMin(_sensors.Ph2.Ph, 0.0, 14.0);
                displayPhSensor2_PH = Math.Round(PhSensor2_AveragePH / sampleSize, 2);

                PhSensor2_AverageTemperature += MaxMin((_sensors.Ph2.Temperature.DegreesFahrenheit), -200, 200.0);
                displayPhSensor2_Temperature = Math.Round(PhSensor2_AverageTemperature / sampleSize, 2);

                if (WaterFlowRateHasChanged)
                {
                    WaterFlowSensor1_AverageFlow += MaxMin(_sensors.WaterFlow1.FlowRate.LitersPerMinute, 0.0, 100.0);
                    displayWaterFlowSensor1_AverageFlow = Math.Round(WaterFlowSensor1_AverageFlow / sampleSize, 2);

                    WaterFlowSensor1_AverageTotal += MaxMin(_sensors.WaterFlow1.TotalLitres.Liters, 0.0, 100.0);
                    displayWaterFlowSensor1_AverageTotal = Math.Round(WaterFlowSensor1_AverageTotal / sampleSize, 2);

                    WaterFlowRateHasChanged = false;
                }

                TemperatureHumidity1_AverageHumidity += MaxMin(_sensors.TemperatureHumidity1.Humidity.Percent, 0.0, 100.0);
                displayTemperatureHumidity1_AverageHumidity = Math.Round(TemperatureHumidity1_AverageHumidity / sampleSize, 2);

                TemperatureHumidity1_AverageTemperature += MaxMin(_sensors.TemperatureHumidity1.Temperature.DegreesFahrenheit, -200.0, 20.0);
                displayTemperatureHumidity1_AverageTemperature = Math.Round(TemperatureHumidity1_AverageTemperature / sampleSize, 2);

                BarometricPressure1_AveragePressure += MaxMin(_sensors.BarometricPressure1.Pressure.Kilopascals, -1000.0, 1000.0);
                displayBarometricPressure1_AveragePressure = Math.Round(BarometricPressure1_AveragePressure / sampleSize, 2);
            }

            if (sw.ElapsedMilliseconds > switchDisplayTime.TotalMilliseconds)
            {
                _ssd1306Text.Clear();

                switch (switchDisplay)
                {
                    case 0:
                    {
                        _ssd1306Text.AppendLine($"Ph 1: {displayPhSensor1_PH:N3}");
                        _ssd1306Text.AppendLine($"Temperature 1: {displayPhSensor1_Temperature:N5}");
                        break;
                    }
                    case 1:
                    {
                        _ssd1306Text.AppendLine($"Ph 2: {displayPhSensor2_PH:N3}");
                        _ssd1306Text.AppendLine($"Temperature 2: {displayPhSensor2_Temperature:N5}");
                        break;
                    }
                    case 2:
                    {
                        _ssd1306Text.AppendLine($"Flow: {FlowRate.LitersPerMinute:N4}");
                        _ssd1306Text.AppendLine($"Total: {TotalLitres.Liters:N6}");
                        break;
                    }
                    case 3:
                    {
                        _ssd1306Text.AppendLine($"Temperature: {Math.Round(TemperatureHumidity.DegreesFahrenheit, 2):N5}");
                        _ssd1306Text.AppendLine($"Humidity: {Math.Round(Humidity.Percent, 4):N4}%");
                        break;
                    }
                    case 4:
                    {
                        _ssd1306Text.AppendLine($"Pressure: {Pressure.Kilopascals:N3}");
                        break;
                    }
                }

                DisplayText(_ssd1306Text.ToString(), 0, 0);

                switchDisplay = (++switchDisplay) % switchDisplayMax;

                sw.Restart();
            }

            //ProgressPrinter.Instance.Report();

            //Utilities.DelayMicroseconds(0);
        }

        keepRunning = false;

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        static double MaxMin(in double value, in double low, in double high)
        {
            return Math.Max(low, Math.Min(value, high));
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        //static async Task RunTasks(IEnumerable<Task> tasks, CancellationToken cancellationToken)
        //{
        //    await foreach (Task task in ToAsync(tasks, cancellationToken))
        //    {
        //        task.Wait(cancellationToken);
        //    }
        //}



        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        static async Task RunTasksAsync(IEnumerable<Task> tasks, CancellationToken cancellationToken)
        {
            foreach (Task task in tasks)
            {
                await task.WaitAsync(WaitAsyncDelay, cancellationToken);
            }
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
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

        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        //static async IAsyncEnumerable<Task> Execute(IAsyncEnumerable<Task> data)
        //{
        //    await foreach (Task item in data)
        //    {
        //        yield return item;
        //    }
        //}
    }

    private static readonly TimeSpan WaitAsyncDelay = new TimeSpan(0, 0, 0, 1, 0, 0);

    private void WaterFlowSensor_PulseEvent(object? sender, WaterFlowSensorEventArgs args)
    {
        _sensors.WaterFlow1.FlowRate = args.FlowRate;
        _sensors.WaterFlow1.TotalLitres = args.TotalLitres;

        WaterFlowRateHasChanged = true;
    }

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

    internal static RPiSettings ImportSettings()
    {
        return new RPiSettings();
        //if (File.Exists(settingsFilePath))
        //{
        //    using (FileStream _inputStream = new FileStream(settingsFilePath, FileMode.Open))
        //    {
        //        using (BufferedStream _inputBuffered = new BufferedStream(_inputStream))
        //        {
        //            XmlSerializer serializer = new XmlSerializer(typeof(DeviceSettings));

        //            using (XmlTextReader reader = new XmlTextReader(_inputBuffered))
        //            {
        //                return serializer.Deserialize(reader) as DeviceSettings ?? throw new Exception("DeviceSettings Deserialize is null");
        //            }
        //        }
        //    }
        //}
        //else
        //{
        //    //ExportSettings(new DeviceSettings());
        //    //return ImportSettings();
        //}
    }

    internal static void ExportSettings(RPiSettings deviceSettings)
    {
        using (FileStream _inputStream = new FileStream(settingsFilePath, FileMode.Create))
        {
            using (BufferedStream _inputBuffered = new BufferedStream(_inputStream))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(RPiSettings));

                using (XmlTextWriter writer = new XmlTextWriter(_inputBuffered, null))
                {
                    serializer.Serialize(writer, deviceSettings);
                }
            }
        }
    }

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
