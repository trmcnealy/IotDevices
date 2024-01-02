using System.Diagnostics.CodeAnalysis;

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



            Console.ReadKey();
        }
    }
}
