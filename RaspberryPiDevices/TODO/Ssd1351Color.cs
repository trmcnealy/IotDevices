using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitsNet;

namespace RaspberryPiDevices;
public readonly struct Ssd1351Color
{
    internal const int ARGBAlphaShift = 24;
    internal const int ARGBRedShift = 16;
    internal const int ARGBGreenShift = 8;
    internal const int ARGBBlueShift = 0;

    public readonly byte A;
    public readonly byte B;
    public readonly byte G;
    public readonly byte R;

    public Ssd1351Color(in Color color)
    {
        A = color.A;
        B = color.R;
        G = color.G;
        R = color.B;
    }
    public Ssd1351Color(in byte a, in byte b, in byte g, in byte r)
    {
        A = a;
        B = b;
        G = g;
        R = r;
    }

    public override string? ToString()
    {
        return $"ABGR:{A:X2}{B:X2}{G:X2}{R:X2}";
    }

    public static implicit operator int(in Ssd1351Color color)
    {
        return (color.A << ARGBAlphaShift) + (color.B << ARGBBlueShift) + (color.G << ARGBGreenShift) + (color.R << ARGBRedShift);
    }

    public static Color Convert(in Color color)
    {
        Ssd1351Color ssd1351Color = new Ssd1351Color(color);
        //Console.WriteLine(ssd1351Color);
        Color newColor = Color.FromArgb(ssd1351Color);
        //Console.WriteLine($"ABGR:{newColor.A:X2}{newColor.B:X2}{newColor.G:X2}{newColor.R:X2}");
        return newColor;
    }
}
