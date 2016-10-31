using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.Agent
{
    public class ScaledAgentBehaviour : AgentBehaviour, IScaledBehaviour
    {
        public float scaleFactor { get; set; }

        public ScaledAgentBehaviour() : this(1){}

        public ScaledAgentBehaviour(int _priority)
        {
            priority = _priority;
            scaleFactor = 1;
        }

        public override void run(IAgent a)
        {
            IStateAgent a_s = a as IStateAgent;
            if (a_s != null) run(a_s);

            IDiscreteAgent a_d = a as IDiscreteAgent;
            if (a_d != null) run(a_d);

            IGraphAgent a_g = a as IGraphAgent;
            if (a_g != null) run(a_g);

            IParticleAgent a_p = a as IParticleAgent;
            if (a_p != null) run(a_p);

            reset();
        }

        public virtual void reset()
        {
            scaleFactor = 1;
        }

        public void scale(float factor)
        {
            scaleFactor = factor;
        }

    }
}
