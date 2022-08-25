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


    public static Vector3 vec(Point p)
        => new Vector3(p.X, p.Y, 0);
    
    public static Vector3 vec(float x, float y)
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