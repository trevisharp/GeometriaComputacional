using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;

using GeometriaComputacional;

public class PointsController : Controller
{
    List<Point> list = new List<Point>();

    public override void OnMouseDown(Bitmap bmp, Graphics g, MouseButtons button, Point point)
    {
        list.Add(point);
    }

    public override void OnTick(Bitmap bmp, Graphics g)
    {
        g.Clear(Color.White);

        if (list.Count > 2)
            g.DrawPolygon(Pens.Black, list.ToArray());
    }
}