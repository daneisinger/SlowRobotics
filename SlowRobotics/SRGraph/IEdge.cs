using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.SRGraph
{
    public interface IEdge<T> 
    {
        INode<T> a { get; set; }
        INode<T> b { get; set; }

        void cleanup();
    }

}
