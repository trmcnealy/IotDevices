using System;
using System.Collections;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Device.Spi;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Iot.Device.Board;
using Iot.Device.FtCommon;
using Iot.Device.Max7219;
using Iot.Device.Tm1637;

using static System.Runtime.InteropServices.JavaScript.JSType;


namespace RaspberryPiDevices;

/// <summary>
/// * LED Segments:         a
/// *                     ----
/// *                   f|    |b
/// *                    |  g |
/// *                     ----
/// *                   e|    |c
/// *                    |    |
/// *                     ----  o dp
/// *                       d
/// *   Register bits:
/// *      bit:  7  6  5  4  3  2  1  0
/// *           dp  a  b  c  d  e  f  g
/// MAX7219 VCC to RPi 5V, Pin 2
/// MAX7219 GND to RPi GND, Pin 6
/// MAX7219 DIN to RPi GPIO 10 (MOSI), Pin 10
/// MAX7219 CS to RPi GPIO 8 (SPI CSO), Pin 8
/// MAX7219 CLK to RPi GPIO11 (SPI CLK), Pin 11
/// </summary>
public class LED8DigitDisplay : IDisposable
{
    public static byte MaxCharacters => 8;

    private Max7219 _devices;

    private bool disposedValue;

    //public LED8DigitDisplay() //: base(Id, nameof(LED8DigitDisplay))
    //{
    //    _devices = new Tm1637(pinClk, pinDio, PinNumberingScheme.Logical, raspberryPiBoard.CreateGpioController(), shouldDispose: false);
    //    _devices.Brightness = 7;
    //    _devices.ScreenOn = true;
    //    _devices.ClearDisplay();
    //}

    public LED8DigitDisplay(RaspberryPiBoard raspberryPiBoard, int busId = 0, int chipSelectLine = 0) //: base(Id, nameof(LED8DigitDisplay))
    {
        SpiConnectionSettings connectionSettings = new SpiConnectionSettings(busId, chipSelectLine)
        {
            ClockFrequency = Iot.Device.Max7219.Max7219.SpiClockFrequency,
            Mode = Iot.Device.Max7219.Max7219.SpiMode
        };

        _devices = new Max7219(raspberryPiBoard.CreateSpiDevice(connectionSettings));

        _devices.Init();
        //_devices.Brightness(15);

        Clear();

        for (int i = 0; i < 8; i++)
        {
            _devices[i] = 0b1111_1111;            
        }
        _devices.Flush();

        //_devices[0, 0] = LEDCharacter.Digit1;
        //_devices[0, 1] = LEDCharacter.Digit1;
        //_devices[0, 2] = LEDCharacter.Digit1 | LEDCharacter.Dot;
        //_devices[0, 3] = LEDCharacter.Digit1;        
        //_devices[1, 0] = LEDCharacter.Digit1;
        //_devices[1, 1] = LEDCharacter.Digit1;
        //_devices[1, 2] = LEDCharacter.Digit1;
        //_devices[1, 3] = LEDCharacter.Digit1;

        //LEDCharacter[] chars = new LEDCharacter[]
        //{
        //    LEDCharacter.Digit0, LEDCharacter.Nothing, LEDCharacter.Nothing, LEDCharacter.Nothing
        //};

        Console.WriteLine($"CascadedDevices {_devices.CascadedDevices}");

        //for (int i = 0; i < _devices.CascadedDevices; i++)
        //{
        //    for (int digit = 0; digit < 8; digit++)
        //    {
        //        _devices[i, digit] = 0b1111_1111;
        //    }
        //}
    }

    #region Dctor
    ~LED8DigitDisplay()
    {
        Dispose(disposing: false);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                //managed
                _devices.ClearAll();
                _devices.Dispose();
            }

            //unmanaged
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion

    private static readonly byte[,] ClearCommand = new byte[,] { { 0 }, { 0 } };
    public void Clear()
    {
        _devices.ClearAll();
        _devices.Flush();
    }

    private static readonly byte[,] charactersToDisplay = new byte[1, 8]
    {
        {
            LEDCharacter.Nothing, LEDCharacter.Nothing, LEDCharacter.Nothing, LEDCharacter.Nothing,
            LEDCharacter.Nothing, LEDCharacter.Nothing, LEDCharacter.Nothing, LEDCharacter.Nothing
        }
    };

    public void Display(ReadOnlySpan<byte> rawData)
    {

        //byte[] toTransfer = new byte[MaxCharacters];
        //_devices.WriteBuffer(MemoryMarshal.AsBytes(rawData));
    }
    public void Display(ReadOnlySpan<LEDCharacter> rawData)
    {
        byte[,] toTransfer = new byte[2, MaxCharacters];
        //_devices.WriteBuffer(MemoryMarshal.AsBytes(rawData));
    }

    public void Display(byte characterPosition, LEDCharacter rawData)
    {
        for (int i = 0; i < 2; i++)
        {
            for (int digit = 0; digit < 8; digit++)
            {
                _devices[i, digit] = (byte)rawData;
            }

            //_devices[i, digit]
            //    toTransfer[_charactersOrder[j]] = (byte)LEDCharacter.Nothing;
        }
    }

    public void Display(int value)
    {
        if (value > 99_999_999 || value < -99_999_999)
        {
            Clear();
            return;
            //throw new Exception();
        }

        byte[] digits = GetDigits(value);

        switch (digits.Length)
        {
            case 1:
            {
                charactersToDisplay[0, 7] = LEDCharacter.GetLEDCharacter(digits[0]);
                charactersToDisplay[0, 6] = LEDCharacter.Nothing;
                charactersToDisplay[0, 5] = LEDCharacter.Nothing;
                charactersToDisplay[0, 4] = LEDCharacter.Nothing;
                charactersToDisplay[0, 3] = LEDCharacter.Nothing;
                charactersToDisplay[0, 2] = LEDCharacter.Nothing;
                charactersToDisplay[0, 1] = LEDCharacter.Nothing;
                charactersToDisplay[0, 0] = LEDCharacter.Nothing;
                break;
            }
            case 2:
            {
                charactersToDisplay[0, 7] = LEDCharacter.GetLEDCharacter(digits[0]);
                charactersToDisplay[0, 6] = LEDCharacter.GetLEDCharacter(digits[1]);
                charactersToDisplay[0, 5] = LEDCharacter.Nothing;
                charactersToDisplay[0, 4] = LEDCharacter.Nothing;
                charactersToDisplay[0, 3] = LEDCharacter.Nothing;
                charactersToDisplay[0, 2] = LEDCharacter.Nothing;
                charactersToDisplay[0, 1] = LEDCharacter.Nothing;
                charactersToDisplay[0, 0] = LEDCharacter.Nothing;
                break;
            }
            case 3:
            {
                charactersToDisplay[0, 7] = LEDCharacter.GetLEDCharacter(digits[0]);
                charactersToDisplay[0, 6] = LEDCharacter.GetLEDCharacter(digits[1]);
                charactersToDisplay[0, 5] = LEDCharacter.GetLEDCharacter(digits[2]);
                charactersToDisplay[0, 4] = LEDCharacter.Nothing;
                charactersToDisplay[0, 3] = LEDCharacter.Nothing;
                charactersToDisplay[0, 2] = LEDCharacter.Nothing;
                charactersToDisplay[0, 1] = LEDCharacter.Nothing;
                charactersToDisplay[0, 0] = LEDCharacter.Nothing;
                break;
            }
            case 4:
            {
                charactersToDisplay[0, 7] = LEDCharacter.GetLEDCharacter(digits[0]);
                charactersToDisplay[0, 6] = LEDCharacter.GetLEDCharacter(digits[1]);
                charactersToDisplay[0, 5] = LEDCharacter.GetLEDCharacter(digits[2]);
                charactersToDisplay[0, 4] = LEDCharacter.GetLEDCharacter(digits[3]);
                charactersToDisplay[0, 3] = LEDCharacter.Nothing;
                charactersToDisplay[0, 2] = LEDCharacter.Nothing;
                charactersToDisplay[0, 1] = LEDCharacter.Nothing;
                charactersToDisplay[0, 0] = LEDCharacter.Nothing;
                break;
            }
            case 5:
            {
                charactersToDisplay[0, 7] = LEDCharacter.GetLEDCharacter(digits[0]);
                charactersToDisplay[0, 6] = LEDCharacter.GetLEDCharacter(digits[1]);
                charactersToDisplay[0, 5] = LEDCharacter.GetLEDCharacter(digits[2]);
                charactersToDisplay[0, 4] = LEDCharacter.GetLEDCharacter(digits[3]);
                charactersToDisplay[0, 3] = LEDCharacter.GetLEDCharacter(digits[4]);
                charactersToDisplay[0, 2] = LEDCharacter.Nothing;
                charactersToDisplay[0, 1] = LEDCharacter.Nothing;
                charactersToDisplay[0, 0] = LEDCharacter.Nothing;
                break;
            }
            case 6:
            {
                charactersToDisplay[0, 7] = LEDCharacter.GetLEDCharacter(digits[0]);
                charactersToDisplay[0, 6] = LEDCharacter.GetLEDCharacter(digits[1]);
                charactersToDisplay[0, 5] = LEDCharacter.GetLEDCharacter(digits[2]);
                charactersToDisplay[0, 4] = LEDCharacter.GetLEDCharacter(digits[3]);
                charactersToDisplay[0, 3] = LEDCharacter.GetLEDCharacter(digits[4]);
                charactersToDisplay[0, 2] = LEDCharacter.GetLEDCharacter(digits[5]);
                charactersToDisplay[0, 1] = LEDCharacter.Nothing;
                charactersToDisplay[0, 0] = LEDCharacter.Nothing;
                break;
            }
            case 7:
            {
                charactersToDisplay[0, 7] = LEDCharacter.GetLEDCharacter(digits[0]);
                charactersToDisplay[0, 6] = LEDCharacter.GetLEDCharacter(digits[1]);
                charactersToDisplay[0, 5] = LEDCharacter.GetLEDCharacter(digits[2]);
                charactersToDisplay[0, 4] = LEDCharacter.GetLEDCharacter(digits[3]);
                charactersToDisplay[0, 3] = LEDCharacter.GetLEDCharacter(digits[4]);
                charactersToDisplay[0, 2] = LEDCharacter.GetLEDCharacter(digits[5]);
                charactersToDisplay[0, 1] = LEDCharacter.GetLEDCharacter(digits[6]);
                charactersToDisplay[0, 0] = LEDCharacter.Nothing;
                break;
            }
            case 8:
            {
                charactersToDisplay[0, 7] = LEDCharacter.GetLEDCharacter(digits[0]);
                charactersToDisplay[0, 6] = LEDCharacter.GetLEDCharacter(digits[1]);
                charactersToDisplay[0, 5] = LEDCharacter.GetLEDCharacter(digits[2]);
                charactersToDisplay[0, 4] = LEDCharacter.GetLEDCharacter(digits[3]);
                charactersToDisplay[0, 3] = LEDCharacter.GetLEDCharacter(digits[4]);
                charactersToDisplay[0, 2] = LEDCharacter.GetLEDCharacter(digits[5]);
                charactersToDisplay[0, 1] = LEDCharacter.GetLEDCharacter(digits[6]);
                charactersToDisplay[0, 0] = LEDCharacter.GetLEDCharacter(digits[7]);
                break;
            }
            default:
            {
                charactersToDisplay[0, 7] = LEDCharacter.GetLEDCharacter(digits[0]);
                charactersToDisplay[0, 6] = LEDCharacter.Nothing;
                charactersToDisplay[0, 5] = LEDCharacter.Nothing;
                charactersToDisplay[0, 4] = LEDCharacter.Nothing;
                charactersToDisplay[0, 3] = LEDCharacter.Nothing;
                charactersToDisplay[0, 2] = LEDCharacter.Nothing;
                charactersToDisplay[0, 1] = LEDCharacter.Nothing;
                charactersToDisplay[0, 0] = LEDCharacter.Nothing;
                break;
            }
        }

        //for (int digit = 0; digit < MaxCharacters; digit++)
        //{
        //    if (digit < digits.Length)
        //    {
        //        _devices[0, digit] = digits[digit];
        //    }
        //    else
        //    {
        //        _devices[0, digit] = LEDCharacter.Nothing;
        //    }
        //}

        _devices.WriteBuffer(charactersToDisplay);
        //_devices.Flush();
    }

    private static readonly double[] multiplies = new double[] { 1, 10, 100, 1_000,
                                                                 10_000, 100_000, 1_000_000,
                                                                 10_000_000, 100_000_000, 1_000_000_000 };

    public void Display(double value)
    {
        LEDCharacter[] Digits = GetDigits(value);

        //for (int i = 0; i < _devices.CascadedDevices; ++i)
        //{
        for (int i = 0; i < MaxCharacters; ++i)
        {
            _devices[i] = Digits[i];
        }
        //}

        _devices.Flush();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static double RoundToMultiple(double x, double m)
    {
        return Math.Round(x / m) * m;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static int Precision(double x)
    {
        int precision = 0;

        while (x * (double)Math.Pow(10, precision) != Math.Round(x * (double)Math.Pow(10, precision)))
        {
            ++precision;
        }

        return precision;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static (int before_count, int after_count) CountDigits(double value)
    {
        bool start = false;
        int before_count = 0;
        int after_count = 0;

        foreach (char s in value.ToString())
        {
            if (s == '.')
            {
                start = true;
            }
            else if (!start)
            {
                ++before_count;
            }
            else if (start)
            {
                ++after_count;
            }
        }

        return new(before_count, after_count);
    }

    private static byte[] GetDigits(int value)
    {
        if (value < 10)
        {
            return new byte[] { (byte)value };
        }

        byte[] digits = GetDigits(value / 10);

        return new byte[] { (byte)(value % 10) }.Concat(digits).ToArray();
    }

    private static LEDCharacter[] GetDigits(double value)
    {
        ReadOnlySpan<char> strDigits = value.ToString();

        int dotIndex = strDigits.IndexOf('.');

        LEDCharacter[] LEDCharacters = new LEDCharacter[Math.Min(strDigits.Length, MaxCharacters)];

        (int before_count, int after_count) digits = CountDigits(value);

        ReadOnlySpan<char> strDigit;
        byte digit;

        if (digits.before_count >= MaxCharacters)
        {
            for (int i = 0; i < MaxCharacters; i++)
            {
                strDigit = strDigits.Slice(i, 1);
                digit = byte.Parse(strDigit);
                LEDCharacters[i] = LEDCharacter.GetLEDCharacter(digit);
            }

            return LEDCharacters;
        }

        if (digits.before_count == 0)
        {
            LEDCharacters[0] = LEDCharacter.Digit0 | LEDCharacter.Dot;

            for (int i = dotIndex; i < dotIndex + digits.after_count; i++)
            {
                strDigit = strDigits.Slice(i, 1);
                digit = byte.Parse(strDigit);
                LEDCharacters[i + 1] = LEDCharacter.GetLEDCharacter(digit);
            }

            return LEDCharacters;
        }


        int counter = 0;

        for (int i = 0; i < digits.before_count; i++)
        {
            strDigit = strDigits.Slice(i, 1);
            digit = byte.Parse(strDigit);
            LEDCharacters[++counter] = LEDCharacter.GetLEDCharacter(digit);
        }

        for (int i = dotIndex; i < dotIndex + digits.after_count; i++)
        {
            strDigit = strDigits.Slice(i, 1);
            if (strDigit[0] == '.')
            {
                continue;
            }
            digit = byte.Parse(strDigit);
            LEDCharacters[++counter] = LEDCharacter.GetLEDCharacter(digit);
        }

        return LEDCharacters;
    }

}
