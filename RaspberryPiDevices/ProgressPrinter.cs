using System.Runtime.CompilerServices;

namespace RaspberryPiDevices;

public sealed class ProgressPrinter : IProgress<float>
    {
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
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Report(float value)
        {
            Console.Write($"\rPlease wait. {value:F0}% done.");
        }
    }
