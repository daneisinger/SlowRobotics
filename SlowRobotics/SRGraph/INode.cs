using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.SRGraph
{
    public interface INode<T> 
    {
        int Index { get; set; }
        int Tag { get; set; } // tag for topological searches, validation etc.

        HashSet<IEdge<T>> Edges { get; set; }

        bool remove(IEdge<T> edge);
        void add(IEdge<T> edge);

        T Geometry { get; set; }
    }
}
