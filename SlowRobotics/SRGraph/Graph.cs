using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.SRGraph
{

    /// <summary>
    /// Generic graph implementation with Edge objects and node lookup table.
    /// </summary>
    /// <typeparam name="T">Geometry type</typeparam>
    /// <typeparam name="E">Edge type</typeparam>
    public class Graph<T,E> : IGraph<T,E> where E : IEdge<T>
    {
        private HashSet<E> _edges;
        private Dictionary<T, INode<T>> _nodeMap;

        public T parent { get; set; } //convenience property for SR

        /// <summary>
        /// Default constructor
        /// </summary>
        public Graph()
        {
            _edges = new HashSet<E>();
            _nodeMap = new Dictionary<T, INode<T>>();
        }

        public Graph(IEqualityComparer<T> comparer)
        {
            _edges = new HashSet<E>();
            _nodeMap = new Dictionary<T, INode<T>>(comparer);
        }

        /// <summary>
        /// Gets number of nodes in the graph
        /// </summary>
        public int Count
        {
            get
            {
                return _nodeMap.Count;
            }
        }

        /// <summary>
        /// Gets nodes in the graph as list
        /// </summary>
        public List<INode<T>> Nodes
        {
            get
            {
                return _nodeMap.Values.ToList();
            }
        }

        /// <summary>
        /// Gets edges in the graph as list
        /// </summary>
        public List<E> Edges
        {
            get
            {
                return _edges.ToList();
            }
        }

        /// <summary>
        /// Gets geometry in the graph as list
        /// </summary>
        public List<T> Geometry
        {
            get
            {
                return _nodeMap.Keys.ToList();
            }
        }

        /// <summary>
        /// Returns all edges that are defined by a given list of nodes
        /// </summary>
        /// <param name="nodes">Nodes defining edges</param>
        /// <returns></returns>
        public List<IEdge<T>> getContainedEdges(IEnumerable<INode<T>> nodes)
        {
            HashSet<IEdge<T>> edges = new HashSet<IEdge<T>>();
            foreach(INode<T> n in nodes)
            {
                foreach(IEdge<T> e in n.Edges)
                {
                    if (nodes.Contains(e.a) && nodes.Contains(e.b)) edges.Add(e);
                }
            }
            return edges.ToList();
        }

        /// <summary>
        /// Gets the node for a given geometry. Returns false if cannot find the node.
        /// </summary>
        /// <param name="geometry">Geometry to test</param>
        /// <param name="node">Node at geometry</param>
        /// <returns></returns>
        public bool getNodeAt(T geometry, out INode<T> node)
        {
            return (_nodeMap.TryGetValue(geometry, out node));
        }

        /// <summary>
        /// Gets the node at a given index. Returns false if cannot find the node.
        /// </summary>
        /// <param name="index">Node index</param>
        /// <param name="node">Note at index</param>
        /// <returns></returns>
        public bool getNodeAt(int index, out INode<T> node)
        {
            if (index > _nodeMap.Count())
            {
                node = null;
                return false;
            }
            node = _nodeMap.Values.ElementAt(index);
            return true;
        }

        /// <summary>
        /// Removes the node with a given geometry
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public bool removeNodeAt(T geometry)
        {
            INode<T> node;
            if (getNodeAt(geometry, out node))
            {
                return removeNode(node);
            }
            return false;
        }

        public void clear()
        {
            removeAllEdges();
            removeAllNodes();
        }
        /// <summary>
        /// Replaces all geometry references in the graph
        /// </summary>
        /// <param name="swapThis">Geometry to replace</param>
        /// <param name="forThat">Replacement</param>
        /// <returns></returns>
        public bool replaceGeometry(T swapThis, T forThat)
        {
            INode<T> a;
            if (!getNodeAt(swapThis, out a)) return false;
            a.Geometry = forThat;
            _nodeMap.Remove(swapThis);
            _nodeMap.Add(forThat, a);
            return true;
        }

        /// <summary>
        /// Merges all nodes and edges of another graph into this one
        /// </summary>
        /// <param name="other">Graph to merge with</param>
        public void merge(Graph<T,E> other)
        {
            foreach (E e in other.Edges) insert(e);
            foreach (INode<T> n in other.Nodes) insert(n);
        }

        /// <summary>
        /// Removes a node and merges all of its edges with replacement
        /// </summary>
        /// <param name="mergeThis">Node to merge</param>
        /// <param name="intoThat">Replacement</param>
        /// <returns></returns>
        public bool mergeInto(T mergeThis, T intoThat)
        {
            INode<T> a;
            INode<T> b;
            if (!getNodeAt(mergeThis, out a) || !getNodeAt(intoThat, out b)) return false;
            if (a == b) return false;
            //add all edges of A into B
            foreach (IEdge<T> e in a.Edges)
            {
                e.replaceNode(a, b);
                b.add(e);
            }
            //remove A
            _nodeMap.Remove(a.Geometry);

            return true;
        }

        /// <summary>
        /// Remove a node from the graph
        /// </summary>
        /// <param name="node">Node to remove</param>
        /// <returns></returns>
        private bool removeNode(INode<T> node)
        {
            foreach (E edge in node.Edges.ToList())
            {
                removeEdge(edge);
            }
            return _nodeMap.Remove(node.Geometry);
        }

        /// <summary>
        /// Removes all edges from the graph
        /// </summary>
        public void removeAllEdges()
        {
            foreach (E edge in _edges) edge.cleanup();
            _edges = new HashSet<E>();
        }

        public void removeAllNodes()
        {
            _nodeMap.Clear();
        }
        /// <summary>
        /// Removes an edge from the graph
        /// </summary>
        /// <param name="edge">Edge to remove </param>
        /// <returns></returns>
        public bool removeEdge(E edge)
        {
            edge.cleanup(); //remove reference from connected nodes
            return _edges.Remove(edge);
        }

        /// <summary>
        /// Gets all edges connecting to a given geometry
        /// </summary>
        /// <param name="geometry">Geometry to get edges for</param>
        /// <returns></returns>
        public IEnumerable<E> getEdgesFor(T geometry)
        {
            INode<T> node;
            if (getNodeAt(geometry, out node))
            {
                foreach (IEdge<T> e in node.Edges) yield return (E)e; //cast to E type for convenience
            }
        }

        /// <summary>
        /// Inserts some geometry into the graph and creates a node if it doesnt exist. Returns the node at the geometry.
        /// </summary>
        /// <param name="geometry">Geometry to insert</param>
        /// <returns></returns>
        public INode<T> insert(T geometry)
        {
            INode<T> node = (_nodeMap.ContainsKey(geometry))? _nodeMap[geometry]:new Node<T>(geometry);
            insert(node);
            return node;
        }

        /// <summary>
        /// Insert a node into the graph
        /// </summary>
        /// <param name="node">Node to insert</param>
        /// <returns></returns>
        private bool insert(INode<T> node)
        {
            lock (_nodeMap)
            {
                bool hasKey = _nodeMap.ContainsKey(node.Geometry);
                if (!hasKey)
                {
                    //set node index to current
                    node.Index = Count;
                    _nodeMap.Add(node.Geometry, node);
                }
                return !hasKey;
            }

        }

        /// <summary>
        /// Insert an edge into the graph and updates any node references
        /// </summary>
        /// <param name="edge">Edge to insert</param>
        public void insert(E edge)
        {
            
            T a = edge.a.Geometry;
            T b = edge.b.Geometry;

            //see if this key already exists and swap pointer if necessary
            if (!insert(edge.a)) edge.a = _nodeMap[a];
            if (!insert(edge.b)) edge.b = _nodeMap[b];

            //add reference to this edge on the nodes
            edge.a.add(edge);
            edge.b.add(edge);

            //insert in graph
            lock (_edges)
            {
                _edges.Add(edge);
            }

        }
    }

}
