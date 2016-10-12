using SlowRobotics.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Agent.Behaviours
{
    //previously orient to best fit
    public class AlignZZToBestFit : ScaledAgentBehaviour
    {
        public float maxDist { get; set; }
        public float orientToBestFit { get; set; }

        private SortedList closestPts;
        private Vec3D n;

        public AlignZZToBestFit(int _priority, float _maxDist, float _orientToBestFit) : base(_priority)
        {
            maxDist = _maxDist;
            orientToBestFit = _orientToBestFit;

            reset();
        }

        public void reset()
        {
            closestPts = new SortedList();
            n = new Vec3D();
        }

        public override void test(PlaneAgent a, Plane3D p)
        {
            float d = p.distanceTo(a);
            if (d > 0 && !closestPts.ContainsKey(d)) closestPts.Add(d, p);
        }

        public override void run(PlaneAgent a)
        {
            if (closestPts.Count >= 3)
            {
                Triangle3D tri = new Triangle3D((Vec3D)closestPts.GetValueList()[0], (Vec3D)closestPts.GetValueList()[1], (Vec3D)closestPts.GetValueList()[2]);
                n = tri.computeNormal();
                if (a.zz.angleBetween(n) > (float)Math.PI / 2) n.invert();
                a.interpolateToZZ(n, orientToBestFit*scaleFactor);
            }
            reset();
        }
    }
}
