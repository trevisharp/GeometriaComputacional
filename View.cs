using System.Drawing;
using System.Windows.Forms;

namespace GeometriaComputacional;

public class View
{
    public Controller Controller { get; set; }

    private Form form;
    private PictureBox pb;
    private Bitmap bmp;
    private Graphics g;
    private Timer tm;

    public void Run()
    {
        ApplicationConfiguration.Initialize();

        form = new Form();
        form.WindowState = FormWindowState.Maximized;
        form.FormBorderStyle = FormBorderStyle.None;

        pb = new PictureBox();
        pb.Dock = DockStyle.Fill;
        form.Controls.Add(pb);

        tm = new Timer();
        tm.Interval = 20;

        form.Load += delegate
        {
            bmp = new Bitmap(pb.Width, pb.Height);
            
            g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            
            pb.Image = bmp;
            pb.Refresh();

            Controller?.SetGraphics(g);
            Controller?.SetBitmap(bmp);
            
            Controller?.OnLoad(bmp, g);

            tm.Start();
        };

        pb.MouseDown += (o, e) =>
        {
            Controller?.OnMouseDown(
                bmp, g, e.Button, e.Location);
        };

        pb.MouseMove += (o, e) =>
        {
            Controller?.OnMouseMove(
                bmp, g, e.Button, e.Location);
        };

        pb.MouseUp += (o, e) =>
        {
            Controller?.OnMouseUp(
                bmp, g, e.Button, e.Location);
        };

        form.KeyDown += (o, e) =>
        {
            if (e.KeyCode == Keys.Escape)
                Application.Exit();
            else if (e.KeyCode == Keys.Space)
                Controller?.OnSpace(bmp, g);
        };

        tm.Tick += delegate
        {
            Controller?.OnTick(bmp, g);
            pb.Refresh();
        };

        Application.Run(form);
    }
}