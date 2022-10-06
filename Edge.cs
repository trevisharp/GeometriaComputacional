using System;
using System.Drawing;

namespace GeometriaComputacional;

public class Edge
{
    public PointF PointA { get; private set; }
    public PointF PointB { get; private set; }
    public Edge Next { get; private set; }
    public Edge Previous { get; private set; }

    public Edge(
        PointF ptA, PointF ptB,
        Edge next, Edge previous
    )
    {
        this.PointA = ptA;
        this.PointB = ptB;

        this.Next = next;
        if (next != null)
            next.Previous = this;

        this.Previous = previous;
        if (previous != null)
            previous.Next = this;
    }

    public Edge(
        float x1, float y1, 
        float x2, float y2,
        Edge next, Edge previous)
    {
        this.PointA = (x1, y1).ToPoint();
        this.PointB = (x2, y2).ToPoint();

        this.Next = next;
        if (next != null)
            next.Previous = this;

        this.Previous = previous;
        if (previous != null)
            previous.Next = this;
    }

    public PointF? TestSplit(PointF pt)
    {
        var p = pt.ToVector();
        var v = PointA.ToVector();
        var u = PointB.ToVector();

        var r = u - v;
        var t = (r.Y, -r.X).ToVector();

        // v + a * r = p + b * t
        // a = (p.X + b * t.X - v.X) / r.X
        // v.Y + (p.X + b * t.X - v.X) * r.Y / r.X = p.Y + b * t.Y
        // v.Y + (p.X - v.X) * r.Y / r.X + b * t.X * r.Y / r.X = p.Y + b * t.Y
        // v.Y + (p.X - v.X) * r.Y / r.X = p.Y + b * t.Y - b * t.X * r.Y / r.X
        // v.Y + (p.X - v.X) * r.Y / r.X - p.Y = b * (t.Y - t.X * r.Y / r.X)
        // (v.Y + (p.X - v.X) * r.Y / r.X - p.Y) / (t.Y - t.X * r.Y / r.X) = b
        var b = (v.Y + (p.X - v.X) * r.Y / r.X - p.Y) / (t.Y - t.X * r.Y / r.X);
        var q = p + t * b;
        var newPt = q.ToPoint();
        
        float mod = (float)(Math.Sqrt(r.X * r.X + r.Y * r.Y));
        float delta = 20 / mod;
        if (b < -delta || b > delta)
            return null;
        
        float a = (p.X + b * t.X - v.X) / r.X;
        if (a < 0 || a > 1)
            return null;

        return newPt;
    }

    public Edge? Split(PointF pt)
    {
        var possiblePt = TestSplit(pt);
        if (!possiblePt.HasValue)
            return null;
        var newPt = possiblePt.Value;

        var pb = this.PointB;
        this.PointB = newPt;
        
        Edge edge = new Edge(newPt, pb, this.Next, this);
        return edge;
    }

    public Edge Connect(Edge edge)
    {
        Edge newEdge = new Edge(
            this.PointB, edge.PointB, edge.Next, this);
        return newEdge;
    }

    public void Draw(Graphics g, bool selected, bool marked)
    {
        Color color = marked ? Color.Red : Color.Black;
        Brush brush = new SolidBrush(color);
        int size = selected ? 20 : 10;
        g.FillEllipse(brush, 
            PointA.X - size / 2, 
            PointA.Y - size / 2, 
            size, size);
        g.FillEllipse(brush, 
            PointB.X - size / 2,
            PointB.Y - size / 2,
            size, size);
        Pen pen = new Pen(color, size / 3);
        g.DrawLine(pen, PointA, PointB);
    }    
}