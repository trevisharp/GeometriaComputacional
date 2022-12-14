// 

// old DCEL

// using System.Linq;
// using System.Drawing;
// using System.Collections.Generic;

// namespace GeometriaComputacional;

// public class DCEL
// {
//     private List<Edge> edges = new List<Edge>();
//     public IEnumerable<Edge> Edges => edges;
//     public Edge Selected { get; set; } = null;
//     public Edge Marked { get; set; } = null;
//     public Edge Target { get; set; } = null;

//     private bool onDown = false;

//     public void Add(Edge edge)
//     {
//         this.edges.Add(edge);
//         this.Selected = edge;
//     }
    
//     public void AddRange(params Edge[] edges)
//     {
//         if (edges.Length == 0)
//             return;
//         this.edges.AddRange(edges);
//         this.Selected = edges[0];
//     }

//     public void Split(PointF p)
//     {
//         for (int i = 0; i < edges.Count; i++)
//         {
//             var edge = edges[i];
//             var newEdge = edge.Split(p);
            
//             if (newEdge != null)
//             {
//                 this.edges.AddRange(newEdge);
//                 return;
//             }
//         }
//     }

//     public void Draw(Graphics g, bool down, PointF cursor)
//     {
//         if (Selected != null)
//             g.FillPolygon(Brushes.Gray, Selected.Face);

//         bool finded = false;
//         foreach (var edge in this.edges)
//         {
//             bool isMarked = Marked != null &&
//                 (edge == Marked || edge?.Twin == Marked);
//             bool isSelected = edge == Selected || edge?.Twin == Selected;
//             bool isTarget = Target != null && (edge == Target || edge?.Twin == Target);
//             bool inOrbit = Selected.Orbit.Any(e => e != null && (e == edge || e == edge?.Twin));
//             edge.Draw(g, isSelected, isMarked, inOrbit, isTarget);
//             if (finded)
//                 continue;
            
//             var pt = edge.TestSplit(cursor);
//             if (!pt.HasValue)
//                 continue;

//             finded = true;
//             g.FillEllipse(Brushes.Black, pt.Value.X - 5,
//                 pt.Value.Y - 5, 10, 10);
//         }

//         if (down)
//         {
//             onDown = true;
//         }
//         else if (onDown)
//         {
//             Target = null;
//             Split(cursor);
//             onDown = false;
//         }
//     }

//     public void Orbit()
//     {
//         if (Target == null)
//         {
//             Target = Selected;
//             return;
//         }

//         var it = Selected.Orbit.GetEnumerator();
//         while (it.MoveNext())
//         {
//             if (it.Current == Target)
//                 break;
//         }
            
//         if (it.MoveNext())
//         {
//             Target = it.Current;
//             return;
//         }
            
//         Target = Selected;
//     }

//     public void Select()
//     {
//         if (Target == null)
//             return;
//         Selected = Target;
//     }

//     public void SelectLeft()
//     {
//         Selected = Selected.Previous;
//         Target = null;
//     }
    
//     public void SelectRight()
//     {
//         Selected = Selected.Next;
//         Target = null;
//     }

//     public void Mark()
//     {
//         Target = null;
//         if (Marked == null)
//             Marked = Selected;
//         else
//         {
//             var newEdge = Marked.Connect(Selected);
//             AddRange(newEdge);
//             Marked = null;
//         }
//     }

//     public void SelectTwin()
//     {
//         this.Selected = this.Selected.Twin 
//             ?? this.Selected;
//     }
// }

// old Edge

// using System;
// using System.Linq;
// using System.Drawing;
// using System.Collections.Generic;

// namespace GeometriaComputacional;

// public class Edge
// {
//     private List<Edge> orbit = new List<Edge>();
//     private Edge next = null;
//     private Edge previous = null;

//     public PointF PointA { get; private set; }
//     public PointF PointB { get; private set; }
//     public Edge Next
//     {
//         get => next;
//         set
//         {
//             AddOrbit(value);
//             this.next = value;
//         }
//     }
//     public Edge Previous
//     {
//         get => previous;
//         set
//         {
//             value?.AddOrbit(this);
//             this.previous = value;
//         }
//     }
//     public IEnumerable<Edge> Orbit => orbit;
//     public Edge Twin { get; set; } = null;

//     public void AddOrbit(Edge value)
//     {
//         if (value == null)
//             return;
//         if (!orbit.Contains(value))
//             orbit.Add(value);
//     }

//     public void AddRangeOrbit(IEnumerable<Edge> value)
//     {
//         foreach (var edge in value)
//             AddOrbit(edge);
//     }

//     public Edge(
//         PointF ptA, PointF ptB,
//         Edge next = null, Edge previous = null
//     )
//     {
//         this.PointA = ptA;
//         this.PointB = ptB;

//         this.Next = next;
//         if (next != null)
//             next.Previous = this;

//         this.Previous = previous;
//         if (previous != null)
//             previous.Next = this;
        
//         this.AddOrbit(this);
//     }

//     public Edge(
//         float x1, float y1, 
//         float x2, float y2,
//         Edge next = null, Edge previous = null)
//     {
//         this.PointA = (x1, y1).ToPoint();
//         this.PointB = (x2, y2).ToPoint();

//         this.Next = next;
//         if (next != null)
//             next.Previous = this;

//         this.Previous = previous;
//         if (previous != null)
//             previous.Next = this;
            
//         this.orbit.Add(this);
//     }

//     public PointF? TestSplit(PointF pt)
//     {
//         var p = pt.ToVector();
//         var v = PointA.ToVector();
//         var u = PointB.ToVector();

//         var r = u - v;
//         var t = (r.Y, -r.X).ToVector();

//         // v + a * r = p + b * t
//         // a = (p.X + b * t.X - v.X) / r.X
//         // v.Y + (p.X + b * t.X - v.X) * r.Y / r.X = p.Y + b * t.Y
//         // v.Y + (p.X - v.X) * r.Y / r.X + b * t.X * r.Y / r.X = p.Y + b * t.Y
//         // v.Y + (p.X - v.X) * r.Y / r.X = p.Y + b * t.Y - b * t.X * r.Y / r.X
//         // v.Y + (p.X - v.X) * r.Y / r.X - p.Y = b * (t.Y - t.X * r.Y / r.X)
//         // (v.Y + (p.X - v.X) * r.Y / r.X - p.Y) / (t.Y - t.X * r.Y / r.X) = b
//         var b = (v.Y + (p.X - v.X) * r.Y / r.X - p.Y) / (t.Y - t.X * r.Y / r.X);
//         var q = p + t * b;
//         var newPt = q.ToPoint();
        
//         float mod = (float)(Math.Sqrt(r.X * r.X + r.Y * r.Y));
//         float delta = 20 / mod;
//         if (b < -delta || b > delta)
//             return null;
        
//         float a = (p.X + b * t.X - v.X) / r.X;
//         if (a < 0 || a > 1)
//             return null;

//         return newPt;
//     }

//     public Edge? split(PointF pt, bool isTwin = false)
//     {
//         var possiblePt = TestSplit(pt);
//         if (!possiblePt.HasValue)
//             return null;
//         var newPt = possiblePt.Value;

//         var pb = this.PointB;
//         this.PointB = newPt;

//         var thisNext = this.next;
        
//         Edge edge = new Edge(newPt, pb);
//         edge.AddRangeOrbit(this.orbit);
//         edge.orbit.Remove(this);

//         edge.next = thisNext;
//         thisNext.previous = edge;

//         edge.previous = this;
//         this.next = edge;

//         return edge;
//     }

//     public Edge[] Split(PointF pt)
//     {
//         var twin = Twin;

//         List<Edge> edges = new List<Edge>();
//         var newEdgeA = this.split(pt);
//         if (newEdgeA == null)
//             return null;
//         edges.Add(newEdgeA);
        
//         if (Twin != null)
//         {
//             var newEdgeB = Twin.split(pt, true);
//             if (newEdgeB == null)
//                 return edges.ToArray();
//             edges.Add(newEdgeB);

//             newEdgeA.Twin = twin;
//             twin.Twin = newEdgeA;

//             newEdgeB.Twin = newEdgeA;
//             newEdgeA.Twin = newEdgeB;

//             twin.orbit.Clear();
//             twin.AddOrbit(twin);
//         }
        
//         this.orbit.Clear();
//         this.AddOrbit(this);
//         this.AddOrbit(newEdgeA);

//         return edges.ToArray();
//     }

//     public Edge[] Connect(Edge edge)
//     {
//         var edgeNext = edge.next;
//         var edgePrevious = edge.Previous;
//         var thisNext = this.Next;
//         var thisPrevious = this.Previous;

//         Edge newEdgeA = new Edge(this.PointB, edge.PointB);

//         newEdgeA.next = edgeNext;
//         edgeNext.previous = newEdgeA;

//         newEdgeA.previous = this;
//         this.next = newEdgeA;
        
//         newEdgeA.AddRangeOrbit(edge.orbit);
//         edge.orbit.Add(newEdgeA);

//         Edge newEdgeB = new Edge(edge.PointB, this.PointB);

//         newEdgeB.next = thisNext;
//         thisNext.previous = newEdgeB;

//         newEdgeB.previous = edge;
//         edge.next = newEdgeB;

//         newEdgeB.AddRangeOrbit(this.orbit);
//         this.orbit.Add(newEdgeB);

//         newEdgeA.Twin = newEdgeB;
//         newEdgeB.Twin = newEdgeA;

//         return new Edge[]
//         {
//             newEdgeA,
//             newEdgeB
//         };
//     }

//     public void Draw(Graphics g, bool selected, bool marked, bool orbit, bool target)
//     {
//         int size = selected ? 20 : 10;
//         Color color = target ? Color.Orange : 
//             (marked ? Color.Red : 
//             (orbit ? Color.LimeGreen : Color.Black));
//         Brush brush = new SolidBrush(color);
//         Pen pen3 = new Pen(color, size / 3);

//         g.FillEllipse(brush, 
//             PointA.X - size / 2, 
//             PointA.Y - size / 2, 
//             size, size);
//         g.FillEllipse(brush, 
//             PointB.X - size / 2,
//             PointB.Y - size / 2,
//             size, size);
//         if (selected || marked)
//         {
//             Pen pen5 = new Pen(color, size / 5);
//             g.DrawEllipse(pen5, 
//                 PointB.X - 2 * size / 3,
//                 PointB.Y - 2 * size / 3,
//                 4 * size / 3, 4 * size / 3);
//         }
//         g.DrawLine(pen3, PointA, PointB);
//     }    

//     public PointF[] Face
//     {
//         get
//         {
//             var it = this;

//             List<PointF> pts = new List<PointF>();
//             pts.Add(it.PointB);
//             it = it.Next;

//             while (it != this)
//             {
//                 pts.Add(it.PointB);
//                 it = it.next;
//             }

//             return pts.ToArray();
//         }
//     }
// }