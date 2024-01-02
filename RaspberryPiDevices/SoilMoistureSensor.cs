using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Device.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspberryPiDevices
{
    public class SoilMoistureSensor : IDisposable
    {
        private readonly int _dataPin;
        private readonly PinValue _liquidPresentPinState;
        private readonly bool _shouldDispose;

        private GpioController _controller;

        [Telemetry]
        public bool IsMoisturePresent()
        {
            return _controller.Read(_dataPin) == _liquidPresentPinState;
        }

        public SoilMoistureSensor(int dataPin,
                                 PinValue liquidPresentPinState,
                                 GpioController? gpioController = null,
                                 PinNumberingScheme pinNumberingScheme = PinNumberingScheme.Logical,
                                 bool shouldDispose = true)
        {
            _controller = gpioController ?? new GpioController(pinNumberingScheme);
            _shouldDispose = shouldDispose || gpioController is null;
            _dataPin = dataPin;
            _liquidPresentPinState = liquidPresentPinState;

            _controller.OpenPin(_dataPin, PinMode.Input);

        }

        public void Dispose()
        {
            if (_shouldDispose)
            {
                _controller?.Dispose();
                _controller = null!;
            }
        }
    }
}
