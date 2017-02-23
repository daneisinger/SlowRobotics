using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.SRGraph
{
    public class Edge<T> : IEdge<T>
    {
        public INode<T> a { get; set; }
        public INode<T> b { get; set; }


        public Edge(INode<T> _start, INode<T> _end)
        {
            a = _start;
            b = _end;
        }

        public void cleanup()
        {
            a.remove(this);
            b.remove(this);
        }

        public INode<T> Other (INode<T> toThis)
        {
            if (a == toThis) return b;
            if (b == toThis) return a;
            return null;
        }

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

        public IEnumerable<INode<T>> getNaked()
        {
            if (a.Naked) yield return a;
            if (b.Naked) yield return b;
        }

        public IEnumerable<Edge<T>> split(INode<T> at)
        {
            //note - does change Node edges
            //TODO - add flag for nodes to update / cleanup

            yield return new Edge<T>(a, at);
            yield return new Edge<T>(at, b);
        }
    }
}
