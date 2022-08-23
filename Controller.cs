using System.Windows.Forms;
using System.Drawing;

namespace GeometriaComputacional;

public class Controller
{
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
}