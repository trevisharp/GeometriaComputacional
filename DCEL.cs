using System.Drawing;
using System.Collections.Generic;

namespace GeometriaComputacional;

public class DCEL
{
    private List<Edge> edges = new List<Edge>();
    public IEnumerable<Edge> Edges => edges;
    public Edge Selected { get; set; } = null;
    public Edge Marked { get; set; } = null;

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
        bool finded = false;
        foreach (var edge in this.edges)
        {
            edge.Draw(g, edge == Selected, edge == Marked);
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
            Split(cursor);
            onDown = false;
        }
    }

    public void SelectLeft()
        => Selected = Selected.Previous;
    
    public void SelectRight()
        => Selected = Selected.Next;

    public void Mark()
    {
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