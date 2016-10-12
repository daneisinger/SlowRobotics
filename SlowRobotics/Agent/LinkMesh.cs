using SlowRobotics.Core;
using SlowRobotics.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Agent
{
    public class LinkMesh : Node, IAgent
    {
        public IWorld world { get; set; }
        public PriorityQueue<IBehaviour> behaviours { get; set; }

        HashSet<Link> tertiaryLinks;

        public LinkMesh(Node n, IWorld _world):base(n){
            behaviours = new PriorityQueue<IBehaviour>();
            world = _world;
            tertiaryLinks = new HashSet<Link>();
        }

        public void connectTertiary(Link l)
        {
            tertiaryLinks.Add(l);
        }

        public bool disconnectTertiary(Link l)
        {
            return tertiaryLinks.Remove(l);
        }

        public List<Link> getTertiaryLinks()
        {
            return tertiaryLinks.ToList();
        }

        public void braceLinks(Link a, Link b, float stiffness)
        {
            Node shared;
            if(getSharedNode(a,b, out shared))
            {
                Link brace = new Link(a.tryGetOther(shared), b.tryGetOther(shared));
                brace.stiffness = stiffness;
                connectTertiary(brace);
            }
        }

        public void braceNthLinks(List<Link> toBrace, float stiffness)
        {
            for (int i = 0; i < toBrace.Count-1; i += 2)
            {
                braceLinks(toBrace[i], toBrace[i + 1], stiffness);
            }
        }

        public void connectNodes(Node a, Node b, float stiffness)
        {
            Link l = new Link(a, b);
            l.stiffness = stiffness;
            connect(l);
        }

        public void connectTertiaryNodes(Node a, Node b, float stiffness)
        {
            Link l = new Link(a, b);
            l.stiffness = stiffness;
            connectTertiary(l);
        }

        public void interconnectTertiaryNodes(List<Node> toConnect, float stiffness)
        {
            foreach(Node a in toConnect)
            {
                foreach(Node b in toConnect)
                {
                    if (a != b) connectTertiaryNodes(a, b, stiffness);
                }
            }
        }

        public static bool getSharedNode(Link _a, Link _b, out Node shared)
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

        public void addBehaviour(IBehaviour b)
        {
            behaviours.Enqueue(b);
        }

        public List<IBehaviour> getBehaviours()
        {
            return behaviours.getData();
        }

        public void setBehaviours(List<IBehaviour> newBehaviours)
        {
            behaviours = new PriorityQueue<IBehaviour>();
            foreach (IBehaviour b in newBehaviours) behaviours.Enqueue(b);
        }

        public override void step(float damping)
        {
            foreach (IBehaviour b in behaviours.getData()) b.run(this);
        }
    }
}
