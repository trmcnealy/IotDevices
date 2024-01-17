using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace RaspberryPiDevices;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public readonly struct Digit : IEquatable<Digit>,
                           IComparable<Digit>,
                           IComparable,
                           IConvertible
{
    private readonly byte m_value;

    public const byte MinValue = 0;
    public const byte MaxValue = 9;
    public const byte ModValue = 10;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Digit()
    {
        m_value = 0x0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Digit(sbyte value)
    {
        m_value = (byte)(value % ModValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Digit(byte value)
    {
        m_value = (byte)(value % ModValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Digit(short value)
    {
        m_value = (byte)(value % ModValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Digit(ushort value)
    {
        m_value = (byte)(value % ModValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Digit(int value)
    {
        m_value = (byte)(value % ModValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Digit(uint value)
    {
        m_value = (byte)(value % ModValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Digit(long value)
    {
        m_value = (byte)(value % ModValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Digit(ulong value)
    {
        m_value = (byte)(value % ModValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override string ToString()
    {
        return m_value.ToString("D1");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public string ToString(IFormatProvider? provider)
    {
        return m_value.ToString("D1");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Equals(Digit other)
    {
        return m_value == other.m_value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override bool Equals(object? obj)
    {
        return obj is Digit other && Equals(other);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override int GetHashCode()
    {
        return m_value.GetHashCode();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator ==(Digit left,
                                   Digit right)
    {
        return left.Equals(right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator !=(Digit left,
                                   Digit right)
    {
        return !left.Equals(right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int CompareTo(Digit other)
    {
        return m_value.CompareTo(other.m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int CompareTo(object? obj)
    {
        if (ReferenceEquals(null,
                           obj))
        {
            return 1;
        }

        return obj is Digit other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(Digit)}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator <(Digit left,
                                  Digit right)
    {
        return left.CompareTo(right) < 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator >(Digit left,
                                  Digit right)
    {
        return left.CompareTo(right) > 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator <=(Digit left,
                                   Digit right)
    {
        return left.CompareTo(right) <= 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator >=(Digit left,
                                   Digit right)
    {
        return left.CompareTo(right) >= 0;
    }

    #region implicit operators
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator sbyte(Digit value)
    {
        return (sbyte)value.m_value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator byte(Digit value)
    {
        return value.m_value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator short(Digit value)
    {
        return value.m_value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator ushort(Digit value)
    {
        return value.m_value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator int(Digit value)
    {
        return value.m_value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator uint(Digit value)
    {
        return value.m_value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator long(Digit value)
    {
        return value.m_value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator ulong(Digit value)
    {
        return value.m_value;
    }
    #endregion

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator +(Digit value)
    {
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator -(Digit value)
    {
        return value;
    }

    #region operator +
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator +(Digit lhs, sbyte rhs)
    {
        return new Digit(lhs.m_value + rhs);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator +(Digit lhs, byte rhs)
    {
        return new Digit(lhs.m_value + rhs);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator +(Digit lhs, short rhs)
    {
        return new Digit(lhs.m_value + rhs);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator +(Digit lhs, ushort rhs)
    {
        return new Digit(lhs.m_value + rhs);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator +(Digit lhs, int rhs)
    {
        return new Digit(lhs.m_value + rhs);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator +(Digit lhs, uint rhs)
    {
        return new Digit(lhs.m_value + rhs);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator +(Digit lhs, long rhs)
    {
        return new Digit(lhs.m_value + rhs);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator +(Digit lhs, ulong rhs)
    {
        return new Digit(lhs.m_value + rhs);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator +(sbyte lhs, Digit rhs)
    {
        return new Digit(lhs + rhs.m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator +(byte lhs, Digit rhs)
    {
        return new Digit(lhs + rhs.m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator +(short lhs, Digit rhs)
    {
        return new Digit(lhs + rhs.m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator +(ushort lhs, Digit rhs)
    {
        return new Digit(lhs + rhs.m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator +(int lhs, Digit rhs)
    {
        return new Digit(lhs + rhs.m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator +(uint lhs, Digit rhs)
    {
        return new Digit(lhs + rhs.m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator +(long lhs, Digit rhs)
    {
        return new Digit(lhs + rhs.m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator +(ulong lhs, Digit rhs)
    {
        return new Digit(lhs + rhs.m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator +(Digit lhs, Digit rhs)
    {
        return new Digit(lhs.m_value + rhs.m_value);
    }
    #endregion

    #region operator -
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator -(Digit lhs, sbyte rhs)
    {
        return new Digit(lhs.m_value - rhs);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator -(Digit lhs, byte rhs)
    {
        return new Digit(lhs.m_value - rhs);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator -(Digit lhs, short rhs)
    {
        return new Digit(lhs.m_value - rhs);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator -(Digit lhs, ushort rhs)
    {
        return new Digit(lhs.m_value - rhs);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator -(Digit lhs, int rhs)
    {
        return new Digit(lhs.m_value - rhs);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator -(Digit lhs, uint rhs)
    {
        return new Digit(lhs.m_value - rhs);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator -(Digit lhs, long rhs)
    {
        return new Digit(lhs.m_value - rhs);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator -(Digit lhs, ulong rhs)
    {
        return new Digit(lhs.m_value - rhs);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator -(sbyte lhs, Digit rhs)
    {
        return new Digit(lhs - rhs.m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator -(byte lhs, Digit rhs)
    {
        return new Digit(lhs - rhs.m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator -(short lhs, Digit rhs)
    {
        return new Digit(lhs - rhs.m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator -(ushort lhs, Digit rhs)
    {
        return new Digit(lhs - rhs.m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator -(int lhs, Digit rhs)
    {
        return new Digit(lhs - rhs.m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator -(uint lhs, Digit rhs)
    {
        return new Digit(lhs - rhs.m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator -(long lhs, Digit rhs)
    {
        return new Digit(lhs - rhs.m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator -(ulong lhs, Digit rhs)
    {
        return new Digit(lhs - rhs.m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator -(Digit lhs, Digit rhs)
    {
        return new Digit(lhs.m_value - rhs.m_value);
    }
    #endregion

    #region operator *
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator *(Digit lhs, sbyte rhs)
    {
        return new Digit(lhs.m_value * rhs);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator *(Digit lhs, byte rhs)
    {
        return new Digit(lhs.m_value * rhs);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator *(Digit lhs, short rhs)
    {
        return new Digit(lhs.m_value * rhs);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator *(Digit lhs, ushort rhs)
    {
        return new Digit(lhs.m_value * rhs);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator *(Digit lhs, int rhs)
    {
        return new Digit(lhs.m_value * rhs);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator *(Digit lhs, uint rhs)
    {
        return new Digit(lhs.m_value * rhs);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator *(Digit lhs, long rhs)
    {
        return new Digit(lhs.m_value * rhs);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator *(Digit lhs, ulong rhs)
    {
        return new Digit(lhs.m_value * rhs);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator *(sbyte lhs, Digit rhs)
    {
        return new Digit(lhs * rhs.m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator *(byte lhs, Digit rhs)
    {
        return new Digit(lhs * rhs.m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator *(short lhs, Digit rhs)
    {
        return new Digit(lhs * rhs.m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator *(ushort lhs, Digit rhs)
    {
        return new Digit(lhs * rhs.m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator *(int lhs, Digit rhs)
    {
        return new Digit(lhs * rhs.m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator *(uint lhs, Digit rhs)
    {
        return new Digit(lhs * rhs.m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator *(long lhs, Digit rhs)
    {
        return new Digit(lhs * rhs.m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator *(ulong lhs, Digit rhs)
    {
        return new Digit(lhs * rhs.m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator *(Digit lhs, Digit rhs)
    {
        return new Digit(lhs.m_value * rhs.m_value);
    }
    #endregion

    #region operator /
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator /(Digit lhs, sbyte rhs)
    {
        return new Digit(lhs.m_value / rhs);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator /(Digit lhs, byte rhs)
    {
        return new Digit(lhs.m_value / rhs);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator /(Digit lhs, short rhs)
    {
        return new Digit(lhs.m_value / rhs);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator /(Digit lhs, ushort rhs)
    {
        return new Digit(lhs.m_value / rhs);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator /(Digit lhs, int rhs)
    {
        return new Digit(lhs.m_value / rhs);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator /(Digit lhs, uint rhs)
    {
        return new Digit(lhs.m_value / rhs);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator /(Digit lhs, long rhs)
    {
        return new Digit(lhs.m_value / rhs);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator /(Digit lhs, ulong rhs)
    {
        return new Digit(lhs.m_value / rhs);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator /(sbyte lhs, Digit rhs)
    {
        return new Digit(lhs / rhs.m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator /(byte lhs, Digit rhs)
    {
        return new Digit(lhs / rhs.m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator /(short lhs, Digit rhs)
    {
        return new Digit(lhs / rhs.m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator /(ushort lhs, Digit rhs)
    {
        return new Digit(lhs / rhs.m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator /(int lhs, Digit rhs)
    {
        return new Digit(lhs / rhs.m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator /(uint lhs, Digit rhs)
    {
        return new Digit(lhs / rhs.m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator /(long lhs, Digit rhs)
    {
        return new Digit(lhs / rhs.m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator /(ulong lhs, Digit rhs)
    {
        return new Digit(lhs / rhs.m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Digit operator /(Digit lhs, Digit rhs)
    {
        return new Digit(lhs.m_value / rhs.m_value);
    }
    #endregion

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public TypeCode GetTypeCode()
    {
        return TypeCode.Double;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool ToBoolean(IFormatProvider? provider)
    {
        return Convert.ToBoolean(m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public char ToChar(IFormatProvider? provider)
    {
        return Convert.ToChar(m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public sbyte ToSByte(IFormatProvider? provider)
    {
        return Convert.ToSByte(m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public byte ToByte(IFormatProvider? provider)
    {
        return m_value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public short ToInt16(IFormatProvider? provider)
    {
        return Convert.ToInt16(m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ushort ToUInt16(IFormatProvider? provider)
    {
        return Convert.ToUInt16(m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int ToInt32(IFormatProvider? provider)
    {
        return Convert.ToInt32(m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public uint ToUInt32(IFormatProvider? provider)
    {
        return Convert.ToUInt32(m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public long ToInt64(IFormatProvider? provider)
    {
        return Convert.ToInt64(m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public ulong ToUInt64(IFormatProvider? provider)
    {
        return Convert.ToUInt64(m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public float ToSingle(IFormatProvider? provider)
    {
        return Convert.ToSingle(m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public double ToDouble(IFormatProvider? provider)
    {
        return Convert.ToDouble(m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public decimal ToDecimal(IFormatProvider? provider)
    {
        return Convert.ToDecimal(m_value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public DateTime ToDateTime(IFormatProvider? provider)
    {
        throw new InvalidCastException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public object ToType(Type type,
                         IFormatProvider? provider)
    {
        return Convert.ChangeType(this, type, provider);
    }

}

