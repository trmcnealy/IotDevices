using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Device.I2c;
using System.Device.Spi;
using System.Diagnostics;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

using Iot.Device.Ads1115;
using Iot.Device.Board;
using Iot.Device.Max7219;

using UnitsNet;
using UnitsNet.Units;


namespace RaspberryPiDevices
{
    //Heating voltage: 5±0.2V (AC -• DC)
    //Working current: 5-10mA
    //The detection concentration range: PH0-14
    //The detection range of temperature: 0-60 centigrade
    //The response time: ≤ 5S
    //Stability time: ≤ 60S
    //Power consumption: ≤ 0.5W
    //The working temperature: -10~50 centigrade (the nominal temperature 20 centigrade)
    //Working humidity: 95%RH (nominal humidity 65%RH)
    //Service life: 3 years
    //Size: 42mm x 32mm x 20mm
    //Weight: 25g
    //The output: analog voltage signal output

    /// <summary>
    /// PH4502C - Potentiometer
    /// ADS1115 A2D
    /// TO – Temperature output
    /// DO – 3.3V pH limit trigger
    /// PO – PH analog output
    /// Gnd – Gnd for PH probe
    /// Gnd – Gnd for board
    /// VCC – 5V DC
    /// POT 1 – Analog reading offset (Nearest to BNC connector)
    /// POT 2 – PH limit setting
    /// </summary>
    public class PHProbeSensor : IDisposable
    {
        public const double PH1MidVoltage = 2.618975761;

        public const string PHCalibrationName = "PH";
        public const string TemperatureCalibrationName = "TEMPERATURE";

        //private static double pH_y_intercept = 20.852173913043476;
        //private static double ph_slope = -5.217391304347824;

        //private static double temperature_y_intercept = 60.0;
        //private static double temperature_slope = -12.0;

        public static readonly TimeSpan SensorResponseTime = new TimeSpan(0, 0, 5);
        public static readonly TimeSpan SensorStabilityTime = new TimeSpan(0, 0, 60);

        private const int I2cBus = 1;

        private bool disposedValue;

        //private const int I2CPin0 = 3;
        //private const int I2CPin1 = 5;
        //private const int I2CPin2 = 7;

        //private const int RST_PIN = 18;
        //private const int CS_PIN = 22;
        //private const int DRDY_PIN = 17;

        //private readonly RaspberryPiBoard _board;
        //private readonly I2cDevice _i2cDevice;
        //private readonly GpioController _gpioController;

        private readonly Ads1115? _ads1115;
        private readonly Ph4502c _ph4502c;

        private static readonly VoltageRange voltageLimits = new VoltageRange(0.0, 5.0);


        ///// <summary>
        ///// Compensate for temperature diff between readings and calibration.
        ///// pH/(V*T). V is in volts, and T is in °C
        ///// </summary>
        //private static double PH_Compensation_Coeff = -0.05694;

        //pH 7 = 3.75V
        //4 = 1.46V

        //private static Func<double, double> _phVoltageSlope = Ph4502c.VOLTAGE_PER_PH;//0.357142857142857;//((5-0)/(14-0));//0.3571428571428570000; //-0.17333333333333333333333333333333;//GetPhVoltageSlope(2.5, 4, 3.89, 7);
        //private static Func<double, double> _phTemperatureSlope = Ph4502c.VOLTAGE_PER_TEMPERATURE;//0.083333333333333;//((5-0)/(60-0));//-0.0065;

        //private static readonly (ElectricPotential voltage, double ph, Temperature temperature) CaliPoint_pH4 = new(ElectricPotential.FromVolts(1.46),
        //                                                                                                                                                            4,
        //                                                                                                                                                            new Temperature(40, TemperatureUnit.DegreeCelsius));

        //private static readonly (ElectricPotential voltage, double ph, Temperature temperature) CaliPoint_pH7 = new(ElectricPotential.FromVolts(3.75),
        //                                                                                                                                                            7,
        //                                                                                                                                                            new Temperature(40, TemperatureUnit.DegreeCelsius));

        private readonly Calibration PHCalibration;
        private readonly Calibration TemperatureCalibration;

        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        static PHProbeSensor()
        {
        }

        #region Properties
        public ElectricPotential Voltage
        {
            //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                if (_ads1115 is not null)
                {
                    return _ads1115.ReadVoltage();
                }
                return ElectricPotential.FromVolts(0);
            }
        }
        public ElectricPotential AIN0
        {
            //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                if (_ads1115 is not null)
                {
                    return _ads1115.ReadVoltage(InputMultiplexer.AIN0);
                }
                return ElectricPotential.FromVolts(0);
            }
        }
        public ElectricPotential AIN1
        {
            //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                if (_ads1115 is not null)
                {
                    return _ads1115.ReadVoltage(InputMultiplexer.AIN1);
                }
                return ElectricPotential.FromVolts(0);
            }
        }
        public ElectricPotential AIN2
        {
            //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                if (_ads1115 is not null)
                {
                    return _ads1115.ReadVoltage(InputMultiplexer.AIN2);
                }
                return ElectricPotential.FromVolts(0);
            }
        }
        public ElectricPotential AIN3
        {
            //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                if (_ads1115 is not null)
                {
                    return _ads1115.ReadVoltage(InputMultiplexer.AIN3);
                }
                return ElectricPotential.FromVolts(0);
            }
        }
        public ElectricPotential AIN0_AIN1
        {
            //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                if (_ads1115 is not null)
                {
                    return _ads1115.ReadVoltage(InputMultiplexer.AIN0_AIN1);
                }
                return ElectricPotential.FromVolts(0);
            }
        }
        public ElectricPotential AIN0_AIN3
        {
            //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                if (_ads1115 is not null)
                {
                    return _ads1115.ReadVoltage(InputMultiplexer.AIN0_AIN3);
                }
                return ElectricPotential.FromVolts(0);
            }
        }
        public ElectricPotential AIN1_AIN3
        {
            //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                if (_ads1115 is not null)
                {
                    return _ads1115.ReadVoltage(InputMultiplexer.AIN1_AIN3);
                }
                return ElectricPotential.FromVolts(0);
            }
        }
        public ElectricPotential AIN2_AIN3
        {
            //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                if (_ads1115 is not null)
                {
                    return _ads1115.ReadVoltage(InputMultiplexer.AIN2_AIN3);
                }
                return ElectricPotential.FromVolts(0);
            }
        }
        public ElectricPotential MaximumVoltage
        {
            //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                return ElectricPotential.FromVolts(5);//_ads1115?.MaxVoltageFromMeasuringRange(_ads1115?.MeasuringRange);
            }
        }
        public double DataFrequency
        {
            //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                if (_ads1115 is not null)
                {
                    return _ads1115.FrequencyFromDataRate(_ads1115.DataRate);
                }
                return 0.0;
            }
        }

        private double _ph;
        public double Ph
        {
            //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                return _ph;
                //double val = GetSensorValue(_ph4502c, _ads1115, InputMultiplexer.AIN0, _ads1115?.MeasuringRange, _phVoltageSlope, _ph4502c.PhCalibration);

                //if (!double.IsNormal(val))
                //{
                //    Console.WriteLine($"Temperature is invalid");
                //    return 0;
                //}

                //return GetPhValue(AIN0);
                //return GetPhValue(CheckVoltage(AIN0.Volts));
            }
        }

        private Temperature _temperature;

        public Temperature Temperature
        {
            //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                return _temperature;
                //return GetAverageTemperature();
                //return new Temperature(GetTemperatureValue(AIN1), TemperatureUnit.DegreeCelsius);

                //double val = GetSensorValue(_ph4502c, _ads1115, InputMultiplexer.AIN1, _ads1115?.MeasuringRange, _phTemperatureSlope);

                //if (!double.IsNormal(val))
                //{
                //    Console.WriteLine($"Temperature is invalid");
                //    return new Temperature(0, TemperatureUnit.DegreeCelsius);
                //}

                //return new Temperature(val, TemperatureUnit.DegreeCelsius);

                //return new Temperature(GetAverageValue(_ads1115, InputMultiplexer.AIN1, SensorResponseTime), TemperatureUnit.DegreeCelsius);
                //return new Temperature(GetTemperatureValue(CheckVoltage(AIN1.Volts)), TemperatureUnit.DegreeCelsius);
                //return new Temperature(GetTemperatureVoltage(this), TemperatureUnit.DegreeFahrenheit);                
            }
        }
        #endregion

        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        //internal PHProbeSensor() : base(GetPHProbeSensorUid(1), GetPHProbeSensorName(1))
        //{
        //    if (CalibrationPoints.Count == 0)
        //    {
        //        CalibrationPoints.Add(PHCalibration);
        //        CalibrationPoints.Add(TemperatureCalibration);
        //    }
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public PHProbeSensor(I2cDevice i2cDevice)//, int selectPhProbe = 1) //: base(GetPHProbeSensorUid(selectPhProbe), GetPHProbeSensorName(selectPhProbe))
        {
            PHCalibration = new Calibration(PHCalibrationName);
            PHCalibration.Points.Add(new CalibrationPoint() { X = 2.677648649, Y = 7 });
            PHCalibration.Points.Add(new CalibrationPoint() { X = 3.189777778, Y = 4 });

            TemperatureCalibration = new Calibration(TemperatureCalibrationName);
            TemperatureCalibration.Points.Add(new CalibrationPoint() { X = 0.0, Y = 66.7 });
            TemperatureCalibration.Points.Add(new CalibrationPoint() { X = 5.0, Y = 6.7 });
            //TemperatureCalibration.Points.Add(new CalibrationPoint() { X = 3.87, Y = 21.1 });

            //_board = raspberryPiBoard;

            //_board.DetermineCurrentPinUsage(0)

            //_i2cDevice = i2cDevice;//raspberryPiBoard.CreateI2cDevice(new I2cConnectionSettings(I2cBus, (int)I2cAddress.GND));
            //_gpioController = gpioController;
            //_spiDevice = raspberryPiBoard.CreateSpiDevice(new SpiConnectionSettings(0));



            //_board.ReservePin(I2CPin0, PinUsage.I2c, _board);
            //_board.ReservePin(I2CPin1, PinUsage.I2c, _board);
            //_board.ReservePin(I2CPin2, PinUsage.I2c, _board);

            //I2cConnectionSettings settings = new(I2cBus, (int)I2cAddress.GND);

            //I2cDevice device = I2cDevice.Create(settings);

            _ads1115 = new Ads1115(i2cDevice, InputMultiplexer.AIN0, MeasuringRange.FS6144, DataRate.SPS860, DeviceMode.Continuous);
            //_ads1115 = new Ads1115(i2cDevice, gpioController, 0, shouldDispose: false, InputMultiplexer.AIN0, MeasuringRange.FS6144, DataRate.SPS860, DeviceMode.Continuous);



            //_ads1115 = new Ads1115(_i2cDevice, _gpioController, 23, false, InputMultiplexer.AIN0, MeasuringRange.FS6144, DataRate.SPS128, DeviceMode.Continuous);

            //_ads1115?.AlertReadyAsserted += () =>
            //{
            //    //Console.WriteLine($"AlertReadyAsserted {_ads1115?.ReadVoltage()}");
            //};

            //_ads1115?.EnableConversionReady();

            _ph4502c = new Ph4502c();

            //Console.WriteLine(_ph4502c);

            //Console.WriteLine($"SensorDelay {GetSensorDelay(_ads1115).Microseconds}");

            //Console.WriteLine($"SensorDataRate {GetSensorDataRate(_ads1115)}");

            //Console.WriteLine($"MaximumVoltage {MaximumVoltage}");

            //Console.WriteLine($"ReadVoltage {Voltage}");

            //Console.WriteLine($"FrequencyFromDataRate {_ads1115?.FrequencyFromDataRate(_ads1115?.DataRate)}");

            //(ElectricPotential lower, ElectricPotential upper) voltageRange = VoltageRange(_ads1115?.MeasuringRange);
            //Console.WriteLine($"VoltageRange {voltageRange.lower} {voltageRange.upper}");

            //if (CalibrationPoints.Count == 0)
            //{
            //    CalibrationPoints.Add(PHCalibration);
            //    CalibrationPoints.Add(TemperatureCalibration);
            //}
        }

        #region Dctor
        ~PHProbeSensor()
        {
            Dispose(disposing: false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // managed
                    //_board.ReleasePin(I2CPin0, PinUsage.I2c, _board);
                    //_board.ReleasePin(I2CPin1, PinUsage.I2c, _board);
                    //_board.ReleasePin(I2CPin2, PinUsage.I2c, _board);

                    //_gpioController.Dispose();
                    //_i2cDevice.Dispose();
                    _ads1115?.Dispose();
                    //_board.Dispose();
                }

                // unmanaged
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion




        //public void Calibrate(double Ph)
        //{
        //    Calibration calibration = CalibrationPoints.First(o => o.Name == PHCalibrationName);

        //    return ((calibration.Slope * voltage.Volts) + calibration.Intercept);


        //    //Linear(IEnumerable<Point> enumerable_points)





        //}






        private static (ElectricPotential Vcc, ElectricPotential Ain0, ElectricPotential Ain1) voltageValues = new(ElectricPotential.FromVolts(0),
                                                                                                                                                                          ElectricPotential.FromVolts(0),
                                                                                                                                                                          ElectricPotential.FromVolts(0));

        //private static (ElectricPotential Vcc, double Ph, Temperature Temperature) values;

        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public (ElectricPotential vcc, double ph, Temperature temperature) GetValues()
        {
            try
            {
                voltageValues = GetVoltages();

                _ph = GetPhValue(voltageValues.Ain0);
                _temperature = new Temperature(GetTemperatureValue(voltageValues.Ain1), TemperatureUnit.DegreeCelsius);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e}");
            }

            return new(voltageValues.Vcc, _ph, _temperature);
        }


        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public (ElectricPotential Vcc, ElectricPotential Ain0, ElectricPotential Ain1) GetVoltages()
        {
            if (_ads1115 is not null)
            {
                double vcc = _ads1115.ReadVoltage().Volts;
                double ain0 = _ads1115.ReadVoltage(InputMultiplexer.AIN0).Volts;
                double ain1 = _ads1115.ReadVoltage(InputMultiplexer.AIN1).Volts;

                return new(ElectricPotential.FromVolts(vcc), ElectricPotential.FromVolts(ain0), ElectricPotential.FromVolts(ain1));
            }
            return new(ElectricPotential.FromVolts(0), ElectricPotential.FromVolts(0), ElectricPotential.FromVolts(0));
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private double GetPhValue(ElectricPotential voltage)
        {
            //Calibration calibration = PHCalibration.Points.First(o => o.Name == PHCalibrationName);

            return ((PHCalibration.Slope * voltage.Volts) + PHCalibration.Intercept);

            //return ((voltage.Volts * maxValue) / maxVolts);
            //return (((voltage.Value - 0) * maxValue) / maxVolts) + 0;

            //Console.WriteLine($"voltage{voltage}");

            //return calibration + ((voltage.Volts - Ph4502c.MID_VOLTAGE) / _voltageSlope);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private double GetTemperatureValue(ElectricPotential voltage)
        {
            //Calibration calibration = CalibrationPoints.First(o => o.Name == TemperatureCalibrationName);

            return ((TemperatureCalibration.Slope * voltage.Volts) + TemperatureCalibration.Intercept);
        }


        //private void TemperatureCompensation(double pH_0, ElectricPotential V, Temperature T, Temperature T_cal_0)
        //{
        //    double T_coeff = -0.057 * (pH_0 / (V.Volts * T.DegreesCelsius));

        //    double T_diff = T.DegreesCelsius - T_cal_0.DegreesCelsius;

        //    double pH = A * PH_Compensation_Coeff * T_diff * V + pH_0;
        //​
        //        }



        //public (ElectricPotential Vcc, double Ph, Temperature Temperature) GetAverageValues()
        //{
        //    (ElectricPotential vcc, ElectricPotential ain0, ElectricPotential ain1) averageVoltages = GetAverageVoltages();

        //    _ph = GetPhValue(averageVoltages.ain0);
        //    _temperature = new Temperature(GetTemperatureValue(averageVoltages.ain1), TemperatureUnit.DegreeCelsius);

        //    return new(averageVoltages.vcc, _ph, _temperature);
        //}

        ////[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        //public (ElectricPotential Vcc, ElectricPotential Ain0, ElectricPotential Ain1) GetAverageVoltages()
        //{
        //    TimeSpan span = new TimeSpan(0, 0, 0, 0, 100);
        //    TimeSpan delay = new TimeSpan(0, 0, 0, 0, 1, 162);

        //    int length = ((int)_ads1115?.FrequencyFromDataRate(_ads1115?.DataRate));


        //    Console.WriteLine($"length{length}");

        //    double vcc = 0.0;
        //    double ain0 = 0.0;
        //    double ain1 = 0.0;

        //    for (int sample = 0; sample < length; ++sample)
        //    {
        //        vcc += _ads1115?.ReadVoltage().Volts ?? 0.0;
        //        ain0 += _ads1115?.ReadVoltage(InputMultiplexer.AIN0).Volts ?? 0.0;
        //        ain1 += _ads1115?.ReadVoltage(InputMultiplexer.AIN1).Volts ?? 0.0;
        //        Utilities.Delay(delay);
        //    }

        //    Console.WriteLine($"vcc{vcc}");
        //    Console.WriteLine($"ain0{ain0}");
        //    Console.WriteLine($"ain1{ain1}");

        //    vcc /= length;
        //    ain0 /= length;
        //    ain1 /= length;

        //    return new(ElectricPotential.FromVolts(vcc), ElectricPotential.FromVolts(ain0), ElectricPotential.FromVolts(ain1));
        //}



        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override string ToString()
        {
            return $"{DateTime.Now.ToLongTimeString()}: Voltage:{Voltage:N6} AIN0:{AIN0:N6} AIN1:{AIN1:N6} Ph:{_ph:N4} Temperature:{_temperature.DegreesFahrenheit:N5}°F";
            //return $"{DateTime.Now.ToLongTimeString()}: Voltage:{averageVoltages.vcc:N} AIN0:{AIN0:N} AIN1:{AIN1:N} AIN2:{AIN2:N} AIN3:{AIN3:N} Ph:{averageVoltages.ph:N} Temperature:{averageVoltages.temperature.DegreesFahrenheit:N}°F";
        }

        ////[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        //public ElectricPotential ReadVoltage(InputMultiplexer inputMultiplexer)
        //{
        //    short raw = _ads1115?.ReadRaw(inputMultiplexer);

        //    return _ads1115?.RawToVoltage(raw);
        //}

        ////[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        //public static ElectricPotential ReadVoltage(Ph4502c ph4502c, Ads1115 ads1115, InputMultiplexer input)
        //{
        //    short raw = ads1115.ReadRaw(input);

        //    return RawToVoltage(ph4502c, ads1115.MeasuringRange, raw);
        //}

        ////[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        //private static ElectricPotential RawToVoltage(Ph4502c ph4502c, MeasuringRange measuringRange, short val)
        //{
        //    double maxVoltage = MaximumVoltage(measuringRange);

        //    return ElectricPotential.FromVolts(val * (maxVoltage / ph4502c.AdcResolution));
        //}


        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static (ElectricPotential lower, ElectricPotential upper) VoltageRange(MeasuringRange measuringRange)
        {
            double voltage = measuringRange switch
            {
                MeasuringRange.FS6144 => 6.144,
                MeasuringRange.FS4096 => 4.096,
                MeasuringRange.FS2048 => 2.048,
                MeasuringRange.FS1024 => 1.024,
                MeasuringRange.FS0512 => 0.512,
                MeasuringRange.FS0256 => 0.256,
                _ => throw new ArgumentOutOfRangeException(nameof(measuringRange), "Unknown measuring range used")
            };

            return new(ElectricPotential.FromVolts(-voltage), ElectricPotential.FromVolts(voltage));
        }

        ////[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        //public static double MaximumVoltage(MeasuringRange measuringRange)
        //{
        //    return measuringRange switch
        //    {
        //        MeasuringRange.FS6144 => 6.144,
        //        MeasuringRange.FS4096 => 4.096,
        //        MeasuringRange.FS2048 => 2.048,
        //        MeasuringRange.FS1024 => 1.024,
        //        MeasuringRange.FS0512 => 0.512,
        //        MeasuringRange.FS0256 => 0.256,
        //        _ => throw new ArgumentOutOfRangeException(nameof(measuringRange), "Unknown measuring range used")
        //    };

        //    //return ElectricPotential.FromVolts(voltage);
        //}

        ////[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        //private static double GetAverageValue(Ads1115 ads1115, InputMultiplexer input, TimeSpan time)
        //{
        //    int length = (time.Seconds * GetSensorDataRate(ads1115));

        //    TimeSpan span = GetSensorDelay(ads1115);

        //    double buf = 0.0;

        //    for (int i = 0; i < length; i++)
        //    {
        //        buf += ads1115.ReadVoltage(input).Volts;
        //        SensorDelay(span);
        //    }

        //    return (buf / length);
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static int GetSensorDataRate(Ads1115 ads1115)
        {
            switch (ads1115.DataRate)
            {
                case DataRate.SPS008:
                    return 8;
                case DataRate.SPS016:
                    return 16;
                case DataRate.SPS032:
                    return 32;
                case DataRate.SPS064:
                    return 64;
                case DataRate.SPS128:
                    return 128;
                case DataRate.SPS250:
                    return 250;
                case DataRate.SPS475:
                    return 475;
                case DataRate.SPS860:
                    return 860;
            }
            return 860;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static TimeSpan GetSensorDelay(Ads1115 ads1115)
        {
            switch (ads1115.DataRate)
            {
                case DataRate.SPS008:
                    return new TimeSpan(0, 0, 0, 124);
                case DataRate.SPS016:
                    return new TimeSpan(0, 0, 0, 0, 61500);
                case DataRate.SPS032:
                    return new TimeSpan(0, 0, 0, 0, 30250);
                case DataRate.SPS064:
                    return new TimeSpan(0, 0, 0, 0, 14625);
                case DataRate.SPS128:
                    return new TimeSpan(0, 0, 0, 0, 6813);
                case DataRate.SPS250:
                    return new TimeSpan(0, 0, 0, 3);
                case DataRate.SPS475:
                    return new TimeSpan(0, 0, 0, 0, 1105);
                case DataRate.SPS860:
                    return new TimeSpan(0, 0, 0, 0, 163);
                default:
                    return new TimeSpan(0, 0, 0, 0, 163);
            };
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static void SensorDelay(Ads1115 ads1115)
        {
            //int dataRate = GetSensorDataRate(ads1115);            
            Utilities.Delay(GetSensorDelay(ads1115), false);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static void SensorDelay(TimeSpan span)
        {
            //int dataRate = GetSensorDataRate(ads1115);            
            Utilities.Delay(span, false);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static double CheckVoltage(double voltage)
        {
            if (voltageLimits != voltage)
            {
                Console.WriteLine($"CheckVoltage ({voltage} > 0 && {voltage} < 5)");
            }
            return voltageLimits.MaxMin(voltage);
        }

        ////[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        //private static double GetPhValue(double voltage, double max_voltage)
        //{
        //    return (voltage / _phVoltageSlope(max_voltage));
        //    //return (MidValue.ph + ((MidValue.voltage - AIN0.Volts) / _phVoltageSlope));
        //}

        ////[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        //private static double GetTemperatureValue(double voltage, double max_voltage)
        //{
        //    return (voltage / _phTemperatureSlope(max_voltage)) - 21.38889;
        //}

        ////[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        //private static double GetPhVoltage(PHProbeSensor pHProbeSensor)
        //{
        //    using (Ads1115 adc = new Ads1115(pHProbeSensor._i2cDevice, InputMultiplexer.AIN0))
        //    {
        //        return pHProbeSensor.Voltage.Volts;
        //    }
        //}

        ////[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        //private static double GetTemperatureVoltage(PHProbeSensor pHProbeSensor)
        //{
        //    using (Ads1115 adc = new Ads1115(pHProbeSensor._i2cDevice, InputMultiplexer.AIN1))
        //    {
        //        return pHProbeSensor.Voltage.Volts;
        //    }
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static double GetPhVoltageSlope(double vp0_voltage, double vp0_ph, double vp1_voltage, double vp1_ph)
        {
            return (vp1_voltage - vp0_voltage) / (vp1_ph - vp0_ph);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static double GetPhVoltageSlope((double voltage, double ph) vp0, (double voltage, double ph) vp1)
        {
            return (vp1.voltage - vp0.voltage) / (vp1.ph - vp0.ph);
        }


    }
}
