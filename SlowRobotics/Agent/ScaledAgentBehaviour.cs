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

        public void scale(float factor)
        {
            scaleFactor = factor;
        }

    }
}
