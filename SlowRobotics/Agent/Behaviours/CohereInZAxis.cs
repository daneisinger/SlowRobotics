using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Agent.Behaviours
{
    public class CohereInZAxis : PlaneAgentBehaviour
    {
        public float strength { get; set; }
        public float maxDist { get; set; }

        public CohereInZAxis(int _priority, float _maxDist, float _strength) : base(_priority)
        {
            strength = _strength;
            maxDist = _maxDist;
        }


        //TODO - make this work with a cutoff so it can be closest only

        public override void test(PlaneAgent a, Plane3D p)
        {
            Vec3D toPlane3D = p.sub(a);
            float ratio = toPlane3D.magnitude() / maxDist;
            float f = ExponentialInterpolation.Squared.interpolate(0, strength, ratio);
            Vec3D zt = a.zz.scale(f);
            float ab = toPlane3D.angleBetween(a.zz, true);
            if (ab > (float)Math.PI / 2) zt.invert();
            a.addForce(zt);
        }
    }
}
