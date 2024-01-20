using System;
using System.Device;
using System.Device.Gpio;
using System.Device.Gpio.Drivers;
using System.Device.I2c;
using System.Device.Pwm;
using System.Device.Pwm.Drivers;
using System.Device.Spi;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

using Iot.Device.Adc;
using Iot.Device.Bmxx80;
using Iot.Device.Bmxx80.PowerMode;
using Iot.Device.Board;
using Iot.Device.Card.CreditCardProcessing;
using Iot.Device.Common;
using Iot.Device.Display;
using Iot.Device.Hx711;
using Iot.Device.Max31865;
using Iot.Device.Mfrc522;
using Iot.Device.Multiplexing;


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


public static class Constants
{
    public const int GPIO1 = 1;
    public const int GPIO2 = 2;
    public const int GPIO3 = 3;
    public const int GPIO4 = 4;
    public const int GPIO5 = 5;
    public const int GPIO6 = 6;
    public const int GPIO7 = 7;
    public const int GPIO8 = 8;
    public const int GPIO9 = 9;
    public const int GPIO10 = 10;
    public const int GPIO11 = 11;
    public const int GPIO12 = 12;
    public const int GPIO13 = 13;
    public const int GPIO14 = 14;
    public const int GPIO15 = 15;
    public const int GPIO16 = 16;
    public const int GPIO17 = 17;
    public const int GPIO18 = 18;
    public const int GPIO19 = 19;
    public const int GPIO20 = 20;
    public const int GPIO21 = 21;
    public const int GPIO22 = 22;
    public const int GPIO23 = 23;
    public const int GPIO24 = 24;
    public const int GPIO25 = 25;
    public const int GPIO26 = 26;
    public const int GPIO27 = 27;


    private const int CE0 = GPIO8;
    private const int CE1 = GPIO7;
    private const int MISO = GPIO9;
    private const int MOSI = GPIO10;
    private const int SCLK = GPIO11;
}

namespace RaspberryPiDevices.Tests
{
    internal class Program
    {
        private const int SpiAddress = 0;

        private static readonly object This;
        //private static readonly SpinWait spinWait = new SpinWait();

        //private static readonly RaspberryPiBoard raspberryPiBoard;
        //private static GpioController gpioController;
        //private static readonly SpiDevice spiDevice;

        static Program()
        {
            This = new object();

            //raspberryPiBoard = new RaspberryPiBoard();

            //SpiConnectionSettings settings = new(0, 0)
            //{
            //    //ClockFrequency = 500_000,
            //    Mode = SpiMode.Mode0,
            //    DataFlow = DataFlow.MsbFirst
            //};

            //GpioController gpioController = raspberryPiBoard.CreateGpioController();

            //spiDevice = raspberryPiBoard.CreateSpiDevice(settings);
        }



        //private static volatile bool keepRunning;
        //private static ConsoleKeyInfo cki;

        static void WriteProgress(int position)
        {
            (int Left, int Top) = Console.GetCursorPosition();

            for (int k = 0; k <= 100; k++)
            {
                Console.SetCursorPosition(Left + position, Top);
                Console.Write($"█{position}%");
            }

            //Console.WriteLine();
        }

        static void WriteOutput<TObject>(TObject obj) where TObject : notnull
        {
            const int position = 0;

            (int Left, int Top) = Console.GetCursorPosition();

            Console.SetCursorPosition(position, Top);

            string? output = obj.ToString();

            if (output is not null)
            {
                int diff = Console.BufferWidth - output.Length;
                if (diff > 0)
                {
                    Console.Write(output);
                    for (int k = 0; k <= diff - 1; k++)
                    {
                        Console.Write(" ");
                    }
                }
                else
                {
                    Console.Write(output);
                }
            }
        }


        static void ScanI2CBus(I2cBus i2cBus)
        {
            const byte lowestAddress = 0x08;
            const byte highestAddress = 0x80;

            (List<byte> FoundDevices, byte LowestAddress, byte HighestAddress) busScan = i2cBus.PerformBusScan(ProgressPrinter.Instance, lowestAddress, highestAddress);


            foreach (byte foundDevice in busScan.FoundDevices)
            {
                Console.WriteLine($"Found Device @{foundDevice:X4}");
            }
        }

        static void Main(string[] args)
        {
            //keepRunning = true;

            Console.Clear();

            RPiHardware device = new RPiHardware();

            //Console.WriteLine(raspberryPiBoard.QueryComponentInformation());

            //TemperatureHumiditySensor temperatureHumiditySensor = new TemperatureHumiditySensor(raspberryPiBoard);
            //BarometricPressureSensor barometricPressureSensor = new BarometricPressureSensor(raspberryPiBoard);

            //LED4DigitDisplay lED4DigitDisplay = new LED4DigitDisplay();

            //RaspberryPiBoard _raspberryPiBoard = new RaspberryPiBoard();


            //PHProbeSensor pHProbeSensor = new PHProbeSensor(raspberryPiBoard);

            //LED8DigitDisplay lED8DigitDisplay = new LED8DigitDisplay(_raspberryPiBoard);

            //WaterFlowSensor waterFlowSensor = new WaterFlowSensor(_raspberryPiBoard);

            CancellationTokenSource cts = new CancellationTokenSource();

            Console.CancelKeyPress += (object? sender, ConsoleCancelEventArgs args) =>
            {
                cts.Cancel();

                Utilities.DelayMilliseconds(100, false);

                device.Dispose();

                //waterFlowSensor.Dispose();
                //_raspberryPiBoard.Dispose();
                //keepRunning = false;
                args.Cancel = true;
            };


            //{
            //    //I2cBus i2cBus = _raspberryPiBoard.CreateOrGetI2cBus(1);
            //    //ScanI2CBus(i2cBus);
            //}

            device.Run(cts.Token);

            //TimeSpan delay = TimeSpan.FromMilliseconds(0);

            //int counter = 0;

            //while (!Console.KeyAvailable || keepRunning)
            //{
            //    //waterFlowSensor.Run();
            //    //Utilities.DelayMilliseconds(1);

            //    //lED8DigitDisplay.Display(waterFlowSensor.FlowRate.LitersPerMinute);

            //    //lED8DigitDisplay.Display(counter++);

            //    Utilities.Delay(delay);

            //    //Task.Run(waterFlowSensor.Run).WaitAsync(cts.Token);
            //}

            //while(!Console.KeyAvailable)
            //{
            //}

            //LED8DigitDisplay _lED8DigitDisplay1 = new LED8DigitDisplay(_raspberryPiBoard);

            //_lED8DigitDisplay1.Display(1);

            if (!device.IsDisposed)
            {
                device.Dispose();
            }
        }

    }
}



//ReservePin(Constants.GPIO17, PinUsage.Gpio);
//ReservePin(Constants.GPIO27, PinUsage.Gpio);
//ReservePin(Constants.GPIO22, PinUsage.Gpio);

//ReservePin(PHProbeSensor.MOSI, PinUsage.Gpio);
//ReservePin(PHProbeSensor.MISO, PinUsage.Gpio);
//ReservePin(PHProbeSensor.SCLK, PinUsage.Gpio);
//ReservePin(PHProbeSensor.CE0, PinUsage.Gpio);
//ReservePin(PHProbeSensor.CE1, PinUsage.Gpio);



////GpioPin gpio07 = gpioController.OpenPin(PHProbeSensor.GPIO07, PinMode.Output);
////GpioPin gpio08 = gpioController.OpenPin(PHProbeSensor.GPIO08, PinMode.Output);

//if (gpioController.IsPinModeSupported(PHProbeSensor.GPIO09, PinMode.Output))
//{
//    GpioPin gpioPin = gpioController.OpenPin(PHProbeSensor.GPIO09, PinMode.Output);
//    Console.WriteLine($"GPIO09 PinMode={gpioController.GetPinMode(PHProbeSensor.GPIO09)}");

//    int pinValue = ((int)gpioPin.Read());
//    Console.WriteLine($"GPIO09 Value={pinValue}");
//}


//if (gpioController.IsPinModeSupported(PHProbeSensor.GPIO10, PinMode.Output))
//{
//    GpioPin gpioPin = gpioController.OpenPin(PHProbeSensor.GPIO10, PinMode.Output);
//    Console.WriteLine($"GPIO10 PinMode={gpioController.GetPinMode(PHProbeSensor.GPIO10)}");

//    int pinValue = ((int)gpioPin.Read());
//    Console.WriteLine($"GPIO10 Value={pinValue}");
//}


//GpioPin gpioPin;
//int pinValue;
////double voltage;

////if (gpioController.IsPinModeSupported(Constants.GPIO17, PinMode.Output))
////{
//gpioPin = gpioController.OpenPin(Constants.GPIO17);
//Console.WriteLine($"GPIO11 PinMode={gpioController.GetPinMode(Constants.GPIO17)}");

//gpioPin.Write(PinValue.Low);

//gpioPin.ValueChanged += (s,e)=>{ };

//pinValue = ((int)gpioPin.Read());
//Console.WriteLine($"GPIO11 Value={pinValue}");

////voltage = pinValue * (5.0 / 1023.0);
////Console.WriteLine($"voltage={voltage}");

////while (keepRunning)
////{
////    pinValue = ((int)gpioPin.Read());
////    Console.WriteLine($"GPIO11 Value={pinValue}");

////    voltage = pinValue * (5.0 / 1023.0);
////    Console.WriteLine($"voltage={voltage}");
////}

//gpioPin = gpioController.OpenPin(Constants.GPIO27);
//Console.WriteLine($"GPIO27 PinMode={gpioController.GetPinMode(Constants.GPIO27)}");

//pinValue = ((int)gpioPin.Read());
//Console.WriteLine($"GPIO27 Value={pinValue}");

////voltage = pinValue * (5.0 / 1023.0);
////Console.WriteLine($"voltage={voltage}");

////while (keepRunning)
////{
////    pinValue = ((int)gpioPin.Read());
////    Console.WriteLine($"GPIO11 Value={pinValue}");

////    voltage = pinValue * (5.0 / 1023.0);
////    Console.WriteLine($"voltage={voltage}");
////}

//gpioPin = gpioController.OpenPin(Constants.GPIO22);
//Console.WriteLine($"GPIO22 PinMode={gpioController.GetPinMode(Constants.GPIO22)}");

//pinValue = ((int)gpioPin.Read());
//Console.WriteLine($"GPIO22 Value={pinValue}");

////voltage = pinValue * (5.0 / 1023.0);
////Console.WriteLine($"voltage={voltage}");

////while (keepRunning)
////{
////    pinValue = ((int)gpioPin.Read());
////    Console.WriteLine($"GPIO11 Value={pinValue}");

////    voltage = pinValue * (5.0 / 1023.0);
////    Console.WriteLine($"voltage={voltage}");
////}
//// }

//while (!Console.KeyAvailable)
//{
//}


//barometricPressureSensor.Begin();


//SpiDevice spiDevice = raspberryPiBoard.CreateSpiDevice(new SpiConnectionSettings(0, 0));

//if (!raspberryPiBoard.IsSpiActivated())
//{
//    throw new Exception("!IsSpiActivated");
//}


//int sensorValue = spiDevice.ReadByte();
//Console.WriteLine($"sensorValue={sensorValue}");
//double voltage = sensorValue * (5.0 / 1023.0);
//Console.WriteLine($"voltage={voltage}");


//for (int busid = 0; busid < 2; busid++)
//{
//    for (int chipSelectLine = -1; chipSelectLine < 3; chipSelectLine++)
//    {
//        var pins = raspberryPiBoard.GetOverlayPinAssignmentForSpi(new SpiConnectionSettings(busid, chipSelectLine));

//        if (pins != null)
//        {
//            Console.WriteLine($"SPI overlay pins on busID {busid}: MISO {pins[0]} MOSI {pins[1]} Clock {pins[2]}.");
//        }
//        else
//        {
//            Console.WriteLine($"No SPI pins defined in the overlay on busID {busid}");
//        }
//    }
//}

//double Voltage3V3 = GetVoltage3V3(spiDevice);Console.WriteLine($"Voltage3V3={Voltage3V3}");
//double VoltageBatteryVcc = GetVoltageBatteryVcc(spiDevice);Console.WriteLine($"VoltageBatteryVcc={VoltageBatteryVcc}");

//int incoming = SpiRead16(spiDevice, SpiMessageType.GetSensor1);
//Console.WriteLine($"GetSensor1={incoming}");
//incoming = SpiRead16(spiDevice, SpiMessageType.GetSensor2);
//Console.WriteLine($"GetSensor2={incoming}");
//incoming = SpiRead16(spiDevice, SpiMessageType.GetSensor3);
//Console.WriteLine($"GetSensor3={incoming}");
//incoming = SpiRead16(spiDevice, SpiMessageType.GetSensor4);
//Console.WriteLine($"GetSensor4={incoming}");

//byte readByte;


//while (keepRunning)
//{
//    readByte = spiDevice.ReadByte();

//    Console.Write($"{readByte} ");


//    //sensorValue = spiDevice.ReadByte();
//    //voltage = sensorValue * (5.0 / 1023.0);
//    //Console.WriteLine($"voltage={voltage}");



//    //if (barometricPressureSensor.IsReady())
//    //{
//    //    Console.Write("Pascal: ");
//    //    Console.WriteLine(barometricPressureSensor.Pascal);
//    //    Console.Write("ATM: ");
//    //    Console.WriteLine(barometricPressureSensor.Atm);
//    //    Console.Write("mmHg: ");
//    //    Console.WriteLine(barometricPressureSensor.MmHg);
//    //    Console.Write("PSI: ");
//    //    Console.WriteLine(barometricPressureSensor.Psi);
//    //}

//    //barometricPressureSensor.WaitReady(10);


//    //Console.WriteLine($"{barometricPressureSensor.Weight}");
//}

//private static readonly int I2cBusId = 1;
//private static readonly int FirstAddress = 0x03;

//private static readonly int LastAddress = 0x77;

//private static void ScanDeviceAddressesOnI2cBus()
//{
//    StringBuilder stringBuilder = new StringBuilder();

//    stringBuilder.Append("     0  1  2  3  4  5  6  7  8  9  a  b  c  d  e  f");
//    stringBuilder.Append(Environment.NewLine);

//    for (int startingRowAddress = 0; startingRowAddress < 128; startingRowAddress += 16)
//    {
//        stringBuilder.Append($"{startingRowAddress:x2}: ");  // Beginning of row.

//        for (int rowAddress = 0; rowAddress < 16; rowAddress++)
//        {
//            int deviceAddress = startingRowAddress + rowAddress;

//            // Skip the unwanted addresses.
//            if (deviceAddress < FirstAddress || deviceAddress > LastAddress)
//            {
//                stringBuilder.Append("   ");
//                continue;
//            }

//            I2cConnectionSettings connectionSettings = new I2cConnectionSettings(I2cBusId, deviceAddress);
//            using (I2cDevice i2cDevice = I2cDevice.Create(connectionSettings))
//            {
//                try
//                {
//                    i2cDevice.ReadByte();  // Only checking if device is present.
//                    stringBuilder.Append($"{deviceAddress:x2} ");
//                }
//                catch
//                {
//                    stringBuilder.Append("-- ");
//                }
//            }
//        }

//        stringBuilder.Append(Environment.NewLine);
//    }

//    Console.WriteLine(stringBuilder.ToString());
//}

//public static I2cDevice CreateI2cDevice(RaspberryPiBoard board)
//{
//    I2cConnectionSettings settings = new I2cConnectionSettings(1, Hx711I2c.DefaultI2cAddress);

//    I2cDevice device = board.CreateI2cDevice(settings);

//    return device;
//}

//private static int SpiRead32(SpiDevice spiDevice, SpiMessageType MessageType)
//{
//    int retVal = -1;
//    byte[] writeBuffer = { SpiAddress, (byte)MessageType, 0, 0, 0, 0, 0, 0 };
//    byte[] reply = TransferArray(spiDevice, writeBuffer);

//    if (reply[3] == 0xA5)
//    {
//        retVal = (int)(reply[4] << 24) | (reply[5] << 16) | (reply[6] << 8) | reply[7];
//    }
//    else
//    {
//        throw new IOException($"{nameof(SpiRead32)} : no SPI response");
//    }

//    return retVal;
//}

//private static int SpiRead16(SpiDevice spiDevice, SpiMessageType MessageType)
//{
//    int retVal = -1;
//    byte[] writeBuffer = { SpiAddress, (byte)MessageType, 0, 0, 0, 0, };
//    byte[] reply = TransferArray(spiDevice, writeBuffer);

//    if (reply[3] == 0xA5)
//    {
//        retVal = (int)(reply[4] << 8) | reply[5];
//    }
//    else
//    {
//        Console.WriteLine($"{reply[0]:X2} {reply[1]:X2} {reply[2]:X2} {reply[3]:X2}");
//        throw new IOException($"{nameof(SpiRead16)} : no SPI response");
//    }

//    return retVal;
//}

//private static byte[] TransferArray(SpiDevice spiDevice, Span<byte> writeBuffer)
//{
//    byte[] readBuffer = new byte[writeBuffer.Length];

//    //writeBuffer[0] = (byte)((int)channelToCharge << 1);
//    spiDevice.TransferFullDuplex(writeBuffer, readBuffer);

//    return readBuffer;
//}

//private static double GetVoltage3V3(SpiDevice spiDevice)
//{
//    double value = SpiRead16(spiDevice, SpiMessageType.GetVoltage3V3);
//    return (value / 1000.0);
//}

//private static double GetVoltage5V(SpiDevice spiDevice)
//{
//    double value = SpiRead16(spiDevice, SpiMessageType.GetVoltage5V);
//    return (value / 1000.0);
//}

//private static double GetVoltage9V(SpiDevice spiDevice)
//{
//    double value = SpiRead16(spiDevice, SpiMessageType.GetVoltage9V);
//    return (value / 1000.0);
//}

//private static double GetVoltageBatteryVcc(SpiDevice spiDevice)
//{
//    double value = SpiRead16(spiDevice, SpiMessageType.GetVoltageVcc);
//    return (value / 1000.0);
//}

//private static void ReservePin(int pin, PinUsage pinUsage = PinUsage.Gpio)
//{
//    try
//    {
//        raspberryPiBoard.ReservePin(pin, pinUsage, This);
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine(ex);
//    }
//}

//private static void ReleasePin(int pin, PinUsage pinUsage = PinUsage.Gpio)
//{
//    try
//    {
//        //if (gpioController.IsPinOpen(pin))
//        //{
//        //    gpioController.ClosePin(pin);
//        //}

//        raspberryPiBoard.ReleasePin(pin, pinUsage, This);
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine(ex);
//    }
//}

//private static void SpiRaspiTestWithHardwareCs(Board raspi)
//{
//    Console.WriteLine("MCP3008 SPI Hardware CS Test");

//    // Runs a test communication against an MCP3008. The CS pin 8 is expected to be controlled by the driver, as
//    // we specify it here (SPI0_CE0 is BCM pin 8 on ALT0)

//    SpiConnectionSettings spiSettings = new SpiConnectionSettings(0, 0) { ChipSelectLineActiveState = PinValue.Low };

//    using SpiDevice dev = raspi.CreateSpiDevice(spiSettings);

//    using Mcp3008 mcp = new Mcp3008(dev);

//    while (!Console.KeyAvailable)
//    {
//        for (int i = 0; i < 8; i++)
//        {
//            int value = mcp.Read(i);
//            Console.WriteLine($"Channel {i} has value {value}.");
//            Thread.Sleep(100);
//        }

//        Thread.Sleep(500);
//    }

//    Console.ReadKey(true);
//}

//bool isPinOpen;
//for (int i = 1; i < raspberryPiBoard.PinCount; i++)
//{
//    isPinOpen = gpioController.IsPinOpen(i);
//    Console.WriteLine($"Pin{i}={isPinOpen}");
//    if(!isPinOpen)
//    {
//        gpioController.OpenPin(11);
//    }
//    gpioController.ClosePin(11);
//}

//PinUsage pinUsage = raspberryPiBoard.DetermineCurrentPinUsage(11);
//Console.WriteLine($"{nameof(pinUsage)}={pinUsage}");

//bool spiActivated = raspberryPiBoard.IsSpiActivated();
//Console.WriteLine($"{nameof(spiActivated)}={spiActivated}");

//SpiConnectionSettings spiSettings = new SpiConnectionSettings(0);
////spiSettings.ClockFrequency = 10000000;
////spiSettings.Mode = SpiMode.Mode0;
////spiSettings.DataBitLength = 8;

//using SpiDevice spiDevice = raspberryPiBoard.CreateSpiDevice(spiSettings);

//byte[] readBuffer = new byte[2];
//byte[] writeBuffer = TransferArray(spiDevice, readBuffer);

//for (int i = 0; i < 2; i++)
//{
//    Console.WriteLine($"{writeBuffer[i]}");
//}

//GpioPin pin;

//for (int i = 1; i < raspberryPiBoard.PinCount; i++)
//{
//    try
//    {
//        pin = gpioController.OpenPin(i);
//        Console.WriteLine($"Pin {i}={pin.Read().}");
//    }
//    catch { }

//    Console.WriteLine($"PinMode {i}={gpioController.GetPinMode(i)}");

//    try
//    {
//        gpioController.ClosePin(i);
//    }
//    catch { }
//}


//raspberryPiBoard.ReservePin(CE0, PinUsage.Spi, This);
//raspberryPiBoard.ReservePin(CE1, PinUsage.Spi, This);
//raspberryPiBoard.ReservePin(MISO, PinUsage.Spi, This);
//raspberryPiBoard.ReservePin(MOSI, PinUsage.Spi, This);
//raspberryPiBoard.ReservePin(SCLK, PinUsage.Spi, This);

//Console.WriteLine(raspberryPiBoard.QueryComponentInformation());

//SpiConnectionSettings spiSettings = new SpiConnectionSettings(0, 0)
//{
//    ChipSelectLineActiveState = PinValue.Low
//};

//using SpiDevice spiDevice = raspberryPiBoard.CreateSpiDevice(spiSettings);

//Console.WriteLine($"{nameof(SCLK)}={raspberryPiBoard.DetermineCurrentPinUsage(SCLK)}");

//try
//{
//    GpioPin SCLKPin = gpioController.OpenPin(SCLK);
//    Console.WriteLine(gpioController.GetPinMode(SCLK));

//    //SoilMoistureSensor sms = new SoilMoistureSensor(raspberryPiBoard, gpioController);

//    while (keepRunning)
//    {
//        Console.WriteLine($"{nameof(SCLK)}={SCLKPin.Read()}");
//    }
//}
//catch (Exception ex)
//{
//    Console.WriteLine(ex);
//}
//finally
//{
//    raspberryPiBoard.ReleasePin(CE0, PinUsage.Spi, This);
//    raspberryPiBoard.ReleasePin(CE1, PinUsage.Spi, This);
//    raspberryPiBoard.ReleasePin(MISO, PinUsage.Spi, This);
//    raspberryPiBoard.ReleasePin(MOSI, PinUsage.Spi, This);
//    raspberryPiBoard.ReleasePin(SCLK, PinUsage.Spi, This);
//}

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

//Console.ReadKey(true);









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