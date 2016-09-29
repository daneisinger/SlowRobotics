using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Behaviours.TrailBehaviours
{
    public class StiffenLinksBehaviour : AgentBehaviour
    {
        float bendResistance;

        public StiffenLinksBehaviour(int _priority, float _bendResistance) : base(_priority)
        {
            bendResistance = _bendResistance;
        }

        override
        public void run(Agent a)
        {
           foreach(LinkPair lp in a.getPairs()) a.addForce(lp.bisectPair(a).scale(bendResistance));
        }

    }
}
