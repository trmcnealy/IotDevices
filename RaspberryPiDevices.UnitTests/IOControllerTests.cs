
using System.Diagnostics.CodeAnalysis;

namespace RaspberryPiDevices.Tests
{
    [TestClass]
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


        [TestMethod]
        public void TestMethod1()
        {
            Assert.IsTrue(true);
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

        [TestMethod]
        public void InputPullResistorsWork()
        {
            iOController.Controller.OpenPin(OpenPin, PinMode.InputPullUp);
            Assert.AreEqual(PinValue.High, iOController.Controller.Read(OpenPin));

            for (int i = 0; i < 100; i++)
            {
                iOController.Controller.SetPinMode(OpenPin, PinMode.InputPullDown);
                Assert.AreEqual(PinValue.Low, iOController.Controller.Read(OpenPin));

                iOController.Controller.SetPinMode(OpenPin, PinMode.InputPullUp);
                Assert.AreEqual(PinValue.High, iOController.Controller.Read(OpenPin));
            }

            // change one more time so that when running test in a loop we start with the inverted option
            iOController.Controller.SetPinMode(OpenPin, PinMode.InputPullDown);
            Assert.AreEqual(PinValue.Low, iOController.Controller.Read(OpenPin));
        }

        [TestMethod]
        public void OpenPinDefaultsModeToLastModeIncludingPulls()
        {
            iOController.Controller.OpenPin(OutputPin);
            iOController.Controller.SetPinMode(OutputPin, PinMode.InputPullDown);
            iOController.Controller.ClosePin(OutputPin);
            iOController.Controller.OpenPin(OutputPin);

            if (IsRaspi4())
            {
                Assert.AreEqual(PinMode.InputPullDown, iOController.Controller.GetPinMode(OutputPin));
            }
            else
            {
                Assert.AreEqual(PinMode.Input, iOController.Controller.GetPinMode(OutputPin));
            }

        }

        [TestMethod]
        public void HighPulledPinDoesNotChangeToLowWhenChangedToOutput()
        {
            bool didTriggerToLow = false;
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
                        if (args.ChangeType == PinEventTypes.Falling)
                        {
                            didTriggerToLow = true;
                        }
                    });

            iOController.Controller.Write(testPin, PinValue.High);
            iOController.Controller.SetPinMode(testPin, PinMode.Output);
            Thread.Sleep(50);
            Assert.IsFalse(didTriggerToLow);

            iOController.Controller.ClosePin(OutputPin);
            iOController.Controller.ClosePin(InputPin);

        }

        [TestMethod]
        public void PinCountReportedCorrectly()
        {
            ////var iOController = new GpioController(PinNumberingScheme.Logical, iOController.Driver);
            Assert.AreEqual(28, iOController.PinCount);
        }

        [TestMethod]
        public void OpenTwiceGpioPinAndClosedTwiceThrows()
        {
            ////var iOController = new GpioController(PinNumberingScheme.Logical, iOController.Driver);
            iOController.Controller.OpenPin(1);
            iOController.Controller.ClosePin(1);
            GpioPin gpioPin1 = iOController.OpenPin(1, PinMode.Input);
            GpioPin gpiopin2 = iOController.OpenPin(1, PinMode.Input);
            Assert.AreEqual(gpioPin1, gpiopin2);
            iOController.ClosePin(1);
            Assert.ThrowsException<InvalidOperationException>(() => iOController.ClosePin(1));
        }

        [TestMethod]
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

        [TestMethod]
        public void GpioControllerCreateOpenClosePin()
        {
            iOController.Controller.OpenPin(1);
            iOController.Controller.IsPinModeSupported(1, PinMode.Output);//.Returns(true);
            iOController.Controller.GetPinMode(1);//.Returns(PinMode.Output);
            iOController.Controller.Write(1, PinValue.High);
            iOController.Controller.ClosePin(1);
            ////var iOController = new GpioController(PinNumberingScheme.Logical, iOController.Driver);
            Assert.IsNotNull(iOController);
            iOController.OpenPin(1, PinMode.Output);
            Assert.IsTrue(iOController.IsPinOpen(1));
            iOController.Write(1, PinValue.High);
            iOController.ClosePin(1);
            Assert.IsFalse(iOController.IsPinOpen(1));
        }

        [TestMethod]
        public void IsPinModeSupported()
        {
            iOController.Controller.IsPinModeSupported(1, PinMode.Input);//.Returns(true);
                                                                         ////var iOController = new GpioController(PinNumberingScheme.Logical, iOController.Driver);
            Assert.IsNotNull(iOController);
            Assert.IsTrue(iOController.IsPinModeSupported(1, PinMode.Input));
        }

        [TestMethod]
        public void GetPinMode()
        {
            iOController.Controller.OpenPin(1);
            iOController.Controller.GetPinMode(1);//.Returns(PinMode.Output);
            ////var iOController = new GpioController(PinNumberingScheme.Logical, iOController.Driver);
            Assert.IsNotNull(iOController);
            // Not open
            Assert.ThrowsException<InvalidOperationException>(() => iOController.GetPinMode(1));
            iOController.OpenPin(1);
            Assert.AreEqual(PinMode.Output, iOController.GetPinMode(1));
        }

        [TestMethod]
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
            Assert.AreEqual(PinValue.High, iOController.Read(2));
            iOController.ClosePin(2);
            iOController.Dispose();
        }

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
        public void CallbackOnEventWorks()
        {
            // Our mock driver maps physical pin 2 to logical pin 1
            iOController.Controller.OpenPin(1);
            //iOController.Controller.AddCallbackForPinValueChangedEvent(1, PinEventTypes.Rising, It.IsAny<PinChangeEventHandler>());
            //var iOController = new GpioController(PinNumberingScheme.Logical, iOController.Driver);
            iOController.OpenPin(1); // logical pin 1 on our test board

            bool callbackSeen = false;

            PinChangeEventHandler eventHandler = (sender, args) =>
            {
                //callbackSeen = true;
                Assert.AreEqual(1, args.PinNumber);
                Assert.AreEqual(PinEventTypes.Falling, args.ChangeType);
            };

            iOController.RegisterCallbackForPinValueChangedEvent(1, PinEventTypes.Rising, eventHandler);

            //iOController.Driver.FireEventHandler(1, PinEventTypes.Falling);

            Assert.IsTrue(callbackSeen);

            iOController.UnregisterCallbackForPinValueChangedEvent(1, eventHandler);
        }

        [TestMethod]
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
            Assert.IsNotNull(iOController);
            iOController.OpenPin(1, PinMode.Output);
            iOController.OpenPin(2, PinMode.Output);
            Assert.IsTrue(iOController.IsPinOpen(1));
            Span<PinValuePair> towrite = stackalloc PinValuePair[2];
            towrite[0] = new PinValuePair(1, PinValue.High);
            towrite[1] = new PinValuePair(2, PinValue.Low);
            iOController.Write(towrite);
            iOController.ClosePin(1);
            iOController.ClosePin(2);
            Assert.IsFalse(iOController.IsPinOpen(1));
        }

        [TestMethod]
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
            Assert.IsNotNull(iOController);
            iOController.OpenPin(1, PinMode.Input);
            iOController.OpenPin(2, PinMode.Input);
            Assert.IsTrue(iOController.IsPinOpen(1));

            // Invalid usage (we need to prefill the array)
            // Was this the intended use case?
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                Span<PinValuePair> wrongArg = stackalloc PinValuePair[2];
                iOController.Read(wrongArg);
            });

            Span<PinValuePair> toread = stackalloc PinValuePair[2];
            toread[0] = new PinValuePair(1, PinValue.Low);
            toread[1] = new PinValuePair(2, PinValue.Low);
            iOController.Read(toread);
            Assert.AreEqual(1, toread[0].PinNumber);
            Assert.AreEqual(2, toread[1].PinNumber);
            Assert.AreEqual(PinValue.Low, toread[0].PinValue);
            Assert.AreEqual(PinValue.High, toread[1].PinValue);
            iOController.ClosePin(1);
            iOController.ClosePin(2);
            Assert.IsFalse(iOController.IsPinOpen(1));
        }

        public async Task WaitForEventAsyncFail()
        {
            iOController.OpenPin(1);
            iOController.Controller.IsPinModeSupported(1, PinMode.Input);//.Returns(true);
            iOController.Controller.WaitForEvent(1, PinEventTypes.Rising | PinEventTypes.Falling, IsAny<CancellationToken>());
            //.Returns(new WaitForEventResult()
            //{
            //    EventTypes = PinEventTypes.None,
            //    TimedOut = true
            //});
            Assert.IsNotNull(iOController);
            iOController.OpenPin(1, PinMode.Input);

            var task = iOController.WaitForEventAsync(1, PinEventTypes.Falling | PinEventTypes.Rising, TimeSpan.FromSeconds(0.01)).AsTask();
            var result = await task.WaitAsync(CancellationToken.None);
            Assert.IsTrue(task.IsCompleted);
            Assert.IsNull(task.Exception);
            Assert.IsTrue(result.TimedOut);
            Assert.AreEqual(PinEventTypes.None, result.EventTypes);
        }

        [TestMethod]
        public void WaitForEventSuccess()
        {
            //var iOController = new GpioController(PinNumberingScheme.Logical, iOController.Driver);
            iOController.OpenPin(1);
            iOController.IsPinModeSupported(1, PinMode.Input);//.Returns(true);
            iOController.Controller.WaitForEvent(1, PinEventTypes.Rising | PinEventTypes.Falling, IsAny<CancellationToken>());
            //.Returns(new WaitForEventResult()
            //{
            //    EventTypes = PinEventTypes.Falling,
            //    TimedOut = false
            //});
            Assert.IsNotNull(iOController);
            iOController.OpenPin(1, PinMode.Input);

            var result = iOController.WaitForEvent(1, PinEventTypes.Falling | PinEventTypes.Rising, TimeSpan.FromSeconds(0.01));
            Assert.IsFalse(result.TimedOut);
            Assert.AreEqual(PinEventTypes.Falling, result.EventTypes);
        }

        private static TValue IsAny<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]TValue>()
        {
            return Activator.CreateInstance<TValue>();
        }
    }
}