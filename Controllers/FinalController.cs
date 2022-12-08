using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Numerics;
using System;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

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
    int centX = 0;
    int centY = 0;

    public override void OnLoad(Bitmap bmp, Graphics g)
    {
        AttachConsole(ATTACH_PARENT_PROCESS);
        img = Image.FromFile("img2.png") as Bitmap;

        centX = (bmp.Width - img.Width) / 2;
        centY = (bmp.Height - img.Height) / 2;
        
        dcel = DCEL.FromPoints(new Point(centX + 1, centY + 1),
            new Point(centX + 1, centY + img.Height - 1),
            new Point(centX + img.Width - 1, centY + img.Height - 1),
            new Point(centX + img.Width - 1, centY + 1)
        );

        addRandomPoints(200, 0, img.Width, 0, img.Height);

        dcel.Selected = dcel.Edges.First();
    }

    private void addSmartPoints(int N)
    {
        List<PointF> pts = new List<PointF>();
        var data = img.LockBits(
            new Rectangle(0, 0, img.Width, img.Height),
            ImageLockMode.ReadWrite, 
            PixelFormat.Format24bppRgb);

        unsafe
        {
            int _N = 4;
            byte* p = (byte*)data.Scan0.ToPointer();

            for (int j = 0; j < data.Height; j += 5)
            {
                byte* l = p + j * data.Stride;
                Queue<byte> queue = new Queue<byte>();
                int sum = 0;

                for (int i = 0; i < 3 * data.Width; i += 3, l += 3)
                {
                    queue.Enqueue(l[0]);
                    sum += l[0];

                    if (queue.Count == _N)
                    {
                        sum -= queue.Dequeue();
                    }
                    int diff = sum / queue.Count - l[0];
                    diff = diff < 0 ? -diff : diff;

                    if (diff > 10)
                    {
                        pts.Add(new PointF(i / 3 + centX, j + centY));
                        queue.Clear();
                        sum = 0;
                        i += 15;
                        l += 15;
                    }
                }
            }
        }

        img.UnlockBits(data);

        var final = pts.Count() < N / 2 ? pts :
            pts.OrderBy(p => Random.Shared.Next())
            .Take(N / 2);
        
        foreach (var pt in final)
        {
            dcel.AddPoint(pt.X - 3, pt.Y - 3);
            dcel.AddPoint(pt.X + 3, pt.Y + 3);
        }
    }

    private void addRandomPoints(float N, int minWid, int maxWid, int minHei, int maxHei)
    {
        if (N < 1f)
            return;
        
        int i = Random.Shared.Next(minWid, maxWid);
        int j = Random.Shared.Next(minHei, maxHei);

        dcel.AddPoint(centX + i, centY + j);
        N--;

        float area = (maxHei - minHei) * (maxWid - minWid);

        float 
            a1 = (j - minHei) * (i - minWid),
            a2 = (j - minHei) * (maxWid - i),
            a3 = (maxHei - j) * (i - minWid),
            a4 = (maxHei - j) * (maxWid - i);
        
        float 
            n1 = N * a1 / area,
            n2 = N * a2 / area,
            n3 = N * a3 / area,
            n4 = N * a4 / area;

        addRandomPoints(n1, minWid, i, minHei, j);
        addRandomPoints(n2, i, maxWid, minHei, j);
        addRandomPoints(n3, minWid, i, j, maxHei);
        addRandomPoints(n4, i, maxWid, j, maxHei);
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

            var p = new Point(
                (int)pts
                    .Where(f => f.X > 0)
                    .Min(f => f.X), 
                (int)pts
                    .Where(f => f.Y > 0)
                    .Min(f => f.Y)
                );
            var q = new Point((int)pts.Max(f => f.X), (int)pts.Max(f => f.Y));
            try
            {
                var pColor = img.GetPixel(p.X - centX, p.Y - centY);
                var qColor = img.GetPixel(q.X - centX, q.Y - centY);
                
                if (p.X == q.X && p.Y == q.Y)
                    continue;

                brush = new LinearGradientBrush(p, q, pColor, qColor);
                g.FillPolygon(brush, pts);
                brush.Dispose();
            }
            catch
            {
                Console.WriteLine($"ERROR ON {p.X} {p.Y} {q.X} {q.Y}");
            }
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
            dcel = DCEL.FromPoints(new Point(centX + 1, centY + 1),
                new Point(centX + 1, centY + img.Height - 1),
                new Point(centX + img.Width - 1, centY + img.Height - 1),
                new Point(centX + img.Width - 1, centY + 1)
            );

            addRandomPoints(200, 0, img.Width, 0, img.Height);
        }
        if (key == Keys.X)
        {
            dcel = DCEL.FromPoints(new Point(centX + 1, centY + 1),
                new Point(centX + 1, centY + img.Height - 1),
                new Point(centX + img.Width - 1, centY + img.Height - 1),
                new Point(centX + img.Width - 1, centY + 1)
            );
            addSmartPoints(200);
        }
        if (key == Keys.C)
        {
            img = Image.FromFile("img2.png") as Bitmap;
        }
        if (key == Keys.V)
        {
            img = Image.FromFile("img.png") as Bitmap;
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