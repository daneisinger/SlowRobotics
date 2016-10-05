using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.Behaviours
{
    class AlignAgentXXWithVelocity : AgentBehaviour
    {

            float strength;

            public AlignAgentXXWithVelocity(int _priority, float _strength) : base(_priority)
            {
                strength = _strength;
            }

            override
            public void run(Agent a)
            {
               a.interpolateToXX(a.getVel(), strength);
            }

    }
}
