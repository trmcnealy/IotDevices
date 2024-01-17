using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Device.Gpio.Drivers;
using System.IO;
using System.Linq;
using System.Threading;
using System.Text;
using System.Threading.Tasks;


namespace RaspberryPiDevices
{
    public class IOControllerTests
    {
        private const int OpenPin = 1;
        private const int LedPin = 5;
        private const int OutputPin = 5;
        private const int InputPin = 6;

        private readonly IOController iOController;

        public IOControllerTests()
        {
            iOController = IOController.New(OpenPin);
        }

        private bool IsRaspi4()
        {
            if (File.Exists("/proc/device-tree/model"))
            {
                string model = File.ReadAllText("/proc/device-tree/model", Encoding.ASCII);

                if (model.Contains("Raspberry Pi 4") || model.Contains("Raspberry Pi Compute Module 4"))
                {
                    return true;
                }
            }

            return false;
        }

        public void InputPullResistorsWork()
        {
            iOController.Controller.OpenPin(OpenPin, PinMode.InputPullUp);
            //Assert.Equal(PinValue.High, iOController.Controller.Read(OpenPin));

            for (int i = 0; i < 100; i++)
            {
                iOController.Controller.SetPinMode(OpenPin, PinMode.InputPullDown);
                //Assert.Equal(PinValue.Low, iOController.Controller.Read(OpenPin));

                iOController.Controller.SetPinMode(OpenPin, PinMode.InputPullUp);
                //Assert.Equal(PinValue.High, iOController.Controller.Read(OpenPin));
            }

            // change one more time so that when running test in a loop we start with the inverted option
            iOController.Controller.SetPinMode(OpenPin, PinMode.InputPullDown);
            //Assert.Equal(PinValue.Low, iOController.Controller.Read(OpenPin));
        }

        public void OpenPinDefaultsModeToLastModeIncludingPulls()
        {
                iOController.Controller.OpenPin(OutputPin);
                iOController.Controller.SetPinMode(OutputPin, PinMode.InputPullDown);
                iOController.Controller.ClosePin(OutputPin);
                iOController.Controller.OpenPin(OutputPin);

                if (IsRaspi4())
                {
                    //Assert.Equal(PinMode.InputPullDown, iOController.Controller.GetPinMode(OutputPin));
                }
                else
                {
                    //Assert.Equal(PinMode.Input, iOController.Controller.GetPinMode(OutputPin));
                }
            
        }

        public void HighPulledPinDoesNotChangeToLowWhenChangedToOutput()
        {
                //bool didTriggerToLow = false;
                int testPin = OutputPin;
                // Set value to low prior to test, so that we have a defined start situation
                iOController.Controller.OpenPin(testPin, PinMode.Output);
                iOController.Controller.Write(testPin, PinValue.Low);
                iOController.Controller.ClosePin(testPin);
                // For this test, we use the input pin as an external pull-up
                iOController.Controller.OpenPin(InputPin, PinMode.Output);
                iOController.Controller.Write(InputPin, PinValue.High);
                Thread.Sleep(2);
                // If we were to use InputPullup here, this would work around the problem it seems, but it would also make our test pass under almost all situations
                iOController.Controller.OpenPin(testPin, PinMode.Input);
                Thread.Sleep(50);
                iOController.Controller.RegisterCallbackForPinValueChangedEvent(testPin, PinEventTypes.Falling, (sender, args) =>
                {
                    //if (args.ChangeType == PinEventTypes.Falling)
                    //{
                    //    didTriggerToLow = true;
                    //}
                });

                iOController.Controller.Write(testPin, PinValue.High);
                iOController.Controller.SetPinMode(testPin, PinMode.Output);
                Thread.Sleep(50);
                //Assert.False(didTriggerToLow);

                iOController.Controller.ClosePin(OutputPin);
                iOController.Controller.ClosePin(InputPin);
            
        }

        public void PinCountReportedCorrectly()
        {
            ////var iOController = new GpioController(PinNumberingScheme.Logical, iOController.Driver);
            //Assert.Equal(28, iOController.PinCount);
        }

        public void OpenTwiceGpioPinAndClosedTwiceThrows()
        {
            ////var iOController = new GpioController(PinNumberingScheme.Logical, iOController.Driver);
            iOController.Controller.OpenPin(1);
            iOController.Controller.ClosePin(1);
            GpioPin gpioPin1 = iOController.OpenPin(1, PinMode.Input);
            GpioPin gpiopin2 = iOController.OpenPin(1, PinMode.Input);
            //Assert.Equal(gpioPin1, gpiopin2);
            iOController.ClosePin(1);
            //Assert.Throws<InvalidOperationException>(() => iOController.ClosePin(1);
        }

        public void WriteInputPinDoesNotThrow()
        {
            iOController.Controller.OpenPin(1);
            //iOController.Controller.IsPinModeSupported(1, It.IsAny<PinMode>().Returns(true));
            //iOController.Controller.SetPinMode(1, It.IsAny<PinMode>());
            iOController.Controller.GetPinMode(1);//.Returns(PinMode.Input);
            ////var iOController = new GpioController(PinNumberingScheme.Logical, iOController.Driver);

            iOController.OpenPin(1, PinMode.Input);
            iOController.Write(1, PinValue.High);
        }

        public void GpioControllerCreateOpenClosePin()
        {
            iOController.Controller.OpenPin(1);
            iOController.Controller.IsPinModeSupported(1, PinMode.Output);//.Returns(true);
            iOController.Controller.GetPinMode(1);//.Returns(PinMode.Output);
            iOController.Controller.Write(1, PinValue.High);
            iOController.Controller.ClosePin(1);
            ////var iOController = new GpioController(PinNumberingScheme.Logical, iOController.Driver);
            //Assert.NotNull(iOController);
            iOController.OpenPin(1, PinMode.Output);
            //Assert.True(iOController.IsPinOpen(1);
            iOController.Write(1, PinValue.High);
            iOController.ClosePin(1);
            //Assert.False(iOController.IsPinOpen(1);
        }

        public void IsPinModeSupported()
        {
            iOController.Controller.IsPinModeSupported(1, PinMode.Input);//.Returns(true);
            ////var iOController = new GpioController(PinNumberingScheme.Logical, iOController.Driver);
            //Assert.NotNull(iOController);
            //Assert.True(iOController.IsPinModeSupported(1, PinMode.Input);
        }

        public void GetPinMode()
        {
            iOController.Controller.OpenPin(1);
            iOController.Controller.GetPinMode(1);//.Returns(PinMode.Output);
            ////var iOController = new GpioController(PinNumberingScheme.Logical, iOController.Driver);
            //Assert.NotNull(iOController);
            // Not open
            //Assert.Throws<InvalidOperationException>(() => iOController.GetPinMode(1);
            iOController.OpenPin(1);
            //Assert.Equal(PinMode.Output, iOController.GetPinMode(1);
        }

        public void UsingBoardNumberingWorks()
        {
            // Our mock driver maps physical pin 2 to logical pin 1
            //iOController.Controller.ConvertPinNumberToLogicalNumberingScheme(2).Returns(1);
            iOController.Controller.OpenPin(1);
            iOController.Controller.SetPinMode(1, PinMode.Output);
            iOController.Controller.IsPinModeSupported(1, PinMode.Output);//.Returns(true);
            iOController.Controller.GetPinMode(1);//.Returns(PinMode.Output);
            iOController.Controller.Write(1, PinValue.High);
            iOController.Controller.Read(1);//.Returns(PinValue.High);
            iOController.Controller.ClosePin(1);
            //var iOController = new GpioController(PinNumberingScheme.Board, iOController.Driver);
            iOController.OpenPin(2, PinMode.Output);
            iOController.Write(2, PinValue.High);
            //Assert.Equal(PinValue.High, iOController.Read(2);
            iOController.ClosePin(2);
            iOController.Dispose();
        }

        public void UsingLogicalNumberingDisposesTheRightPin()
        {
            iOController.Controller.OpenPin(1);
            iOController.Controller.ClosePin(1);
            iOController.Controller.IsPinModeSupported(1, PinMode.Output);//.Returns(true);
            iOController.Controller.GetPinMode(1);//.Returns(PinMode.Output);
            //var iOController = new GpioController(PinNumberingScheme.Logical, iOController.Driver);
            iOController.OpenPin(1, PinMode.Output);
            iOController.Write(1, PinValue.High);
            // No close on the pin here, we want to check that the Controller's Dispose works correctly
            iOController.Dispose();
        }

        public void UsingBoardNumberingDisposesTheRightPin()
        {
            // Our mock driver maps physical pin 2 to logical pin 1
            //iOController.Controller.ConvertPinNumberToLogicalNumberingScheme(2).Returns(1);
            iOController.Controller.OpenPin(1);
            iOController.Controller.SetPinMode(1, PinMode.Output);
            iOController.Controller.ClosePin(1);
            iOController.Controller.IsPinModeSupported(1, PinMode.Output);//.Returns(true);
            //var iOController = new GpioController(PinNumberingScheme.Board, iOController.Driver);
            iOController.OpenPin(2, PinMode.Output);
            // No close on the pin here, we want to check that the Controller's Dispose works correctly
            iOController.Dispose();
        }

        public void CallbackOnEventWorks()
        {
            // Our mock driver maps physical pin 2 to logical pin 1
            iOController.Controller.OpenPin(1);
            //iOController.Controller.AddCallbackForPinValueChangedEvent(1, PinEventTypes.Rising, It.IsAny<PinChangeEventHandler>());
            //var iOController = new GpioController(PinNumberingScheme.Logical, iOController.Driver);
            iOController.OpenPin(1); // logical pin 1 on our test board
            //bool callbackSeen = false;
            PinChangeEventHandler eventHandler = (sender, args) =>
            {
                //callbackSeen = true;
                //Assert.Equal(1, args.PinNumber);
                //Assert.Equal(PinEventTypes.Falling, args.ChangeType);
            };

            iOController.RegisterCallbackForPinValueChangedEvent(1, PinEventTypes.Rising, eventHandler);

            //iOController.Driver.FireEventHandler(1, PinEventTypes.Falling);

            //Assert.True(callbackSeen);

            iOController.UnregisterCallbackForPinValueChangedEvent(1, eventHandler);
        }

        public void WriteSpan()
        {
            iOController.Controller.OpenPin(1);
            iOController.Controller.OpenPin(2);
            iOController.Controller.IsPinModeSupported(1, PinMode.Output);//.Returns(true);
            iOController.Controller.IsPinModeSupported(2, PinMode.Output);//.Returns(true);
            iOController.Controller.GetPinMode(1);//.Returns(PinMode.Output);
            iOController.Controller.GetPinMode(2);//.Returns(PinMode.Output);
            iOController.Controller.Write(1, PinValue.High);
            iOController.Controller.Write(2, PinValue.Low);
            iOController.Controller.ClosePin(1);
            iOController.Controller.ClosePin(2);
            //var iOController = new GpioController(PinNumberingScheme.Logical, iOController.Driver);
            //Assert.NotNull(iOController);
            iOController.OpenPin(1, PinMode.Output);
            iOController.OpenPin(2, PinMode.Output);
            //Assert.True(iOController.IsPinOpen(1);
            Span<PinValuePair> towrite = stackalloc PinValuePair[2];
            towrite[0] = new PinValuePair(1, PinValue.High);
            towrite[1] = new PinValuePair(2, PinValue.Low);
            iOController.Write(towrite);
            iOController.ClosePin(1);
            iOController.ClosePin(2);
            //Assert.False(iOController.IsPinOpen(1);
        }

        public void ReadSpan()
        {
            iOController.Controller.OpenPin(1);
            iOController.Controller.OpenPin(2);
            iOController.Controller.IsPinModeSupported(1, PinMode.Input);//.Returns(true);
            iOController.Controller.IsPinModeSupported(2, PinMode.Input);//.Returns(true);
            iOController.Controller.Read(1);//.Returns(PinValue.Low);
            iOController.Controller.Read(2);//.Returns(PinValue.High);
            iOController.Controller.ClosePin(1);
            iOController.Controller.ClosePin(2);
            //var iOController = new GpioController(PinNumberingScheme.Logical, iOController.Driver);
            //Assert.NotNull(iOController);
            iOController.OpenPin(1, PinMode.Input);
            iOController.OpenPin(2, PinMode.Input);
            //Assert.True(iOController.IsPinOpen(1);

            // Invalid usage (we need to prefill the array)
            // Was this the intended use case?
            //Assert.Throws<InvalidOperationException>(() =>
            //{
            //    Span<PinValuePair> wrongArg = stackalloc PinValuePair[2];
            //    iOController.Read(wrongArg);
            //});

            Span<PinValuePair> toread = stackalloc PinValuePair[2];
            toread[0] = new PinValuePair(1, PinValue.Low);
            toread[1] = new PinValuePair(2, PinValue.Low);
            iOController.Read(toread);
            //Assert.Equal(1, toread[0].PinNumber);
            //Assert.Equal(2, toread[1].PinNumber);
            //Assert.Equal(PinValue.Low, toread[0].PinValue);
            //Assert.Equal(PinValue.High, toread[1].PinValue);
            iOController.ClosePin(1);
            iOController.ClosePin(2);
            //Assert.False(iOController.IsPinOpen(1);
        }

        public async Task WaitForEventAsyncFail()
        {
            iOController.OpenPin(1);
            iOController.Controller.IsPinModeSupported(1, PinMode.Input);//.Returns(true);
            //iOController.Controller.WaitForEvent(1, PinEventTypes.Rising | PinEventTypes.Falling, It.IsAny<CancellationToken>())
            //    .Returns(new WaitForEventResult()
            //    {
            //        EventTypes = PinEventTypes.None,
            //        TimedOut = true
            //    });
            //Assert.NotNull(iOController);
            iOController.OpenPin(1, PinMode.Input);

            Task<WaitForEventResult> task = iOController.WaitForEventAsync(1, PinEventTypes.Falling | PinEventTypes.Rising, TimeSpan.FromSeconds(0.01)).AsTask();
            WaitForEventResult result = await task.WaitAsync(CancellationToken.None);
            //Assert.True(task.IsCompleted);
            //Assert.Null(task.Exception);
            //Assert.True(result.TimedOut);
            //Assert.Equal(PinEventTypes.None, result.EventTypes);
        }

        public void WaitForEventSuccess()
        {
            //var iOController = new GpioController(PinNumberingScheme.Logical, iOController.Driver);
            iOController.OpenPin(1);
            iOController.IsPinModeSupported(1, PinMode.Input);//.Returns(true);
            //iOController.Controller.WaitForEvent(1, PinEventTypes.Rising | PinEventTypes.Falling, It.IsAny<CancellationToken>())
            //    .Returns(new WaitForEventResult()
            //    {
            //        EventTypes = PinEventTypes.Falling,
            //        TimedOut = false
            //    });
            //Assert.NotNull(iOController);
            iOController.OpenPin(1, PinMode.Input);

            WaitForEventResult result = iOController.WaitForEvent(1, PinEventTypes.Falling | PinEventTypes.Rising, TimeSpan.FromSeconds(0.01));
            //Assert.False(result.TimedOut);
            //Assert.Equal(PinEventTypes.Falling, result.EventTypes);
        }
    }
}
