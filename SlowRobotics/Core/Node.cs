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
        public Node parent { get; set; }

        public Node(float _x, float _y, float _z) : this(new Vec3D(_x,_y,_z)){ }
        public Node(Vec3D _o) : this(new Plane3D(_o)) { }

        public Node(Plane3D plane) : base(plane)
        {
            links = new HashSet<Link>();
        }

        public Node(Node n) : base(n)
        {
            links = n.links;
        }


        virtual public void update() { }

        public void connect(Link l)
        {
            links.Add(l);
        }

        public bool disconnect(Link b)
        {
            return links.Remove(b);
        }

        public void isolate()
        {
            links = new HashSet<Link>();
        }

        public List<Link> getLinks()
        {
            return links.ToList();
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
