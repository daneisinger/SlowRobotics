using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toxiclibs.core;


namespace SlowRobotics.Behaviours.NeighbourBehaviours
{
    public class CohereZBehaviour : NeighbourBehaviour
    {
        float strength;
        float maxDist;

        public CohereZBehaviour(int _priority, float _maxDist, float _strength) : base(_priority)
        {
            strength = _strength;
            maxDist = _maxDist;
        }

        override
        public void run(Agent a, Agent b)
        {

            Plane3D j = (Plane3D)b;
            if (j != a)
            {
                //move by normal
                Vec3D toPlane3D = j.sub(a);
                float ratio = toPlane3D.magnitude() / maxDist;
                float f = ExponentialInterpolation.Squared.interpolate(0, strength, ratio);
                Vec3D zt = a.zz.scale(f);
                float ab = toPlane3D.angleBetween(a.zz, true);
                if (ab > (float)Math.PI / 2) zt.invert();
                a.addForce(zt);
            }
        }
    }
}
