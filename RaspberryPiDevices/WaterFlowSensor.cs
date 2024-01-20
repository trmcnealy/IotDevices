using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Device.Gpio;
using System.Device.I2c;
using System.Device.Spi;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

using Iot.Device.Ads1115;
using Iot.Device.Board;
using Iot.Device.Max7219;

using UnitsNet;
using UnitsNet.Units;

namespace RaspberryPiDevices;


public class WaterFlowSensor : ISensor<WaterFlowSensor>
{
    #region static

    public const string CalibrationName = "WaterFlow";
    //private static readonly Calibration Calibration;

    //private static (ElectricPotential Vcc, ElectricPotential Ain0) voltageValues = new(ElectricPotential.FromVolts(0), ElectricPotential.FromVolts(0));

    //private StringBuilder output;

    //public static Guid Uid
    //{
    //    get;
    //}
    //public static string? Name
    //{
    //    get;
    //}

    static WaterFlowSensor()
    {
        //Uid = SensorId1;
        //Name = SensorName1;

        //output = new StringBuilder();

        //Calibration = new Calibration(CalibrationName);
        //Calibration.Points.Add(new CalibrationPoint() { X = 0, Y = 10 });
        //Calibration.Points.Add(new CalibrationPoint() { X = 5, Y = 0 });
    }

    private const long TicksPerSecond = TimeSpan.TicksPerSecond;
    private const long TicksPerMillisecond = TimeSpan.TicksPerMillisecond;
    private const long TicksPerMicrosecond = TimeSpan.TicksPerMillisecond / 1000;
    #endregion

    #region Variables
    private volatile int _pulseCount;

    private bool disposedValue;

    //private readonly GpioController _gpioController;

    private readonly Ads1115 _ads1115;
    private VolumeFlow _flowRate;
    #endregion

    #region Properties
    public ElectricPotential Voltage
    {
        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return _ads1115.ReadVoltage();
        }
    }
    public ElectricPotential AIN0
    {
        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return _ads1115.ReadVoltage(InputMultiplexer.AIN0);
        }
    }
    public double DataFrequency
    {
        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return _ads1115.FrequencyFromDataRate(_ads1115.DataRate);
        }
    }

    //private VolumeFlow _flowRate;
    public VolumeFlow FlowRate
    {
        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return _flowRate;
        }
    }

    //private Stopwatch _stopwatch = Stopwatch.StartNew();

    private static readonly double s_tickFrequency = ((double)TimeSpan.TicksPerSecond) / ((double)Stopwatch.Frequency);

    private long _firstTimestamp;
    public long FirstTimestamp
    {
        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return _firstTimestamp;
        }
        private set
        {
            _firstTimestamp = value;
        }
    }

    public long CurrentTimestamp
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return Stopwatch.GetTimestamp();
        }
    }

    public TimeSpan DeltaTimestamp
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return Stopwatch.GetElapsedTime(_firstTimestamp);
        }
    }

    public Volume TotalLitres
    {
        get; set;
    }
    #endregion


    private const int gpoiPin = 26;
    private const int i2cAddress = (int)I2cAddress.GND;


    private double _calibrationFactor = 23.0;

    #region Cctor
    //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    //public WaterFlowSensor() //: base(SensorId1, CalibrationName)
    //{
    //    if (CalibrationPoints.Count == 0)
    //    {
    //        CalibrationPoints.Add(Calibration);
    //    }
    //}


    //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public WaterFlowSensor(I2cDevice i2cDevice, GpioController gpioController) //: base(SensorId1, CalibrationName)
    {
        //_ads1115 = new Ads1115(_raspberryPiBoard.CreateI2cDevice(new I2cConnectionSettings(1, (int)I2cAddress.GND)), InputMultiplexer.AIN0, MeasuringRange.FS6144, DataRate.SPS860, DeviceMode.Continuous);

        _ads1115 = new Ads1115(i2cDevice, gpioController,
                               4,
                               shouldDispose: false,
                               InputMultiplexer.AIN0, MeasuringRange.FS6144, DataRate.SPS860, DeviceMode.Continuous);

        _ads1115.ComparatorPolarity = ComparatorPolarity.Low;
        _ads1115.ComparatorLatching = ComparatorLatching.Latching;

        _ads1115.AlertReadyAsserted += OnAlertReadyAsserted;

        _ads1115.EnableConversionReady();

        _ads1115.EnableComparator(_ads1115.VoltageToRaw(ElectricPotential.FromVolts(1.0)),
                                  _ads1115.VoltageToRaw(ElectricPotential.FromVolts(4.0)),
                                  ComparatorMode.Traditional,
                                  ComparatorQueue.AssertAfterTwo);
    }

    ////[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    //public WaterFlowSensor(RaspberryPiBoard _raspberryPiBoard) //: base(SensorId1, CalibrationName)
    //{
    //    //_ads1115 = new Ads1115(_raspberryPiBoard.CreateI2cDevice(new I2cConnectionSettings(1, (int)I2cAddress.GND)), InputMultiplexer.AIN0, MeasuringRange.FS6144, DataRate.SPS860, DeviceMode.Continuous);

    //    _ads1115 = new Ads1115(_raspberryPiBoard.CreateI2cDevice(new I2cConnectionSettings(1, (int)I2cAddress.GND)),
    //                           _raspberryPiBoard.CreateGpioController(),
    //                           4,
    //                           shouldDispose: false,
    //                           InputMultiplexer.AIN0,
    //                           MeasuringRange.FS6144,
    //                           DataRate.SPS860,
    //                           DeviceMode.Continuous);

    //    _ads1115.ComparatorPolarity = ComparatorPolarity.Low;
    //    _ads1115.ComparatorLatching = ComparatorLatching.Latching;

    //    _ads1115.AlertReadyAsserted += OnAlertReadyAsserted;

    //    _ads1115.EnableConversionReady();

    //    _ads1115.EnableComparator(_ads1115.VoltageToRaw(ElectricPotential.FromVolts(1.0)),
    //                              _ads1115.VoltageToRaw(ElectricPotential.FromVolts(4.0)),
    //                              ComparatorMode.Traditional,
    //                              ComparatorQueue.AssertAfterTwo);
    //}
    #endregion

    #region Dctor
    ~WaterFlowSensor()
    {
        Dispose(disposing: false);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            ///managed
            if (disposing)
            {
                _ads1115.Dispose();
                //_gpioController.ClosePin(gpoiPin);
            }

            /// unmanaged
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion

    //public void OnPinValueRising(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
    //{
    //    //pulseCount++;
    //}
    //public void OnPinValueFalling(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
    //{
    //    pulseCount++;
    //}

    private bool _FirstPulse = false;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void OnAlertReadyAsserted()
    {
        if (!_FirstPulse)
        {
            Reset();
            _FirstPulse = true;
        }
        else
        {
            ++_pulseCount;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Reset()
    {
        _FirstPulse = false;
        _pulseCount = 0;
        FirstTimestamp = CurrentTimestamp;
        TotalLitres = Volume.FromLiters(0.0);
    }

    ////[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    //public VolumeFlow GetValues(double pulseCount, double calibrationFactor)
    //{
    //    try
    //    {
    //        //voltageValues = GetVoltages();

    //        _flowRate = GetFlowRateValue(pulseCount, calibrationFactor);
    //    }
    //    catch (Exception e)
    //    {
    //        Console.WriteLine($"Error: {e}");
    //    }

    //    return _flowRate;
    //}

    //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    //public (ElectricPotential Vcc, ElectricPotential Ain0) GetVoltages()
    //{
    //    double vcc = _ads1115.ReadVoltage().Volts;
    //    double ain0 = _ads1115.ReadVoltage(InputMultiplexer.AIN0).Volts;

    //    return new(ElectricPotential.FromVolts(vcc), ElectricPotential.FromVolts(ain0));
    //}

    /// <summary>
    /// L/min
    /// </summary>
    /// <param name="voltage"></param>
    /// <returns></returns>
    //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private VolumeFlow GetFlowRateValue(double pulseCount, double calibrationFactor)
    {
        if (Math.Abs(pulseCount - 0.0) <= double.Epsilon)
        {
            return VolumeFlow.FromLitersPerMinute(0.0);
        }

        return VolumeFlow.FromLitersPerMinute((pulseCount / calibrationFactor));
    }

    private static TimeSpan deltaTimestampDelay = TimeSpan.FromMilliseconds(100);

    //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static double GetTimeInSeconds(TimeSpan time)
    {
        double value = 0.0;

        if (time.Seconds > 0)
        {
            value += ((double)time.Seconds);
        }
        if (time.Milliseconds > 0)
        {
            value += (((double)time.Milliseconds) / 1_000.0);
        }
        if (time.Microseconds > 0)
        {
            value += (((double)time.Microseconds) / 1_000_000.0);
        }
        return value;
    }

    //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static double GetTimeInMinutes(TimeSpan time)
    {
        double value = 0.0;

        if (time.Seconds > 0)
        {
            value += ((double)time.Minutes);
        }
        if (time.Seconds > 0)
        {
            value += (((double)time.Seconds) / 60.0);
        }
        if (time.Milliseconds > 0)
        {
            value += (((double)time.Milliseconds) / 60_000.0);
        }
        if (time.Microseconds > 0)
        {
            value += (((double)time.Microseconds) / 60_000_000.0);
        }
        return value;
    }

    private TimeSpan _delta;

    //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Run()
    {
        _delta = DeltaTimestamp;

        if (_FirstPulse && (_delta > deltaTimestampDelay))
        {
            double minutes = GetTimeInMinutes(_delta);

            _flowRate = GetFlowRateValue(_pulseCount, _calibrationFactor);
            TotalLitres += Volume.FromLiters(_flowRate.LitersPerMinute * minutes);

            //Console.Write($"pulseCount: {_pulseCount} ");

            //Console.WriteLine($"{DateTime.Now.ToString("mm:ss:fff")}: {_delta} FlowRate:{_flowRate.LitersPerMinute:N4} TotalLitres:{TotalLitres:N4}");

            _pulseCount = 0;
            FirstTimestamp = CurrentTimestamp;
        }
    }

    //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override string ToString()
    {
        return $"{DateTime.Now.ToLongTimeString()}: FlowRate:{_flowRate.LitersPerMinute:N4} TotalLitres:{TotalLitres:N4}";
    }
}
