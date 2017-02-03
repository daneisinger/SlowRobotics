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

        public bool getNodeAt(T geometry, out INode<T> node)
        {
            return (_nodeMap.TryGetValue(geometry, out node));
        }

        public bool getNodeAt(int index, out INode<T> node)
        {
            if (index > _nodeMap.ToList().Count)
            {
                node = null;
                return false;
            }
            node = _nodeMap.Values.ToList()[index];
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

        private bool removeNode(INode<T> node)
        {
            foreach (E edge in node.Edges.ToList())
            {
                removeEdge(edge);
            }
            return _nodeMap.Remove(node.Geometry);
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
            bool hasKey = _nodeMap.ContainsKey(node.Geometry);
            if(!hasKey) _nodeMap.Add(node.Geometry, node);
            return !hasKey;
        }

        public void insert(E edge)
        {
            
            T a = edge.a.Geometry;
            T b = edge.b.Geometry;

            //see if this key already exists and swap pointer if necessary
            if (!insert(edge.a)) edge.a = _nodeMap[a];
            if (!insert(edge.b)) edge.b = _nodeMap[b];

            edge.a.add(edge);
            edge.b.add(edge);

            _edges.Add(edge);

        }
    }

    /*

    //OLD GRAPH FUNCTIONS

    
        public void swapConnections(LegacyNode oldNode, LegacyNode newNode, bool tryToBrace, float linkStiffness, float braceStiffness)
        {

            //Get node connected to parent
            if (oldNode.hasLinks())
            {
                LegacyLink l = oldNode.getLinks()[0]; //only works if agent has just a single link
                LegacyNode other = l.tryGetOther(oldNode);

                connectNodes(newNode, other, linkStiffness);
                disconnect(l);
            }

            connectNodes(oldNode, newNode, linkStiffness);


        }

        public void connectTertiary(LegacyLink l)
        {
            tertiaryLinks.Add(l);
        }

        public bool disconnectTertiary(LegacyLink l)
        {
            return tertiaryLinks.Remove(l);
        }

        public List<LegacyLink> getTertiaryLinks()
        {
            return tertiaryLinks.ToList();
        }

        public void braceLinks(LegacyLink a, LegacyLink b, float stiffness)
        {
            LegacyNode shared;
            if (getSharedNode(a, b, out shared))
            {
                LegacyLink brace = new LegacyLink(a.tryGetOther(shared), b.tryGetOther(shared));
                brace.stiffness = stiffness;
                connectTertiary(brace);
            }
        }

        public void braceNthLinks(List<LegacyLink> toBrace, float stiffness)
        {
            for (int i = 0; i < toBrace.Count - 1; i += 2)
            {
                braceLinks(toBrace[i], toBrace[i + 1], stiffness);
            }
        }

        public void connectNodes(LegacyNode a, LegacyNode b, float stiffness)
        {
            LegacyLink l = new LegacyLink(a, b);
            l.stiffness = stiffness;
            connect(l);
        }

        public void connectTertiaryNodes(LegacyNode a, LegacyNode b, float stiffness)
        {
            LegacyLink l = new LegacyLink(a, b);
            l.stiffness = stiffness;
            connectTertiary(l);
        }

        public void interconnectTertiaryNodes(List<LegacyNode> toConnect, float stiffness)
        {
            foreach (LegacyNode a in toConnect)
            {
                foreach (LegacyNode b in toConnect)
                {
                    if (a != b) connectTertiaryNodes(a, b, stiffness);
                }
            }
        }

        public void connectByProximity(List<LegacyNode> toConnect, float minDist, float maxDist, float stiffness)
        {
            Plane3DOctree dynamicTree = new Plane3DOctree(new Vec3D(-200, -200, -200), 200 * 2);
            foreach (LegacyNode n in toConnect) dynamicTree.addPoint(n);

            foreach (LegacyNode n in toConnect)
            {
                List<Vec3D> inProx = dynamicTree.getPointsWithinSphere(n, maxDist);
                foreach (Vec3D v in inProx)
                {
                    if (v.distanceTo(n) > minDist)
                    {
                        if (v is LegacyNode)
                        {
                            if (n != (LegacyNode)v) connectTertiaryNodes(n, (LegacyNode)v, stiffness);
                        }
                    }
                }
            }
        }

        public static float angleBetweenSharedNode(LegacyLink _a, LegacyLink _b)
        {
            LegacyNode shared;
            if (getSharedNode(_a, _b, out shared))
            {
                Vec3D ab = _a.tryGetOther(shared).sub(shared);
                Vec3D abo = _b.tryGetOther(shared).sub(shared);
                return ab.angleBetween(abo, true);
            }
            return 0;
        }

        public bool hasLink(LegacyLink l)
        {
            return (links.Contains(l) || tertiaryLinks.Contains(l));
        }

        public static bool getSharedNode(LegacyLink _a, LegacyLink _b, out LegacyNode shared)
        {
            if (_a.a == _b.a || _a.a == _b.b)
            {
                shared = _a.a;
                return true;
            }
            if (_a.b == _b.a || _a.b == _b.b)
            {
                shared = _a.b;
                return true;
            }
            shared = null;
            return false;
        }

        public float getDeltaForStep()
        {
            return 1;
            
            return getNodes().Sum(x => {
                return (x is Particle) ? ((Particle)x).getDelta() : 0;
            });
        }

        public List<LegacyNode> getNodes()
        {
            return nodes.ToList();
        }

        public LegacyNode getLastNode()
        {
            List<LegacyNode> nodes = getNodes();
            return nodes[nodes.Count - 1];
        }

        public LegacyLink getLastLink()
        {
            return getLinks()[getLinks().Count - 1];
        }

        public List<LegacyLink> getLinks()
        {
            return links.ToList();
        }

        public bool hasLinks()
        {
            return links.Count > 0;
        }

        // This is hugely problematic - creates all sorts of duplication

        public void connect(LegacyLink l)
        {
            l.a.connect(l);
            l.b.connect(l);
            links.Add(l);
            nodes.Add(l.a);
            nodes.Add(l.b);
        }

        public bool disconnect(LegacyLink l)
        {
            l.a.disconnect(l);
            l.b.disconnect(l);
            nodes.Remove(l.a);
            nodes.Remove(l.b);
            return links.Remove(l);

        }
    }

    */
}
