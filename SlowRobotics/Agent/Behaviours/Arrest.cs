using SlowRobotics.Core;
using SlowRobotics.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Agent.Behaviours
{
    public class Arrest
    {
        public class Friction : ScaledAgentBehaviour
        {
            public float frictionCof { get; set; }
            public float inertiaMod { get; set; }
            public float maxDist { get; set; }

            public Friction(int _priority, float _maxDist, float _frictionCof) : base(_priority)
            {
                frictionCof = _frictionCof;
                maxDist = _maxDist;
                inertiaMod = 0;
            }

            public override void test(PlaneAgent a, Plane3D p)
            {
                Vec3D ab = p.sub(a);
                float d = ab.magnitude();
                if (d > 0 && d < maxDist)
                {
                    float f = SR_Math.map(d, 0, maxDist, 1, 0);
                    float sf = ExponentialInterpolation.Squared.interpolate(0, frictionCof, f);
                    inertiaMod += frictionCof * scaleFactor;
                }

            }

            public override void run(PlaneAgent a)
            {
                a.addInertia(inertiaMod);
                inertiaMod = 0;
            }
        }

        public class Freeze : ScaledAgentBehaviour
        {

            public float minInertia { get; set; }
            public float speedFactor { get; set; }
            public int minAge { get; set; }

            public Freeze(int _priority, float _minInertia, float _speedFactor, int _minAge) : base(_priority)
            {
                minInertia = _minInertia;
                speedFactor = _speedFactor;
                minAge = _minAge;
            }

            public override void run(PlaneAgent a)
            {
                if (a.getInertia() + (a.getSpeed() * speedFactor * scaleFactor) < minInertia && a.age > minAge)
                {
                    a.setInertia(0);
                }
            }
        }

        public class Z : AgentBehaviour
        {
            public float minZ
            {
                get; set;
            }

            public Z(int _priority, float _minZ) : base(_priority)
            {
                minZ = _minZ;
            }

            public override void run(PlaneAgent a)
            {
                if (a.z < minZ) a.setInertia(0);
            }
        }

    }
}
