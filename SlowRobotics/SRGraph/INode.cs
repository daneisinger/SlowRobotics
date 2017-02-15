﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.SRGraph
{
    public interface INode<T> 
    {
        int Index { get; set; }
        int Cost { get; set; }

        bool IsOpenList(IEnumerable<INode<T>> openList);
        void SetOpenList(bool value);
        bool IsClosedList(IEnumerable<INode<T>> closedList);
        void SetClosedList(bool value);

        HashSet<IEdge<T>> Edges { get; set; }
        INode<T> parent { get; set; }

        bool remove(IEdge<T> edge);
        void add(IEdge<T> edge);

        T Geometry { get; set; }
    }
}
