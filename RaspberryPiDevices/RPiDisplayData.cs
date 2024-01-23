//using UnitsNet;

//namespace RaspberryPiDevices;

//public sealed class PhSensorData
//{
//    public double PH
//    {
//        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
//    }
//    public double AveragePH
//    {
//        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
//    }
//    public Temperature Temperature
//    {
//        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
//    }
//    public double AverageTemperature
//    {
//        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
//    }

//    public PhSensorData()
//    {
//        PH = 0.0;
//        AveragePH = 0.0;
//        Temperature = Temperature.FromDegreesCelsius(0.0);
//        AverageTemperature = 0.0;
//    }
//}

//public sealed class FlowData
//{
//    public VolumeFlow Rate
//    {
//        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
//    }
//    public Volume Total
//    {
//        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
//    }

//    public FlowData()
//    {
//        Rate = VolumeFlow.FromLitersPerMinute(0);
//        Total = Volume.FromLiters(0);
//    }
//}

//public sealed class TemperatureHumidityData
//{
//    public Temperature Temperature
//    {
//        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
//    }
//    public RelativeHumidity Humidity
//    {
//        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
//    }

//    public TemperatureHumidityData()
//    {
//        Temperature = Temperature.FromDegreesCelsius(0.0);
//        Humidity = RelativeHumidity.From(0.0, UnitsNet.Units.RelativeHumidityUnit.Percent);
//    }
//}

//public sealed class BarometricPressureData
//{
//    public Pressure Pressure
//    {
//        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
//    }

//    public BarometricPressureData()
//    {
//        Pressure = Pressure.FromPoundsForcePerSquareInch(0);
//    }
//}

//public sealed class RPiDisplayData
//{
//    public int Index
//    {
//        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
//    }

//    public PhSensorData PhSensor1
//    {
//        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
//    }

//    public PhSensorData PhSensor2
//    {
//        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
//    }
//    public FlowData FlowSensor1
//    {
//        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
//    }

//    public TemperatureHumidityData TemperatureHumiditySensor1
//    {
//        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
//    }

//    public BarometricPressureData BarometricPressureSensor1
//    {
//        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
//    }

//    public RPiDisplayData()
//    {
//        Index = 0;
//        PhSensor1 = new PhSensorData();
//        PhSensor2 = new PhSensorData();
//        FlowSensor1 = new FlowData();
//        TemperatureHumiditySensor1 = new TemperatureHumidityData();
//        BarometricPressureSensor1 = new BarometricPressureData();
//    }
//}

//public abstract class RPiSensor
//{
//}

//public sealed class RPiPhSensorValues : RPiSensor
//{
//    public ElectricPotential Vcc
//    {
//        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
//    }
//    public double Ph
//    {
//        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
//    }
//    public Temperature Temperature
//    {
//        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
//    }

//    public RPiPhSensorValues()
//    {
//        Vcc = ElectricPotential.FromVolts(0.0);
//        Ph = 0.0;
//        Temperature = Temperature.FromDegreesCelsius(0.0);
//    }

//    public RPiPhSensorValues(ElectricPotential vcc, in double ph, Temperature temperature)
//    {
//        Vcc = vcc;
//        Ph = ph;
//        Temperature = temperature;
//    }
//}

//public sealed class RPiWaterFlowSensorValues : RPiSensor
//{
//    public VolumeFlow FlowRate
//    {
//        ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
//        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
//    }
//    public Volume TotalLitres
//    {
//        ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
//        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
//    }

//    public RPiWaterFlowSensorValues()
//    {
//        FlowRate = VolumeFlow.FromLitersPerMinute(0.0);
//        TotalLitres = Volume.FromLiters(0.0);
//    }

//    public RPiWaterFlowSensorValues(VolumeFlow flowRate, Volume totalLitres)
//    {
//        FlowRate = flowRate;
//        TotalLitres = totalLitres;
//    }
//}

//public sealed class RPiBarometricPressureSensorValues : RPiSensor
//{
//    public Pressure Pressure
//    {
//        ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
//        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
//    }

//    public RPiBarometricPressureSensorValues()
//    {
//        Pressure = Pressure.FromPoundsForcePerSquareInch(0.0);
//    }
//    public RPiBarometricPressureSensorValues(Pressure pressure)
//    {
//        Pressure = pressure;
//    }
//}

//public sealed class RPiTemperatureHumiditySensorValues : RPiSensor
//{
//    public Temperature Temperature
//    {
//        ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
//        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
//    }
//    public RelativeHumidity Humidity
//    {
//        ///*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
//        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
//    }

//    public RPiTemperatureHumiditySensorValues()
//    {
//        Temperature = Temperature.FromDegreesCelsius(0.0);
//        Humidity = RelativeHumidity.FromPercent(0.0);
//    }
//    public RPiTemperatureHumiditySensorValues(Temperature temperatureHumidity, RelativeHumidity humidity)
//    {
//        Temperature = temperatureHumidity;
//        Humidity = humidity;
//    }
//}

//public sealed class RPiSensors
//{
//    public RPiPhSensorValues Ph1
//    {
//        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
//    }
//    public RPiPhSensorValues Ph2
//    {
//        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
//    }
//    public RPiWaterFlowSensorValues WaterFlow1
//    {
//        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
//    }
//    public RPiTemperatureHumiditySensorValues TemperatureHumidity1
//    {
//        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
//    }
//    public RPiBarometricPressureSensorValues BarometricPressure1
//    {
//        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
//    }

//    public RPiSensors()
//    {
//        Ph1 = new RPiPhSensorValues();
//        Ph2 = new RPiPhSensorValues();
//        WaterFlow1 = new RPiWaterFlowSensorValues();
//        TemperatureHumidity1 = new RPiTemperatureHumiditySensorValues();
//        BarometricPressure1 = new RPiBarometricPressureSensorValues();
//    }

//    public RPiSensors(RPiPhSensorValues ph1,
//                      RPiPhSensorValues ph2,
//                      RPiWaterFlowSensorValues waterFlow1,
//                      RPiTemperatureHumiditySensorValues temperatureHumidity1,
//                      RPiBarometricPressureSensorValues barometricPressure1)
//    {
//        Ph1 = ph1;
//        Ph2 = ph2;
//        WaterFlow1 = waterFlow1;
//        TemperatureHumidity1 = temperatureHumidity1;
//        BarometricPressure1 = barometricPressure1;
//    }
//}
