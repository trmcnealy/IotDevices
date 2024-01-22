using System.Numerics;
using System.Runtime.CompilerServices;

namespace RaspberryPiDevices;

public record struct Range<T> : IEquatable<T>
    where T : INumber<T>
{
    public T Upper;
    public T Lower;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Range(T upper, T lower)
    {
        Upper = upper;
        Lower = lower;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool WithIn(T value)
    {
        return (value - Lower) <= (Upper - Lower);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public T Limit(T value)
    {
        if (value > Upper)
        {
            return Upper;
        }
        if (value < Lower)
        {
            return Lower;
        }
        return value;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly bool Equals(T? other)
    {
        if ((other is null) || (other > Upper) || (other < Lower))
        {
            return false;
        }
        return true;
    }
}
