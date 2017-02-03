using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.SRGraph
{
    public class Node<T> : INode<T>
    {
        public int Index { get; set; }
        public int Tag { get; set; }

        public HashSet<IEdge<T>> Edges { get; set; }
        public T Geometry { get; set; }

        public Node(T _geometry, int Index = 0, int Tag = 0)
        {
            Geometry = _geometry;
            Edges = new HashSet<IEdge<T>>();
        }

        public bool remove(IEdge<T> edge)
        {
            return Edges.Remove(edge);
        }

        public void add(IEdge<T> edge)
        {
            Edges.Add(edge);

        }
    }
}
