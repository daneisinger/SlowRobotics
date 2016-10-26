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
            if (a is PlaneAgent) run((PlaneAgent)a);
            if (a is LinkMesh) run((LinkMesh)a);
            reset(); //scale agents reset scale factor
        }

        public virtual void reset()
        {
            scaleFactor = 1; //reset scale factor
        }

        public void scale(float factor)
        {
            scaleFactor = factor;
        }

    }
}
