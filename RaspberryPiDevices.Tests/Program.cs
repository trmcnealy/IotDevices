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
        static void Main(string[] args)
        {
            Console.WriteLine("Raspberry Pi Devices Tests");

            Console.WriteLine($"{nameof(IsRaspi4)}={IsRaspi4()}");


            RaspberryPiBoard raspberryPiBoard = new RaspberryPiBoard();
            GpioController gpioController = raspberryPiBoard.CreateGpioController();

            bool spiActivated = raspberryPiBoard.IsSpiActivated();
            Console.WriteLine($"{nameof(spiActivated)}={spiActivated}");

            //PinUsage.AnalogOut

            SpiConnectionSettings connectionSettings;
            SpiDevice spi;
            int[] pinAssignmentSpi;
            int[] overlayPinAssignmentForSpi;

            int busId = 0;
            int maxBusId = 0;
            int chipSelectLine = -1;

            while (busId < 40)
            {
                Console.WriteLine($"{nameof(busId)}={busId}");
                try
                {
                    connectionSettings = new SpiConnectionSettings(busId, chipSelectLine);
                    spi = (raspberryPiBoard is RaspberryPiBoard) ? SpiDevice.Create(connectionSettings) : raspberryPiBoard.CreateSpiDevice(connectionSettings);
                    pinAssignmentSpi = raspberryPiBoard.GetDefaultPinAssignmentForSpi(connectionSettings);
                    overlayPinAssignmentForSpi = raspberryPiBoard.GetOverlayPinAssignmentForSpi(connectionSettings);

                    foreach (int pinAssignment in pinAssignmentSpi)
                    {
                        Console.WriteLine($"{nameof(pinAssignment)}={pinAssignment}");
                    }
                    foreach (int overlayPinAssignment in overlayPinAssignmentForSpi)
                    {
                        Console.WriteLine($"{nameof(overlayPinAssignment)}={overlayPinAssignment}");
                    }
                }
                catch
                {
                    break;
                }
                finally
                {
                    maxBusId = busId;
                    ++busId;
                }
            }

            chipSelectLine = 0;

            while (busId < 40)
            {
                Console.WriteLine($"{nameof(busId)}={busId}");
                try
                {
                    connectionSettings = new SpiConnectionSettings(busId, chipSelectLine);
                    spi = (raspberryPiBoard is RaspberryPiBoard) ? SpiDevice.Create(connectionSettings) : raspberryPiBoard.CreateSpiDevice(connectionSettings);
                    pinAssignmentSpi = raspberryPiBoard.GetDefaultPinAssignmentForSpi(connectionSettings);
                    overlayPinAssignmentForSpi = raspberryPiBoard.GetOverlayPinAssignmentForSpi(connectionSettings);

                    foreach (int pinAssignment in pinAssignmentSpi)
                    {
                        Console.WriteLine($"{nameof(pinAssignment)}={pinAssignment}");
                    }
                    foreach (int overlayPinAssignment in overlayPinAssignmentForSpi)
                    {
                        Console.WriteLine($"{nameof(overlayPinAssignment)}={overlayPinAssignment}");
                    }
                }
                catch
                {
                    break;
                }
                finally
                {
                    maxBusId = busId;
                    ++busId;
                }
            }

            chipSelectLine = 1;

            while (busId < 40)
            {
                Console.WriteLine($"{nameof(busId)}={busId}");
                try
                {
                    connectionSettings = new SpiConnectionSettings(busId, chipSelectLine);
                    spi = (raspberryPiBoard is RaspberryPiBoard) ? SpiDevice.Create(connectionSettings) : raspberryPiBoard.CreateSpiDevice(connectionSettings);
                    pinAssignmentSpi = raspberryPiBoard.GetDefaultPinAssignmentForSpi(connectionSettings);
                    overlayPinAssignmentForSpi = raspberryPiBoard.GetOverlayPinAssignmentForSpi(connectionSettings);

                    foreach (int pinAssignment in pinAssignmentSpi)
                    {
                        Console.WriteLine($"{nameof(pinAssignment)}={pinAssignment}");
                    }
                    foreach (int overlayPinAssignment in overlayPinAssignmentForSpi)
                    {
                        Console.WriteLine($"{nameof(overlayPinAssignment)}={overlayPinAssignment}");
                    }
                }
                catch
                {
                    break;
                }
                finally
                {
                    maxBusId = busId;
                    ++busId;
                }
            }

            //Console.ReadKey();
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
