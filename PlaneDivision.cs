using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

namespace GeometriaComputacional;

public class PlaneDivision
{
    public PlaneDivision(Point start, Point end)
    {
        this.Rectangle = new Rectangle(start.X, start.Y, 
            end.X - start.X, end.Y - start.Y);
    }

    public PlaneDivision(int sx, int sy, int ex, int ey) 
        : this(new Point(sx, sy), new Point(ex, ey)) { }

    public Rectangle Rectangle { get; private set; }

    private List<PlaneDivision> child = new List<PlaneDivision>();
    public IEnumerable<PlaneDivision> Children => child;

    public void Divide(Point pt)
    {
        if (!Rectangle.Contains(pt))
            return;
        
        if (child.Count == 0)
        {
            divide(pt);
            return;
        }

        foreach (var child in Children)
            child.divide(pt);
    }

    private void divide(Point pt)
    {
        var s = Rectangle.Location;
        var e = Rectangle.Location + Rectangle.Size;
        child.Add(new PlaneDivision(s, pt));
        child.Add(new PlaneDivision(pt.X, s.Y, e.X, pt.Y));
        child.Add(new PlaneDivision(s.X, pt.Y, pt.X, s.Y));
        child.Add(new PlaneDivision(pt, e));
    }
}