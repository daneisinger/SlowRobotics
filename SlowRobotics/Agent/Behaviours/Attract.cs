using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Agent.Behaviours
{
    public class Attract : PlaneAgentBehaviour
    {
        public bool inXY { get; set; }
        public float strength { get; set; }
        public float minDist { get; set; }
        public float maxDist { get; set; }

        public Attract(int _priority, float _minDist, float _maxDist, bool _inXY, float _strength) : base(_priority)
        {
            inXY = _inXY;
            strength = _strength;
            maxDist = _maxDist;
            minDist = _minDist;
        }

        public override void test(PlaneAgent a, Plane3D p)
        {
            Vec3D force = new Vec3D();
            if (!inXY)
            {
                force = attract(a, p, minDist, maxDist, strength, ExponentialInterpolation.Squared);
            }
            else
            {
                ToxiPlane tp = new ToxiPlane(a, a.zz);
                Vec3D op = tp.getProjectedPoint(p);
                force = attract(a, op, minDist, maxDist, strength, ExponentialInterpolation.Squared);
            }
            a.addForce(force);
        }
    }
}
