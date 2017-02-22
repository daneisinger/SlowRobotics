using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.SRGraph
{
    public class Node<T> : INode<T>
    {
        public int Index { get; set; }
        public int Cost { get; set; }

        private bool isOpenList = false;
        private bool isClosedList = false;

        public INode<T> parent { get; set; }

        public HashSet<IEdge<T>> Edges { get; set; }
        public T Geometry { get; set; }

        public Node(T _geometry, int Index = 0, int Tag = 0)
        {
            Geometry = _geometry;
            Edges = new HashSet<IEdge<T>>();
        }

        public bool Naked
        {
            get
            {
                return Edges.Count <=1 ;
            }
        }

		public bool IsOpenList(IEnumerable<INode<T>> openList)
        {
            return isOpenList;
        }

        public void SetOpenList(bool value)
        {
            isOpenList = value;
        }

        public bool IsClosedList(IEnumerable<INode<T>> closedList)
        {
            return isClosedList;
        }

        public void SetClosedList(bool value)
        {
            isClosedList = value;
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
