using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace GeometriaComputacional;

public class HullController : Controller
{
    int N = 10000;
    Graphics g = null;
    List<Point> list = new List<Point>();
    Point[] hull;
    Point p = Point.Empty;
    bool include = false;

    public override void OnLoad(Bitmap bmp, Graphics g)
    {
        this.g = g;
        Random rand = new Random(
            DateTime.Now.Millisecond
            );
        int widmar = bmp.Width / 5,
            heimar = bmp.Height / 5;
        for (int i = 0; i < N; i++)
        {
            int w = rand.Next((bmp.Width - widmar) / 2);
            int h = rand.Next((bmp.Height - heimar) / 2);
            Point q = new Point(
                rand.Next(bmp.Width / 2 - w, bmp.Width  / 2 + w),
                rand.Next(bmp.Height / 2 - h, bmp.Height  / 2 + h)
            );
            this.list.Add(q);
        }
    }

    private Point[] ConvexHull(Point[] pts)
    {
        Point xmin = pts[0];
        Point xmax = pts[0];
        for (int i = 1; i < pts.Length; i++)
        {
            int crrx = pts[i].X;
            if (crrx < xmin.X)
                xmin = pts[i];
            else if (crrx > xmax.X)
                xmax = pts[i]; 
        }

        List<Point> s1 = new List<Point>();
        List<Point> s2 = new List<Point>();
        for (int i = 0; i < pts.Length; i++)
        {
            if (pts[i] == xmin)
                continue;
            
            if (pts[i] == xmax)
                continue;

            var r = pts[i].ToVector();
            var v = xmin.ToVector();
            var u = xmax.ToVector();
            var z = (v - u).Cross(r - u);

            if (z > 0) 
                s1.Add(pts[i]);
            else 
                s2.Add(pts[i]);
        }

        List<Point> hull = new List<Point>();
        hull.Add(xmin);
        hull.AddRange(recHull(hull, s1, xmin, xmax));
        hull.Add(xmax);
        hull.AddRange(recHull(hull, s2, xmax, xmin));

        return hull.ToArray();
    }

    private List<Point> recHull(List<Point> hull,
        List<Point> pts, Point p, Point q)
    {
        List<Point> subHull = new List<Point>();
        if (pts.Count == 0)
            return subHull;
        
        var v = p.ToVector();
        var u = q.ToVector();

        float maxValue = float.MinValue;
        Point newPt = Point.Empty;
        foreach (var pt in pts)
        {
            int dpx = pt.X - p.X;
            int dpy = pt.Y - p.Y;
            int dqx = pt.X - q.X;
            int dqy = pt.Y - q.Y;
            var dp = dpx * dpx + dpy * dpy;
            var dq = dqx * dqx + dqy * dqy;

            var value = dp * dq;
            if (value > maxValue)
            {
                maxValue = value;
                newPt = pt;
            }
        }
        var n = newPt.ToVector();

        List<Point> s1 = new List<Point>();
        List<Point> s2 = new List<Point>();
        for (int i = 0; i < pts.Count; i++)
        {   
            var c = pts[i].ToVector();
            var z1 = (v - n).Cross(c - n);
            var z2 = (n - u).Cross(c - u);

            if (z1 > 0) s1.Add(pts[i]);
            else if (z2 > 0) s2.Add(pts[i]);
        }

        subHull.AddRange(recHull(hull, s1, p, newPt));
        subHull.Add(newPt);
        subHull.AddRange(recHull(hull, s2, newPt, q));

        return subHull;
    }

    public override void OnMouseDown(Bitmap bmp, Graphics g, MouseButtons button, Point point)
    {
        list.Add(point);
    }

    public override void OnTick(Bitmap bmp, Graphics g)
    {
        g.Clear(Color.White);

        foreach (var pt in list)
        {
            g.FillEllipse(Brushes.DarkBlue,
                pt.X - 5, pt.Y - 5, 10, 10);
        }
        if (include)
            g.FillEllipse(Brushes.DarkBlue,
                p.X - 5, p.Y - 5, 10, 10);
        var pts = include ? list.Append(p) : list;
        hull = ConvexHull(pts.ToArray());
        g.DrawPolygon(Pens.Black, hull);
    }

    public override void OnMouseMove(Bitmap bmp, Graphics g, MouseButtons button, Point point)
    {
        this.p = point;
    }

    public override void OnSpace(Bitmap bmp, Graphics g)
    {
        include = !include;
    }
}