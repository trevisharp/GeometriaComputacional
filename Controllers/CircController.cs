using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Numerics;
using System;

namespace GeometriaComputacional;

public class CircController : Controller
{
    List<Point> list = new List<Point>();
    Point p = Point.Empty;

    public override void OnLoad(Bitmap bmp, Graphics g)
    {
        // Cursor.Hide();
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

        // var pts = list.Count < 4 ? list.Append(p).ToArray() : list.ToArray();
        var pts = list.ToArray();

        Pen pen1 = new Pen(Color.Blue, 2f),
            pen2 = new Pen(Color.Red, 2f);
        
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
                drawCircle(pts[0..3]);
                break;
            case 4:
                g.DrawPolygon(pen1, pts[2..4]
                    .Prepend(pts[0]).ToArray());
                goto case 3;
        }


        void drawCircle(Point[] arr)
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

            float d = dist(a);
            g.DrawEllipse(pen2,
                new RectangleF(x - d, 
                    y - d, 2 * d, 2 * d));
            
            float dist(PointF p)
            {
                float dx = p.X - x,
                    dy = p.Y - y;
                return (float)Math.Sqrt(dx * dx + dy * dy);
            }
        }
    }

    public override void OnMouseMove(Bitmap bmp, Graphics g, MouseButtons button, Point point)
    {
        this.p = point;
    }
}