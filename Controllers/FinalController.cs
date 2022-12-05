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
        
        for (int i = 0; i < 1; i++)
        {
            Point pt = new Point(
                Random.Shared.Next(centX, centX + img.Width),
                Random.Shared.Next(centY, centY + img.Height)
            );
            dcel.AddPoint(pt);
        }
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
        if (approxOn) drawAprox(bmp, g);
        else drawReal(bmp, g);
        dcel.Draw(g);
    }

    public override void OnKeyDown(Bitmap bmp, Graphics g, Keys key)
    {
        if (key == Keys.Space)
            approxOn = true;
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