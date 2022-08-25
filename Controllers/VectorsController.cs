using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;

namespace GeometriaComputacional;

public class VectorsController : Controller
{
    private Point? p = null;
    private Vector3? vector = null;

    public override void OnMouseDown(Bitmap bmp, Graphics g, MouseButtons button, Point point)
    {
        if (vector.HasValue)
            return;
        
        if (p.HasValue)
        {
            var v = vec(p.Value);
            var u = vec(point);
            vector = u - v;
            return;
        }

        p = point;
    }

    public override void OnMouseMove(Bitmap bmp, Graphics g, MouseButtons button, Point point)
    {
        g.Clear(Color.White);
        if (!p.HasValue)
            return;
        var v = vec(point) - vec(p.Value);
        
        if (vector.HasValue)
        {
            var u = vector.Value;
            draw(p.Value, u, v);
            draw(p.Value, u);
        }
        draw(p.Value, v);
    }
}