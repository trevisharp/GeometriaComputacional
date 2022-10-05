using System.Drawing;
using System.Collections.Generic;

namespace GeometriaComputacional;

public class DCEL
{
    private List<Edge> edges = new List<Edge>();
    public IEnumerable<Edge> Edges => edges;

    public void Add(Edge edge)
        => this.edges.Add(edge);
    
    public void AddRange(params Edge[] edges)
        => this.edges.AddRange(edges);

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
            edge.Draw(g, false);
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
            Split(cursor);
        }
    }
}