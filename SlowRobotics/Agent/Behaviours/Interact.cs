using SlowRobotics.Core;
using SlowRobotics.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Agent.Behaviours
{
    public class Interact : Behaviour
    {
        private PriorityQueue<IBehaviour> behaviours;

        public Interact(int _priority, List<IBehaviour> _interactionBehaviours) : base(_priority)
        {
            behaviours = new PriorityQueue<IBehaviour>();
            foreach (IBehaviour b in _interactionBehaviours) behaviours.Enqueue(b);
        }

        public override void interact(IAgentT<object> a, object b)
        {
            throw new NotImplementedException();
        }

        public override void run(IAgentT<object> a)
        {

            if (a.hasNeighbours())
            {
                // Testing step - loop through all neighbours
                // used for closest point search etc.

                //TODO - should loop through agents
                foreach (Vec3D p in a.neighbours)
                {
                    //TODO - neighbour search doesnt return IAgent
                    foreach (IBehaviour b in behaviours.getData()) b.interact(a, p);
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
