using System;
using System.Linq;
using System.Drawing;
using System.Numerics;
using System.Collections.Generic;

namespace GeometriaComputacional;

public class Edge
{
    private List<Edge> orbit = new List<Edge>();

    public DCEL Parent { get; private set; }
    public PointF PointA { get; private set; }
    public PointF PointB { get; private set; }
    public Edge Next { get; private set; }
    public Edge Previous { get; private set; }
    public Edge Twin { get; private set; }
    public IEnumerable<Edge> Orbit => orbit;

    public bool Horizontal => this.PointA.Y == this.PointB.Y;
    public bool Vertical => this.PointA.X == this.PointB.X;

    public Edge Oposite
    {
        get
        {
            var crr = this.Next;

            if (Horizontal)
            {
                while (!crr.Horizontal ||
                    this.PointB.X != crr.PointA.X ||
                    this.PointA.Y == crr.PointA.Y)
                {
                    if (crr == this)
                        break;
                    crr = crr.Next;
                }
            }
            else if (Vertical)
            {
                while (!crr.Vertical ||
                    this.PointB.Y != crr.PointA.Y ||
                    this.PointA.X == crr.PointA.X)
                {
                    if (crr == this)
                        break;
                    crr = crr.Next;
                }
            }
            else return null;
            
            if (crr == this || crr == this.Twin)
                return null;
            
            return crr;
        }
    }

    public Edge NextK(int k)
    {
        Edge crr = this;
        for (int i = 0; i < k; i++)
            crr = crr.Next;
        return crr;
    }

    public Edge[] Connect(Edge next)
    {
        var oldNext = this.Next;
        var oldPrevious = next.Previous;

        Edge edge = new Edge();
        edge.PointA = this.PointB;
        edge.PointB = next.PointA;
        edge.Parent = this.Parent;

        this.Next = edge;
        edge.Previous = this;
        next.Previous = edge;
        edge.Next = next;


        Edge twin = new Edge();
        twin.PointA = next.PointA;
        twin.PointB = this.PointB;
        twin.Parent = this.Parent;

        oldNext.Previous = twin;
        twin.Next = oldNext;
        oldPrevious.Next = twin;
        twin.Previous = oldPrevious;

        edge.Twin = twin;
        twin.Twin = edge;

        return new Edge[]
        {
            edge, twin
        };
    }

    public Edge[] Split(PointF splitPoint, DCEL dcel)
    {
        var nullabeNewEdgePoints = this.split(splitPoint);

        if (!nullabeNewEdgePoints.HasValue)
            return new Edge[0];
        var newEdgePoints = nullabeNewEdgePoints.Value;

        this.PointB = newEdgePoints.pa;
        
        Edge edge = new Edge();
        edge.PointA = newEdgePoints.pa;
        edge.PointB = newEdgePoints.pb;
        edge.Parent = dcel;

        edge.Next = this.Next;
        this.Next.Previous = edge;

        edge.Previous = this;
        this.Next = edge;


        bool needTwin = Twin != null;
        if (needTwin)
            Twin.PointA = newEdgePoints.pa;

        if (!needTwin)
            return new Edge[]
            {
                edge
            };
        
        Edge twin = new Edge();
        twin.PointA = newEdgePoints.pb;
        twin.PointB = newEdgePoints.pa;
        twin.Parent = dcel;

        twin.Previous = Twin.Previous;
        Twin.Previous.Next = twin;

        twin.Next = Twin;
        Twin.Previous = twin;

        twin.Twin = edge;
        edge.Twin = twin;

        return new Edge[]
        {
            edge, twin
        };
    }

    private (PointF pa, PointF pb)? split(PointF splitPoint)
    {
        var va = new Vector2(PointA.X, PointA.Y);
        var vb = new Vector2(PointB.X, PointB.Y);
        var r = vb - va;
        var rMod = Vector2.Distance(va, vb);

        var vp = new Vector2(splitPoint.X, splitPoint.Y);
        var s = vp - va;

        var vetorProj = Vector2.Dot(r, s) / rMod;

        if (vetorProj < 0 || vetorProj > rMod)
            return null;

        var rUnit = r / rMod;
        var realSplitVetor = va + rUnit * vetorProj;
        var realSplitPoint = new PointF(
            realSplitVetor.X, realSplitVetor.Y);
        
        PointF pointA = realSplitPoint;
        PointF pointB = this.PointB;
        
        return (pointA, pointB);
    }

    public static Edge[] NewEdge(
        PointF pa, PointF pb, 
        DCEL dcel,
        Edge prevEdge = null,
        Edge nextEdge = null,
        bool makeTwin = true)
    {
        Edge edge = new Edge();
        edge.PointA = pa;
        edge.PointB = pb;
        edge.Parent = dcel;

        Edge twin = new Edge();
        twin.PointA = pb;
        twin.PointB = pa;
        twin.Parent = dcel;

        if (makeTwin)
        {
            edge.Twin = twin;
            twin.Twin = edge;
        }

        if (prevEdge != null)
        {
            prevEdge.Next = edge;
            edge.Previous = prevEdge;

            if (makeTwin && prevEdge.Twin != null)
            {
                prevEdge.Twin.Previous = twin;
                twin.Next = prevEdge.Twin;
            }
        }

        if (nextEdge != null)
        {
            nextEdge.Previous = edge;
            edge.Next = nextEdge;

            if (makeTwin && nextEdge.Twin != null)
            {
                nextEdge.Twin.Next = twin;
                twin.Previous = nextEdge.Twin;
            }
        }

        if (!makeTwin)
            return new Edge[] { edge };

        return new Edge[]
        {
            edge, twin
        };
    }

    public override string ToString()
        => $"({PointA}, {PointB})";
}