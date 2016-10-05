using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Behaviours.TrailBehaviours
{
    public class SplitLinkBehaviour : AgentBehaviour
    {

        List<Behaviour> behaviours;
        float stiffness;

        public SplitLinkBehaviour(int _priority, float _stiffness, List<Behaviour> _behaviours) : base(_priority)
        {
            stiffness = _stiffness;
            behaviours = _behaviours;
        }

        override
        public void run(Agent a)
        {

        }


    }
}
