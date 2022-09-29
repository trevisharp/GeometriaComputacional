using System.Linq;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;
using GeometriaComputacional;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System;

public class LigthController : Controller
{
    List<Point> list = new List<Point>();
    Point p = Point.Empty;
    bool completed = false;

    public override void OnLoad(Bitmap bmp, Graphics g)
    {
        Cursor.Hide();
    }

    public override void OnMouseDown(Bitmap bmp, Graphics g, MouseButtons button, Point point)
    {
        if (completed)
            return;
        list.Add(point);
    }

    public override void OnTick(Bitmap bmp, Graphics g)
    {
        g.Clear(Color.White);

        var pts = !completed ? list.Append(p).ToArray() : list.ToArray();
        
        if (pts.Length > 2)
        {
            g.FillPolygon(Brushes.Red, pts);
            g.DrawPolygon(new Pen(Color.Black, 4f), pts);
        }
        
        if (completed)
        {
            g.FillEllipse(Brushes.Blue, p.X - 5, p.Y - 5, 10, 10);
            drawLigth(g);
        }
        else
        {
            g.FillEllipse(Brushes.Black, p.X - 5, p.Y - 5, 10, 10);
        }
    }

    public override void OnMouseMove(Bitmap bmp, Graphics g, MouseButtons button, Point point)
    {
        this.p = point;
    }

    public override void OnSpace(Bitmap bmp, Graphics g)
    {
        completed = true;
    }

    private void drawLigth(Graphics g)
    {
        List<PointF> ligth = new List<PointF>();
        var start = vec(p);

        for (int i = 0; i < list.Count; i++)
            computeElement(ligth, start, i, g);

        var arr = ligth.ToArray();
        if (arr.Length < 2)
            return;
        
        GraphicsPath path = new GraphicsPath();
        path.AddRectangle(new Rectangle(0, 0, 2048, 2048));
        PathGradientBrush pthGrBrush = new PathGradientBrush(path);
        pthGrBrush.CenterColor = Color.Yellow;
        pthGrBrush.CenterPoint = p;
        Color[] colors = { Color.Red };
        pthGrBrush.SurroundColors = colors;
        g.FillPolygon(pthGrBrush, ligth.ToArray());
        g.FillEllipse(Brushes.Blue, p.X, p.Y, 10, 10);
        
    }

    private void computeElement(List<PointF> ligth, Vector3 start, int i, Graphics g)
    {
        var end = vec(list[i]);
        var v = end - start;

        if (!isInternalLine(start, end, g))
            return;
        
        var a = getArtificalPoint(start, i, g);

        if (a.HasValue)
        {
            ligth.Add(new PointF(a.Value.X, a.Value.Y));
        }
        ligth.Add(new PointF(end.X, end.Y));
    }

    private Vector3? getArtificalPoint(Vector3 p, int i, Graphics g)
    {
        int i1 = i - 1 >= 0 ? i - 1 : list.Count - 1;
        int i2 = i;
        int i3 = i + 1 < list.Count ? i + 1 : 0;

        var v1 = vec(list[i1]);
        var v2 = vec(list[i2]);
        var v3 = vec(list[i3]);

        var v = v1 - v2;
        var u = v3 - v2;

        var modV = Math.Sqrt(v.X * v.X + v.Y * v.Y);
        var modU = Math.Sqrt(u.X * u.X + u.Y * u.Y);
        var crossUV = cross(u, v).Z;
        var crossOverMod = crossUV / (modV * modU);
        var theta = Math.Abs(Math.Asin(crossOverMod));

        if (crossUV > 0)
            theta = 2 * Math.PI - theta;
        
        if (theta < Math.PI)
            return null;
        
        var start = p;
        var mid = v2;

        return getProjection(start, mid, g);
    }

    private Vector3? getProjection(Vector3 start, Vector3 mid, Graphics g)
    {
        //start + a*v = b

        //s_x + a * v_x = p_x + t * u_x
        //s_y + a * v_y = p_y + t * u_y
        
        //a = (p_x + t * u_x - s_x) / v_x
        //s_y + v_y * (p_x - s_x) / v_x + v_y * u_x * t / v_x = p_y + t * u_y
        //s_y + v_y * (p_x - s_x) / v_x - p_y = t * u_y - v_y * u_x * t / v_x
        //s_y + v_y * (p_x - s_x) / v_x - p_y = t * (u_y - v_y * u_x / v_x)
        //t = (s_y + v_y * (p_x - s_x) / v_x - p_y) / (u_y - v_y * u_x / v_x)

        var v = mid - start;
        for (int i = 0; i < list.Count; i++)
        {
            var p = vec(list[i]);
            var q = vec(list[i + 1 < list.Count ? i + 1 : 0]);
            var u = q - p;

            float b = u.Y - v.Y * u.X / v.X;
            if (b < .01f && b > -.01f)
                continue;

            var t = (start.Y + v.Y * (p.X - start.X) / v.X - p.Y) / b;

            var modU = (float)Math.Sqrt(u.X * u.X + u.Y * u.Y);
            if (t < 0 || t > 1)
                continue;

            var a = (p.X + t * u.X - start.X) / v.X;
            if (a < 0)
                continue;

            var r = p + u * t;
            // g.DrawString(t.ToString(), SystemFonts.CaptionFont, 
            //     Brushes.Black, new PointF(5, 5));
            // g.DrawLine(new Pen(Brushes.Blue, 10), p.X, p.Y, q.X, q.Y);
            // g.FillEllipse(Brushes.Orange, mid.X, mid.Y, 100, 100);
            // g.FillEllipse(Brushes.Green, r.X, r.Y, 100, 100);

            return r;
        }
        
        return null;
    }

    private bool isInternalLine(Vector3 p, Vector3 q, Graphics g)
    {
        for (int i = 0; i < list.Count; i++)
        {
            var r = vec(list[i]);
            var s = vec(list[i + 1 < list.Count ? i + 1 : 0]);

            var u = s - r;
            var v1 = p - r;
            var v2 = q - r;
            if (cross(u, v1).Z * cross(u, v2).Z >= 0)
                continue;

            var v = q - p;
            var u1 = r - p;
            var u2 = s - p;
            if (cross(v, u1).Z * cross(v, u2).Z >= 0)
                continue;

            return false;
        }
        return true;
    }
}