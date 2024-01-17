using System;
using System.Collections.Generic;
using System.Device.I2c;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Iot.Device.Ahtxx;
using Iot.Device.Board;

using UnitsNet;

namespace RaspberryPiDevices
{
    /// <summary>
    /// I2C
    /// DC 2.0-5.5V
    /// AHT21
    /// IIC
    /// DHT11
    /// AHT10
    /// </summary>
    public class TemperatureHumiditySensor : IDisposable
    {
        private const int I2cBus = 1;
        private const int I2CPin0 = 3;
        private const int I2CPin1 = 5;

        private readonly Aht20 _sensor;

        private bool disposedValue;

        public Aht20 Sensor
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                return _sensor;
            }
        }

        public Temperature Temperature
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                return _sensor.GetTemperature();
            }
        }

        public RelativeHumidity Humidity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                return _sensor.GetHumidity();
            }
        }

        public TemperatureHumiditySensor(I2cDevice i2cDevice)
        {
            _sensor = new Aht20(i2cDevice);
        }

        #region Dctor
        ~TemperatureHumiditySensor()
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

                    _sensor.Dispose();
                }

                // unmanaged
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        } 
        #endregion


        public override string ToString()
        {
            return $"{DateTime.Now.ToLongTimeString()}: {Temperature.DegreesFahrenheit:N5}°F, {Humidity.Percent:N4}%";
        }

    }
}
