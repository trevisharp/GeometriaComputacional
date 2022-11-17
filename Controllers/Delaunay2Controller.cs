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

    public override void OnLoad(Bitmap bmp, Graphics g)
    {
        Cursor.Hide();
    }

    public override void OnMouseDown(Bitmap bmp, Graphics g, MouseButtons button, Point point)
    {
        if (list.Count == 4)
            return;
        list.Add(point);
    }

    public override void OnTick(Bitmap bmp, Graphics g)
    {
        g.Clear(Color.White);

        var pts = (list.Count < 4 ? list.Append(p) : list)
            .ToArray();

        Pen pen1 = new Pen(Color.Blue, 2f),
            pen2 = new Pen(Color.Black, 2f);
        
        switch (pts.Count())
        {
            case 1:
                g.FillEllipse(Brushes.Blue,
                    new Rectangle(p.X - 2, p.Y - 2, 4, 4));
                break;
            case 2:
                g.DrawLine(pen1, pts[0], pts[1]);
                break;
            case 3:
                g.DrawPolygon(pen1, pts[0..3]);
                var circ = getCircle(pts[0..3]);
                drawCircle(circ.p, circ.r);
                break;
            case 4:
                var circ2 = getCircle(pts[0..3]);
                if (inCircle(circ2.p, circ2.r, pts[3]))
                    fillCircle(circ2.p, circ2.r, Brushes.Red);
                else fillCircle(circ2.p, circ2.r, Brushes.Green);
                drawCircle(circ2.p, circ2.r);
                g.DrawPolygon(pen1, pts[0..3]);
                g.DrawPolygon(pen1, pts[2..4]
                    .Prepend(pts[0]).ToArray());
                break;
        }

        (PointF p, float r) getCircle(Point[] arr)
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

            return (new PointF(x, y), dist(a));
            
            float dist(PointF p)
            {
                float dx = p.X - x,
                    dy = p.Y - y;
                return (float)Math.Sqrt(dx * dx + dy * dy);
            }
        }

        void drawCircle(PointF p, float r)
        {
            g.DrawEllipse(pen2,
                new RectangleF(p.X - r, 
                    p.Y - r, 2 * r, 2 * r));
        }

        void fillCircle(PointF p, float r, Brush color)
        {
            g.FillEllipse(color,
                new RectangleF(p.X - r, 
                    p.Y - r, 2 * r, 2 * r));
        }

        bool inCircle(PointF p, float r, Point ot)
        {
            float dx = p.X - ot.X,
                  dy = p.Y - ot.Y;
            return dx * dx + dy * dy < r * r;
        }
    }

    public override void OnMouseMove(Bitmap bmp, Graphics g, MouseButtons button, Point point)
    {
        this.p = point;
    }
}