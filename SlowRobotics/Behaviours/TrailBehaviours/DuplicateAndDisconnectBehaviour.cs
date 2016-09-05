using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Behaviours.TrailBehaviours
{
    public class DuplicateAndDisconnectBehaviour : AgentBehaviour
    {

        Vec3D offset;
        List<Behaviour> behaviours;
        int frequency;
        int ctr;
        float stiffness;

        public DuplicateAndDisconnectBehaviour(int _priority, int _frequency, Vec3D _offset, float _stiffness, List<Behaviour> _behaviours) : base(_priority)
        {
            offset = _offset;
            frequency = _frequency;
            ctr = 0;
            behaviours = _behaviours;
            stiffness = _stiffness;
        }

        override
        public void run(Agent a)
        {
            if (ctr % frequency == 0)
            {
                Agent b = new Agent(a, a.locked(), a.world);
                b.addSelf(offset);
                b.parent = a.parent;

                //replace current connections
                foreach (Link l in a.getLinks()) l.replaceNode(a, b);
                a.isolate();

                Link connection = new Link(a, b);
                connection.stiffness = stiffness;
                a.connect(connection);
                b.connect(connection);

                foreach (Behaviour nb in behaviours) b.addBehaviour(nb); 

                a.world.addDynamic(b);//add to world
            }
            ctr++;

        }

    }
}
