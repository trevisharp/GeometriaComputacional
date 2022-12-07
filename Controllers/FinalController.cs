using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Numerics;
using System;
using System.Runtime.InteropServices;

namespace GeometriaComputacional;

public class FinalController : Controller
{
    [DllImport( "kernel32.dll" )]
    static extern bool AttachConsole( int dwProcessId );
    private const int ATTACH_PARENT_PROCESS = -1;

    Bitmap img = null;
    DCEL dcel = null;
    bool approxOn = false;
    bool showDCEL = true;
    bool showImg = true;

    public override void OnLoad(Bitmap bmp, Graphics g)
    {
        AttachConsole(ATTACH_PARENT_PROCESS);
        img = Image.FromFile("img.png") as Bitmap;
        int centX = (bmp.Width - img.Width) / 2,
            centY = (bmp.Height - img.Height) / 2;
        dcel = DCEL.FromPoints(new Point(centX + 1, centY + 1),
            new Point(centX + 1, centY + img.Height - 1),
            new Point(centX + img.Width - 1, centY + img.Height - 1),
            new Point(centX + img.Width - 1, centY + 1));
        
        dcel.AddPoint(centX + img.Width / 2, centY + img.Height / 2);
        dcel.AddPoint(centX + img.Width / 4, centY + 3 * img.Height / 4);
        dcel.AddPoint(centX + img.Width / 4 + 50, centY + img.Height / 4 + 50);

        dcel.Selected = dcel.Edges.FirstOrDefault().Previous.Oposite;
    }

    private void drawAprox(Bitmap bmp, Graphics g)
    {
        int centX = (bmp.Width - img.Width) / 2,
            centY = (bmp.Height - img.Height) / 2;
        var faces = dcel.Faces;
        Brush brush;
        
        foreach (var face in faces)
        {
            var pts = face.Select(f => f.PointB).ToArray();

            // foreach (var x in pts)
            //     Write(x + " ");
            // WriteLine();

            var p = new Point((int)pts.Min(f => f.X), (int)pts.Min(f => f.Y));
            var q = new Point((int)pts.Max(f => f.X), (int)pts.Max(f => f.Y));
            var pColor = img.GetPixel(p.X - centX, p.Y - centY);
            var qColor = img.GetPixel(q.X - centX, q.Y - centY);
            
            if (p.X == q.X && p.Y == q.Y)
                continue;

            brush = new LinearGradientBrush(p, q, pColor, qColor);
            g.FillPolygon(brush, pts);
            brush.Dispose();
        }
    }

    private void drawReal(Bitmap bmp, Graphics g)
    {
        int centX = (bmp.Width - img.Width) / 2,
            centY = (bmp.Height - img.Height) / 2;
        g.DrawImage(img,
            new Rectangle(centX, centY, img.Width, img.Height),
            new Rectangle(0, 0, img.Width, img.Height),
            GraphicsUnit.Pixel);
    }

    public override void OnTick(Bitmap bmp, Graphics g)
    {
        g.Clear(Color.White);
        if (showImg)
        {
            if (approxOn) drawAprox(bmp, g);
            else drawReal(bmp, g);
        }
        if (showDCEL) dcel.Draw(g);
    }

    public override void OnKeyDown(Bitmap bmp, Graphics g, Keys key)
    {
        if (key == Keys.Space)
            approxOn = true;
        if (key == Keys.S)
            showDCEL = !showDCEL;
        if (key == Keys.I)
            showImg = !showImg;
        if (key == Keys.N)
            dcel.Selected = dcel.Selected?.Next;
        if (key == Keys.T)
            dcel.Selected = dcel.Selected.Twin ?? dcel.Selected;
        if (key == Keys.O)
            dcel.Selected = dcel.Selected.Oposite ?? dcel.Selected;
        if (key == Keys.P)
            dcel.Selected = dcel.Selected?.Previous;
        if (key == Keys.Z)
        {
            int centX = (bmp.Width - img.Width) / 2,
                centY = (bmp.Height - img.Height) / 2;
            dcel.AddPoint(centX + img.Width / 6 + 50, centY + img.Height / 4 + 150, true);
        }   
    }

    public override void OnKeyUp(Bitmap bmp, Graphics g, Keys key)
    {
        if (key == Keys.Space)
            approxOn = false;
    }

    public override void OnMouseDown(Bitmap bmp, Graphics g, MouseButtons button, Point point)
    {
        dcel.AddPoint(point);
    }
}