using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.SRGraph
{
    public class Graph
    {

        private readonly NodeList _nodes;
        private readonly EdgeList _edges;


        /// <summary>
        /// 
        /// </summary>
        public Graph()
        {
            _nodes = new NodeList(this);
            _edges = new EdgeList(this);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeCapacity"></param>
        /// <param name="edgeCapacity"></param>
        public Graph(int nodeCapacity, int edgeCapacity)
        {
            _nodes = new NodeList(this, nodeCapacity);
            _edges = new EdgeList(this, edgeCapacity);
        }


        /// <summary>
        /// Creates a deep copy of the given graph.
        /// </summary>
        /// <param name="other"></param>
        public Graph(Graph other)
        {
            var otherNodes = other._nodes;
            var otherEdges = other._edges;

            _nodes = new NodeList(this, otherNodes.Count);
            _edges = new EdgeList(this, otherEdges.Count);

            // add all nodes
            for (int i = 0; i < otherNodes.Count; i++)
                _nodes.Add(otherNodes[i].EdgeCapacity);

            // add all edges
            for (int i = 0; i < otherEdges.Count; i++)
            {
                Edge e = otherEdges[i];

                if (e.IsUnused)
                    _edges.Add(new Edge());
                else
                    _edges.AddImpl(_nodes[e.Start.Index], _nodes[e.End.Index]);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public NodeList Nodes
        {
            get { return _nodes; }
        }


        /// <summary>
        /// 
        /// </summary>
        public EdgeList Edges
        {
            get { return _edges; }
        }


        /// <summary>
        /// Removes all flagged nodes and edges from the graph.
        /// </summary>
        public void Compact()
        {
            _nodes.Compact();
            _edges.Compact();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("Graph (N:{0} E:{1})", _nodes.Count, _edges.Count);
        }
    }
}
