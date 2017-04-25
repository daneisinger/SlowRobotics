using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.SRGraph
{
    public class Graph<T,E> : IGraph<T,E> where E :IEdge<T>
    {
        private HashSet<E> _edges;
        private Dictionary<T, INode<T>> _nodeMap;

        public T parent { get; set; } //convenience property for SR

        public Graph()
        {
            _edges = new HashSet<E>();
            _nodeMap = new Dictionary<T, INode<T>>();
        }

        public int Count
        {
            get
            {
                return _nodeMap.Count;
            }
        }

        public List<INode<T>> Nodes
        {
            get
            {
                return _nodeMap.Values.ToList();
            }
        }

        public List<E> Edges
        {
            get
            {
                return _edges.ToList();
            }
        }

        public List<T> Geometry
        {
            get
            {
                return _nodeMap.Keys.ToList();
            }
        }

        
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


        public bool getNodeAt(T geometry, out INode<T> node)
        {
            return (_nodeMap.TryGetValue(geometry, out node));
        }

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

        public bool removeNodeAt(T geometry)
        {
            INode<T> node;
            if (getNodeAt(geometry, out node))
            {
                return removeNode(node);
            }
            return false;
        }


        public bool replaceGeometry(T swapThis, T forThat)
        {
            INode<T> a;
            if (!getNodeAt(swapThis, out a)) return false;

            a.Geometry = forThat;
            _nodeMap.Remove(swapThis);
            _nodeMap.Add(forThat, a);
            return true;
        }

        public void merge(Graph<T,E> other)
        {
            foreach (E e in other.Edges) _edges.Add(e);
            foreach (INode<T> n in other.Nodes) insert(n);
        }

        public bool mergeInto(T mergeThis, T intoThat)
        {
            INode<T> a;
            INode<T> b;
            if (!getNodeAt(mergeThis, out a) || !getNodeAt(intoThat, out b)) return false;

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

        private bool removeNode(INode<T> node)
        {
            foreach (E edge in node.Edges.ToList())
            {
                removeEdge(edge);
            }
            return _nodeMap.Remove(node.Geometry);
        }

        public void removeAllEdges()
        {
            foreach (E edge in _edges) edge.cleanup();
            _edges = new HashSet<E>();
        }

        public bool removeEdge(E edge)
        {
            edge.cleanup(); //remove reference from connected nodes
            return _edges.Remove(edge);
        }

        public IEnumerable<E> getEdgesFor(T geometry)
        {
            INode<T> node;
            if (getNodeAt(geometry, out node))
            {
                foreach (IEdge<T> e in node.Edges) yield return (E)e; //cast to E type for convenience
            }
        }

        public INode<T> insert(T geometry)
        {
            INode<T> node = (_nodeMap.ContainsKey(geometry))? _nodeMap[geometry]:new Node<T>(geometry);
            insert(node);
            return node;
        }

        private bool insert(INode<T> node)
        {
            lock (_nodeMap)
            {
                bool hasKey = _nodeMap.ContainsKey(node.Geometry);
                if (!hasKey) _nodeMap.Add(node.Geometry, node);
                return !hasKey;
            }

        }

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
            _edges.Add(edge);

        }
    }

}
