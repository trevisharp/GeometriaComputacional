using System.Windows.Forms;
using System.Drawing;

namespace GeometriaComputacional;

public class Controller
{
    public virtual void OnTick() { }

    public virtual void OnMouseDown(
        MouseButtons button,
        Point point) { }

    public virtual void OnMouseUp(
        MouseButtons button,
        Point point) { }

    public virtual void OnMouseMove(
        MouseButtons button,
        Point point) { }

    public virtual void OnSpace() { }
}