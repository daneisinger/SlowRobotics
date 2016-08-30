using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Behaviours.TrailBehaviours
{
    public class StiffenTrailBehaviour : TrailBehaviour
    {
        float bendResistance;

        public StiffenTrailBehaviour(int _priority, float _bendResistance) : base(_priority)
        {
            bendResistance = _bendResistance;
        }

        override
        public void run(Agent a, int i)
        {
            if (i>0)
            {

                    Link prev = a.trail[i - 1];
                    Link next = a.trail[i];
                    if (!prev.isEnd())
                    {
                        float currentAngle = prev.angleBetween(next, true);
                        Vec3D ab = prev.a.add(next.b).scale((float)0.5);
                        float targetAngle = prev.linkAngle;
                        float diff = (targetAngle) - currentAngle / (float)Math.PI; //max is 1, min is -1
                        Vec3D avg = ab.sub(prev.b).scale(bendResistance * diff);
                        prev.b.addForce(avg);
                    }
            }
        }

    }
}
