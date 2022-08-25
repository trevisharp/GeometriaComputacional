using System.Windows.Forms;
using System.Numerics;
using System.Drawing;

namespace GeometriaComputacional;

public class Controller
{
    private Graphics g;
    private Bitmap bmp;

    public void SetGraphics(Graphics g)
        => this.g = g;
    
    public void SetBitmap(Bitmap bmp)
        => this.bmp = bmp;

    protected void draw(Point p, Vector3 vec, Brush? color = null)
    {
        var u = new Vector3(vec.Y, -vec.X, 0);

        float uX = u.X * 0.1f,
              uY = u.Y * 0.1f;
        
        if (uX > 10f)
            uX = 10f;
        else if (uX < -10f)
            uX = -10f;
        
        if (uY > 10f)
            uY = 10f;
        else if (uY < -10f)
            uY = -10f;

        PointF[] pts = new PointF[]
        {
            new PointF(p.X + uX, p.Y + uY),
            new PointF(p.X + vec.X * .8f + uX, p.Y + vec.Y * .8f + uY),
            new PointF(p.X + vec.X * .8f + 2 * uX, p.Y + vec.Y * .8f + 2 * uY),
            new PointF(p.X + vec.X, p.Y + vec.Y),
            new PointF(p.X + vec.X * .8f - 2 * uX, p.Y + vec.Y * .8f - 2 * uY),
            new PointF(p.X + vec.X * .8f - uX, p.Y + vec.Y * .8f - uY),
            new PointF(p.X - uX, p.Y - uY),
        };
        g.FillPolygon(color ?? Brushes.Red, pts);
    }

    protected void draw(Point p, Vector3 v, Vector3 u, Brush? color = null)
    {
        var pts = new PointF[]
        {
            p,
            new PointF(p.X + v.X, p.Y + v.Y),
            new PointF(p.X + v.X + u.X, p.Y + v.Y + u.Y),
            new PointF(p.X + u.X, p.Y + u.Y)
        };
        var area = Vector3.Cross(v, u).Z;
        g.FillPolygon(color ?? Brushes.Blue, pts);

        PointF center = new PointF(p.X + (v.X + u.X) / 2, p.Y + (v.Y + u.Y) / 2);
        g.DrawString(area.ToString(), SystemFonts.CaptionFont, Brushes.Black, center);
    }

    protected Vector3 vec(Point p)
        => new Vector3(p.X, p.Y, 0);
    
    protected Vector3 vec(float x, float y)
        => new Vector3(x, y, 0);

    public virtual void OnTick(
        Bitmap bmp,
        Graphics g) { }

    public virtual void OnMouseDown(
        Bitmap bmp,
        Graphics g,
        MouseButtons button,
        Point point) { }

    public virtual void OnMouseUp(
        Bitmap bmp,
        Graphics g,
        MouseButtons button,
        Point point) { }

    public virtual void OnMouseMove(
        Bitmap bmp,
        Graphics g,
        MouseButtons button,
        Point point) { }

    public virtual void OnSpace(
        Bitmap bmp,
        Graphics g) { }
    
    public virtual void OnLoad(
        Bitmap bmp,
        Graphics g) { }
}