using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Iot.Device.Ads1115;
using Iot.Device.Board;
using Iot.Device.Mcp3428;

using UnitsNet;

namespace RaspberryPiDevices;

public record class Ph4502c
{
    public const int DEFAULT_PH_TRIGGER_PIN = 14;

    public const InputMultiplexer DEFAULT_PH_PIN = InputMultiplexer.AIN0;
    public const InputMultiplexer DEFAULT_TEMPERATURE_PIN = InputMultiplexer.AIN1;

    public const int DEFAULT_READING_INTERVAL = 100;
    public const int DEFAULT_READING_COUNT = 10;

    public const double DEFAULT_PH_CALIBRATION = 7;
    public const double DEFAULT_ADC_RESOLUTION = 1024.0;//4096.0;

    //public const double MAX_VOLTAGE = 5.0;
    //public const double MID_VOLTAGE = (5/2);

    public static Func<double,double> VOLTAGE_PER_PH = (double maxVolts)=>(maxVolts / 2) / 14;
    public static Func<double,double> VOLTAGE_PER_TEMPERATURE = (double maxVolts)=>(maxVolts / 2) / 60;

    /// <summary>
    /// The analog pin connected to the pH level sensor.
    /// </summary>
    public InputMultiplexer PhPin;
    /// <summary>
    /// The analog pin connected to the temperature sensor.
    /// </summary>
    public InputMultiplexer TemperaturePin;
    /// <summary>
    /// The interval between pH readings in milliseconds.
    /// </summary>
    public int DelayInterval;
    /// <summary>
    /// The number of readings to average.
    /// </summary>
    public int ReadingCount;
    /// <summary>
    /// The pH calibration value.
    /// </summary>
    public double PhCalibration;
    /// <summary>
    /// The ADC resolution for analog-to-digital conversion.
    /// </summary>
    public double AdcResolution;

    public Ph4502c(InputMultiplexer phPin = DEFAULT_PH_PIN,
                   InputMultiplexer temperaturePin = DEFAULT_TEMPERATURE_PIN,
                   int delayInterval = DEFAULT_READING_INTERVAL,
                   int readingCount = DEFAULT_READING_COUNT,
                   double pHCalibration = DEFAULT_PH_CALIBRATION,
                   double adcResolution = DEFAULT_ADC_RESOLUTION)
    {
        PhPin = phPin;
        TemperaturePin = temperaturePin;
        DelayInterval = delayInterval;
        ReadingCount = readingCount;
        PhCalibration = pHCalibration;
        AdcResolution = adcResolution;
    }

    public override string ToString()
    {
        return $"PhPin:{PhPin} TemperaturePin:{TemperaturePin} DelayInterval:{DelayInterval} ReadingCount:{ReadingCount} PhCalibration:{PhCalibration:N} AdcResolution:{AdcResolution:N}";
    }
}



///// <summary>
///// 
///// </summary>
//public class Ph4502c
//{
//    /// Default calibration value for the PH4502C sensor.
//    private const double PH4502C_DEFAULT_CALIBRATION = 14.8;

//    /// Default reading interval (in milliseconds) between pH readings.
//    private const int PH4502C_DEFAULT_READING_INTERVAL = 100;

//    /// Default number of pH readings to average.
//    private const int PH4502C_DEFAULT_READING_COUNT = 10;

//    /// Default ADC resolution for the PH4502C sensor.
//    private const double PH4502C_DEFAULT_ADC_RESOLUTION = 1024.0;

//    /// Operating voltage for the PH4502C sensor.
//    private const double PH4502C_VOLTAGE = 5.0;

//    /// Voltage that represents a neutral pH reading (pH = 7).
//    private const double PH4502C_MID_VOLTAGE = 2.5;

//    /// Rate of change in voltage per unit change in pH.
//    private const double PH4502C_PH_VOLTAGE_PER_PH = 0.18;

//    private const int GpioPinD0 = 4;

//    private int _ph_level_pin;           ///< The analog pin connected to the pH level sensor.
//    private int _temp_pin;              ///< The analog pin connected to the temperature sensor.
//    private int _reading_interval;           ///< The interval between pH readings in milliseconds.
//    private int _reading_count;              ///< The number of readings to average.
//    private double _calibration;             ///< The pH calibration value.
//    private double _adc_resolution;          ///< The ADC resolution for analog-to-digital conversion.


//    private GpioController _gpioController;


//    public Ph4502c(RaspberryPiBoard raspberryPiBoard,
//                   double calibration = PH4502C_DEFAULT_CALIBRATION,
//                   int reading_interval = PH4502C_DEFAULT_READING_INTERVAL,
//                   int reading_count = PH4502C_DEFAULT_READING_COUNT,
//                   double adc_resolution = PH4502C_DEFAULT_ADC_RESOLUTION)
//    {
//        _calibration = calibration;
//        _reading_interval = reading_interval;

//        _reading_count = reading_count;
//        _adc_resolution = adc_resolution;

//        _gpioController = raspberryPiBoard.CreateGpioController();

//        _gpioController.OpenPin(GpioPinD0, PinMode.Input);


//        //pinMode(_ph_level_pin, INPUT);
//        //pinMode(_temp_pin, INPUT);
//    }

//    public void recalibrate(double calibration)
//    {
//        _calibration = calibration;
//    }

//    public double read_ph_level()
//    {
//        double reading = 0.0f;

//        PinValue pinValue;

//        for (int i = 0; i < _reading_count; i++)
//        {
//            pinValue = _gpioController.Read(GpioPinD0);

//            reading += analogRead(_ph_level_pin);
//            delayMicroseconds(_reading_interval);
//        }

//        reading = PH4502C_VOLTAGE / _adc_resolution * reading;
//        reading /= _reading_count;
//        reading = _calibration + ((PH4502C_MID_VOLTAGE - reading)) / PH4502C_PH_VOLTAGE_PER_PH;

//        return reading;
//    }

//    public double read_ph_level_single()
//    {
//        double reading = 0;//analogRead(_ph_level_pin);

//        reading = PH4502C_VOLTAGE / _adc_resolution * reading;
//        reading /= _reading_count;

//        return _calibration + ((PH4502C_MID_VOLTAGE - reading)) / PH4502C_PH_VOLTAGE_PER_PH;
//    }

//    //public int read_temp()
//    //{
//    //    return analogRead(_temp_pin);
//    //}






//}
