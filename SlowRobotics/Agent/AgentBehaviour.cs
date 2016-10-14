using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlowRobotics.Core;
using Toxiclibs.core;
using SlowRobotics.Utils;

namespace SlowRobotics.Agent
{
    public class AgentBehaviour : IBehaviour
    {

        public int priority;
        /// <summary>
        /// Empty constructor with default priority of 1
        /// </summary>
        public AgentBehaviour() : this(1)
        {

        }
        /// <summary>
        /// Create new behaviour with a given priority
        /// </summary>
        /// <param name="_priority">Behaviour priority, higher runs first</param>
        public AgentBehaviour(int _priority)
        {
            priority = _priority;
        }

        public int CompareTo(IBehaviour other)
        {
            if (other.getPriority() > priority) return -1;
            if (other.getPriority() < priority) return 1;
            return 0;
        }

        public int getPriority()
        {
            return priority;
        }
        

        //TODO - rethink this to be more generic

        /// <summary>
        /// Cast to appropriate behaviour methods
        /// </summary>
        /// <param name="a"></param>
        public virtual void run(IAgent a)
        {
            if (a is PlaneAgent) run((PlaneAgent)a);
            if (a is LinkMesh) run((LinkMesh)a);
        }

        public virtual void test(IAgent a, Plane3D p)
        {
            if (a is PlaneAgent) test((PlaneAgent)a, p);
            if (a is LinkMesh) test((LinkMesh)a, p);
        }

        /// <summary>
        /// Function for behaviour to run. Override this function in new behaviours
        /// </summary>
        /// <param name="a">Current agent</param>
        public virtual void run(PlaneAgent a) {}

        public virtual void test(PlaneAgent a, Plane3D p) { }

        public virtual void run(LinkMesh a) { }

        public virtual void test(LinkMesh a, Plane3D p) { }

    }
}
