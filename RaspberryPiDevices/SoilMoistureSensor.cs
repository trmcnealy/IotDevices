//using System;
//using System.Collections.Generic;
//using System.Device.Gpio;
//using System.Device.Model;
//using System.Device.Spi;
//using System.Linq;
//using System.Numerics;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;

//using Iot.Device.Board;

//using UnitsNet;

//namespace RaspberryPiDevices
//{
//    public class SoilMoistureSensor
//    {
//        private GpioController _gpioController;
//        private GpioPin _gpioPin;
//        private int[] _pins;

//        private int _busid = 0;

//        public int MISOPinId
//        {
//            get
//            {
//                return _pins[0];
//            }
//        }

//        public int MOSIPinId
//        {
//            get
//            {
//                return _pins[1];
//            }
//        }

//        public int ClockPinId
//        {
//            get
//            {
//                return _pins[2];
//            }
//        }


//        public SoilMoistureSensor(RaspberryPiBoard raspberryPiBoard, GpioController gpioController)
//        {
//            _gpioController = gpioController;

//            _pins = raspberryPiBoard.GetOverlayPinAssignmentForSpi();

//            raspberryPiBoard.ReservePin(ClockPinId, PinUsage.Gpio, this);
            
//            _gpioPin = _gpioController.OpenPin(ClockPinId, PinMode.Input);
//        }

//        ~SoilMoistureSensor()
//        {
//            _gpioController.ClosePin(ClockPinId);
//        }

//        public override string ToString()
//        {
//            //return $"{_gpioController.GetPinMode(_pinId)}";
//            return $"{_gpioPin.Read()}";
//        }


//    }
//}
