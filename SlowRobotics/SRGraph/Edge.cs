using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.SRGraph
{
    /*
    public class Edge : IEdge
    {
        public int ai
        {
            get;
        }
        public int bi
        { 
            get;
        }

        public Edge(int _ai, int _bi)
        {
            ai = _ai;
            bi = _bi;
        }
    }*/

        /// <summary>
        /// Generic edge class implementation
        /// </summary>
        /// <typeparam name="T"></typeparam>
    public class Edge<T> : IEdge<T>
    {
        public INode<T> a { get; set; }
        public INode<T> b { get; set; }
        public int index { get; set; }
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="_start">Node at start of edge</param>
        /// <param name="_end">Node at end of edge</param>
        public Edge(INode<T> _start, INode<T> _end) 
        {
            a = _start;
            b = _end;
        }

        /// <summary>
        /// removes references to this edge in nodes
        /// </summary>
        public void cleanup()
        {
            a.remove(this);
            b.remove(this);
        }

        /// <summary>
        /// Gets the other node in the edge if it exists
        /// </summary>
        /// <param name="toThis">Node not to get</param>
        /// <returns></returns>
        public INode<T> Other (INode<T> toThis)
        {
            if (a == toThis) return b;
            if (b == toThis) return a;
            return null;
        }

        /// <summary>
        /// Gets a common node between two edges if it exists
        /// </summary>
        /// <param name="toThis">Test edge</param>
        /// <returns></returns>
        public INode<T> Common (IEdge<T> toThis)
        {
            if (a == toThis.a || a == toThis.b) return a;
            if (b == toThis.b || b == toThis.b) return b;
            return null;
        }

        /// <summary>
        /// Replace a node in the edge if it exists
        /// </summary>
        /// <param name="replaceThis">Node to replace</param>
        /// <param name="withThat">Replacement</param>
        /// <returns></returns>
        public bool replaceNode(INode<T> replaceThis, INode<T> withThat)
        {
            if (a == replaceThis)
            {
                a = withThat;
                return true;
            }else if (b == replaceThis)
            {
                b = withThat;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns nodes that are naked
        /// </summary>
        /// <returns></returns>
        public IEnumerable<INode<T>> getNaked()
        {
            if (a.Naked) yield return a;
            if (b.Naked) yield return b;
        }

        /// <summary>
        /// Splits this edge into two new edges
        /// </summary>
        /// <param name="at">Splitting node shared between new edges</param>
        /// <returns></returns>
        public IEnumerable<Edge<T>> split(INode<T> at)
        {
            //note - does change Node edges
            //TODO - add flag for nodes to update / cleanup

            yield return new Edge<T>(a, at);
            yield return new Edge<T>(at, b);
        }
    }
}
