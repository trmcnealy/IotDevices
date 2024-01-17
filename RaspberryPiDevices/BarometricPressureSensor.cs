using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Device.I2c;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Iot.Device.Board;
using Iot.Device.Display;
using Iot.Device.Gpio;
using Iot.Device.Hx711;

using UnitsNet;

using static System.Formats.Asn1.AsnWriter;

namespace RaspberryPiDevices
{

    /// <summary>
    /// 
    /// 3.3V-5V
    /// Pressure: 0-40KPa
    /// </summary>
    public class BarometricPressureSensor : IDisposable
    {
        public const int PinDout = 23;
        public const int PinPdSck = 24;

        private bool disposedValue;

        //private int PD_SCK;    // Power Down and Serial Clock Input Pin
        //private int DOUT;      // Serial Data Output Pin
        //private int _gain;
        //private int READ_TIMES = 10;
        //private int _offset = -540000;  // used for tare weight
        //private double _scale = 1.0;    // used to return weight in grams, kg, ounces, whatever
        //private double RES = 2.98023e-7;

        private readonly Hx711 _sensor;

        //public Mass Tare
        //{
        //    get
        //    {
        //        _sensor.Tare();
        //        return _sensor.TareValue;
        //    }
        //}

        //public Mass Weight
        //{
        //    get
        //    {
        //        return _sensor.GetWeight();
        //    }
        //}

        //// returns pressure in kilopascals
        //public double Pascal
        //{
        //    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        //    get
        //    {
        //        return ((ReadAverage(READ_TIMES) - _offset) * RES) * 20 - 50;
        //    }
        //}

        ////returns pressure in atm
        //public double Atm
        //{
        //    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        //    get
        //    {
        //        return Pascal * 9.86923E-3;
        //    }
        //}

        ////returns pressure in mmHg
        //public double MmHg
        //{
        //    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        //    get
        //    {
        //        return Pascal * 7.50062;

        //    }
        //}

        ////returns pressure in psi
        //public double Psi
        //{
        //    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        //    get
        //    {
        //        return Pascal * 0.145038;
        //    }
        //}


        //// set OFFSET, the value that's subtracted from the actual reading (tare weight)
        //public int Offset
        //{
        //    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        //    get
        //    {
        //        return _offset;
        //    }
        //    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        //    set
        //    {
        //        _offset = value;
        //    }
        //}

        //// set the SCALE value; this value is used to convert the raw data to "human readable" data (measure units)
        //public double Scale
        //{
        //    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        //    get
        //    {
        //        return _scale;
        //    }
        //    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        //    set
        //    {
        //        _scale = value;
        //    }
        //}

        //public double Gain
        //{
        //    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        //    get
        //    {
        //        return _gain;
        //    }
        //    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        //    set
        //    {
        //        switch (value)
        //        {
        //            case 128:       // channel A, gain factor 128
        //            {
        //                _gain = 1;
        //                break;
        //            }
        //            case 64:        // channel A, gain factor 64
        //            {
        //                _gain = 3;
        //                break;
        //            }
        //            case 32:        // channel B, gain factor 32
        //            {
        //                _gain = 2;
        //                break;
        //            }
        //            default:
        //            {
        //                _gain = 0;
        //                break;
        //            }
        //        }
        //    }
        //}

        public Mass Tare
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                _sensor.Tare();
                return _sensor.TareValue;
            }
        }
        public Mass Weight
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                return _sensor.GetWeight();
            }
        }

        #region Cctor
        private readonly RaspberryPiBoard _board;
        
        private readonly int _pinDout;
        private readonly int _pinPdSck;

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public BarometricPressureSensor(RaspberryPiBoard raspberryPiBoard, GpioController gpioController, int pinDout = PinDout, int pinPdSck = PinPdSck)
        {
            _board = raspberryPiBoard;
            _pinDout = pinDout;
            _pinPdSck = pinPdSck;

            //_board.ReservePin(_pinDout, PinUsage.Gpio, _board);
            //_board.ReservePin(_pinPdSck, PinUsage.Gpio, _board);

            _sensor = new Hx711(_pinDout, _pinPdSck, gpioController: gpioController, shouldDispose: false);

            _sensor.PowerUp();

            //_sensor.SetCalibration(Mass.FromGrams(0));
            
            // 1 gram every 107.55 hx711 units
            _sensor.SetConversionRatio(conversionRatio: 107.55);

            ////_sensor.SampleAveraging = 10;
            //PowerUp();
            //_sensor.SetCalibration(Mass.FromGrams(0));
        }

        //public static I2cDevice CreateI2cDevice(RaspberryPiBoard board)
        //{
        //    int[] pins = board.GetDefaultPinAssignmentForI2c(1);
        //    I2cConnectionSettings settings = new I2cConnectionSettings(1, Hx711I2c.GetI2cAddress(pins[0], pins[1]));
        //    I2cDevice device = board.CreateI2cDevice(settings);
        //    return device;
        //}

        #endregion

        #region Dctor
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        ~BarometricPressureSensor()
        {
            Dispose(disposing: false);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                // managed objects
                if (disposing)
                {
                    //_board.ReleasePin(_pinDout, PinUsage.Gpio, _board);
                    //_board.ReleasePin(_pinPdSck, PinUsage.Gpio, _board);
                    _sensor.PowerDown();
                }
                // unmanaged objects
                disposedValue = true;
            }
        }
        #endregion

        public override string ToString()
        {
            return $"{DateTime.Now.ToLongTimeString()}: {Weight.Pounds:N}lb";
        }

        //// Initialize library with data output pin, clock input pin and gain factor.
        //// Channel selection is made by passing the appropriate gain:
        //// - With a gain factor of 64 or 128, channel A is selected
        //// - With a gain factor of 32, channel B is selected
        //// The library default is "128" (Channel A).
        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        //public void Begin(int dout = pinDout, int pd_sck = pinPdSck, int gain = 128)
        //{
        //    PD_SCK = pd_sck;
        //    DOUT = dout;

        //    _controller.OpenPin(PD_SCK, PinMode.Output);
        //    _controller.OpenPin(DOUT, PinMode.InputPullUp);

        //    Gain = gain;

        //    PowerUp();
        //}

        //// Check if HX710B is ready
        //// from the datasheet: When output data is not ready for retrieval, digital output pin DOUT is high. Serial clock
        //// input PD_SCK should be low. When DOUT goes to low, it indicates data is ready for retrieval.
        //public bool IsReady()
        //{
        //    return _controller.Read(DOUT) == PinValue.Low;
        //}

        //// Wait for the HX710B to become ready
        //public void WaitReady(int delay_ms = 0)
        //{
        //    // Wait for the chip to become ready.
        //    // This is a blocking implementation and will
        //    // halt the sketch until a load cell is connected.
        //    while (!IsReady())
        //    {
        //        // Probably will do no harm on AVR but will feed the Watchdog Timer (WDT) on ESP.
        //        // https://github.com/bogde/HX710B/issues/73
        //        Utilities.Delay(delay_ms);
        //    }
        //}
        //public bool WaitReadyRetry(int retries = 3, int delay_ms = 0)
        //{
        //    // Wait for the chip to become ready by
        //    // retrying for a specified amount of attempts.
        //    // https://github.com/bogde/HX710B/issues/76
        //    int count = 0;
        //    while (count < retries)
        //    {
        //        if (IsReady())
        //        {
        //            return true;
        //        }
        //        Utilities.Delay(delay_ms);
        //        count++;
        //    }
        //    return false;
        //}
        //public bool WaitReadyTimeout(int timeout = 1000, int delay_ms = 0)
        //{
        //    // Wait for the chip to become ready until timeout.
        //    // https://github.com/bogde/HX710B/pull/96
        //    int millisStarted = millis();
        //    while (millis() - millisStarted < timeout)
        //    {
        //        if (IsReady())
        //        {
        //            return true;
        //        }
        //        Utilities.Delay(delay_ms);
        //    }
        //    return false;
        //}

        //// waits for the chip to be ready and returns a reading
        //public int Read()
        //{

        //    // Wait for the chip to become ready.
        //    WaitReady();

        //    // Define structures for reading data into.
        //    uint value = 0;
        //    int[] data = { 0, 0, 0 };
        //    int filler = 0x00;

        //    // Protect the read sequence from system interrupts.  If an interrupt occurs during
        //    // the time the PD_SCK signal is high it will stretch the length of the clock pulse.
        //    // If the total pulse time exceeds 60 uSec this will cause the HX710B to enter
        //    // power down mode during the middle of the read sequence.  While the device will
        //    // wake up when PD_SCK goes low again, the reset starts a new conversion cycle which
        //    // forces DOUT high until that cycle is completed.
        //    //
        //    // The result is that all subsequent bits read by shiftIn() will read back as 1,
        //    // corrupting the value returned by read().  The ATOMIC_BLOCK macro disables
        //    // interrupts during the sequence and then restores the interrupt mask to its previous
        //    // state after the sequence completes, insuring that the entire read-and-gain-set
        //    // sequence is not interrupted.  The macro has a few minor advantages over bracketing
        //    // the sequence between `noInterrupts()` and `interrupts()` calls.

        //    // Disable interrupts.
        //    //noInterrupts();


        //    // Pulse the clock pin 24 times to read the data.
        //    data[2] = SHIFTIN_WITH_SPEED_SUPPORT(DOUT, PD_SCK, ByteFormat.Msb);
        //    data[1] = SHIFTIN_WITH_SPEED_SUPPORT(DOUT, PD_SCK, ByteFormat.Msb);
        //    data[0] = SHIFTIN_WITH_SPEED_SUPPORT(DOUT, PD_SCK, ByteFormat.Msb);

        //    // Set the channel and the gain factor for the next reading using the clock pin.
        //    for (uint i = 0; i < _gain; i++)
        //    {
        //        _controller.Write(PD_SCK, PinValue.High);
        //        _controller.Write(PD_SCK, PinValue.Low);
        //    }

        //    //interrupts();


        //    // Replicate the most significant bit to pad out a 32-bit signed integer
        //    if ((data[2] & 0x80) != 0)
        //    {
        //        filler = 0xFF;
        //    }
        //    else
        //    {
        //        filler = 0x00;
        //    }

        //    // Construct a 32-bit signed integer
        //    value = ((static_cast<uint>(filler) << 24)
        //            | (static_cast<uint>(data[2]) << 16)
        //            | (static_cast<uint>(data[1]) << 8)
        //            | (static_cast<uint>(data[0])));

        //    return (int)value;
        //}

        //public int ReadAverage(int times = 10)
        //{
        //    int sum = 0;
        //    for (int i = 0; i < times; i++)
        //    {
        //        sum += Read();
        //        Utilities.Delay(0);
        //    }
        //    return sum / times;
        //}

        //public double GetValue(int times = 1)
        //{
        //    return ReadAverage(times) - _offset;
        //}

        //public void PowerUp()
        //{
        //}

        //public void PowerDown()
        //{
        //}

        //public void Reset()
        //{
        //    PowerDown();
        //    PowerUp();
        //}










        //// returns get_value() divided by SCALE, that is the raw value divided by a value obtained via calibration
        //// times = how many readings to do
        //public double GetUnits(int times = 1)
        //{
        //    return GetValue(times) / _scale;
        //}

        //// set the OFFSET value for tare weight; times = how many times to read the tare value
        //public void Tare(int times = 10)
        //{
        //    int sum = ReadAverage(times);
        //    Offset = sum;
        //}














        #region Utilities

        private static int _startTime = DateTime.Now.Millisecond;


        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static int millis()
        {
            return _startTime - DateTime.Now.Millisecond;
        }


        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        //private int SHIFTIN_WITH_SPEED_SUPPORT(int dataPin, int clockPin, ByteFormat bitOrder)
        //{
        //    return shiftInSlow(dataPin, clockPin, bitOrder);
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        //private int shiftInSlow(int dataPin, int clockPin, ByteFormat bitOrder)
        //{
        //    int value = 0;

        //    for (int i = 0; i < 8; ++i)
        //    {
        //        _controller.Write(clockPin, PinValue.High);

        //        Utilities.DelayMicroseconds(1);

        //        if (bitOrder == ByteFormat.Lsb)
        //        {
        //            value |= ((int)_controller.Read(dataPin)) << i;
        //        }
        //        else
        //        {
        //            value |= ((int)_controller.Read(dataPin)) << (7 - i);
        //        }

        //        _controller.Write(clockPin, PinValue.Low);
        //        Utilities.DelayMicroseconds(1);
        //    }
        //    return value;
        //}


        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static TOut static_cast<TOut>(in sbyte value) where TOut : unmanaged
        {
            return Unsafe.BitCast<sbyte, TOut>(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static TOut static_cast<TOut>(in byte value) where TOut : unmanaged
        {
            return Unsafe.BitCast<int, TOut>(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static TOut static_cast<TOut>(in short value) where TOut : unmanaged
        {
            return Unsafe.BitCast<short, TOut>(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static TOut static_cast<TOut>(in ushort value) where TOut : unmanaged
        {
            return Unsafe.BitCast<ushort, TOut>(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static TOut static_cast<TOut>(in int value) where TOut : unmanaged
        {
            return Unsafe.BitCast<int, TOut>(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static TOut static_cast<TOut>(in uint value) where TOut : unmanaged
        {
            return Unsafe.BitCast<uint, TOut>(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static TOut static_cast<TOut>(in long value) where TOut : unmanaged
        {
            return Unsafe.BitCast<long, TOut>(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static TOut static_cast<TOut>(in ulong value) where TOut : unmanaged
        {
            return Unsafe.BitCast<ulong, TOut>(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static TOut static_cast<TOut>(in float value) where TOut : unmanaged
        {
            return Unsafe.BitCast<float, TOut>(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static TOut static_cast<TOut>(in double value) where TOut : unmanaged
        {
            return Unsafe.BitCast<double, TOut>(value);
        }
        #endregion

    }
}
