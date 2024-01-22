using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using static System.Formats.Asn1.AsnWriter;

namespace RaspberryPiDevices;

public sealed class DeviceCalibration
{
    public string Name
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]get; [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]set;
    }

    public List<Regression.Point> Points
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]get; [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]set;
    }

    public Regression.Line Line
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]get; [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]set;
    }

    public DeviceCalibration(string name, List<Regression.Point> points)
    {
        Name = name;
        Points = points;
        
        Line = Regression.Linear(points);
    }
    public DeviceCalibration(string name, Regression.Line line, List<Regression.Point> points)
    {
        Name = name;
        Line = line;
        Points = points;
    }

    public double GetY(double x)
    {
        return (Line.Slope * x) + Line.Intercept;
    }

    //public override string? ToString()
    //{
    //    StringBuilder sb = new StringBuilder();



    //    return base.ToString();
    //}

    //public static Regression.Line LineFromPoints(List<Regression.Point> points)
    //{

    //    double a = (Q.Y - P.Y);
    //    double b = (P.X - Q.X);
    //    double c = (a * P.X) + (b * P.Y);

    //    a /= -b;
    //    c /= b;

    //    return new(a, c);
    //}


}


public sealed class RPiDevice
{
    public Guid Uid
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]get; [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]set;
    }

    public string Name
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]get; [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]set;
    }

    public List<DeviceCalibration> Calibrations
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]get; [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]set;
    }

    public RPiDevice(Guid uid, string name)
    {
        Uid = uid;
        Name = name;
        Calibrations = new List<DeviceCalibration>();
    }

    public RPiDevice(Guid uid, string name, List<DeviceCalibration> calibrations)
    {
        Uid = uid;
        Name = name;
        Calibrations = calibrations;
    }
}

public sealed class RPiSettings
{
    public List<RPiDevice> RPiDevices
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]get; [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]set;
    }

    public RPiSettings()
    {
        RPiDevices = new List<RPiDevice>();
    }
}
