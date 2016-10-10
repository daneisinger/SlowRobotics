using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.Agent.Behaviours
{
    public class Duplicate : PlaneAgentBehaviour
    {

        public List<Behaviour> behaviours { get; set; }
        public float stiffness { get; set; }
        public LinkMesh parent { get; set; }

        public Duplicate(int _priority, LinkMesh _parent, float _stiffness, List<Behaviour> _behaviours) : base(_priority)
        {
            stiffness = _stiffness;
            behaviours = _behaviours;
            parent = _parent;
        }

        public override void run(PlaneAgent a)
        {
            PlaneAgent b = new PlaneAgent(a, a.world);
            b.parent = a.parent;
            duplicateNode(parent, false, a, b, stiffness, behaviours);
        }

        public static void duplicateNode(LinkMesh parent, bool tertiary, PlaneAgent a, PlaneAgent b, float _stiffness, List<Behaviour> newBehaviours)
        {

            if (tertiary)
            {
                parent.connectTertiaryNodes(a, b, _stiffness);
                
            }
            else
            {
                parent.connectNodes(a, b, _stiffness);
            }
            
            foreach (Behaviour nb in newBehaviours)
            {
                b.addBehaviour(nb); //add new behaviours
            }
            a.world.addDynamic(b);//add to world
        }
    }
}
