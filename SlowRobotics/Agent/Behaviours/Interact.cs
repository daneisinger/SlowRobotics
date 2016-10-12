using SlowRobotics.Core;
using SlowRobotics.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.Agent.Behaviours
{
    public class Interact : AgentBehaviour
    {
        private PriorityQueue<IBehaviour> behaviours;

        public Interact(int _priority, List<IBehaviour> _interactionBehaviours) : base(_priority)
        {
            behaviours = new PriorityQueue<IBehaviour>();
            foreach (IBehaviour b in _interactionBehaviours) behaviours.Enqueue(b);
        }

        public override void run(PlaneAgent a)
        {
            if (a.hasNeighbours())
            {
                // Testing step - loop through all neighbours
                // used for closest point search etc.
                foreach (Plane3D p in a.neighbours)
                {
                    foreach (IBehaviour b in behaviours.getData()) b.test(a,p); 
                }
                //update the agent using test data
                foreach (IBehaviour b in behaviours.getData()) b.run(a);
            }
        }

        public void setBehaviours(List<IBehaviour> newBehaviours)
        {
            behaviours = new PriorityQueue<IBehaviour>();
            foreach (IBehaviour b in newBehaviours) behaviours.Enqueue(b);
        }

    }
}
