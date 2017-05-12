using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.SRGraph
{
    /*
    public interface IEdge
    {
        int ai { get; }
        int bi { get; }
    }*/

    /// <summary>
    /// Generic edge interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEdge<T>
    {
        INode<T> a { get; set; }
        INode<T> b { get; set; }
        INode<T> Other(INode<T> toThis);
        INode<T> Common(IEdge<T> toThis);
        bool replaceNode(INode<T> replaceThis, INode<T> withThat);
        void cleanup();
    }

}
