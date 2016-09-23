using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlowRobotics.Core;
using Toxiclibs.core;

namespace SlowRobotics.Behaviours.NeighbourBehaviours
{
    public class NeighbourBehaviour : AgentBehaviour
    {
       
        //protected ExponentialInterpolation interp = new ExponentialInterpolation(2);
       // public int priority;
        /// <summary>
        /// Empty constructor with default priority of 1
        /// </summary>
        public NeighbourBehaviour() : this(1)
        {

        }
        /// <summary>
        /// Create new behaviour with a given priority
        /// </summary>
        /// <param name="_priority">Behaviour priority, higher runs first</param>
        public NeighbourBehaviour(int _priority)
        {
            priority = _priority;
        }
        /*
        public int CompareTo(Behaviour other)
        {
            if (other.getPriority() > priority) return -1;
            if (other.getPriority() < priority) return 1;
            return 0;
        }

        public int getPriority()
        {
            return priority;
        }
        /// <summary>
        /// Function for behaviour to run. Override this function in new behaviours
        /// </summary>
        /// <param name="a">Current agent</param>
        public virtual void run(Agent a)
        {

        }
        public virtual void run(Agent a, Agent b)
        {

        }*/
    }
}
