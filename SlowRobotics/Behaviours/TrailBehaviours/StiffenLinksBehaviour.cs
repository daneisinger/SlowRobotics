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
           Link prev = null;
           foreach(Link next in a.getLinks())
            {
                if(prev != null)
                {
                    Node nextOther = next.tryGetOther(a);
                    Node prevOther = prev.tryGetOther(a);

                    float currentAngle = prev.angleBetween(next, false);
                    Vec3D ab = prevOther.add(nextOther).scale((float)0.5);
                    float targetAngle = prev.linkAngle;
                    float diff = (targetAngle) - currentAngle / (float)Math.PI; //max is 1, min is -1
                    Vec3D avg = ab.sub(a).scale(bendResistance * diff);
                    a.addForce(avg);
                }
                prev = next;
            }
        }

    }
}
