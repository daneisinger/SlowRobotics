using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Agent.Behaviours
{
    public class Add
    {

        public class StateToWorld : AgentBehaviour
        {
            public int frequency { get; set;}

            int c;

            public StateToWorld(int _priority, int _frequency) :base(_priority)
            {

                frequency = _frequency;
                c = 0;
            }

            public override void run(PlaneAgent a)
            {
                if (c % frequency == 0)
                {
                    Node n = new Node((Plane3D)a);
                    n.parent = a;
                    n.age = a.age;
                    a.world.addStatic(n);

                }
                c++;
            }
        }

        


        public class BracedLinks : ScaledAgentBehaviour
        {

            public Vec3D offset { get; set; }
            public List<IBehaviour> behaviours { get; set; }
            public LinkMesh parent { get; set; }

            public int frequency { get; set; }
            public int ctr;
            public float stiffness { get; set; }
            public float braceStiffness { get; set; }
            public bool dynamic { get; set; }
            public bool tryToBrace { get; set; }

            public BracedLinks(int _priority, LinkMesh _parent, bool _tryToBrace, int _frequency, Vec3D _offset, float _stiffness, float _braceStiffness, List<IBehaviour> _behaviours, bool _dynamic) : base(_priority)
            {
                offset = _offset;
                frequency = _frequency;
                ctr = 0;
                behaviours = _behaviours;
                stiffness = _stiffness;
                braceStiffness = _braceStiffness;
                dynamic = _dynamic;
                tryToBrace = _tryToBrace;
                parent = _parent;
            }

            public override void run(PlaneAgent a)
            {

                if (ctr % frequency == 0)
                {
                    PlaneAgent b = new PlaneAgent(a, a.world);
                    b.addSelf(offset);
                    b.parent = a.parent; //need to rethink parenting

                    //shonky bracing - need stiffness control and more explicit search for appropriate nodes
                    int ctr = parent.getLinks().Count;

                    if (tryToBrace && ctr > 1)
                    {
                        Link last = parent.getLinks()[ctr - 1];
                        Link secondLast = parent.getLinks()[ctr - 2];
                        Node shared;
                        if (LinkMesh.getSharedNode(last, secondLast, out shared))
                        {
                            float linkLengths = last.l + secondLast.l;
                            Link nL = new Link(b, secondLast.tryGetOther(shared), linkLengths);
                            nL.stiffness = braceStiffness * scaleFactor;
                            parent.connectTertiary(nL);
                        }
                    }

                    parent.connectNodes(a, b, stiffness * scaleFactor);

                    if (dynamic)
                    {
                        foreach (IBehaviour nb in behaviours) b.addBehaviour(nb);
                        a.world.addDynamic(b);//add to world
                    }
                    else
                    {
                        a.world.addStatic(b);
                    }
                }
                ctr++;

            }

            public static void createLink(LinkMesh parent, bool tertiary, PlaneAgent a, PlaneAgent b, float _stiffness, List<IBehaviour> newBehaviours)
            {

                if (tertiary)
                {
                    parent.connectTertiaryNodes(a, b, _stiffness);

                }
                else
                {
                    parent.connectNodes(a, b, _stiffness);
                }

                foreach (IBehaviour nb in newBehaviours)
                {
                    b.addBehaviour(nb); //add new behaviours
                }
                a.world.addDynamic(b);//add to world
            }
        }
    }
}
