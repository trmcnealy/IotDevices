﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Device.I2c;
using System.Numerics;

namespace RaspberryPiDevices
{
    /// <summary>
    /// Helpers for short waits.
    /// </summary>
    public static partial class Utilities
    {

        public const long TicksPerMicrosecond = 10;
        public const long TicksPerMillisecond = TicksPerMicrosecond * 1000;
        public const long TicksPerSecond = TicksPerMillisecond * 1000;   // 10,000,000
        public const long TicksPerMinute = TicksPerSecond * 60;         // 600,000,000
        public const long TicksPerHour = TicksPerMinute * 60;        // 36,000,000,000
        public const long TicksPerDay = TicksPerHour * 24;          // 864,000,000,000

        public const long RaspberryPi4BFrequency = 10000000;

        public static readonly double TickFrequency = (double)TicksPerSecond / (double)RaspberryPi4BFrequency;


        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static double GetTimeInSeconds(this TimeSpan time)
        {
            double value = 0.0;

            if (time.Seconds > 0)
            {
                value += ((double)time.Seconds);
            }
            if (time.Milliseconds > 0)
            {
                value += (((double)time.Milliseconds) / 1_000.0);
            }
            if (time.Microseconds > 0)
            {
                value += (((double)time.Microseconds) / 1_000_000.0);
            }
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static double GetTimeInMinutes(this TimeSpan time)
        {
            double value = 0.0;

            if (time.Seconds > 0)
            {
                value += ((double)time.Minutes);
            }
            if (time.Seconds > 0)
            {
                value += (((double)time.Seconds) / 60.0);
            }
            if (time.Milliseconds > 0)
            {
                value += (((double)time.Milliseconds) / 60_000.0);
            }
            if (time.Microseconds > 0)
            {
                value += (((double)time.Microseconds) / 60_000_000.0);
            }
            return value;
        }

        #region Delay
        /// <summary>
        /// Delay for at least the specified <paramref name="time" />.
        /// </summary>
        /// <param name="time">The amount of time to delay.</param>
        /// <param name="allowThreadYield">
        /// True to allow yielding the thread. If this is set to false, on single-proc systems
        /// this will prevent all other code from running.
        /// </param>
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        public static void Delay(TimeSpan timeSpan, bool allowThreadToYield = true)
        {
            TimeSpan time = timeSpan;
            bool allowThreadYield = allowThreadToYield;

            long start = Stopwatch.GetTimestamp();
            long target = start + (long)(time.Ticks / TickFrequency);

            if (!allowThreadYield)
            {
                do
                {
                    Thread.SpinWait(1);
                }
                while (Stopwatch.GetTimestamp() < target);
            }
            else
            {
                SpinWait spinWait = new SpinWait();
                do
                {
                    spinWait.SpinOnce();
                }
                while (Stopwatch.GetTimestamp() < target);
            }
        }

        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        public static void Delay(int seconds, int milliseconds = 0, int microseconds = 0, bool allowThreadYield = true)
        {
            Delay(TimeSpan.FromTicks((seconds * TicksPerSecond) + (milliseconds * TicksPerMillisecond) + (microseconds * TicksPerMicrosecond)), allowThreadYield);
        }
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        public static void Delay(uint seconds, uint milliseconds = 0, uint microseconds = 0, bool allowThreadYield = true)
        {
            Delay(TimeSpan.FromTicks((seconds * TicksPerSecond) + (milliseconds * TicksPerMillisecond) + (microseconds * TicksPerMicrosecond)), allowThreadYield);
        }

        /// <summary>
        /// Delay for at least the specified <paramref name="microseconds"/>.
        /// </summary>
        /// <param name="microseconds">The number of microseconds to delay.</param>
        /// <param name="allowThreadYield">
        /// True to allow yielding the thread. If this is set to false, on single-proc systems
        /// this will prevent all other code from running.
        /// </param>
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        public static void DelayMicroseconds(int microseconds, bool allowThreadYield = true)
        {
            Delay(TimeSpan.FromTicks(microseconds * TicksPerMicrosecond), allowThreadYield);
        }

        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        public static void DelayMicroseconds(uint microseconds, bool allowThreadYield = true)
        {
            Delay(TimeSpan.FromTicks(microseconds * TicksPerMicrosecond), allowThreadYield);
        }

        /// <summary>
        /// Delay for at least the specified <paramref name="milliseconds"/>
        /// </summary>
        /// <param name="milliseconds">The number of milliseconds to delay.</param>
        /// <param name="allowThreadYield">
        /// True to allow yielding the thread. If this is set to false, on single-proc systems
        /// this will prevent all other code from running.
        /// </param>
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        public static void DelayMilliseconds(int milliseconds, bool allowThreadYield = true)
        {
            /* We have this as a separate method for now to make calling code clearer
             * and to allow us to add additional logic to the millisecond wait in the
             * future. If waiting only 1 millisecond we still have ample room for more
             * complicated logic. For 1 microsecond that isn't the case.
             */

            Delay(TimeSpan.FromTicks(milliseconds * TicksPerMillisecond), allowThreadYield);
        }
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        public static void DelayMilliseconds(uint milliseconds, bool allowThreadYield = true)
        {
            /* We have this as a separate method for now to make calling code clearer
             * and to allow us to add additional logic to the millisecond wait in the
             * future. If waiting only 1 millisecond we still have ample room for more
             * complicated logic. For 1 microsecond that isn't the case.
             */

            Delay(TimeSpan.FromTicks(milliseconds * TicksPerMillisecond), allowThreadYield);
        }
        #endregion

        #region ToDigitArray
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        public static Digit[] ToDigitArray(this in sbyte value, out bool isPositive)
        {
            isPositive = (value >= 0);

            int n = value;
            int j = 0;
            int len = value.ToString().Length;
            Digit[] arr = new Digit[len];

            while (n != 0)
            {
                arr[len - j - 1] = new Digit(n % 10);
                n = n / 10;
                j++;
            }

            return arr;
        }

        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        public static Digit[] ToDigitArray(this in byte value, out bool isPositive)
        {
            isPositive = (true);

            uint n = value;
            int j = 0;
            int len = value.ToString().Length;
            Digit[] arr = new Digit[len];

            while (n != 0)
            {
                arr[len - j - 1] = new Digit(n % 10);
                n = n / 10;
                j++;
            }

            return arr;
        }

        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        public static Digit[] ToDigitArray(this in int value, out bool isPositive)
        {
            isPositive = (value >= 0);

            int n = value;
            int j = 0;
            int len = value.ToString().Length;
            Digit[] arr = new Digit[len];

            while (n != 0)
            {
                arr[len - j - 1] = new Digit(n % 10);
                n = n / 10;
                j++;
            }

            return arr;
        }

        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        public static Digit[] ToDigitArray(this in uint value, out bool isPositive)
        {
            isPositive = (true);

            uint n = value;
            int j = 0;
            int len = value.ToString().Length;
            Digit[] arr = new Digit[len];

            while (n != 0)
            {
                arr[len - j - 1] = new Digit(n % 10);
                n = n / 10;
                j++;
            }

            return arr;
        }

        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        public static Digit[] ToDigitArray(this in long value, out bool isPositive)
        {
            isPositive = (value >= 0);

            long n = value;
            int j = 0;
            int len = value.ToString().Length;
            Digit[] arr = new Digit[len];

            while (n != 0)
            {
                arr[len - j - 1] = new Digit(n % 10);
                n = n / 10;
                j++;
            }

            return arr;
        }

        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        public static Digit[] ToDigitArray(this in ulong value, out bool isPositive)
        {
            isPositive = (true);

            ulong n = value;
            int j = 0;
            int len = value.ToString().Length;
            Digit[] arr = new Digit[len];

            while (n != 0)
            {
                arr[len - j - 1] = new Digit(n % 10);
                n = n / 10;
                j++;
            }

            return arr;
        }
        #endregion

        #region DelayAsync
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        public static async Task DelayAsync(TimeSpan timeSpan)
        {
            await Task.Delay(timeSpan).ConfigureAwait(false);
        }

        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        public static async Task DelayAsync(int seconds, int milliseconds = 0, int microseconds = 0)
        {
            await DelayAsync(TimeSpan.FromTicks((seconds * TicksPerSecond) + (milliseconds * TicksPerMillisecond) + (microseconds * TicksPerMicrosecond))).ConfigureAwait(false);
        }
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        public static async Task DelayAsync(uint seconds, uint milliseconds = 0, uint microseconds = 0)
        {
            await DelayAsync(TimeSpan.FromTicks((seconds * TicksPerSecond) + (milliseconds * TicksPerMillisecond) + (microseconds * TicksPerMicrosecond))).ConfigureAwait(false);
        }


        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        public static async Task DelayMicrosecondsAsync(int microseconds)
        {
            await DelayAsync(TimeSpan.FromTicks(microseconds * TicksPerMicrosecond)).ConfigureAwait(false);
        }

        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        public static async Task DelayMicrosecondsAsync(uint microseconds)
        {
            await DelayAsync(TimeSpan.FromTicks(microseconds * TicksPerMicrosecond)).ConfigureAwait(false);
        }


        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        public static async Task DelayMillisecondsAsync(int milliseconds)
        {
            await DelayAsync(TimeSpan.FromTicks(milliseconds * TicksPerMillisecond)).ConfigureAwait(false);
        }
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
        public static async Task DelayMillisecondsAsync(uint milliseconds)
        {
            await DelayAsync(TimeSpan.FromTicks(milliseconds * TicksPerMillisecond)).ConfigureAwait(false);
        }
        #endregion


        public static void ThrowIfFalse(this bool value,
                                        [CallerMemberName] string memberName = "",
                                        [CallerFilePath] string sourceFilePath = "",
                                        [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!value)
            {
                throw new Exception($"{memberName} {sourceFilePath} {sourceLineNumber}");
            }
        }


        public static byte BinaryOp_OR(this IEnumerable<byte> source)
        {
            byte sum = 0;

            foreach (byte value in source)
            {
                sum |= value;
            }

            return sum;
        }

        //public static TSource BinaryOp_OR<TSource>(this IEnumerable<TSource> source)
        //    where TSource : struct, IBinaryInteger<TSource>
        //{
        //    TSource sum = TSource.Zero;

        //    foreach (TSource value in source)
        //    {
        //        Console.WriteLine(Convert.ToString(value, toBase: 2));

        //        sum |= value;

        //        Console.WriteLine($"{sum:X2}");
        //    }

        //    return sum;
        //}
        public static List<byte> PerformBusScan(this I2cBus bus, in byte lowest = 0x3, in byte highest = 0x77)
        {
            byte result = 0;
            List<byte> ret = new List<byte>();
            for (byte addr = lowest; addr <= highest; addr++)
            {
                try
                {
                    using (I2cDevice device = bus.CreateDevice(addr))
                    {
                        result = device.ReadByte();
                        ret.Add(addr);

                        if (result != 0)
                        {
                            Console.WriteLine("PerformBusScan ReadByte Error");
                        }

                        bus.RemoveDevice(addr);
                    }
                }
                catch
                {
                }
            }

            return ret;
        }

        public static List<int> PerformBusScan(this I2cBus bus, in int lowest = 0x3, in int highest = 0x77)
        {
            byte result = 0;
            List<int> ret = new List<int>();
            for (int addr = lowest; addr <= highest; addr++)
            {
                try
                {
                    using (I2cDevice device = bus.CreateDevice(addr))
                    {
                        result = device.ReadByte();
                        ret.Add(addr);

                        if (result != 0)
                        {
                            Console.WriteLine("PerformBusScan ReadByte Error");
                        }

                        bus.RemoveDevice(addr);
                    }
                }
                catch
                {
                }
            }

            return ret;
        }

        public static (List<byte> FoundDevices, byte LowestAddress, byte HighestAddress) PerformBusScan(this I2cBus bus,
                                                                                                        IProgress<float>? progress,
                                                                                                        in byte lowestAddress = 0x08,
                                                                                                        in byte highestAddress = 0x77)
        {
            List<byte> addresses = new();

            byte numberOfDevicesToScan = (byte)(highestAddress - lowestAddress);

            float currentPercentage = 0;
            float stepPerDevice = 100.0f / numberOfDevicesToScan;

            if (numberOfDevicesToScan <= 0)
            {
                return (addresses, 0, 0);
            }

            for (byte addr = lowestAddress; addr <= highestAddress; addr++)
            {
                try
                {
                    using (I2cDevice device = bus.CreateDevice(addr))
                    {
                        device.ReadByte();
                        addresses.Add(addr);
                    }
                }
                catch (Exception x) when (x is IOException || x is UnauthorizedAccessException || x is TimeoutException)
                {
                }

                currentPercentage += stepPerDevice;

                if (progress != null)
                {
                    progress.Report(currentPercentage);
                }
            }

            if (progress != null)
            {
                progress.Report(100.0f);
            }

            return (addresses, lowestAddress, highestAddress);
        }


        public static string ToUserReadableTable(this (List<byte> FoundDevices, byte LowestAddress, byte HighestAddress) data)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("\n     0  1  2  3  4  5  6  7  8  9  a  b  c  d  e  f");
            stringBuilder.Append(Environment.NewLine);

            for (byte startingRowAddress = 0; startingRowAddress < 128; startingRowAddress += 16)
            {
                stringBuilder.Append($"{startingRowAddress:x2}: ");

                for (byte rowAddress = 0; rowAddress < 16; rowAddress++)
                {
                    byte deviceAddress = (byte)(startingRowAddress + rowAddress);

                    // Skip the unwanted addresses.
                    if (deviceAddress < data.LowestAddress || deviceAddress > data.HighestAddress)
                    {
                        stringBuilder.Append("   ");
                        continue;
                    }

                    if (data.FoundDevices.Contains(deviceAddress))
                    {
                        stringBuilder.Append($"{deviceAddress:x2} ");
                    }
                    else
                    {
                        stringBuilder.Append("-- ");
                    }

                }

                stringBuilder.Append(Environment.NewLine);
            }

            return stringBuilder.ToString();
        }

        public static string ToTable(this List<byte> FoundDevices)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("Table");

            foreach (byte deviceAddress in FoundDevices)
            {
                stringBuilder.Append($"{deviceAddress:x2} ");
            }

            return stringBuilder.ToString();
        }

        public static string ToTable(this List<int> FoundDevices)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine();

            foreach (int deviceAddress in FoundDevices)
            {
                stringBuilder.AppendLine($"{deviceAddress:x2} ");
            }

            stringBuilder.AppendLine();

            return stringBuilder.ToString();
        }
    }
}
