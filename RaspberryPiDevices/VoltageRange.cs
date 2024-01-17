using System.Runtime.CompilerServices;

using UnitsNet;

namespace RaspberryPiDevices
{
    public record struct VoltageRange(double Lower,
                                      double Upper) : IEquatable<double>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public double MaxMin(double voltage)
        {
            return Math.Max(Lower, Math.Min(voltage, Upper));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public readonly bool Equals(double voltage)
        {
            if ((Lower <= voltage) && (voltage <= Upper))
            {
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool operator ==(VoltageRange range, double value)
        {
            return range.Equals(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool operator !=(VoltageRange range, double value)
        {
            return !range.Equals(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool operator ==(double value, VoltageRange range)
        {
            return range.Equals(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool operator !=(double value, VoltageRange range)
        {
            return !range.Equals(value);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public override string? ToString()
        {
            return $"Lower:{Lower} Upper:{Upper}";
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static implicit operator (double Lower, double Upper)(VoltageRange value)
        {
            return (value.Lower, value.Upper);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static implicit operator VoltageRange((double Lower, double Upper) value)
        {
            return new VoltageRange(value.Lower, value.Upper);
        }


    }
}
