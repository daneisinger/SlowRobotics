using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Behaviours.TrailBehaviours
{
    public class DuplicateNodeBehaviour : AgentBehaviour
    {

        List<Behaviour> behaviours;
        float stiffness;

        public DuplicateNodeBehaviour(int _priority, float _stiffness, List<Behaviour> _behaviours) : base(_priority)
        {
            stiffness = _stiffness;
            behaviours = _behaviours;
        }

        override
        public void run(Agent a)
        {
           Agent b = new Agent(a, a.locked(), a.world);
            b.parent = a.parent;

           duplicateNode(a, b, stiffness, behaviours);
        }

        public static void duplicateNode(Agent a, Agent b, float _stiffness, List<Behaviour> newBehaviours)
        {
            Link connection = new Link(a, b);
            connection.stiffness = _stiffness;
            a.connect(connection);
            b.connect(connection);
            foreach(Behaviour nb in newBehaviours)
            {
                b.addBehaviour(nb); //add new behaviours
            }
            a.world.addDynamic(b);//add to world
        }
    }
}
