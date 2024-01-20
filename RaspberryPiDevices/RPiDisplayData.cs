using UnitsNet;

namespace RaspberryPiDevices;


public sealed class PhSensorData
{
    public double PH
    {
        get; set;
    }
    public double AveragePH
    {
        get; set;
    }
    public Temperature Temperature
    {
        get; set;
    }
    public double AverageTemperature
    {
        get; set;
    }

    public PhSensorData()
    {
        PH = 0.0;
        AveragePH = 0.0;
        Temperature = Temperature.FromDegreesCelsius(0.0);
        AverageTemperature = 0.0;
    }
}
public sealed class FlowData
{
    public VolumeFlow Rate
    {
        get; set;
    }
    public Volume Total
    {
        get; set;
    }

    public FlowData()
    {
        Rate = VolumeFlow.FromLitersPerMinute(0);
        Total = Volume.FromLiters(0);
    }
}
public sealed class TemperatureHumidityData
{
    public Temperature Temperature
    {
        get; set;
    }
    public RelativeHumidity Humidity
    {
        get; set;
    }

    public TemperatureHumidityData()
    {
        Temperature = Temperature.FromDegreesCelsius(0.0);
        Humidity = RelativeHumidity.From(0.0, UnitsNet.Units.RelativeHumidityUnit.Percent);
    }
}
public sealed class BarometricPressureData
{
    public Pressure Pressure
    {
        get; set;
    }

    public BarometricPressureData()
    {
        Pressure = Pressure.FromPoundsForcePerSquareInch(0);
    }
}


public sealed class RPiDisplayData
{
    public int Index
    {
        get; set;
    }

    public PhSensorData PhSensor1
    {
        get; set;
    }

    public PhSensorData PhSensor2
    {
        get; set;
    }
    public FlowData FlowSensor1
    {
        get; set;
    }

    public TemperatureHumidityData TemperatureHumiditySensor1
    {
        get; set;
    }

    public BarometricPressureData BarometricPressureSensor1
    {
        get; set;
    }

    public RPiDisplayData()
    {
        Index = 0;
        PhSensor1 = new PhSensorData();
        PhSensor2 = new PhSensorData();
        FlowSensor1 = new FlowData();
        TemperatureHumiditySensor1 = new TemperatureHumidityData();
        BarometricPressureSensor1 = new BarometricPressureData();
    }
}


public abstract class RPiSensor
{
}

public sealed class RPiPhSensorValues : RPiSensor
{
    public ElectricPotential Vcc
    {
        get; set;
    }
    public double Ph
    {
        get; set;
    }
    public Temperature Temperature
    {
        get; set;
    }

    public RPiPhSensorValues()
    {
        Vcc = ElectricPotential.FromVolts(0.0);
        Ph = 0.0;
        Temperature = Temperature.FromDegreesCelsius(0.0);
    }

    public RPiPhSensorValues(ElectricPotential vcc, in double ph, Temperature temperature)
    {
        Vcc = vcc;
        Ph = ph;
        Temperature = temperature;
    }
}

public sealed class RPiWaterFlowSensorValues : RPiSensor
{
    public VolumeFlow FlowRate
    {
        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get; set;
    }
    public Volume TotalLitres
    {
        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get; set;
    }

    public RPiWaterFlowSensorValues()
    {
        FlowRate = VolumeFlow.FromLitersPerMinute(0.0);
        TotalLitres = Volume.FromLiters(0.0);
    }

    public RPiWaterFlowSensorValues(VolumeFlow flowRate, Volume totalLitres)
    {
        FlowRate = flowRate;
        TotalLitres = totalLitres;
    }
}

public sealed class RPiBarometricPressureSensorValues : RPiSensor
{
    public Pressure Pressure
    {
        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get; set;
    }

    public RPiBarometricPressureSensorValues()
    {
        Pressure = Pressure.FromPoundsForcePerSquareInch(0.0);
    }
    public RPiBarometricPressureSensorValues(Pressure pressure)
    {
        Pressure = pressure;
    }
}

public sealed class RPiTemperatureHumiditySensorValues : RPiSensor
{
    public Temperature Temperature
    {
        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get; set;
    }
    public RelativeHumidity Humidity
    {
        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get; set;
    }

    public RPiTemperatureHumiditySensorValues()
    {
        Temperature = Temperature.FromDegreesCelsius(0.0);
        Humidity = RelativeHumidity.FromPercent(0.0);
    }
    public RPiTemperatureHumiditySensorValues(Temperature temperatureHumidity, RelativeHumidity humidity)
    {
        Temperature = temperatureHumidity;
        Humidity = humidity;
    }
}

public sealed class RPiSensors
{
    public RPiPhSensorValues Ph1
    {
        get; set;
    }
    public RPiPhSensorValues Ph2
    {
        get; set;
    }
    public RPiWaterFlowSensorValues WaterFlow1
    {
        get; set;
    }
    public RPiTemperatureHumiditySensorValues TemperatureHumidity1
    {
        get; set;
    }
    public RPiBarometricPressureSensorValues BarometricPressure1
    {
        get; set;
    }

    public RPiSensors()
    {
        Ph1 = new RPiPhSensorValues();
        Ph2 = new RPiPhSensorValues();
        WaterFlow1 = new RPiWaterFlowSensorValues();
        TemperatureHumidity1 = new RPiTemperatureHumiditySensorValues();
        BarometricPressure1 = new RPiBarometricPressureSensorValues();
    }

    public RPiSensors(RPiPhSensorValues ph1,
                      RPiPhSensorValues ph2,
                      RPiWaterFlowSensorValues waterFlow1,
                      RPiTemperatureHumiditySensorValues temperatureHumidity1,
                      RPiBarometricPressureSensorValues barometricPressure1)
    {
        Ph1 = ph1;
        Ph2 = ph2;
        WaterFlow1 = waterFlow1;
        TemperatureHumidity1 = temperatureHumidity1;
        BarometricPressure1 = barometricPressure1;
    }
}