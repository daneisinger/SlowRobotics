using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Agent.Behaviours
{
    public class AddLink : PlaneAgentBehaviour
    {

        public Vec3D offset { get; set; }
        public List<Behaviour> behaviours { get; set; }
        public LinkMesh parent { get; set; }

        public int frequency { get; set; }
        public int ctr;
        public float stiffness { get; set; }
        public float braceStiffness { get; set; }
        public bool dynamic { get; set; }
        public bool tryToBrace { get; set; }

        public AddLink(int _priority, LinkMesh _parent, bool _tryToBrace, int _frequency, Vec3D _offset, float _stiffness, float _braceStiffness, List<Behaviour> _behaviours, bool _dynamic) : base(_priority)
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
                        nL.stiffness = braceStiffness;
                        parent.connectTertiary(nL);
                    }
                }

                parent.connectNodes(a, b, stiffness);

                if (dynamic)
                {
                    foreach (Behaviour nb in behaviours) b.addBehaviour(nb);
                    a.world.addDynamic(b);//add to world
                }
                else
                {
                    a.world.addStatic(b);
                }
            }
            ctr++;

        }

    }
}
