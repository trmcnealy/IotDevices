using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RaspberryPiDevices;

public readonly struct FloatingPointDigitSize
{
    public readonly int DigitsBefore;
    public readonly int DigitsAfter;

    public FloatingPointDigitSize(int digitsBefore, int digitsAfter)
    {
        DigitsBefore = digitsBefore;
        DigitsAfter = digitsAfter;
    }

    public override string? ToString()
    {
        return $"DigitSize:{DigitsBefore}.{DigitsAfter}";
    }
}

public sealed class NumericStringFormatter : IFormatProvider, ICustomFormatter
{

    private readonly static Regex stringFormatRegex = new Regex("FP(?<DigitsBefore>\\d+)-(?<DigitsAfter>\\d+)", RegexOptions.ExplicitCapture | RegexOptions.Compiled);

    private static FloatingPointDigitSize ParseFormat(string format)
    {
        Match m = stringFormatRegex.Match(format);

        if (!m.Success)
        {
            throw new Exception();
        }

        return new FloatingPointDigitSize(int.Parse(m.Groups["DigitsBefore"].Value), int.Parse(m.Groups["DigitsAfter"].Value));
    }

    private readonly static Regex floatingPointRegex = new Regex("(?<DigitsBefore>[0-9]+)(?<DigitsAfter>[.0-9]*)", RegexOptions.ExplicitCapture | RegexOptions.Compiled);

    private static FloatingPointDigitSize ParseDigits(string numericString, out int dot)
    {
        Match m = floatingPointRegex.Match(numericString);

        if (!m.Success)
        {
            throw new Exception();
        }

        string DigitsBeforeString = m.Groups["DigitsBefore"].Value;
        string DigitsAfterString = m.Groups["DigitsAfter"].Value;

        int digitsBefore = DigitsBeforeString.Length;

        dot = digitsBefore + 1;
        
        if (DigitsAfterString.StartsWith("."))
        {
            DigitsAfterString = DigitsAfterString.Substring(1);
        }

        if (DigitsAfterString.Length == 0)
        {
            dot = -1;
            return new FloatingPointDigitSize(digitsBefore, 0);
        }

        int digitsAfter = DigitsAfterString.Length;

        return new FloatingPointDigitSize(digitsBefore, digitsAfter - 1);
    }


    public const int FloatSignificantDigits = 7;
    public const int DoubleSignificantDigits = 15;
    public const int ByteSignificantDigits = 7;
    public const int ShortSignificantDigits = 15;
    public const int IntSignificantDigits = 7;
    public const int LongSignificantDigits = 15;

    public static NumericStringFormatter Instance
    {
        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        [MethodImpl(MethodImplOptions.NoInlining)]
        get
        {
            ThreadLocal<NumericStringFormatter> thread = new ThreadLocal<NumericStringFormatter>(() =>
                {
                    return new NumericStringFormatter();
                });

            return thread.Value ?? throw new Exception($"ThreadLocal<{nameof(NumericStringFormatter)}> is null.");
        }
    }

    public object? GetFormat(Type? formatType)
    {
        if (formatType == typeof(ICustomFormatter))
        {
            return this;
        }
        else
        {
            return null;
        }
    }

    public string Format(string? format, object? arg, IFormatProvider? formatProvider)
    {
        if (!Equals(formatProvider))
        {
            return string.Empty;
        }

        if (string.IsNullOrEmpty(format))
        {
            format = "N";
        }

        if (arg is not null)
        {
            string? numericString = arg.ToString();

            if (numericString is not null)
            {
                if (format.StartsWith("N"))
                {
                    return string.Format("#######.#######", numericString);
                }
                if (format.StartsWith("FP"))
                {
                    //Console.WriteLine(numericString);

                    FloatingPointDigitSize fpDS = ParseFormat(format);
                    //Console.WriteLine(fpDS);

                    //int dashIndex = format.IndexOf('-');
                    //int beforeDash = Math.Min(DoubleSignificantDigits, int.Parse(format.Substring(2, dashIndex - 2)));
                    //int afterDash = Math.Min(DoubleSignificantDigits, int.Parse(format.Substring(dashIndex + 1, (format.Length - dashIndex + 1 - 2))));

                    if (numericString.Length < (fpDS.DigitsBefore + fpDS.DigitsAfter))
                    {
                        return numericString;
                    }
                    //Console.WriteLine($"FP{fpDS.DigitsBefore}-{fpDS.DigitsAfter}");

                    FloatingPointDigitSize numericStringDS = ParseDigits(numericString, out int dotIndex);
                    //Console.WriteLine(numericStringDS);
                    //Console.WriteLine(dotIndex);


                    ////Console.WriteLine($"numericStringLength{numericString.Length}");
                    //int dotIndex = numericString.IndexOf('.');
                    ////Console.WriteLine($"dotIndex{dotIndex}");
                    //int beforeDot = numericString.Substring(0, dotIndex).Length;
                    ////Console.WriteLine($"beforeDot{beforeDot}");
                    //int afterDot = numericString.Substring(dotIndex + 1, (numericString.Length - dotIndex - 1)).Length;
                    ////Console.WriteLine($"afterDot{afterDot}");

                    //if (numericStringDS.DigitsBefore >= 15)
                    //{
                    //    beforeDot = 15 - afterDot;
                    //}
                    //if (afterDot >= 15)
                    //{
                    //    afterDot = 15 - beforeDot;
                    //}

                    int beforeDot = Math.Min(numericStringDS.DigitsBefore, fpDS.DigitsBefore);
                    int afterDot = Math.Min(numericStringDS.DigitsAfter, fpDS.DigitsAfter);
                    //Console.WriteLine($"{beforeDot}-{afterDot}");

                    if (dotIndex < 0)
                    {
                        return numericString + ".0";
                    }
                    else if (dotIndex == 0)
                    {
                        return "0" + numericString;
                    }
                    else if (dotIndex == numericString.Length - 1)
                    {
                        return numericString + "0";
                    }
                    else
                    {                    
                        //Console.WriteLine($"numericString.Substring({dotIndex} - {beforeDot}, {beforeDot}) + '.' + numericString.Substring({dotIndex + 1}, {afterDot})");

                        return numericString.Substring(dotIndex - beforeDot, beforeDot) + '.' + numericString.Substring(dotIndex + 1, afterDot);
                    }
                }

                return numericString;
            }
        }

        return string.Empty;
    }

    public static string GetNumericString<T>(T value, int beforeDot, int afterDot)
    {
        return string.Format(Instance, $"{{0:FP{beforeDot}-{afterDot}}}", value);
    }

    //private static byte[] GetDigits(int value)
    //{
    //    if (value < 10)
    //    {
    //        return new byte[] { (byte)value };
    //    }

    //    byte[] digits = GetDigits(value / 10);

    //    return new byte[] { (byte)(value % 10) }.Concat(digits).ToArray();
    //}

    //private static LEDCharacter[] GetDigits(double value)
    //{
    //    ReadOnlySpan<char> strDigits = value.ToString();

    //    int dotIndex = strDigits.IndexOf('.');

    //    LEDCharacter[] LEDCharacters = new LEDCharacter[Math.Min(strDigits.Length, MaxCharacters)];

    //    (int before_count, int after_count) digits = CountDigits(value);

    //    ReadOnlySpan<char> strDigit;
    //    byte digit;

    //    if (digits.before_count >= MaxCharacters)
    //    {
    //        for (int i = 0; i < MaxCharacters; i++)
    //        {
    //            strDigit = strDigits.Slice(i, 1);
    //            digit = byte.Parse(strDigit);
    //            LEDCharacters[i] = LEDCharacter.GetLEDCharacter(digit);
    //        }

    //        return LEDCharacters;
    //    }

    //    if (digits.before_count == 0)
    //    {
    //        LEDCharacters[0] = LEDCharacter.Digit0 | LEDCharacter.Dot;

    //        for (int i = dotIndex; i < dotIndex + digits.after_count; i++)
    //        {
    //            strDigit = strDigits.Slice(i, 1);
    //            digit = byte.Parse(strDigit);
    //            LEDCharacters[i + 1] = LEDCharacter.GetLEDCharacter(digit);
    //        }

    //        return LEDCharacters;
    //    }


    //    int counter = 0;

    //    for (int i = 0; i < digits.before_count; i++)
    //    {
    //        strDigit = strDigits.Slice(i, 1);
    //        digit = byte.Parse(strDigit);
    //        LEDCharacters[++counter] = LEDCharacter.GetLEDCharacter(digit);
    //    }

    //    for (int i = dotIndex; i < dotIndex + digits.after_count; i++)
    //    {
    //        strDigit = strDigits.Slice(i, 1);
    //        if (strDigit[0] == '.')
    //        {
    //            continue;
    //        }
    //        digit = byte.Parse(strDigit);
    //        LEDCharacters[++counter] = LEDCharacter.GetLEDCharacter(digit);
    //    }

    //    return LEDCharacters;
    //}
}
