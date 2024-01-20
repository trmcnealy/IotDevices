using UnitsNet;

namespace RaspberryPiDevices;


public class PhSensorData
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
public class FlowData
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

public class TemperatureHumidityData
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

public class BarometricPressureData
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


public class RPiDisplayData
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

public class RPiPhSensorValues
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

    public RPiPhSensorValues(ElectricPotential vcc, double ph, Temperature temperature)
    {
        Vcc = vcc;
        Ph = ph;
        Temperature = temperature;
    }
}