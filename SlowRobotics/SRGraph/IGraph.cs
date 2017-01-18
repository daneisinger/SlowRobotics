﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.SRGraph
{
    public interface IGraph<T, E> where E :IEdge<T>
    {
        List<E> Edges();
        IEnumerable<E> getEdgesFor(T geometry);
        bool removeEdge(E edge);

        bool getNodeAt(T geometry, out INode<T> node);
        bool removeNodeAt(T geometry);
        bool replaceGeometry(T swapThis, T forThat);

        INode<T> insert(T geometry);
        void insert(E edge);

    }
}
