using SlowRobotics.Core;
using SlowRobotics.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.Agent.Behaviours
{
    public class Interact : PlaneAgentBehaviour
    {
        private PriorityQueue<Behaviour> behaviours;

        public Interact(int _priority, List<Behaviour> _interactionBehaviours) : base(_priority)
        {
            behaviours = new PriorityQueue<Behaviour>();
            foreach (Behaviour b in _interactionBehaviours) behaviours.Enqueue(b);
        }

        public override void run(PlaneAgent a)
        {
            if (a.hasNeighbours())
            {
                // Testing step - loop through all neighbours
                // used for closest point search etc.
                foreach (Plane3D p in a.neighbours)
                {
                    foreach (Behaviour b in behaviours.getData()) b.test(a,p); 
                }
                //update the agent using test data
                foreach (Behaviour b in behaviours.getData()) b.run(a);
            }
        }

        public void setBehaviours(List<Behaviour> newBehaviours)
        {
            behaviours = new PriorityQueue<Behaviour>();
            foreach (Behaviour b in newBehaviours) behaviours.Enqueue(b);
        }

    }
}
