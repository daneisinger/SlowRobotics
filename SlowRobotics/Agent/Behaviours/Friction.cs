using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlowRobotics.Core;
using Toxiclibs.core;

namespace SlowRobotics.Agent.Behaviours
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
            inertiaMod += (frictionCof-normalizeDistance(p.sub(a), 0, maxDist, frictionCof, ExponentialInterpolation.Squared)) * scaleFactor;
        }

        public override void run(PlaneAgent a)
        {
            a.addInertia(inertiaMod);
            inertiaMod = 0;
        }
    }
}
