using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
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
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

using Iot.Device.Ads1115;
using Iot.Device.Board;
using Iot.Device.Max7219;

using UnitsNet;
using UnitsNet.Units;

namespace RaspberryPiDevices;

public record struct FlowRateEventData
{
    public VolumeFlow FlowRate;
    public Volume TotalVolume;
}

public sealed class WaterFlowPulseEventArgs : EventArgs
{
    public VolumeFlow FlowRate
    {
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
    }
    public Volume TotalVolume
    {
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
    }

    public WaterFlowPulseEventArgs()
    {
        FlowRate = VolumeFlow.FromLitersPerMinute(0.0);
        TotalVolume = Volume.FromLiters(0.0);
    }

    public WaterFlowPulseEventArgs(VolumeFlow flowRate, Volume totalVolume)
    {
        FlowRate = flowRate;
        TotalVolume = totalVolume;
    }
}

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

    #endregion

    #region Variables
    //private volatile int _pulseCount;

    private bool disposedValue;

    //private readonly GpioController _gpioController;

    private readonly Ads1115 _ads1115;
    //private VolumeFlow _flowRate;
    private Dictionary<long, (VolumeFlow FlowRate, double Time)> _flowRates = new Dictionary<long, (VolumeFlow FlowRate, double Time)>(100000);
    #endregion

    #region Properties
    public ElectricPotential Voltage
    {
        ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        get
        {
            return _ads1115.ReadVoltage();
        }
    }
    public ElectricPotential AIN0
    {
        ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        get
        {
            return _ads1115.ReadVoltage(InputMultiplexer.AIN0);
        }
    }
    public double DataFrequency
    {
        ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        get
        {
            return _ads1115.FrequencyFromDataRate(_ads1115.DataRate);
        }
    }

    //private VolumeFlow _flowRate;
    //public VolumeFlow FlowRate
    //{
    //    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    //    get
    //    {
    //        return _flowRate;
    //    }
    //}

    //private Stopwatch _stopwatch = Stopwatch.StartNew();

    private long _firstTimestamp;
    public long FirstTimestamp
    {
        ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
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
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        get
        {
            return Stopwatch.GetTimestamp();
        }
    }

    public TimeSpan DeltaTimestamp
    {
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        get
        {
            return Stopwatch.GetElapsedTime(_firstTimestamp);
        }
    }

    public long TimestampFrom(TimeSpan span)
    {
        //Console.WriteLine(Utilities.TickFrequency);
        return (long)(((double)span.Ticks / Utilities.TickFrequency) + (double)_firstTimestamp);
    }

    //public Volume TotalVolume
    //{
    //    /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
    //}
    #endregion


    public event EventHandler<WaterFlowPulseEventArgs>? FlowRateChangedEvent;

    /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    //private void OnPulseEvent(WaterFlowPulseEventArgs e)
    //{
    //    PulseEvent?.Invoke(this, e);
    //}

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private void OnFlowRateChanged<T>(T @this, VolumeFlow flowRate, Volume volume) where T : class
    {
        FlowRateChangedEvent?.Invoke(@this, new WaterFlowPulseEventArgs(flowRate, volume));
    }

    private const int gpoiPin = 26;
    private const int i2cAddress = (int)I2cAddress.GND;

    private double _calibrationFactor = 23.0;


    public EventWaitHandle HandleToSignal
    {
        get; private set;
    }
    public EventWaitHandle HandleToWaitOn;



    #region Cctor
    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    //public WaterFlowSensor() //: base(SensorId1, CalibrationName)
    //{
    //    if (CalibrationPoints.Count == 0)
    //    {
    //        CalibrationPoints.Add(Calibration);
    //    }
    //}

    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    public WaterFlowSensor(EventWaitHandle handleToSignal, I2cDevice i2cDevice, GpioController gpioController) //: base(SensorId1, CalibrationName)
    {
        HandleToSignal = handleToSignal;
        HandleToWaitOn = new EventWaitHandle(false, EventResetMode.AutoReset);

        //_autoResetEvent = new AutoResetEvent(false);

        //_ads1115 = new Ads1115(_raspberryPiBoard.CreateI2cDevice(new I2cConnectionSettings(1, (int)I2cAddress.GND)), InputMultiplexer.AIN0, MeasuringRange.FS6144, DataRate.SPS860, DeviceMode.Continuous);

        _ads1115 = new Ads1115(i2cDevice, gpioController,
                               4,
                               shouldDispose: false,
                               InputMultiplexer.AIN0, MeasuringRange.FS6144, DataRate.SPS860, DeviceMode.Continuous);

        _ads1115.ComparatorPolarity = ComparatorPolarity.Low;
        _ads1115.ComparatorLatching = ComparatorLatching.Latching;

        _ads1115.EnableConversionReady();

        _ads1115.EnableComparator(_ads1115.VoltageToRaw(ElectricPotential.FromVolts(1.0)),
                                  _ads1115.VoltageToRaw(ElectricPotential.FromVolts(4.0)),
                                  ComparatorMode.Traditional,
                                  ComparatorQueue.AssertAfterTwo);


        _ads1115.AlertReadyAsserted += OnAlertReadyAsserted;

        Reset();
    }

    /////*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
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
                //_autoResetEvent.Set();

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

    //private bool _FirstPulse = false;

    /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    public void OnAlertReadyAsserted()
    {
        //if (!_FirstPulse)
        //{
        //    Reset();
        //    _FirstPulse = true;
        //}
        //else
        //{

        TimeSpan delta = DeltaTimestamp;
        long timestamp = TimestampFrom(delta);

        //_flowRate = GetFlowRateValue(1, _calibrationFactor);
        //TotalVolume += Volume.FromLiters(_flowRate.LitersPerMinute * GetTimeInMinutes(delta));

        VolumeFlow flowRate = VolumeFlow.FromLitersPerMinute(1.0 / _calibrationFactor);
        double minutes = delta.GetTimeInMinutes();

        _flowRates.Add(timestamp, (flowRate, minutes));

        OnFlowRateChanged(this, flowRate, Volume.FromLiters(flowRate.LitersPerMinute * minutes));

               

        //    ++_pulseCount;
        //}


        //Interlocked.Increment(ref threadCount);

        HandleToSignal.WaitOne();

        //Interlocked.Decrement(ref threadCount);

        HandleToWaitOn.Set();
    }

    /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    public void Reset()
    {
        //_FirstPulse = false;
        //_pulseCount = 0;
        FirstTimestamp = CurrentTimestamp;
        //TotalVolume = Volume.FromLiters(0.0);

        _flowRates.Clear();
        _flowRates.Add(FirstTimestamp, (VolumeFlow.FromLitersPerMinute(0.0), 0));
    }

    /////*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
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

    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
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
    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    private VolumeFlow GetFlowRateValue(double pulseCount, double calibrationFactor)
    {
        if (Math.Abs(pulseCount - 0.0) <= double.Epsilon)
        {
            return VolumeFlow.FromLitersPerMinute(0.0);
        }

        return VolumeFlow.FromLitersPerMinute((pulseCount / calibrationFactor));
    }

    private static TimeSpan deltaTimestampDelay = TimeSpan.FromMilliseconds(100);



    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    //public void Run()
    //{
    //    _delta = DeltaTimestamp;

    //    if (_FirstPulse && (_delta > deltaTimestampDelay))
    //    {
    //        double minutes = GetTimeInMinutes(_delta);

    //        _flowRate = GetFlowRateValue(_pulseCount, _calibrationFactor);
    //        TotalVolume += Volume.FromLiters(_flowRate.LitersPerMinute * minutes);

    //        Pulse(_flowRate, TotalVolume);

    //        //Console.Write($"pulseCount: {_pulseCount} ");

    //        //Console.WriteLine($"{DateTime.Now.ToString("mm:ss:fff")}: {_delta} FlowRate:{_flowRate.LitersPerMinute:N4} TotalVolume:{TotalVolume:N4}");

    //        _pulseCount = 0;
    //        FirstTimestamp = CurrentTimestamp;
    //    }
    //}

    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    //public async Task RunAsync(CancellationToken cancellationToken)
    //{
    //    await Task.Run(() =>
    //    {
    //        _delta = DeltaTimestamp;

    //        if (_FirstPulse && (_delta > deltaTimestampDelay))
    //        {
    //            double minutes = GetTimeInMinutes(_delta);

    //            _flowRate = GetFlowRateValue(_pulseCount, _calibrationFactor);
    //            TotalVolume += Volume.FromLiters(_flowRate.LitersPerMinute * minutes);

    //            //Console.Write($"pulseCount: {_pulseCount} ");

    //            //Console.WriteLine($"{DateTime.Now.ToString("mm:ss:fff")}: {_delta} FlowRate:{_flowRate.LitersPerMinute:N4} TotalVolume:{TotalVolume:N4}");

    //            _pulseCount = 0;
    //            FirstTimestamp = CurrentTimestamp;
    //        }
    //    }).WaitAsync(cancellationToken);
    //}

    ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    //public override string ToString()
    //{
    //    KeyValuePair<long, (VolumeFlow FlowRate, double Time)> record = _flowRates.LastOrDefault();

    //    return $"Flow: {FlowRate.LitersPerMinute:N4}\nTotal: {TotalVolume.Liters:N6}";
    //    //return $"{DateTime.Now.ToLongTimeString()}: FlowRate:{_flowRate.LitersPerMinute:N4} TotalVolume:{TotalVolume:N4}";
    //}
}
