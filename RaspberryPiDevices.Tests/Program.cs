using System;
using System.Device;
using System.Device.Gpio;
using System.Device.Gpio.Drivers;
using System.Device.I2c;
using System.Device.Pwm;
using System.Device.Pwm.Drivers;
using System.Device.Spi;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using Iot.Device.Adc;
using Iot.Device.Board;
using Iot.Device.Card.CreditCardProcessing;
using Iot.Device.Mfrc522;


public static class Try
{
    public static void Catch(Action action)
    {
        try
        {
            action.Invoke();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{action.Method.Name}:" + ex.Message);
        }
        finally
        {
            Console.WriteLine($"{action.Method.Name}");
        }
    }

    //public static object? Catch<TDelegate>(this TDelegate action) where TDelegate : Delegate
    //{
    //    object? obj = null;
    //    try
    //    {
    //        obj = action.DynamicInvoke();
    //    }
    //    catch(Exception ex)
    //    {
    //        Console.WriteLine($"{action.Method.Name}:"+ex.Message);
    //    }
    //    finally
    //    {
    //        Console.WriteLine($"{action.Method.Name}");
    //    }
    //    return obj;
    //}
}


namespace RaspberryPiDevices.Tests
{
    internal class Program
    {
        private static volatile bool keepRunning;
        private static ConsoleKeyInfo cki;

        static void Main(string[] args)
        {
            Console.CancelKeyPress += (object? sender, ConsoleCancelEventArgs args) =>
            {
                args.Cancel = true;
                keepRunning = false;
            };

            Console.Clear();

            Console.WriteLine("Raspberry Pi Devices Tests");
            Console.WriteLine($"{nameof(IsRaspi4)}={IsRaspi4()}");

            RaspberryPiBoard raspberryPiBoard = new RaspberryPiBoard();
            GpioController gpioController = raspberryPiBoard.CreateGpioController();

            bool spiActivated = raspberryPiBoard.IsSpiActivated();
            Console.WriteLine($"{nameof(spiActivated)}={spiActivated}");


            Console.WriteLine(raspberryPiBoard.QueryComponentInformation());

            try
            {
                SoilMoistureSensor sms = new SoilMoistureSensor(raspberryPiBoard, gpioController);

                while (keepRunning)
                {
                    Console.WriteLine(sms);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            //keepRunning = true;

            //GpioPin CE0 = gpioController.OpenPin(24, PinMode.Input);
            //GpioPin CE1 = gpioController.OpenPin(26, PinMode.Input);

            //CE0.ValueChanged += (s, e) =>
            //{
            //    if (s is GpioPin pin)
            //    {
            //        Console.WriteLine($"{pin.PinNumber}={pin.Read()}");
            //    }
            //    else
            //    {
            //        Console.WriteLine($"{CE0.PinNumber}={CE0.Read()}");
            //    }
            //};

            //CE1.ValueChanged += (s, e) =>
            //{
            //    if (s is GpioPin pin)
            //    {
            //        Console.WriteLine($"{pin.PinNumber}={pin.Read()}");
            //    }
            //    else
            //    {
            //        Console.WriteLine($"{CE1.PinNumber}={CE1.Read()}");
            //    }
            //};

            //while (keepRunning)
            //{
            //    Console.WriteLine($"{CE0.PinNumber}={CE0.Read()}");
            //    Console.WriteLine($"{CE1.PinNumber}={CE1.Read()}");
            //}
        }

        private static void CE0_ValueChanged(object sender, PinValueChangedEventArgs pinValueChangedEventArgs)
        {

        }

        private static bool IsRaspi4()
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
    }
}


//SpiConnectionSettings connectionSettings;
//SpiDevice spi;
//int[] pinAssignmentSpi;
//int[] overlayPinAssignmentForSpi;

//int busId = 0;
//int maxBusId = 0;
//int chipSelectLine = 1;

//Console.WriteLine($"{nameof(chipSelectLine)}={chipSelectLine}");

//while (busId < 40)
//{
//    Console.WriteLine($"{nameof(busId)}={busId}");
//    try
//    {
//        connectionSettings = new SpiConnectionSettings(busId, chipSelectLine);
//        spi = (raspberryPiBoard is RaspberryPiBoard) ? SpiDevice.Create(connectionSettings) : raspberryPiBoard.CreateSpiDevice(connectionSettings);
//        pinAssignmentSpi = raspberryPiBoard.GetDefaultPinAssignmentForSpi(connectionSettings);
//        overlayPinAssignmentForSpi = raspberryPiBoard.GetOverlayPinAssignmentForSpi(connectionSettings);

//        foreach (int pinAssignment in pinAssignmentSpi)
//        {
//            Console.WriteLine($"{nameof(pinAssignment)}={pinAssignment}");
//        }
//        foreach (int overlayPinAssignment in overlayPinAssignmentForSpi)
//        {
//            Console.WriteLine($"{nameof(overlayPinAssignment)}={overlayPinAssignment}");
//        }
//    }
//    catch
//    {
//        break;
//    }
//    finally
//    {
//        maxBusId = busId;
//        ++busId;
//    }
//}

//busId = 0;
//chipSelectLine = 0;
//Console.WriteLine($"{nameof(chipSelectLine)}={chipSelectLine}");

//while (busId < 40)
//{
//    Console.WriteLine($"{nameof(busId)}={busId}");
//    try
//    {
//        connectionSettings = new SpiConnectionSettings(busId, chipSelectLine);
//        spi = (raspberryPiBoard is RaspberryPiBoard) ? SpiDevice.Create(connectionSettings) : raspberryPiBoard.CreateSpiDevice(connectionSettings);
//        pinAssignmentSpi = raspberryPiBoard.GetDefaultPinAssignmentForSpi(connectionSettings);
//        overlayPinAssignmentForSpi = raspberryPiBoard.GetOverlayPinAssignmentForSpi(connectionSettings);

//        foreach (int pinAssignment in pinAssignmentSpi)
//        {
//            Console.WriteLine($"{nameof(pinAssignment)}={pinAssignment}");
//        }
//        foreach (int overlayPinAssignment in overlayPinAssignmentForSpi)
//        {
//            Console.WriteLine($"{nameof(overlayPinAssignment)}={overlayPinAssignment}");
//        }
//    }
//    catch
//    {
//        break;
//    }
//    finally
//    {
//        maxBusId = busId;
//        ++busId;
//    }
//}

//busId = 0;
//chipSelectLine = 1;
//Console.WriteLine($"{nameof(chipSelectLine)}={chipSelectLine}");

//while (busId < 40)
//{
//    Console.WriteLine($"{nameof(busId)}={busId}");
//    try
//    {
//        connectionSettings = new SpiConnectionSettings(busId, chipSelectLine);
//        spi = (raspberryPiBoard is RaspberryPiBoard) ? SpiDevice.Create(connectionSettings) : raspberryPiBoard.CreateSpiDevice(connectionSettings);
//        pinAssignmentSpi = raspberryPiBoard.GetDefaultPinAssignmentForSpi(connectionSettings);
//        overlayPinAssignmentForSpi = raspberryPiBoard.GetOverlayPinAssignmentForSpi(connectionSettings);

//        foreach (int pinAssignment in pinAssignmentSpi)
//        {
//            Console.WriteLine($"{nameof(pinAssignment)}={pinAssignment}");
//        }
//        foreach (int overlayPinAssignment in overlayPinAssignmentForSpi)
//        {
//            Console.WriteLine($"{nameof(overlayPinAssignment)}={overlayPinAssignment}");
//        }
//    }
//    catch
//    {
//        break;
//    }
//    finally
//    {
//        maxBusId = busId;
//        ++busId;
//    }
//}

//Console.ReadKey();