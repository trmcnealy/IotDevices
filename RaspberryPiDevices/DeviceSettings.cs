using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace RaspberryPiDevices;


[Serializable]
[XmlType("CalibrationPoint")]
public class CalibrationPoint
{
    [XmlElement]
    public double X
    {
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
    }

    [XmlElement]
    public double Y
    {
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
    }

    public CalibrationPoint()
    {
    }

}

[Serializable]
[XmlType("Calibration")]
public class Calibration
{
    [XmlElement]
    public string Name
    {
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
    }

    [XmlArray]
    //[XmlArrayItem("Points", Type = typeof(CalibrationPoint))]
    public List<CalibrationPoint> Points
    {
        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
    }


    private double _slope;

    [XmlElement]
    public double Slope
    {
        get
        {
            if ((Points.Count == 2) || (_slope == 0.0) || double.IsNaN(_slope))
            {
                (double Slope, double Intercept) line = LineFromPoints(Points[0], Points[1]);
                Intercept = line.Intercept;
                _slope = line.Slope;
                return _slope;
            }
            return _slope;
        }
        set
        {
            _slope = value;
        }
    }
    
    private double _intercept;
    [XmlElement]
    public double Intercept
    {
        get
        {
            if ((Points.Count == 2) || (_intercept == 0.0) || double.IsNaN(_intercept))
            {
                (double Slope, double Intercept) line = LineFromPoints(Points[0], Points[1]);
                _intercept = line.Intercept;
                _slope = line.Slope;
                return _intercept;
            }
            return _intercept;
        }
        set
        {
            _intercept = value;
        }
    }

    public Calibration()
    {
        _slope = 0.0;
        _intercept = 0.0;
        Name = string.Empty;
        Points = new List<CalibrationPoint>();
    }
    public Calibration(string name)
    {
        _slope = 0.0;
        _intercept = 0.0;
        Name = name;
        Points = new List<CalibrationPoint>();
    }

    public static (double Slope, double Intercept) LineFromPoints(CalibrationPoint P, CalibrationPoint Q)
    {
        double a = (Q.Y - P.Y);
        double b = (P.X - Q.X);
        double c = (a * P.X) + (b * P.Y);

        a /= -b;
        c /= b;

        return new(a, c);
    }
}


//[Serializable]
//[XmlType("Device")]
//[XmlInclude(typeof(PHProbeSensor))]
////[XmlInclude(typeof(LED4DigitDisplay))]
//public class Device
//{
//    [XmlElement]
//    public Guid Uid
//    {
//        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
//    }
//    [XmlElement]
//    public string Name
//    {
//        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
//    }

//    [XmlArray]
//    //[XmlArrayItem("Calibration", Type = typeof(Calibration))]
//    public List<Calibration> CalibrationPoints
//    {
//        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
//    }

//    public Device()
//    {
//        Uid = Guid.Empty;
//        Name = string.Empty;
//        CalibrationPoints = new List<Calibration>();
//    }

//    public Device(Guid uid, string name)
//    {
//        Uid = uid;
//        Name = name;
//        CalibrationPoints = new List<Calibration>();
//    }
//}


//[Serializable]
//[XmlRoot]
//public class DeviceSettings
//{
//    [XmlArray]
//    public List<Device> Devices
//    {
//        /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/get; /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/set;
//    }

//    internal DeviceSettings()
//    {
//        Devices = new List<Device>();
//    }

//    public TDevice? GetDevice<TDevice>(Guid uid) where TDevice : Device
//    {
//        return (TDevice?)Devices.FirstOrDefault(o => o.Uid == uid);
//    }
//}