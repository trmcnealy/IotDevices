using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Device.I2c;
using System.Device.Spi;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection.PortableExecutable;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

using Iot.Device.Ads1115;
using Iot.Device.Bmp180;
using Iot.Device.Bno055;
using Iot.Device.Board;
using Iot.Device.Common;
using Iot.Device.FtCommon;
using Iot.Device.Media;
using Iot.Device.Mpr121;
using Iot.Device.Pcx857x;
using Iot.Device.Tca954x;

using Microsoft.Extensions.Logging;

using UnitsNet;
using UnitsNet.Units;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RaspberryPiDevices;

public class RPiHardware : IDisposable
{
    public static readonly Acceleration Gravity = new Acceleration(32.17405, AccelerationUnit.FootPerSecondSquared);

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


    static Dictionary<Guid, int> DeviceRegistry;

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

    private const int I2cBus = 1;

    //public const byte BarometricPressureSensor_Address = 0x27;



    public const byte Pcf8574_BaseAddress = 0x27;
    internal static byte Pcf8574_Address(Pcf8574I2CSlaveSwitch slaveSwitch)
    {
        return (byte)(Pcf8574_BaseAddress - slaveSwitch);
    }
    public const byte Tca9548A_Address = 0x70;
    public const byte PHProbeSensor1_Address = 0x48;
    public const byte PHProbeSensor2_Address = 0x48;
    public const byte TemperatureHumiditySensor_Address = 0x38;
    public const byte WaterFlowSensor_Address = 0x48;

    public const int PHProbeSensor1_Channel = 0;
    public const int PHProbeSensor2_Channel = 1;
    public const int TemperatureHumiditySensor_Channel = 2;
    public const int WaterFlowSensor_Channel = 3;

    public const int LED1_Channel = 4;
    public const int LED2_Channel = 5;
    public const int LED3_Channel = 6;
    public const int LED4_Channel = 7;

    public const int LED1_PinClk = 17;
    public const int LED1_PinDio = 27;
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
    internal readonly I2cDevice _pCF8574A_i2cDevice;
    internal readonly I2cDevice _tca9548A_i2cDevice;

    internal readonly Pcf8574 _pcf8574;
    internal readonly Tca9548A _tca9548a;


    public readonly Dictionary<Guid, I2cDevice> I2cDevices;

    //internal readonly I2cDevice _i2cDevice1;
    ////internal readonly I2cDevice _i2cDevice2;
    internal readonly GpioController _gpioController;

    internal readonly BarometricPressureSensor BarometricPressureSensor;
    internal readonly WaterFlowSensor WaterFlowSensor;
    internal readonly PHProbeSensor _pHProbeSensor1;
    internal readonly PHProbeSensor _pHProbeSensor2;
    internal readonly TemperatureHumiditySensor TemperatureHumiditySensor;

    internal readonly LED4DigitDisplay _lED4DigitDisplay1;
    internal readonly LED4DigitDisplay _lED4DigitDisplay2;
    internal readonly LED4DigitDisplay _lED4DigitDisplay3;
    internal readonly LED4DigitDisplay _lED4DigitDisplay4;

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
    public double TotalLitres
    {
        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get; private set;
    }


    //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public RPiHardware(Pcf8574I2CSlaveSwitch slaveSwitch = Pcf8574I2CSlaveSwitch.None)
    {
        _settings = new RPiSettings();

        _raspberryPiBoard = new RaspberryPiBoard()
        {

        };

        _gpioController = _raspberryPiBoard.CreateGpioController();

        _i2cBus = _raspberryPiBoard.CreateOrGetI2cBus(I2cBus);

        //const byte lowestAddress = 0x08;
        //const byte highestAddress = 0x77;

        //(List<byte> FoundDevices, byte LowestAddress, byte HighestAddress) busScan = _i2cBus.PerformBusScan(ProgressPrinter.Instance, lowestAddress, highestAddress);

        //Console.WriteLine(busScan.ToUserReadableTable());

        I2cDevices = new Dictionary<Guid, I2cDevice>();

        BarometricPressureSensor = new BarometricPressureSensor(_raspberryPiBoard, _gpioController);

        _pCF8574A_i2cDevice = _raspberryPiBoard.CreateI2cDevice(new I2cConnectionSettings(I2cBus, Pcf8574_Address(slaveSwitch)));

        I2cDevices.Add(Pcf8574Id, _pCF8574A_i2cDevice);
        _pcf8574 = new Pcf8574(_pCF8574A_i2cDevice, GpioInterruptPinNumber, _gpioController, false);

        //_pCF8574A_i2cDevice.WriteByte(0b11111111);

        _tca9548A_i2cDevice = _raspberryPiBoard.CreateI2cDevice(new I2cConnectionSettings(I2cBus, Tca9548A_Address));
        I2cDevices.Add(Tca9548AId, _tca9548A_i2cDevice);
        _tca9548a = new Tca9548A(_tca9548A_i2cDevice, _i2cBus, false);

        using I2cBus pHProbeSensor1Channel = CreateI2cBusFromChannel(MultiplexerChannel.Channel0);
        using I2cBus pHProbeSensor2Channel = CreateI2cBusFromChannel(MultiplexerChannel.Channel1);
        using I2cBus temperatureHumiditySensorChannel = CreateI2cBusFromChannel(MultiplexerChannel.Channel2);
        using I2cBus waterFlowSensorChannel = CreateI2cBusFromChannel(MultiplexerChannel.Channel3);

        I2cDevice PHProbeSensor1_I2cDevice = pHProbeSensor1Channel.CreateDevice(PHProbeSensor1_Address);
        I2cDevice PHProbeSensor2_I2cDevice = pHProbeSensor2Channel.CreateDevice(PHProbeSensor2_Address);
        I2cDevice TemperatureHumiditySensor_I2cDevice = temperatureHumiditySensorChannel.CreateDevice(TemperatureHumiditySensor_Address);
        I2cDevice WaterFlowSensor_I2cDevice = waterFlowSensorChannel.CreateDevice(WaterFlowSensor_Address);

//        byte sscan = PerformBusScan(waterFlowSensorChannel, 0x3, 0x77).First(); Console.WriteLine(sscan);        

        I2cDevices.Add(WaterFlowSensorId1, WaterFlowSensor_I2cDevice);
        I2cDevices.Add(PHProbeSensorId1, PHProbeSensor1_I2cDevice);
        I2cDevices.Add(PHProbeSensorId2, PHProbeSensor2_I2cDevice);
        I2cDevices.Add(TemperatureHumiditySensorId1, TemperatureHumiditySensor_I2cDevice);

        WaterFlowSensor = new WaterFlowSensor(I2cDevices[WaterFlowSensorId1], _gpioController);
        _pHProbeSensor1 = new PHProbeSensor(I2cDevices[PHProbeSensorId1], 1);
        _pHProbeSensor2 = new PHProbeSensor(I2cDevices[PHProbeSensorId1], 2);
        TemperatureHumiditySensor = new TemperatureHumiditySensor(I2cDevices[TemperatureHumiditySensorId1]);

        _lED4DigitDisplay1 = new LED4DigitDisplay(_gpioController, LED1_PinClk, LED1_PinDio);
        _lED4DigitDisplay2 = new LED4DigitDisplay(_gpioController, LED2_PinClk, LED2_PinDio);
        _lED4DigitDisplay3 = new LED4DigitDisplay(_gpioController, LED3_PinClk, LED3_PinDio);
        _lED4DigitDisplay4 = new LED4DigitDisplay(_gpioController, LED4_PinClk, LED4_PinDio);

        
        static List<byte> PerformBusScan(I2cBus bus, byte lowest = 0x3, byte highest = 0x77)
        {
            List<byte> ret = new List<byte>();
            for (byte addr = lowest; addr <= highest; addr++)
            {
                try
                {
                    using (I2cDevice device = bus.CreateDevice(addr))
                    {
                        device.ReadByte();
                        ret.Add(addr);
                    }
                }
                catch
                {
                }
            }

            return ret;
        }
    }

    public I2cBus CreateI2cBusFromChannel(MultiplexerChannel channel)
    {
        _tca9548a.SelectChannel(channel);

        if (_tca9548a.TryGetSelectedChannel(out MultiplexerChannel selectedChannel))
        {
            return _tca9548a.GetChannel(selectedChannel);
        }

        throw new Exception("CreateI2cBusFromChannel Failed.");
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
        if (!disposedValue)
        {
            if (disposing)
            {
                keepRunning = false;

                //ExportSettings(Settings);
                //_lED4DigitDisplay1.Dispose();
                //_lED4DigitDisplay2.Dispose();
                //_pHProbeSensor1.Dispose();
                //_pHProbeSensor2.Dispose();

                _gpioController.ClosePin(GpioInterruptPinNumber);

                _pCF8574A_i2cDevice.WriteByte(0b11111111);

                //Span<PinValuePair> pinValues = stackalloc PinValuePair[8]
                //{
                //    new PinValuePair(0, PinValue.Low),                    
                //    new PinValuePair(1, PinValue.Low),                    
                //    new PinValuePair(2, PinValue.Low),                    
                //    new PinValuePair(3, PinValue.Low),                    
                //    new PinValuePair(4, PinValue.Low),                    
                //    new PinValuePair(5, PinValue.Low),                    
                //    new PinValuePair(6, PinValue.Low),
                //    new PinValuePair(7, PinValue.Low)
                //};

                //_pcf8574.Read(pinValues);

                //Span<PinValuePair> values = stackalloc PinValuePair[1];

                //foreach (PinValuePair item in pinValues)
                //{
                //    try
                //    {
                //        values[0] = new PinValuePair(item.PinNumber, PinValue.Low);
                //        _pcf8574.Write(values);

                //    }
                //    catch { }
                //}

                foreach (KeyValuePair<Guid, I2cDevice> i2cDevice in I2cDevices)
                {
                    try
                    {
                        i2cDevice.Value.Dispose();
                    }
                    catch { }
                }

                try
                {
                    _pcf8574.Dispose();
                }
                catch { }

                try
                {
                    _tca9548a.Dispose();
                }
                catch { }

                try
                {
                    _gpioController.Dispose();
                }
                catch { }

                try
                {
                    _raspberryPiBoard.Dispose();
                }
                catch { }
            }

            // unmanaged
            disposedValue = true;
        }
    }

    //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion

    //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Run(CancellationToken token)
    {
        keepRunning = true;

        TimeSpan delay = new TimeSpan(0, 0, 0, 0, 0, 1);

        (ElectricPotential vcc, double ph, Temperature temperature) values1;
        (ElectricPotential vcc, double ph, Temperature temperature) values2;

        //const int sampleSize = 86;//(int)Math.Round((_pHProbeSensor.DataFrequency * (((double)delay.Seconds)+(((double)delay.Milliseconds)/1000.0)+(((double)delay.Microseconds)/1000000.0))));

        //int displayIndex = 0;
        int displayPH1;
        int displayPH2;
        int displayTemp;
        double averagePH1;
        double averagePH2;
        double averageTemp;

        while (keepRunning)
        {
            if (token.IsCancellationRequested)
            {
                break;
            }

            displayPH1 = 0;
            displayPH2 = 0;
            displayTemp = 0;
            averagePH1 = 0.0;
            averagePH2 = 0.0;
            averageTemp = 0.0;

            //for (int i = 0; i < sampleSize; i++)
            {
                Utilities.Delay(delay);
                values1 = _pHProbeSensor1.GetValues();
                values2 = _pHProbeSensor1.GetValues();

                averagePH1 += MaxMin((values1.ph * 100.0), 0.0, 1400.0);
                averagePH2 += MaxMin((values2.ph * 100.0), 0.0, 1400.0);
                averageTemp += MaxMin((values1.temperature.DegreesFahrenheit), -2000, 200.0);

                displayPH1 += ((int)Math.Round(averagePH1));
                displayPH2 += ((int)Math.Round(averagePH2));
                displayTemp += ((int)Math.Round(averageTemp));
            }

            //_lED4DigitDisplay1.Display(displayAveragePH);
            //_lED4DigitDisplay2.Display(displayAverageTemp);

            //Console.Write($"PH 1 Avg {displayPH1} ");
            //Console.Write($"AIN0 {_pHProbeSensor1.AIN0.Volts:N6} AIN1 {_pHProbeSensor1.AIN1.Volts:N6} ");
            //Console.Write($"PH 2 Avg {displayPH2} ");
            //Console.Write($"AIN0 {_pHProbeSensor2.AIN0.Volts:N6} AIN1 {_pHProbeSensor2.AIN1.Volts:N6} ");
            //Console.WriteLine($"TEMP Avg {displayTemp}");

            Ph1 = _pHProbeSensor1.Ph;
            PhTemperature1 = _pHProbeSensor1.Temperature;

            Ph2 = _pHProbeSensor2.Ph;
            PhTemperature2 = _pHProbeSensor2.Temperature;

            TemperatureHumidity = TemperatureHumiditySensor.Temperature;
            Humidity = TemperatureHumiditySensor.Humidity;

            Pressure = (BarometricPressureSensor.Weight * Gravity) / Area.FromSquareInches(0.03937);

            FlowRate = WaterFlowSensor.FlowRate;
            TotalLitres = WaterFlowSensor.TotalLitres;

            _lED4DigitDisplay1.Display(Ph1);
            _lED4DigitDisplay2.Display(PhTemperature1.DegreesFahrenheit);
            _lED4DigitDisplay3.Display(Ph2);
            _lED4DigitDisplay4.Display(PhTemperature1.DegreesFahrenheit);
        }

        keepRunning = false;

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        double MaxMin(double value, double low, double high)
        {
            return Math.Max(low, Math.Min(value, high));
        }
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

}
