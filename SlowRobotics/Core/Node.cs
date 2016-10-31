using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Core
{

    public class Node : Plane3D, IState
    {

        public HashSet<Link> links;
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
            parent = n.parent;
        }

        virtual public void step(float damping) {}

        public void connect(Link l)
        {
            links.Add(l);
        }

        public bool disconnect(Link b)
        {
            return links.Remove(b);

        }

        public Link last()
        {
            return links.LastOrDefault();
        }

        public void replaceLink(Link toReplace, Link newLink)
        {

            links.Remove(toReplace);
            links.Add(newLink);
        }

        
        public void isolate()
        {
            links = new HashSet<Link>();
        }

        public List<Link> getLinks()
        {
            return links.ToList();
        }

        public bool hasLinks()
        {
            return getLinks().Count > 0;
        }

        public List<Node> getConnectedNodes()
        {
            List<Node> output = new List<Node>();
            links.ToList().ForEach(l => {
                output.Add(l.tryGetOther(this));
            });
            return output;
        }
        public Node getNode()
        {
            return this;
        }
        public Vec3D getPos()
        {
            return this;
        }
        public IState copyState()
        {
            //TODO copy links
            return new Node(this);
        }

    }
}
