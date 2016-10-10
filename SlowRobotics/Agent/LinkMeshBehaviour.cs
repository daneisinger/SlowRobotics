using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlowRobotics.Core;

namespace SlowRobotics.Agent
{
    public class LinkMeshBehaviour : Behaviour
    {
        public int priority;
        /// <summary>
        /// Empty constructor with default priority of 1
        /// </summary>
        public LinkMeshBehaviour() : this(1)
        {

        }
        /// <summary>
        /// Create new behaviour with a given priority
        /// </summary>
        /// <param name="_priority">Behaviour priority, higher runs first</param>
        public LinkMeshBehaviour(int _priority)
        {
            priority = _priority;
        }

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
        /// Cast to plane agent
        /// </summary>
        /// <param name="a"></param>
        public virtual void run(Agent a)
        {
            if (a is LinkMesh) run((LinkMesh)a);
        }

        public virtual void test(Agent a, Plane3D p)
        {
            if (a is LinkMesh) test((LinkMesh)a, p);
        }

        /// <summary>
        /// Function for behaviour to run. Override this function in new behaviours
        /// </summary>
        /// <param name="a">Current agent</param>
        public virtual void run(LinkMesh a) { }

        public virtual void test(LinkMesh a, Plane3D p) { }
    }
}
