﻿using System;
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
        INode<T> Other(INode<T> toThis);
        bool replaceNode(INode<T> replaceThis, INode<T> withThat);
        void cleanup();
    }

}
