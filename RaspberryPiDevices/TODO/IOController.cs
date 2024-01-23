using System.Device;
using System.Device.Gpio;
using System.Device.Gpio.Drivers;
using System.Device.I2c;
using System.Device.Pwm;
using System.Device.Spi;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;


using Iot.Device.HardwareMonitor;
using Iot.Device.Pn532.RfConfiguration;

using Microsoft.Win32;


namespace RaspberryPiDevices
{
    public class IOController : IDisposable
    {
        private bool disposedValue;

        public int Pin
        {
            /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
        }

        public GpioController Controller
        {
            /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
        }

        #region Ctor
        public IOController(int pin, PinNumberingScheme numberingScheme = PinNumberingScheme.Logical)
        {
            Pin = pin;

            //OpenHardwareMonitor hw = new OpenHardwareMonitor();
                    
            //Driver = new SysFsDriver();
            //Driver = new LibGpiodDriver();
            //Driver = new RaspberryPi3Driver();

            Controller = new GpioController(numberingScheme);
        }
        
        public IOController(int pin, GpioDriver driver, PinNumberingScheme numberingScheme = PinNumberingScheme.Logical)
        {
            Pin = pin;

            Controller = new GpioController(numberingScheme, driver);
        }
        
        public IOController(int pin, GpioController controller)
        {
            Pin = pin;

            Controller = controller;
        }

        public static IOController New(int pin, PinNumberingScheme numberingScheme = PinNumberingScheme.Logical)
        {
            return new IOController(pin, numberingScheme);
        }
        
        public static IOController New(int pin, GpioDriver driver, PinNumberingScheme numberingScheme = PinNumberingScheme.Logical)
        {
            return new IOController(pin, driver, numberingScheme);
        }

        public static IOController New(int pin, GpioController controller)
        {
            return new IOController(pin, controller);
        }
        #endregion

        #region Dtor
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                ///managed objects
                if (disposing)
                {
                }

                /// unmanaged objects
                disposedValue = true;
            }
        }

        ~IOController()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Gpio
        public int PinCount
        {
            get
            {
                return Controller.PinCount;
            }
        }

        public PinNumberingScheme NumberingScheme
        {
            get
            {
                return Controller.NumberingScheme;
            }
        }

        //private IEnumerable<GpioPin> OpenPins
        //{
        //    get
        //    {
        //        return _gpioPins.Values;
        //    }
        //}
        
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        public bool IsPinOpen(int pinNumber)
        {
            return Controller.IsPinOpen(pinNumber);
        }
        
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        public void OpenPin(int pinNumber)
        {
            Controller.OpenPin(pinNumber);
        }
        
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        public GpioPin OpenPin(int pinNumber, PinMode mode)
        {
            return Controller.OpenPin(pinNumber, mode);
        }
        
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        public GpioPin OpenPin(int pinNumber, PinMode mode, PinValue initialValue)
        {
            return Controller.OpenPin(pinNumber, mode, initialValue);
        }

        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        public void ClosePin(int pinNumber)
        {
            Controller.ClosePin(pinNumber);
        }
        
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        public void SetPinMode(int pinNumber, PinMode mode)
        {
            Controller.SetPinMode(pinNumber, mode);
        }
        
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        public PinMode GetPinMode(int pinNumber)
        {
            return Controller.GetPinMode(pinNumber);
        }
        
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        public bool IsPinModeSupported(int pinNumber, PinMode mode)
        {
            return Controller.IsPinModeSupported(pinNumber, mode);
        }
        
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        public PinValue Read(int pinNumber)
        {
            return Controller.Read(pinNumber);
        }
        
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        public void Read(Span<PinValuePair> pinValuePairs)
        {
            Controller.Read(pinValuePairs);
        }
        
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        public void Write(int pinNumber, PinValue value)
        {
            Controller.Write(pinNumber, value);
        }
        
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        public void Write(ReadOnlySpan<PinValuePair> pinValuePairs)
        {
            Controller.Write(pinValuePairs);
        }
                
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        public WaitForEventResult WaitForEvent(int pinNumber, PinEventTypes eventTypes, TimeSpan timeout)
        {
            return Controller.WaitForEvent(pinNumber, eventTypes, timeout);
        }
        
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        public WaitForEventResult WaitForEvent(int pinNumber, PinEventTypes eventTypes, CancellationToken cancellationToken)
        {
            return Controller.WaitForEvent(pinNumber, eventTypes, cancellationToken);
        }
        
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        public ValueTask<WaitForEventResult> WaitForEventAsync(int pinNumber, PinEventTypes eventTypes, CancellationToken cancellationToken)
        {
            return Controller.WaitForEventAsync(pinNumber, eventTypes, cancellationToken);
        }
        
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        public async ValueTask<WaitForEventResult> WaitForEventAsync(int pinNumber, PinEventTypes eventTypes, TimeSpan timeout)
        {
            return await Controller.WaitForEventAsync(pinNumber, eventTypes, timeout);
        }
               
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        public void Toggle(int pinNumber)
        {
            Controller.Toggle(pinNumber);
        }
        
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        public void RegisterCallbackForPinValueChangedEvent(int pinNumber, PinEventTypes eventTypes, PinChangeEventHandler callback)
        {
            Controller.RegisterCallbackForPinValueChangedEvent(pinNumber, eventTypes, callback);
        }
        
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        public void UnregisterCallbackForPinValueChangedEvent(int pinNumber, PinChangeEventHandler callback)
        {
            Controller.UnregisterCallbackForPinValueChangedEvent(pinNumber, callback);
        }
        
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        public ComponentInformation QueryComponentInformation()
        {
            return Controller.QueryComponentInformation();
        }
        #endregion

    }
}
