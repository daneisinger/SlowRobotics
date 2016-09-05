using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Behaviours.TrailBehaviours
{
    public class SpringBehaviour : AgentBehaviour
    {
        float damping;
        bool verlet;

        public SpringBehaviour(int _priority, float _damping, bool _verlet) : base(_priority)
        {
            damping = _damping;
            verlet = _verlet;
        }

        override
        public void run(Agent a)
        {
            foreach (Link l in a.getLinks())
            {
                Node notA = l.tryGetOther(a);

                float d = (l.l - (l.getLength()));
                Vec3D ab = notA.sub(a).getNormalized();
                a.addForce(ab.scale(-d * l.stiffness*damping));
                if (notA is Particle && verlet)
                {
                    Particle p = (Particle)notA;
                    p.addForce(ab.scale(d * l.stiffness * damping));
                }
            }
        }
    }
}
