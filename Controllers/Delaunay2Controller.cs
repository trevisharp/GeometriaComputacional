using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Numerics;
using System;

namespace GeometriaComputacional;

public class Delaunay2Controller : Controller
{
    List<Point> list = new List<Point>();
    Point p = Point.Empty;
    bool include = false;

    public override void OnLoad(Bitmap bmp, Graphics g)
    {
        Cursor.Hide();
    }

    public override void OnMouseDown(Bitmap bmp, Graphics g, MouseButtons button, Point point)
    {
        list.Add(point);
    }

    public override void OnMouseMove(Bitmap bmp, Graphics g, MouseButtons button, Point point)
    {
        this.p = point;
    }

    public override void OnSpace(Bitmap bmp, Graphics g)
    {
        include = !include;
    }

    public override void OnTick(Bitmap bmp, Graphics g)
    {
        g.Clear(Color.White);

        var pts = (include ? list.Append(p) : list)
            .ToArray();

        delaunay(pts, g);

        foreach (var q in pts)
            g.FillEllipse(Brushes.Black,
                new Rectangle(q.X - 4, q.Y - 4, 8, 8));

        g.FillEllipse(Brushes.Black,
            new Rectangle(p.X - 2, p.Y - 2, 4, 4));
    }

    //Não é ainda kk
    private void delaunay(Point[] pts, Graphics g)
    {
        if (pts.Length < 3)
            return;
        
        var triangulation = new List<
            (Point a, Point b, Point c, (PointF o, float r) circle)>();
        
        pts = pts.OrderBy(p => p.X).ToArray();

        triangulation.Add((pts[0], pts[1], pts[2], getCircle(pts[0], pts[1], pts[2])));

        for (int i = 3; i < pts.Length; i++)
        {
            var vis = getVisibleList(pts[0..i], triangulation, pts[i]);
            if (vis.Length < 2)
                continue;
            
            for (int j = 0; j < vis.Length - 1; j++)
            {
                triangulation.Add((vis[j], vis[j + 1], pts[i], 
                    getCircle(vis[j], vis[j + 1], pts[i])));
            }
        }

        Pen pen = new Pen(Brushes.Black, 2f);
        Pen pen2 = new Pen(Color.FromArgb(180, 128, 128, 128), 1f);
        foreach (var t in triangulation)
        {
            g.DrawPolygon(pen, new Point[] { 
                t.a,
                t.b,
                t.c
            });
            if (inCircle(t.circle, this.p))
                drawCircle(t.circle, Pens.Red, g);
            else drawCircle(t.circle, pen2, g);
        }
    }

    private Point[] getVisibleList(
        Point[] pts, 
        List<(Point a, Point b, Point c, (PointF o, float r) circle)> ts,
        Point p)
    {
        List<Point> visible = new List<Point>();

        foreach (var q in pts)
        {
            if (q == p)
                continue;
            bool isVisible = true;
            foreach (var t in ts)
            {
                if (collison(t.a, t.b, p, q))
                {
                    isVisible = false;
                    break;
                }
                else if (collison(t.b, t.c, p, q))
                {
                    isVisible = false;
                    break;
                }
                else if (collison(t.c, t.a, p, q))
                {
                    isVisible = false;
                    break;
                }
            }
            if (isVisible)
                visible.Add(q);
        }

        return visible.ToArray();
    }

    private bool collison(Point p, Point q, Point r, Point s, Graphics g = null)
    {
        if (p == r || p == s || q == r || q == s)
            return false;
        
        var vp = vec(p);
        var vq = vec(q);
        var vr = vec(r);
        var vs = vec(s);

        var l1 = vp - vq;
        var l2 = vr - vs;
        var l3 = vp - vs;
        var l4 = vq - vs;
        var l5 = vr - vq;
        var l6 = vs - vq;

        var c1 = cross(l1, l5).Z;
        var c2 = cross(l1, l6).Z;
        var c3 = cross(l2, l3).Z;
        var c4 = cross(l2, l4).Z;

        if (c1 * c2 < 0 && c3 * c4 < 0)
            return true;
        
        return false;
    }

    private (
        (Point a, Point b, Point c, (PointF o, float r)),
        (Point a, Point b, Point c, (PointF o, float r))
                                                ) flip(
            (Point a, Point b, Point c, (PointF o, float r)) t, Point n
        )
        {
            Point p1 = Point.Empty,
                p2 = Point.Empty,
                p3 = Point.Empty;
                
            float da = dist(t.a, n.X, n.Y);
            float db = dist(t.b, n.X, n.Y);
            float dc = dist(t.c, n.X, n.Y);

            if (da > db && da > dc)
            {
                p3 = t.a;
                p2 = t.b;
                p1 = t.c;
            }
            else if (db > dc)
            {
                p3 = t.b;
                p2 = t.a;
                p1 = t.c;
            }
            else
            {
                p3 = t.c;
                p2 = t.b;
                p1 = t.a;
            }

            var c1 = getCircle(n, p1, p3);
            var c2 = getCircle(n, p2, p3);

            return (
                (n, p1, p3, c1),
                (n, p2, p3, c2)
            );
        }
    
    private (Point a, Point b, Point c, (PointF o, float r)) includePt(Point[] pts, Point n)
    {
        var minpts = pts
            .Select(p => new {
            point = p,
            distance = dist(p, n.X, n.Y)
        })
            .OrderBy(x => x.distance)
            .Select(x => x.point)
            .Take(2)
            .ToArray();
        
        return (minpts[0], minpts[1], n, getCircle(minpts[0], minpts[1], n));
    }

    private (PointF p, float r) getCircle(params Point[] arr)
    {
        PointF a = arr[0],
            b = arr[1],
            c = arr[2];
        
        float A = a.X * a.X + a.Y * a.Y;
        float B = b.X * b.X + b.Y * b.Y;
        float C = c.X * c.X + c.Y * c.Y;
        float x = 
            (
                (B - C) / (c.Y - b.Y) + 
                (A - B) / (a.Y - b.Y)
            ) / (
                (2 * a.X - 2 * b.X) / (a.Y - b.Y) + 
                (2 * b.X - 2 * c.X) / (c.Y - b.Y)
            );
        float y = (B - A + 2 * x * (a.X - b.X))
            / (2 * b.Y - 2 * a.Y);

        return (new PointF(x, y), dist(a, x, y));
    }
        
    private float dist(PointF p, float x, float y)
    {
        float dx = p.X - x,
            dy = p.Y - y;
        return (float)Math.Sqrt(dx * dx + dy * dy);
    }

    private void drawCircle((PointF p, float r) c, Pen pen, Graphics g)
    {
        g.DrawEllipse(pen,
            new RectangleF(c.p.X - c.r, 
                c.p.Y - c.r, 2 * c.r, 2 * c.r));
    }

    private void fillCircle((PointF p, float r) c, Brush color, Graphics g)
    {
        g.FillEllipse(color,
            new RectangleF(c.p.X - c.r, 
                c.p.Y - c.r, 2 * c.r, 2 * c.r));
    }

    private bool inCircle((PointF p, float r) t, Point ot)
    {
        float dx = t.p.X - ot.X,
            dy = t.p.Y - ot.Y;
        return dx * dx + dy * dy < t.r * t.r;
    }
}