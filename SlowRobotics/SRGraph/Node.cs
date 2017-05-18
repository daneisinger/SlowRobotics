using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.SRGraph
{
    /// <summary>
    /// Generic node implementation
    /// </summary>
    /// <typeparam name="T">Geometry type</typeparam>
    public class Node<T> : INode<T>
    {
        public int Index { get; set; }
        public int Cost { get; set; }
        public string Tag { get; set; } = "";

        private bool isOpenList = false;
        private bool isClosedList = false;

        public INode<T> parent { get; set; }

        public HashSet<IEdge<T>> Edges { get; set; }
        public T Geometry { get; set; }

        /// <summary>
        /// Default constructor creates a node from a given geometry
        /// </summary>
        /// <param name="_geometry">Node geometry</param>
        /// <param name="Index">Node index</param>
        /// <param name="Tag">Node tag</param>
        /// <param name="Cost">Node cost (used for search)</param>
        public Node(T _geometry, int Index = 0, int Tag = 0, int Cost =0)
        {
            Geometry = _geometry;
            Edges = new HashSet<IEdge<T>>();
        }

        /// <summary>
        /// Compare cost of two nodes
        /// </summary>
        /// <param name="other">Node to compare</param>
        /// <returns></returns>
        public int CompareTo(INode<T> other)
        {
            if (other.Cost > Cost) return -1;
            if (other.Cost < Cost) return 1;
            return 0;
        }

        /// <summary>
        /// Returns true if the node has any connecting edges
        /// </summary>
        public bool Naked
        {
            get
            {
                return Edges.Count <=1 ;
            }
        }

        /// <summary>
        /// Returns the number of connecting edges
        /// </summary>
        public int Valence
        {
            get
            {
                return Edges.Count;
            }
        }

        /// <summary>
        /// Returns connecting nodes
        /// </summary>
        public IEnumerable<INode<T>> Neighbours
        {
            get
            {
                foreach(IEdge<T> e in Edges)
                {
                    yield return e.Other(this);
                }
            }

        }

        /// <summary>
        /// NOT IMPLEMENTED
        /// </summary>
        /// <param name="openList"></param>
        /// <returns></returns>
		public bool IsOpenList(IEnumerable<INode<T>> openList)
        {
            return isOpenList;
        }

        /// <summary>
        /// NOT IMPLEMENTED
        /// </summary>
        /// <param name="value"></param>
        public void SetOpenList(bool value)
        {
            isOpenList = value;
        }

        /// <summary>
        /// NOT IMPLEMENTED
        /// </summary>
        /// <param name="closedList"></param>
        /// <returns></returns>
        public bool IsClosedList(IEnumerable<INode<T>> closedList)
        {
            return isClosedList;
        }

        /// <summary>
        /// NOT IMPLEMENTED
        /// </summary>
        /// <param name="value"></param>
        public void SetClosedList(bool value)
        {
            isClosedList = value;
        }

        /// <summary>
        /// Remove edge from node
        /// </summary>
        /// <param name="edge">Edge to remove</param>
        /// <returns></returns>
        public bool remove(IEdge<T> edge)
        {
            return Edges.Remove(edge);
        }

        /// <summary>
        /// Add edge to the node
        /// </summary>
        /// <param name="edge">Edge to add</param>
        public void add(IEdge<T> edge)
        {
            Edges.Add(edge);

        }
    }
}
