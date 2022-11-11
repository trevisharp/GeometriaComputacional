using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Numerics;

namespace GeometriaComputacional;

public class PointsController : Controller
{
    List<Point> list = new List<Point>();
    Point p = Point.Empty;
    bool include = false;

    public override void OnMouseDown(Bitmap bmp, Graphics g, MouseButtons button, Point point)
    {
        list.Add(point);
    }

    public override void OnTick(Bitmap bmp, Graphics g)
    {
        g.Clear(Color.White);

        var pts = include ? list.Append(p).ToArray() : list.ToArray();

        if (pts.Length > 2)
            g.FillPolygon(Brushes.Red, pts);
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