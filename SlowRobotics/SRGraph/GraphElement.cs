using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.SRGraph
{
    //Based on David Reeves SlurGraph
    //https://github.com/daveReeves/SpatialSlur/blob/master/SpatialSlur/SlurGraph

    public abstract class GraphElement
    {
        private int _index = -1;
        private int _tag; // tag for topological searches, validation etc.


        /// <summary>
        /// Returns the element's position within the collection of the parent graph.
        /// </summary>
        public int Index
        {
            get { return _index; }
            internal set { _index = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        internal int Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public abstract bool IsUnused { get; }


        /// <summary>
        /// 
        /// </summary>
        internal void UsedCheck()
        {
            if (IsUnused)
                throw new ArgumentException("The given element must be in use.");
        }
    }
}
