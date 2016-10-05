using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Behaviours.TrailBehaviours
{
    public class AlignAgentXXWithLinks : AgentBehaviour
    {

        float strength;

        public AlignAgentXXWithLinks(int _priority, float _strength) : base(_priority)
        {
            strength = _strength;
        }

        override
        public void run(Agent a)
        {
            foreach(Link l in a.getLinks())
            {
                a.interpolateToXX(l.getDir(), strength);

                //try adjusting
                float delta = a.xx.angleBetween(l.getDir(), true);
                a.addForce(a.yy.scale(-delta * strength));
            }
        }

    }
}
