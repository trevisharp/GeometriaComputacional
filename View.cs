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

            tm.Start();
        };

        form.MouseDown += (o, e) =>
        {
            Controller?.OnMouseDown(e.Button, e.Location);
        };

        form.MouseMove += (o, e) =>
        {
            Controller?.OnMouseMove(e.Button, e.Location);
        };

        form.MouseUp += (o, e) =>
        {
            Controller?.OnMouseUp(e.Button, e.Location);
        };

        form.KeyDown += (o, e) =>
        {
            if (e.KeyCode == Keys.Escape)
                Application.Exit();
            else if (e.KeyCode == Keys.Space)
                Controller?.OnSpace();
        };

        tm.Tick += delegate
        {
            Controller?.OnTick();
        };

        Application.Run(form);
    }
}