using System.Linq;
using System.Drawing;
using System.Collections.Generic;

namespace GeometriaComputacional;

public class DCEL
{
    private List<Edge> edges = new List<Edge>();
    public IEnumerable<Edge> Edges => edges;
    public Edge Selected { get; set; } = null;
    public Edge Marked { get; set; } = null;
    public Edge Target { get; set; } = null;

    private bool onDown = false;

    public void Add(Edge edge)
    {
        this.edges.Add(edge);
        this.Selected = edge;
    }
    
    public void AddRange(params Edge[] edges)
    {
        if (edges.Length == 0)
            return;
        this.edges.AddRange(edges);
        this.Selected = edges[0];
    }

    public void Split(PointF p)
    {
        for (int i = 0; i < edges.Count; i++)
        {
            var edge = edges[i];
            var newEdge = edge.Split(p);
            
            if (newEdge != null)
            {
                this.edges.Add(newEdge);
                return;
            }
        }
    }

    public void Draw(Graphics g, bool down, PointF cursor)
    {
        g.FillPolygon(Brushes.Gray, Selected.Area);

        bool finded = false;
        foreach (var edge in this.edges)
        {
            edge.Draw(g, 
                edge == Selected, 
                edge == Marked,
                Selected.Orbit.Any(e => e == edge),
                edge == Target);
            if (finded)
                continue;
            
            var pt = edge.TestSplit(cursor);
            if (!pt.HasValue)
                continue;

            finded = true;
            g.FillEllipse(Brushes.Black, pt.Value.X - 5,
                pt.Value.Y - 5, 10, 10);
        }

        if (down)
        {
            onDown = true;
        }
        else if (onDown)
        {
            Target = null;
            Split(cursor);
            onDown = false;
        }
    }

    public void Orbit()
    {
        if (Target == null)
        {
            Target = Selected;
            return;
        }

        var it = Selected.Orbit.GetEnumerator();
        while (it.MoveNext())
        {
            if (it.Current == Target)
                break;
        }
            
        if (it.MoveNext())
        {
            Target = it.Current;
            return;
        }
            
        Target = Selected;
    }

    public void Select()
    {
        if (Target == null)
            return;
        Selected = Target;
    }

    public void SelectLeft()
    {
        Selected = Selected.Previous;
        Target = null;
    }
    
    public void SelectRight()
    {
        Selected = Selected.Next;
        Target = null;
    }

    public void Mark()
    {
        Target = null;
        if (Marked == null)
            Marked = Selected;
        else
        {
            var newEdge = Marked.Connect(Selected);
            Add(newEdge);
            Marked = null;
        }
    }
}