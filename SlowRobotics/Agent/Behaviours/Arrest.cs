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
        public class Friction : ScaledBehaviour<Particle>
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

            public override void interactWith(Particle a, object b)
            {
                Vec3D b_v = b as Vec3D;
                if (b_v != null)
                {
                    Vec3D ab = b_v.sub(a);
                    float d = ab.magnitude();
                    if (d > 0 && d < maxDist)
                    {
                        float f = SR_Math.map(d, 0, maxDist, 1, 0);
                        float sf = ExponentialInterpolation.Squared.interpolate(0, frictionCof, f);
                        inertiaMod += frictionCof * scaleFactor;
                    }
                }

            }

            public override void runOn(Particle a)
            {

                    a.addInertia(inertiaMod);
                    inertiaMod = 0;
            }
        }

        public class Freeze : ScaledBehaviour<Particle>
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

            public override void runOn(Particle p)
            {
                    if (p.getInertia() + (p.getSpeed() * speedFactor * scaleFactor) < minInertia && p.age > minAge)
                    {
                        p.setInertia(0);
                    }
            }
        }

        public class Z : Behaviour<Particle>
        {
            public float minZ
            {
                get; set;
            }

            public Z(int _priority, float _minZ) : base(_priority)
            {
                minZ = _minZ;
            }

            public override void runOn(Particle a)
            {
                if (a.z < minZ) a.setInertia(0);
            }
        }

    }
}
