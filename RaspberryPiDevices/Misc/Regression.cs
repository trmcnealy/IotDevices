using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RaspberryPiDevices;
public static class Regression
{
    [StructLayout(LayoutKind.Sequential)]
    public record struct Point
    {
        public double X;
        public double Y;

        public Point()
        {
            X = 0.0;
            Y = 0.0;
        }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }
    }


    [StructLayout(LayoutKind.Sequential)]
    public record struct Line
    {
        public double Slope;

        public double Intercept;

        public Line()
        {
            Slope = 0.0;
            Intercept = 0.0;
        }

        public Line(double m, double b)
        {
            Slope = m;
            Intercept = b;
        }
    }

    public static Point[] MakePoints(double px0, double py0, double px1, double py1, double px2, double py2)
    {
        return new Point[3] { new(px0, py0), new(px1, py1), new(px2, py2) };
    }

    public static Point[] MakePoints(Point p0, Point p1, Point p2)
    {
        return new Point[3] { p0, p1, p2 };
    }

    /// <summary>
    /// https://ltcconline.net/greenl/courses/117/derivnvar/leastsquares.htm
    /// 
    /// y = m*x + b
    /// d_1^2 + d_2^2 + d_3^2 
    /// y = m*x + b
    /// d_1^2 + d_2^2 + d_3^2  = f = [y_1 - (m*x_1+ b)]^2 + [y_2 - (m*x_2+ b)]^2 + [y_3 - (m*x_3+ b)]^2  
    /// derivatives to get
    /// fm = 2*[y_1 - (m*x1+ b)]*(-x_1) + 2*[y_2 - (m*x_2+ b)]*(-x_2) + 2*[y_3 - (m*x_3+ b)]*(-x_3) = 0
    /// and
    /// fb = -2*[y_1 - (m*x1+ b)] - 2*[y_2 - (m*x_2+ b)] - 2*[y_3 - (m*x3+ b)] = 0 
    ///         
    /// A. x_1*(y_1 - (m*x1+ b)) + x_2*(y_2 - (m*x_2+ b)) + x_3*(y_3 - (m*x3+ b)) = 0    
    /// B. (y_1 - (m*x1+ b)) + (y_2 - (m*x_2+ b)) + (y_3 - (m*x3+ b)) = 0
    /// 
    /// In S notation, we have
    /// Sum(x_i*y_i) - m*Sum(x_i^2) - b*Sum(x_i) = 0
    /// Sum(y_i) - m*Sum(x_i) - n*b = 0 
    /// </summary>
    /// <returns></returns>
    public static Line Linear(IEnumerable<Point> enumerable_points)
    {
        List<Point> points = enumerable_points.ToList();

        double n = points.Count;

        double sum_x = points.Sum(o => o.X);
        double sum_y = points.Sum(o => o.Y);
        double sum_x_sqr = points.Sum(o => Math.Pow(o.X, 2));
        double sum_xy = points.Sum(o => (o.X * o.Y));

        double m = ((n * sum_xy) - (sum_x * sum_y)) / ((n * sum_x_sqr) - Math.Pow(sum_x, 2));

        double b = (1.0 / n) * (sum_y - (m * sum_x));

        return new(m, b);
    }
}
