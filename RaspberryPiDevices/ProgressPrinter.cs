using System.Runtime.CompilerServices;

namespace RaspberryPiDevices;

public sealed class ProgressPrinter : IProgress<float>
{
    internal const char Lower8thBlockChar = '▁';
    internal const char FullBlockChar = '█';

    internal static readonly char[] Pattern0 = new char[10]
    {
        Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar
    };
    internal static readonly string PatternString0 = new string(Pattern0);

    internal static readonly char[] Pattern1 = new char[10]
    {
        FullBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar
    };
    internal static readonly string PatternString1 = new string(Pattern1);

    internal static readonly char[] Pattern2 = new char[10]
    {
        Lower8thBlockChar, FullBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar
    };
    internal static readonly string PatternString2 = new string(Pattern2);

    internal static readonly char[] Pattern3 = new char[10]
    {
        Lower8thBlockChar, Lower8thBlockChar, FullBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar
    };
    internal static readonly string PatternString3 = new string(Pattern3);

    internal static readonly char[] Pattern4 = new char[10]
    {
        Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, FullBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar
    };
    internal static readonly string PatternString4 = new string(Pattern4);

    internal static readonly char[] Pattern5 = new char[10]
    {
        Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, FullBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar
    };
    internal static readonly string PatternString5 = new string(Pattern5);

    internal static readonly char[] Pattern6 = new char[10]
    {
        Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, FullBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar
    };
    internal static readonly string PatternString6 = new string(Pattern6);

    internal static readonly char[] Pattern7 = new char[10]
    {
        Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, FullBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar
    };
    internal static readonly string PatternString7 = new string(Pattern7);

    internal static readonly char[] Pattern8 = new char[10]
    {
        Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, FullBlockChar, Lower8thBlockChar, Lower8thBlockChar
    };
    internal static readonly string PatternString8 = new string(Pattern8);

    internal static readonly char[] Pattern9 = new char[10]
    {
        Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, Lower8thBlockChar, FullBlockChar, Lower8thBlockChar
    };
    internal static readonly string PatternString9 = new string(Pattern9);

    internal static readonly string[] Patterns =
    {
        PatternString0, PatternString1, PatternString2, PatternString3, PatternString4, PatternString5, PatternString6, PatternString7, PatternString8, PatternString9
    };

    internal int _patternIndex;

    //private static object? _lock;
    //private static ProgressPrinter _Instance;

    //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    static ProgressPrinter()
    {
        //if (_lock is null)
        //{
        //    _lock = new object();

        //    lock (_lock)
        //    {
        //        if (_Instance == null)
        //        {
        //            _Instance = new ProgressPrinter();
        //        }
        //    }
        //}
    }

    public static ProgressPrinter Instance
    {
        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        [MethodImpl(MethodImplOptions.NoInlining)]
        get
        {
            ThreadLocal<ProgressPrinter> progressPrinterThread = new ThreadLocal<ProgressPrinter>(() =>
                {
                    return new ProgressPrinter();
                });

            return progressPrinterThread.Value ?? throw new Exception("ThreadLocal<ProgressPrinter> is null.");
        }
    }

    //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private ProgressPrinter()
    {
        _patternIndex = 0;
    }

    //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Report(float value)
    {
        Console.Write($"\rPlease wait. {value:F0}% done.");
    }

    //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Report()
    {
        Console.Write($"\r{Patterns[_patternIndex % 10]}");
        ++_patternIndex;
    }
}
