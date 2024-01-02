using System;
using System.Device;
using System.Device.Gpio;
using System.Device.Gpio.Drivers;
using System.Device.I2c;
using System.Device.Pwm;
using System.Device.Pwm.Drivers;
using System.Device.Spi;
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
        catch(Exception ex)
        {
            Console.WriteLine($"{action.Method.Name}:"+ex.Message);
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
        static void Main(string[] args)
        {
            Console.WriteLine("Raspberry Pi Devices Tests");

            Console.WriteLine($"{nameof(IsRaspi4)}={IsRaspi4()}");
           

            RaspberryPiBoard raspberryPiBoard = new RaspberryPiBoard();
            GpioController gpioController = raspberryPiBoard.CreateGpioController();



            //PinUsage.AnalogOut

            SpiConnectionSettings connectionSettings = new SpiConnectionSettings(24, 0);
            SpiDevice spi = (raspberryPiBoard is RaspberryPiBoard) ? SpiDevice.Create(connectionSettings) : raspberryPiBoard.CreateSpiDevice(connectionSettings);

            bool spiActivated = raspberryPiBoard.IsSpiActivated();
            Console.WriteLine($"{nameof(spiActivated)}={spiActivated}");

            int[] pinAssignmentSpi = raspberryPiBoard.GetDefaultPinAssignmentForSpi(connectionSettings);
            foreach(int pinAssignment in pinAssignmentSpi)
            {                
                Console.WriteLine($"{nameof(pinAssignment)}={pinAssignment}");
            }

            int[] overlayPinAssignmentForSpi = raspberryPiBoard.GetOverlayPinAssignmentForSpi(connectionSettings);
            foreach(int overlayPinAssignment in overlayPinAssignmentForSpi)
            {                
                Console.WriteLine($"{nameof(overlayPinAssignment)}={overlayPinAssignment}");
            }






            Console.ReadKey();
        }

        static bool IsRaspi4()
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
