using System.Linq;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;
using System.Collections.Generic;

namespace GeometriaComputacional;

public class StartController : Controller
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
            g.DrawPolygon(Pens.Black, polygon);

            for (int i = 0; i < polygon.Length; i++)
            {
                var p = polygon[i];

                var index = i + 1 >= polygon.Length ?
                    i + 1 - polygon.Length :
                    i + 1;
                var q = polygon[index];

                index = i - 1 < 0 ?
                    polygon.Length - 1 :
                    i - 1;
                var r = polygon[index];

                var v = vec(r) - vec(p);
                var u = vec(q) - vec(p);
                var A = Vector3.Cross(u, v).Z;

                if (isdown(p, r) && isdown(p, q))
                {
                    if (A > 0)
                        g.DrawString("Start", SystemFonts.MessageBoxFont,
                            Brushes.Green, p);
                    else 
                        g.DrawString("Split", SystemFonts.MessageBoxFont,
                            Brushes.Red, p);
                }
                else if (isdown(r, p) && isdown(q, p))
                {
                    if (A > 0)
                        g.DrawString("End", SystemFonts.MessageBoxFont,
                            Brushes.Blue, p);
                    else 
                        g.DrawString("Merge", SystemFonts.MessageBoxFont,
                            Brushes.Orange, p);
                }
                else
                {
                    g.DrawString("Regular", SystemFonts.MessageBoxFont,
                        Brushes.Purple, p);
                }     

            }

            bool isdown(Point p, Point q)
            {
                return p.Y < q.Y || (p.Y == q.Y && p.X > q.X);
            }
        }
    }

    public override void OnSpace(Bitmap bmp, Graphics g)
    {
        pts = new List<Point>();
        lists.Add(pts);
    }
}