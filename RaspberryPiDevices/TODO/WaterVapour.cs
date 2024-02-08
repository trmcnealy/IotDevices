using System.Runtime.CompilerServices;

using UnitsNet;

namespace RaspberryPiDevices;

public static class WaterVapour
{
    private const double A = -10440.397;
    private const double B = -11.29465;
    private const double C = -0.027022355;
    private const double D = 0.00001289036;
    private const double E = -0.0000000024780681;
    private const double F = 6.5459673;
    
    /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    public static Pressure SaturationVaporPressure(Temperature T)
    {
        return Pressure.FromPoundsForcePerSquareInch(Math.Exp((A / T.DegreesRankine)
                                                                           + (B)
                                                                           + (C * T.DegreesRankine)
                                                                           + (D * Math.Pow(T.DegreesRankine, 2))
                                                                           + (E * Math.Pow(T.DegreesRankine, 3))
                                                                           + (F * Math.Log(T.DegreesRankine))));
    }
    
    /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    public static Pressure PressureDeficitAir(Temperature T, RelativeHumidity h)
    {
        Pressure v_psat = SaturationVaporPressure(T);

        double humidity = (Math.Abs(h.Value - h.Percent) < double.Epsilon) ? (h.Value / 100.0) : (h.Value);

        Pressure vpd = v_psat * (1.0 - humidity);

        return vpd;
    }

    /*[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]*/
    public static Pressure PartialPressureAir(Temperature T, RelativeHumidity h)
    {
        Pressure v_psat = SaturationVaporPressure(T);

        double humidity = (Math.Abs(h.Value - h.Percent) < double.Epsilon) ? (h.Value / 100.0) : (h.Value);

        Pressure v_pair = v_psat * humidity;

        return v_pair;
    }
}
