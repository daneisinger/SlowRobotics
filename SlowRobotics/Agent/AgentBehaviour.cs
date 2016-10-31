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
        
        
        public virtual void run(IAgent a)
        {
            IStateAgent a_s = a as IStateAgent;
            if (a_s != null) run(a_s);

            IDiscreteAgent a_d = a as IDiscreteAgent;
            if (a_d != null) run(a_d);

            IGraphAgent a_g = a as IGraphAgent;
            if (a_g != null) run(a_g);

            IParticleAgent a_p = a as IParticleAgent;
            if (a_p != null) run(a_p);
        }

        public virtual void run(IStateAgent a) { }
        public virtual void run(IGraphAgent a) { }
        public virtual void run(IParticleAgent a) { }
        public virtual void run(IDiscreteAgent a) { }

        public virtual void interact(IAgent a, IAgent b)
        {
            IStateAgent a_s = a as IStateAgent;
            if (a_s != null) interact(a_s,b);

            IDiscreteAgent a_d = a as IDiscreteAgent;
            if (a_d != null) interact(a_d,b);

            IGraphAgent a_g = a as IGraphAgent;
            if (a_g != null) interact(a_g,b);

            IParticleAgent a_p = a as IParticleAgent;
            if (a_p != null) interact(a_p,b);
        }
        public virtual void interact(IStateAgent a, IAgent b) { }
        public virtual void interact(IGraphAgent a, IAgent b) { }
        public virtual void interact(IParticleAgent a, IAgent b) { }
        public virtual void interact(IDiscreteAgent a, IAgent b) { }
    }
}
