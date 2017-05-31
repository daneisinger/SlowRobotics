using SlowRobotics.Core;
using SlowRobotics.Utils;
using System;
using SlowRobotics.SRMath;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Agent.Behaviours
{
    public class Arrest
    {
        /// <summary>
        /// Adds inertia to a particle after it reaches a certain age
        /// </summary>
        public class Slow : ScaledBehaviour<SRParticle>
        {
            public int minAge { get; set; }
            //TODO implement falloff
            public Slow(int _p, int _minAge) : base(_p)
            {
                minAge = _minAge;
            }

            public override void runOn(SRParticle p)
            {
                float particleAge = p.age;
                if (particleAge > minAge)
                {
                    p.setInertia((float)minAge / p.age);
                }
            }
        }

        /// <summary>
        /// Adds inertia to a particle 
        /// </summary>
        public class Friction : ScaledBehaviour<SRParticle>
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

            public override void interactWith(SRParticle a, object b)
            {
                Vec3D b_v = b as Vec3D;
                if (b_v != null)
                {
                    Vec3D ab = b_v.sub(a);
                    float d = ab.magnitude();
                    if (d > 0 && d < maxDist)
                    {
                        float f = MathUtils.map(d, 0, maxDist, 1, 0);
                        float sf = interpolator.interpolate(0, frictionCof, f);
                        inertiaMod += frictionCof * scaleFactor;
                    }
                }

            }

            public override void runOn(SRParticle a)
            {
                    a.addInertia(inertiaMod);
                    inertiaMod = 0;
            }
        }

        /// <summary>
        /// Locks particles that satisfy conditions for inertia, speed and age
        /// </summary>
        public class Freeze : ScaledBehaviour<SRParticle>
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

            public override void runOn(SRParticle p)
            {
                    if (p.getInertia() + (p.getSpeed() * speedFactor * scaleFactor) < minInertia && p.age > minAge)
                    {
                        p.setInertia(0);
                    }
            }
        }

        /// <summary>
        /// Constrains particle z to >= minZ
        /// </summary>
        public class Z : Behaviour<SRParticle>
        {
            public float minZ
            {
                get; set;
            }

            public Z(int _priority, float _minZ) : base(_priority)
            {
                minZ = _minZ;
            }

            public override void runOn(SRParticle a)
            {
                if (a.z < minZ) a.z = minZ;
            }
        }

    }
}
