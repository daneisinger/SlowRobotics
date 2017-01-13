using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.SRGraph
{

    //Based on David Reeves SlurGraph
    //https://github.com/daveReeves/SpatialSlur/blob/master/SpatialSlur/SlurGraph

    public class Edge : GraphElement
    {

        private Node _start;
        private Node _end;
        //private E _data;


        /// <summary>
        /// 
        /// </summary>
        internal Edge()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        internal Edge(Node start, Node end)
        {
            _start = start;
            _end = end;
        }


        /// <summary>
        /// 
        /// </summary>
        public Node Start
        {
            get { return _start; }
            internal set { _start = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        public Node End
        {
            get { return _end; }
            internal set { _end = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public Node Other(Node node)
        {
            if (node == _start)
                return _end;
            else if (node == _end)
                return _start;

            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        public override bool IsUnused
        {
            get { return _start == null; }
        }


        /// <summary>
        /// 
        /// </summary>
        internal void OnRemove()
        {
            _start.Degree--;
            _end.Degree--;
            _start = null; // flag as unused
        }
    }
}
