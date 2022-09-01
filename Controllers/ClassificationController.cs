using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System;
using System.Windows.Forms;

namespace GeometriaComputacional;

public class ClassificationController : Controller
{
    private List<List<Point>> lists = new List<List<Point>>();
    private List<Point> pts = new List<Point>();
    Point cursor = Point.Empty;

    public override void OnMouseMove(Bitmap bmp, Graphics g, MouseButtons button, Point point)
    {
        cursor = point;
    }

    public override void OnLoad(Bitmap bmp, Graphics g)
    {
        lists.Add(pts);
    }

    public override void OnMouseDown(Bitmap bmp, Graphics g, MouseButtons button, Point point)
    {
        pts.Add(point);
    }

    public override void OnTick(Bitmap bmp, Graphics g)
    {
        g.Clear(Color.DarkGray);
        foreach (var points in lists)
        {
            if ((points != pts && points.Count < 3) ||
                points == pts && points.Count < 2)
                continue;
            
            var polygon = points == pts ?
                points.Append(cursor).ToArray() :
                points.ToArray();
            
            g.FillPolygon(Brushes.LightGray, polygon);

            for (int i = 0; i < polygon.Length; i++)
            {
                var p = polygon[i];

                var index = i + 1 >= polygon.Length ?
                    i + 1 - polygon.Length :
                    i + 1;
                var q = polygon[index];

                index = i + 2 >= polygon.Length ?
                    i + 2 - polygon.Length :
                    i + 2;
                var r = polygon[index];

                var v = vec(q) - vec(p);
                var u = vec(r) - vec(p);
                var A = Vector3.Cross(u, v).Z;
                
                var sinTheta = A / (v.Length() * u.Length());
                Color color = sinTheta > 0 ?Color.Red : Color.Blue;
                var pen = new Pen(color, 2f);

                double start = 180 * Math.Atan2(q.Y - p.Y, q.X - p.X) / Math.PI;
                double end = 180 * Math.Atan2(q.Y - r.Y, q.X - r.X) / Math.PI;
                double theta = end - start;
                if (sinTheta < 0 && theta > 180 ||
                    sinTheta > 0 && theta < 180)
                {
                    start = end;
                    theta = 360 - theta;
                }
                Brush brush = new SolidBrush(
                    Color.FromArgb(
                        (color.R + 100) / 2,
                        (color.G + 100) / 2,
                        (color.B + 100) / 2
                    )
                );
                g.FillPie(brush,
                    q.X - 25, q.Y - 25,
                    50, 50, (int)start + 180, (int)theta);
                g.DrawPie(pen,
                    q.X - 25, q.Y - 25,
                    50, 50, (int)start + 180, (int)theta);
                    
                g.DrawLine(Pens.Black, q.X, q.Y, (p.X + q.X) / 2, (p.Y + q.Y) / 2);
                g.DrawLine(Pens.Black, q.X, q.Y, (r.X + q.X) / 2, (r.Y + q.Y) / 2);
            }
        }
    }

    public override void OnSpace(Bitmap bmp, Graphics g)
    {
        pts = new List<Point>();
        lists.Add(pts);
    }
}