using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Iot.Device.Ahtxx;
using Iot.Device.Board;
using Iot.Device.Nmea0183;
using Iot.Device.Tm1637;


namespace RaspberryPiDevices;

public class LED4DigitDisplay : IDisposable
{

    private Tm1637 _sensor;

    private bool disposedValue;

    //public LED4DigitDisplay() //: base(Id, nameof(LED4DigitDisplay))
    //{
    //    _sensor = new Tm1637(pinClk, pinDio, PinNumberingScheme.Logical, raspberryPiBoard.CreateGpioController(), shouldDispose: false);
    //    _sensor.Brightness = 7;
    //    _sensor.ScreenOn = true;
    //    _sensor.ClearDisplay();
    //}

    public LED4DigitDisplay(GpioController gpioController, in int pinClk, in int pinDio) //: base(Id, nameof(LED4DigitDisplay))
    {
        _sensor = new Tm1637(pinClk, pinDio, PinNumberingScheme.Logical, gpioController, shouldDispose: false);

        _sensor.Brightness = 7;
        _sensor.ScreenOn = true;

        _sensor.ClearDisplay();
    }

    #region Dctor
    ~LED4DigitDisplay()
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
                Clear();
                _sensor.ScreenOn = false;
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

    public void Clear()
    {
        charactersToDisplay[0] = Character.Nothing;
        charactersToDisplay[1] = Character.Nothing;
        charactersToDisplay[2] = Character.Nothing;
        charactersToDisplay[3] = Character.Nothing;
        _sensor.Display(charactersToDisplay);
    }

    private static readonly Character[] charactersToDisplay = new Character[6]
    {
            Character.Nothing, Character.Nothing, Character.Nothing,
            Character.Nothing, Character.Nothing, Character.Nothing
    };

    private static readonly Character[] nanCharactersToDisplay = new Character[4]
    {
            Character.Minus, Character.Minus, Character.Minus, Character.Minus
    };

    public void Display(ReadOnlySpan<Character> rawData)
    {
        _sensor.Display(rawData);
    }

    public void Display(in byte characterPosition, Character rawData)
    {
        _sensor.Display(characterPosition, rawData);
    }

    public void Display(in TimeOnly time)
    {
        charactersToDisplay[0] = (Character)Enum.Parse(typeof(Character), $"Digit{time.Minute / 10}");
        charactersToDisplay[1] = (Character)Enum.Parse(typeof(Character), $"Digit{time.Minute % 10}") | Character.Dot;
        charactersToDisplay[2] = (Character)Enum.Parse(typeof(Character), $"Digit{time.Second / 10}");
        charactersToDisplay[3] = (Character)Enum.Parse(typeof(Character), $"Digit{time.Second % 10}");
        _sensor.Display(charactersToDisplay);
    }

    public void Display(in int value)
    {
        if (value > 9999 || value < 0)
        {
            Clear();

            _sensor.Display(nanCharactersToDisplay);

            return;
            //throw new Exception();
        }

        byte[] digits = GetDigits(value);

        switch (digits.Length)
        {
            case 1:
            {
                charactersToDisplay[3] = (Character)Enum.Parse(typeof(Character), $"Digit{digits[0]}");
                charactersToDisplay[2] = Character.Nothing;
                charactersToDisplay[1] = Character.Nothing;
                charactersToDisplay[0] = Character.Nothing;
                break;
            }
            case 2:
            {
                charactersToDisplay[3] = (Character)Enum.Parse(typeof(Character), $"Digit{digits[0]}");
                charactersToDisplay[2] = (Character)Enum.Parse(typeof(Character), $"Digit{digits[1]}");
                charactersToDisplay[1] = Character.Nothing;
                charactersToDisplay[0] = Character.Nothing;
                break;
            }
            case 3:
            {
                charactersToDisplay[3] = (Character)Enum.Parse(typeof(Character), $"Digit{digits[0]}");
                charactersToDisplay[2] = (Character)Enum.Parse(typeof(Character), $"Digit{digits[1]}");
                charactersToDisplay[1] = (Character)Enum.Parse(typeof(Character), $"Digit{digits[2]}");
                charactersToDisplay[0] = Character.Nothing;
                break;
            }
            case 4:
            {
                charactersToDisplay[3] = (Character)Enum.Parse(typeof(Character), $"Digit{digits[0]}");
                charactersToDisplay[2] = (Character)Enum.Parse(typeof(Character), $"Digit{digits[1]}");
                charactersToDisplay[1] = (Character)Enum.Parse(typeof(Character), $"Digit{digits[2]}");
                charactersToDisplay[0] = (Character)Enum.Parse(typeof(Character), $"Digit{digits[3]}");
                break;
            }
            default:
            {
                charactersToDisplay[3] = Character.Nothing;
                charactersToDisplay[2] = Character.Nothing;
                charactersToDisplay[1] = Character.Nothing;
                charactersToDisplay[0] = Character.Nothing;
                break;
            }
        }

        _sensor.Display(charactersToDisplay);
    }

    private static readonly double[] multiplies = new double[] { 1, 10, 100, 1_000,
                                                                 10_000, 100_000, 1_000_000,
                                                                 10_000_000, 100_000_000, 1_000_000_000 };

    public void Display(in double value)
    {
        int decimals = CountDigitsAfterDecimal(value);
        int precision = Precision(value);
        int sigs = precision - decimals;

        double Ms = Math.Pow(value, sigs);
        double Mp = Math.Pow(value, precision);

        byte[] digits = new byte[precision + sigs];

        for (int i = 0; i < sigs; i++)
        {
            digits[i] = (byte)(Math.Floor((value % multiplies[sigs]) / multiplies[sigs]) % 10);
        }

        for (int i = sigs; i < precision + sigs; i++)
        {
            digits[sigs + i] = (byte)(Math.Floor((value * multiplies[i - sigs]) % 10) % 10);
        }

        for (int i = 0; i < digits.Length; i++)
        {
            charactersToDisplay[i] = (Character)Enum.Parse(typeof(Character), $"Digit{digits[i]}");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static double RoundToMultiple(in double x, in double m)
    {
        return Math.Round(x / m) * m;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static int Precision(in double x)
    {
        int precision = 0;

        while (x * (double)Math.Pow(10, precision) != Math.Round(x * (double)Math.Pow(10, precision)))
        {
            precision++;
        }

        return precision;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static int CountDigitsAfterDecimal(in double value)
    {
        bool start = false;
        int count = 0;

        foreach (char s in value.ToString())
        {
            if (s == '.')
            {
                start = true;
            }
            else if (start)
            {
                count++;
            }
        }

        return count;
    }

    private static byte[] GetDigits(in int value)
    {

        if (value < 10)
        {
            return new byte[] { (byte)value };
        }

        byte[] digits = GetDigits(value / 10);

        return new byte[] { (byte)(value % 10) }.Concat(digits).ToArray();
    }

}
