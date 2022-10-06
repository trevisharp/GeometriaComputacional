using System.Drawing;
using System.Windows.Forms;

namespace GeometriaComputacional;

public class DCELController : Controller
{
    public DCEL dcel = new DCEL();

    private bool mouseDown = false;
    private PointF cursor = PointF.Empty;
    private int mode = 0;

    public override void OnLoad(Bitmap bmp, Graphics g)
    {
        Edge edgeA = new Edge(300, 300, 500, 300, null, null);
        Edge edgeB = new Edge(500, 300, 700, 500, null, edgeA);
        Edge edgeC = new Edge(700, 500, 500, 900, null, edgeB);
        Edge edgeD = new Edge(500, 900, 400, 1000, null, edgeC);
        Edge edgeE = new Edge(400, 1000, 300, 300, edgeA, edgeD);
        dcel.AddRange(edgeA, edgeB, edgeC, edgeD, edgeE);
    }

    public override void OnMouseDown(
        Bitmap bmp, Graphics g, 
        MouseButtons button, Point point)
        => mouseDown = true;

    public override void OnMouseUp(
            Bitmap bmp, Graphics g, 
            MouseButtons button, Point point)
        => mouseDown = false;

    public override void OnMouseMove(
        Bitmap bmp, Graphics g, 
        MouseButtons button, Point point)
        => cursor = point;

    public override void OnTick(Bitmap bmp, Graphics g)
    {
        g.Clear(Color.White);
        dcel.Draw(g, mouseDown, cursor);
    }

    public override void OnSpace(Bitmap bmp, Graphics g)
    {
        
    }

    public override void OnKeyDown(Bitmap bmp, Graphics g, Keys key)
    {
        if (key == Keys.Left)
        {
            dcel.SelectLeft();
        }
        else if (key == Keys.Right)
        {
            dcel.SelectRight();
        }
        else if (key == Keys.S)
        {
            dcel.Mark();
        }
        else if (key == Keys.O)
        {
            dcel.Orbit();
        }
        else if (key == Keys.P)
        {
            dcel.Select();
        }
    }
}