using System.Numerics;
using System.Drawing;

namespace GeometriaComputacional;

public static class VecPointExtesion
{
    public static Vector3 ToVector(this PointF pt)
        => new Vector3(pt.X, pt.Y, 0);

    public static Vector3 ToVector(this Point pt)
        => new Vector3(pt.X, pt.Y, 0);
    
    public static PointF ToPoint(this Vector3 vec)
        => new PointF(vec.X, vec.Y);
    
    public static float Cross(this Vector3 v, Vector3 u)
        => Vector3.Cross(v, u).Z;

    public static PointF ToPoint(this (float, float) t)
        => new PointF(t.Item1, t.Item2);

    public static PointF ToPoint(this (double, double) t)
        => new PointF((float)t.Item1, (float)t.Item2);

    public static PointF ToPoint(this (int, int) t)
        => new PointF(t.Item1, t.Item2);

    public static Vector3 ToVector(this (float, float) t)
        => new Vector3(t.Item1, t.Item2, 0);

    public static Vector3 ToVector(this (double, double) t)
        => new Vector3((float)t.Item1, (float)t.Item2, 0);

    public static Vector3 ToVector(this (int, int) t)
        => new Vector3(t.Item1, t.Item2, 0);
    
    public static bool IsIn(this Point p, PointF[] pts)
    {
        for (int i = 0; i < pts.Length; i++)
        {
            int j = i + 1 < pts.Length ? i + 1 : 0;
            var b = pts[i].ToVector();

            var v = pts[j].ToVector() - b;
            var u = p.ToVector() - b;

            if (v.Cross(u) < 0)
                return false;
        }
        return true;
    }
}