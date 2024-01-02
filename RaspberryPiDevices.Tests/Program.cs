using System;
using System.Device;
using System.Device.Gpio;
using System.Device.Gpio.Drivers;
using System.Device.I2c;
using System.Device.Pwm;
using System.Device.Pwm.Drivers;
using System.Device.Spi;
using System.Runtime.InteropServices;
using System.Threading;

using Iot.Device.Adc;
using Iot.Device.Board;
using Iot.Device.Mfrc522;

namespace RaspberryPiDevices.Tests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            IOControllerTests tests = new IOControllerTests();



            tests.TestMethod1();
            tests.InputPullResistorsWork();
            tests.OpenPinDefaultsModeToLastModeIncludingPulls();
            tests.HighPulledPinDoesNotChangeToLowWhenChangedToOutput();
            tests.PinCountReportedCorrectly();
            tests.OpenTwiceGpioPinAndClosedTwiceThrows();
            tests.WriteInputPinDoesNotThrow();
            tests.GpioControllerCreateOpenClosePin();
            tests.IsPinModeSupported();
            tests.GetPinMode();
            tests.UsingBoardNumberingWorks();
            tests.UsingLogicalNumberingDisposesTheRightPin();
            tests.UsingBoardNumberingDisposesTheRightPin();
            tests.CallbackOnEventWorks();
            tests.WriteSpan();
            tests.ReadSpan();
            //public async Task WaitForEventAsyncFail();
            tests.WaitForEventSuccess();
            //private static TValue IsAny<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]TValue>();


            RaspberryPiBoard raspberryPiBoard = new RaspberryPiBoard();
            GpioController gpioController = raspberryPiBoard.CreateGpioController();


            int busId = 0;
            int pinNumber = 0;
            int channel = 0;
            int chip = 1;
            int pwmChannel = 0;
            int i2cAddress = 0;

            SpiConnectionSettings connectionSettings = new SpiConnectionSettings(busId, chip);
            SpiDevice spi = (raspberryPiBoard is RaspberryPiBoard) ? SpiDevice.Create(connectionSettings) : raspberryPiBoard.CreateSpiDevice(connectionSettings);
            bool SpiActivated = raspberryPiBoard.IsSpiActivated();

            int[] pinAssignmentSpi = raspberryPiBoard.GetDefaultPinAssignmentForSpi(connectionSettings);
            int[] OverlayPinAssignmentForSpi = raspberryPiBoard.GetOverlayPinAssignmentForSpi(connectionSettings);

            Assert.AreEqual(9, pinAssignmentSpi[0]);
            Assert.AreEqual(10, pinAssignmentSpi[1]);
            Assert.AreEqual(11, pinAssignmentSpi[2]);



            I2cConnectionSettings settings = new I2cConnectionSettings(raspberryPiBoard.GetDefaultI2cBusNumber(), i2cAddress);
            I2cDevice i2c = raspberryPiBoard.CreateI2cDevice(settings);

            bool I2cActivated = raspberryPiBoard.IsI2cActivated();
            int I2cBusNumber = raspberryPiBoard.GetDefaultI2cBusNumber();
            int[] pinAssignmentI2c = raspberryPiBoard.GetDefaultPinAssignmentForI2c(busId);
            int[] OverlayPinAssignmentForI2c = raspberryPiBoard.GetOverlayPinAssignmentForI2c(busId);

            Assert.AreEqual(0, pinAssignmentI2c[0]);
            Assert.AreEqual(1, pinAssignmentI2c[1]);

            busId = 1;
            pinAssignmentI2c = raspberryPiBoard.GetDefaultPinAssignmentForI2c(busId);
            Assert.AreEqual(2, pinAssignmentI2c[0]);
            Assert.AreEqual(3, pinAssignmentI2c[1]);


            int PinAssignmentForPwm = raspberryPiBoard.GetDefaultPinAssignmentForPwm(chip, channel);
            int OverlayPinAssignmentForPwm = raspberryPiBoard.GetOverlayPinAssignmentForPwm(pwmChannel);

            using (PwmChannel pwm = CreatePwmChannel(300))
            {
                Assert.AreEqual(300, pwm.Frequency);

                pwm.Frequency = 100;
                Assert.AreEqual(100, pwm.Frequency);

                pwm.Frequency = 200;
                Assert.AreEqual(200, pwm.Frequency);

                pwm.Frequency = 500;
                Assert.AreEqual(500, pwm.Frequency);
            }



            PinUsage CurrentPinUsage = raspberryPiBoard.DetermineCurrentPinUsage(pinNumber);

            ComponentInformation QueryComponentInformation = raspberryPiBoard.QueryComponentInformation();


            Console.ReadKey();
        }


        public static PwmChannel CreatePwmChannel(int frequency = 10000, bool stopped = false, double dutyCycle = 0.5)
        {
            PwmChannel pwm;

            try
            {
                pwm = PwmChannel.Create(0, 0, frequency, dutyCycle);
            }
            catch (ArgumentException) when (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                pwm = new SoftwarePwmChannel(18, frequency, dutyCycle, usePrecisionTimer: true);
            }

            if (!stopped)
            {
                pwm.Start();
            }

            return pwm;
        }
    }
}
