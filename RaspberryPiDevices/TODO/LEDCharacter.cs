using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RaspberryPiDevices;


[StructLayout(LayoutKind.Sequential)]
public readonly struct LEDCharacter
{
    //public static readonly Dictionary<LEDCharacter, string> ValueNames;
    //public static readonly Dictionary<string, LEDCharacter> NameValues;

    public static readonly LEDCharacter[] Characters = new LEDCharacter[]{Digit0, Digit1,Digit2, Digit3, Digit4, Digit5, Digit6, Digit7, Digit8, Digit9};

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    static LEDCharacter()
    {
        Nothing = new LEDCharacter(NothingConst);
        Top = new LEDCharacter(TopConst);
        TopRight = new LEDCharacter(TopRightConst);
        BottomRight = new LEDCharacter(BottomRightConst);
        Bottom = new LEDCharacter(BottomConst);
        BottomLeft = new LEDCharacter(BottomLeftConst);
        TopLeft = new LEDCharacter(TopLeftConst);
        Middle = new LEDCharacter(MiddleConst);
        Dot = new LEDCharacter(DotConst);
        Digit0 = new LEDCharacter(Digit0Const);
        Digit1 = new LEDCharacter(Digit1Const);
        Digit2 = new LEDCharacter(Digit2Const);
        Digit3 = new LEDCharacter(Digit3Const);
        Digit4 = new LEDCharacter(Digit4Const);
        Digit5 = new LEDCharacter(Digit5Const);
        Digit6 = new LEDCharacter(Digit6Const);
        Digit7 = new LEDCharacter(Digit7Const);
        Digit8 = new LEDCharacter(Digit8Const);
        Digit9 = new LEDCharacter(Digit9Const);
        A = new LEDCharacter(AConst);
        B = new LEDCharacter(BConst);
        C = new LEDCharacter(CConst);
        D = new LEDCharacter(DConst);
        E = new LEDCharacter(EConst);
        F = new LEDCharacter(FConst);
        Minus = new LEDCharacter(MinusConst);

        //ValueNames = new Dictionary<LEDCharacter, string>();
        //{
        //    {Nothing, "Nothing"},
        //    {Top, "Top"},
        //    {TopRight, "TopRight"},
        //    {BottomRight, "BottomRight"},
        //    {Bottom, "Bottom"},
        //    {BottomLeft, "BottomLeft"},
        //    {TopLeft, "TopLeft"},
        //    {Middle, "Middle"},
        //    {Dot, "Dot"},
        //    {Digit0, "Digit0"},
        //    {Digit1, "Digit1"},
        //    {Digit2, "Digit2"},
        //    {Digit3, "Digit3"},
        //    {Digit4, "Digit4"},
        //    {Digit5, "Digit5"},
        //    {Digit6, "Digit6"},
        //    {Digit7, "Digit7"},
        //    {Digit8, "Digit8"},
        //    {Digit9, "Digit9"},
        //    {A, "A"},
        //    {B, "B"},
        //    {C, "C"},
        //    {D, "D"},
        //    {E, "E"},
        //    {F, "F"},
        //    {Minus, "Minus"},
        //};

        //NameValues = new Dictionary<string, LEDCharacter>();
        //{
        //    { "Nothing", Nothing},
        //    {"Top", Top},
        //    {"TopRight", TopRight},
        //    {"BottomRight", BottomRight},
        //    {"Bottom", Bottom},
        //    {"BottomLeft",BottomLeft},
        //    {"TopLeft", TopLeft},
        //    {"Middle", Middle},
        //    {"Dot", Dot},
        //    {"Digit0", Digit0},
        //    {"Digit1", Digit1},
        //    {"Digit2", Digit2},
        //    {"Digit3", Digit3},
        //    {"Digit4", Digit4},
        //    {"Digit5", Digit5},
        //    {"Digit6", Digit6},
        //    {"Digit7", Digit7},
        //    {"Digit8", Digit8},
        //    {"Digit9", Digit9},
        //    {"A", A},
        //    {"B", B},
        //    {"C", C},
        //    {"D", D},
        //    {"E", E},
        //    {"F", F},
        //    {"Minus", Minus},
        //};

    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public LEDCharacter()
    {
        _value = default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public LEDCharacter(byte value)
    {
        _value = value;
    }

    private readonly byte _value;

    public byte Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get
        {
            return _value;
        }
    }


    public static readonly LEDCharacter Nothing;
    public const byte NothingConst = 0b0000_0000;

    public static readonly LEDCharacter Top;
    public const byte TopConst = 0b0000_0001;

    public static readonly LEDCharacter TopRight;
    public const byte TopRightConst = 0b0000_0010;

    public static readonly LEDCharacter BottomRight;
    public const byte BottomRightConst = 0b0000_0100;

    public static readonly LEDCharacter Bottom;
    public const byte BottomConst = 0b0000_1000;

    public static readonly LEDCharacter BottomLeft;
    public const byte BottomLeftConst = 0b0001_0000;

    public static readonly LEDCharacter TopLeft;
    public const byte TopLeftConst = 0b0010_0000;

    public static readonly LEDCharacter Middle;
    public const byte MiddleConst = 0b0100_0000;

    public static readonly LEDCharacter Dot;
    public const byte DotConst = 0b1000_0000;

    public static readonly LEDCharacter Digit0;
    public const byte Digit0Const = TopConst | TopRightConst | BottomRightConst | BottomConst | BottomLeftConst | TopLeftConst;

    public static readonly LEDCharacter Digit1;
    public const byte Digit1Const = TopRightConst | BottomRightConst;

    public static readonly LEDCharacter Digit2;
    public const byte Digit2Const = TopConst | TopRightConst | MiddleConst | BottomLeftConst | BottomConst;

    public static readonly LEDCharacter Digit3;
    public const byte Digit3Const = TopConst | TopRightConst | MiddleConst | BottomRightConst | BottomConst;

    public static readonly LEDCharacter Digit4;
    public const byte Digit4Const = TopLeftConst | MiddleConst | TopRightConst | BottomRightConst;

    public static readonly LEDCharacter Digit5;
    public const byte Digit5Const = TopConst | TopLeftConst | MiddleConst | BottomRightConst | BottomConst;

    public static readonly LEDCharacter Digit6;
    public const byte Digit6Const = TopConst | TopLeftConst | MiddleConst | BottomRightConst | BottomConst | BottomLeftConst;

    public static readonly LEDCharacter Digit7;
    public const byte Digit7Const = TopConst | TopRightConst | BottomRightConst;

    public static readonly LEDCharacter Digit8;
    public const byte Digit8Const = TopConst | TopLeftConst | TopRightConst | MiddleConst | BottomConst | BottomLeftConst | BottomRightConst;

    public static readonly LEDCharacter Digit9;
    public const byte Digit9Const = TopConst | TopLeftConst | TopRightConst | MiddleConst | BottomConst | BottomRightConst;

    public static readonly LEDCharacter A;
    public const byte AConst = TopConst | TopLeftConst | TopRightConst | MiddleConst | BottomLeftConst | BottomRightConst;

    public static readonly LEDCharacter B;
    public const byte BConst = TopLeftConst | MiddleConst | BottomConst | BottomLeftConst | BottomRightConst;

    public static readonly LEDCharacter C;
    public const byte CConst = TopConst | TopLeftConst | BottomLeftConst | BottomConst;

    public static readonly LEDCharacter D;
    public const byte DConst = TopRightConst | MiddleConst | BottomConst | BottomLeftConst | BottomRightConst;

    public static readonly LEDCharacter E;
    public const byte EConst = MiddleConst | TopLeftConst | BottomLeftConst | BottomConst | TopConst;

    public static readonly LEDCharacter F;
    public const byte FConst = MiddleConst | TopLeftConst | BottomLeftConst | TopConst;

    public static readonly LEDCharacter Minus;
    public const byte MinusConst = MiddleConst;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator byte(in LEDCharacter ledcharacter)
    {
        return ledcharacter.Value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator LEDCharacter(byte value)
    {
        return new LEDCharacter(value);
    }

    //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    //public static string GetName(in LEDCharacter ledcharacter)
    //{
    //    return ValueNames[ledcharacter];
    //}

    //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    //public static LEDCharacter GetLEDCharacter(in string name)
    //{
    //    return NameValues[name];
    //}

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static LEDCharacter GetLEDCharacter(in byte value)
    {
        return Characters[value];
        //switch (value)
        //{
        //    case 0:
        //        return Digit0;
        //    case 1:
        //        return Digit1;
        //    case 2:
        //        return Digit2;
        //    case 3:
        //        return Digit3;
        //    case 4:
        //        return Digit4;
        //    case 5:
        //        return Digit5;
        //    case 6:
        //        return Digit6;
        //    case 7:
        //        return Digit7;
        //    case 8:
        //        return Digit8;
        //    case 9:
        //        return Digit9;
        //    default:
        //        return Digit0;
        //}
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static LEDCharacter operator ~(in LEDCharacter left)
    {
        return (LEDCharacter)(~left.Value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static LEDCharacter operator <<(in LEDCharacter left, int right)
    {
        return (LEDCharacter)(left.Value << right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static LEDCharacter operator >>(in LEDCharacter left, int right)
    {
        return (LEDCharacter)(left.Value >> right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static LEDCharacter operator ^(in LEDCharacter left, in LEDCharacter right)
    {
        return (LEDCharacter)(left.Value ^ right.Value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static LEDCharacter operator &(in LEDCharacter left, in LEDCharacter right)
    {
        return (LEDCharacter)(left.Value & right.Value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static LEDCharacter operator |(in LEDCharacter left, in LEDCharacter right)
    {
        return (LEDCharacter)(left.Value | right.Value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Equals(LEDCharacter other)
    {
        return Value == other.Value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override bool Equals(object? obj)
    {
        return obj is LEDCharacter other && Equals(other);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator ==(in LEDCharacter left, in LEDCharacter right)
    {
        return Equals(left, right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator ==(in LEDCharacter left, byte right)
    {
        return left.Value == right;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator ==(byte left, in LEDCharacter right)
    {
        return left == right.Value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator !=(in LEDCharacter left, in LEDCharacter right)
    {
        return !Equals(left, right);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator !=(in LEDCharacter left, byte right)
    {
        return left.Value != right;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool operator !=(byte left, in LEDCharacter right)
    {
        return left != right.Value;
    }

    //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    //public override string? ToString()
    //{
    //    return ValueNames[this];
    //}

}

