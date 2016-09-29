using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Core
{

    public class Node : Plane3D
    {

        HashSet<Link> links;
        HashSet<LinkPair> pairs;

        public Node parent { get; set; }

        public Node(float _x, float _y, float _z) : this(new Vec3D(_x,_y,_z)){ }
        public Node(Vec3D _o) : this(new Plane3D(_o)) { }

        public Node(Plane3D plane) : base(plane)
        {
            links = new HashSet<Link>();
            pairs = new HashSet<LinkPair>();
        }

        public Node(Node n) : base(n)
        {
            links = n.links;
            pairs = n.pairs;
        }

        virtual public void step(float damping) { }

        public void connect(Link l)
        {
            links.Add(l);
        }

        public bool disconnect(Link b)
        {
            return (tryRemoveMatchingPairs(b) && links.Remove(b));
        }

        public void pairLinks(LinkPair pair)
        {
            pairs.Add(pair);

        }
        public bool tryRemoveMatchingPairs(Link shared)
        {
            List<LinkPair> toRemove = new List<LinkPair>();
            pairs.ToList().ForEach(lp => {
                if (lp.hasLink(shared)) toRemove.Add(lp);
            });

            foreach(LinkPair l in toRemove)
            {
                pairs.Remove(l);
            }

            if (toRemove.Count > 0) return true;
            return false;
        }

        public void pairLinksByListOrder()
        {
            if (links.Count > 1)
            {
                for (int i = 0; i < links.Count; i++)
                {
                    int j = (i==0)?links.Count-1:i-1;
                    pairLinks(new LinkPair(getLinks()[i], getLinks()[j]));
                }
            }
        }

        public void pairLinksBySortedAngles()
        {
            if (links.Count > 1)
            {
                List<Link> remaining = new List<Link>();
                remaining.AddRange(getLinks());
                Link first = remaining[0];
                remaining.Remove(first);
                Link next = null;

                while (remaining.Count>0)
                {
                    float angle = 1000;
                    foreach (Link j in remaining)
                    {
                            float thisAngle = j.angleBetween(first, true);
                            if (thisAngle < angle)
                            {
                                next = j;
                                angle = thisAngle;
                            }
                    }
                    if (next != null)
                    {
                        pairLinks(new LinkPair(first, next));
                        remaining.Remove(next);
                        first = next;
                    }
                }

                //create last pair
               pairLinks(new LinkPair(first, getLinks()[0]));
            }
        }

        public void isolate()
        {
            links = new HashSet<Link>();
        }

        public List<Link> getLinks()
        {
            return links.ToList();
        }

        public List<LinkPair> getPairs()
        {
            return pairs.ToList();
        }

        public List<Node> getConnectedNodes()
        {
            List<Node> output = new List<Node>();
            links.ToList().ForEach(l => {
                output.Add(l.tryGetOther(this));
            });
            return output;
        }

        public Dictionary<int,List<Node>> marchNodes(Node last, List<Node> current, ref int index)
        {
            Dictionary<int,List<Node>> nodes = new Dictionary<int,List<Node>>(); //create dictionary

            foreach (Node n in current)
            {
                nodes.Add(index, new List<Node>());//create initial list
                List<Node> childNodes = n.getConnectedNodes();
                childNodes.Remove(last);
                nodes[index].AddRange(childNodes);
                marchNodes(n, childNodes, ref index).ToList().ForEach(x => {
                    if (nodes.ContainsKey(x.Key))
                    {
                        nodes[x.Key].AddRange(x.Value);
                    }
                    else
                    {
                        nodes.Add(x.Key, x.Value);
                    }
                     
                 });
                index++;
            }

            return nodes;
        }
    }
}
