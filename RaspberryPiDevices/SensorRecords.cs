using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using UnitsNet;

namespace RaspberryPiDevices;

public abstract record class SensorRecord
{
}

public record class PhSensorRecord : SensorRecord
{
    public ElectricPotential Vcc;
    public double Ph;
    public Temperature Temperature;
    
    /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    public PhSensorRecord()
    {
        Vcc = ElectricPotential.FromVolts(0.0);
        Ph = 0.0;
        Temperature = Temperature.FromDegreesCelsius(0.0);
    }
    
    /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    public PhSensorRecord(ElectricPotential vcc, double ph, Temperature temperature)
    {
        Vcc = vcc;
        Ph = ph;
        Temperature = temperature;
    }
}

public record class WaterFlowSensorRecord : SensorRecord
{
    public VolumeFlow FlowRate;
    public Volume TotalLitres;
    
    /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    public WaterFlowSensorRecord()
    {
        FlowRate = VolumeFlow.FromLitersPerMinute(0.0);
        TotalLitres = Volume.FromLiters(0.0);
    }
    /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    public WaterFlowSensorRecord(VolumeFlow flowRate, Volume totalLitres)
    {
        FlowRate = flowRate;
        TotalLitres = totalLitres;
    }
}

public record class BarometricPressureSensorRecord : SensorRecord
{
    public Pressure Pressure;
    
    /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    public BarometricPressureSensorRecord()
    {
        Pressure = Pressure.FromKilopoundsForcePerSquareInch(0.0);
    }
    
    /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    public BarometricPressureSensorRecord(Pressure pressure)
    {
        Pressure = pressure;
    }
}

public record class TemperatureHumiditySensorRecord : SensorRecord
{
    public Temperature Temperature;
    public RelativeHumidity Humidity;
    
    /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    public TemperatureHumiditySensorRecord()
    {
        Temperature = Temperature.FromDegreesCelsius(0.0);
        Humidity = RelativeHumidity.FromPercent(0.0);
    }
    /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    public TemperatureHumiditySensorRecord(Temperature temperature, RelativeHumidity humidity)
    {
        Temperature = temperature;
        Humidity = humidity;
    }
}

public record class SensorRecords
{
    private Dictionary<Guid, Dictionary<DateTime, SensorRecord>> _data;

    private Dictionary<Guid, Dictionary<DateTime, SensorRecord>> Data
    {/*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        get
        {
            return _data;
        }
    }
    /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    public SensorRecords(int sensors)
    {
        _data = new Dictionary<Guid, Dictionary<DateTime, SensorRecord>>(sensors);
    }
    /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    public void AddDevice(Guid uid, int allowNumberOfEntries = 10000)
    {
        Data.Add(uid, new Dictionary<DateTime, SensorRecord>(allowNumberOfEntries));
    }
    /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    public bool Add<TRecord>(Guid uid, DateTime timestamp, TRecord record) where TRecord : SensorRecord
    {
        return Data[uid].TryAdd(timestamp, record);
    }
    /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    public bool Add<TRecord>(Guid uid, TRecord record) where TRecord : SensorRecord
    {
        return Data[uid].TryAdd(DateTime.Now, record);
    }
    /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    public bool Get<TRecord>(Guid uid, DateTime timestamp, out TRecord? record) where TRecord : SensorRecord
    {
        if (Data[uid].TryGetValue(timestamp, out SensorRecord? base_record))
        {
            record = (TRecord?)base_record;
            return true;
        }

        record = null;
        return false;
    }
}
