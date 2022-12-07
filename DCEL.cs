using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

namespace GeometriaComputacional;

public class DCEL
{
    private List<Edge> edges = new List<Edge>();
    public IEnumerable<Edge> Edges => edges;
    public Edge Selected { get; set; }

    public Edge AddEdge(PointF pa, PointF pb, 
        Edge prev = null, Edge next = null, bool gettwin = true)
    {
        var edges = Edge.NewEdge(pa, pb, this, prev, next, gettwin);
        
        if (edges.Length == 0)
            return null;
        
        if (!gettwin)
            this.edges.Add(edges[0]);
        else this.edges.AddRange(edges);
        return edges[0];
    }

    public (Edge, Edge) Split(Edge edge, PointF splitPoint)
    {
        if (!this.edges.Contains(edge))
            return (null, null);
        
        var edges = edge.Split(splitPoint, this);
        this.edges.AddRange(edges);

        if (edges.Length == 0)
            return (null, null);

        return (edges[0], edges.Length > 1 ? edges[1] : null);
    }

    public (Edge, Edge) Connect(Edge edgeA, Edge edgeB)
    {
        var edges = edgeA.Connect(edgeB);
        this.edges.AddRange(edges);
        return (edges[0], edges[1]);
    }

    public void AddPoint(float x, float y, bool test = false)
        => this.AddPoint(new PointF(x, y), test);

    public void AddPoint(PointF point, bool test = false)
    {
        var face = FindFace(point);
        if (face == null)
            return;
    
        foreach (var edge in face)
            Split(edge, point);

        (Edge ed, Edge tw) = Connect(face[0], face[2].Next);

        if (test)
            return;

        Split(ed, point);
        
        Connect(ed, face[0].Previous);
        Connect(tw.Previous, face[1].Next);
    }

    public Edge[] FindFace(PointF point)
    {
        foreach (var face in this.Faces)
        {
            var poly = (
                from edge in face
                select edge.PointB into pt
                select new Vector3(pt.X, pt.Y, 0)
            ).ToArray();
            bool isIn = true;
            var vp = new Vector3(point.X, point.Y, 0);
            
            for (int i = 0; i < poly.Length; i++)
            {
                int j = (i + 1) % poly.Length;
                var v = poly[j] - poly[i];
                var r = vp - poly[i];
                var prod = Vector3.Cross(v, r);
                if (prod.Z > 0)
                {
                    isIn = false;
                    break;
                }
            }

            if (!isIn)
                continue;

            return face;
        }

        return null;
    }

    public void Draw(Graphics g)
    {
        Brush brush;
        Pen pen;
        foreach (var edge in this.edges)
        {
            brush = edge.Next == null || edge.Previous == null
                ? Brushes.Red : Brushes.Green;
            pen = edge.Next == null || edge.Previous == null
                ? Pens.Red : Pens.Green;
            g.FillEllipse(brush,
                edge.PointA.X - 4, edge.PointA.Y - 4,
                8, 8);
            g.FillEllipse(brush,
                edge.PointB.X - 4, edge.PointB.Y - 4,
                8, 8);
            g.DrawLine(pen, edge.PointA, edge.PointB);
        }
        if (Selected == null)
            return;
        pen = new Pen(Brushes.Yellow, 3f);
        brush = Brushes.Yellow;
        g.FillEllipse(brush,
            Selected.PointA.X - 4, Selected.PointA.Y - 4,
            8, 8);
        g.FillEllipse(brush,
            Selected.PointB.X - 4, Selected.PointB.Y - 4,
            8, 8);
        g.DrawLine(pen, Selected.PointA, Selected.PointB);

    }

    public IEnumerable<Edge[]> Faces
    {
        get
        {
            List<Edge> visited = new List<Edge>();
            List<Edge> edges = null;

            foreach (var edge in this.edges)
            {
                if (visited.Contains(edge))
                    continue;
                edges = new List<Edge>();
                var crr = edge;

                while (crr != null && !visited.Contains(crr))
                {
                    // Write($"{crr} -> ");
                    visited.Add(crr);
                    edges.Add(crr);
                    crr = crr.Next;
                }
                // WriteLine();

                yield return edges.ToArray();
            }
        }
    }

    public static DCEL FromPoints(params Point[] pts)
    {
        DCEL dcel = new DCEL();

        Edge first = null, last = null;
        first = last = dcel.AddEdge(pts[0], pts[1], null, null, false);

        for (int i = 1; i < pts.Length - 1; i++)
            last = dcel.AddEdge(pts[i], pts[i + 1], last, null, false);
        
        dcel.AddEdge(
            pts[pts.Length - 1], 
            pts[0],
            last, first, false);
        
        return dcel;
    }
}