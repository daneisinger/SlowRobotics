using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Core
{
    /// <summary>
    /// Extension of vec3d to handle topology and physics updates
    /// </summary>
    public class Node : Vec3D
    {
        //get rid of this sorted list - need one shared link object reference for each connection!

        //  SortedList<Node,Link> links;
        HashSet<Link> links;
        public Node parent { get; set; }

        public Node(float _x, float _y, float _z) : this(new Vec3D(_x,_y,_z)){ }

        public Node(Node n) : this(new Vec3D(n))
        {
            links = n.links;
        }

        public Node(Vec3D _o) :base (_o)
        {
            //   links = new SortedList<Node,Link>();
            links = new HashSet<Link>();
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
