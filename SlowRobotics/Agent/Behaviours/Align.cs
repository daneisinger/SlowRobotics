using SlowRobotics.Core;
using SlowRobotics.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Agent.Behaviours
{
    public abstract class Align : ScaledAgentBehaviour
    {
        public float strength { get; set; }
        public float maxDist { get; set; }
        public float minDist { get; set; }

        public Align(int _priority, float _strength, float _maxDist) : this(_priority, _strength, 0, _maxDist) { }
        public Align(int _priority, float _strength) : this(_priority, _strength, 0, 1) { }
        public Align(int _priority, float _strength, float _minDist, float _maxDist) : base(_priority)
        {
            strength = _strength;
            minDist = _minDist;
            maxDist = _maxDist;
        }

        public class Planes : Align
        {

            public Planes(int _priority, float _maxDist, float _strength) : base(_priority, _strength, _maxDist) { }

            public override void test(PlaneAgent a, Plane3D p)
            {
                Vec3D ab = p.sub(a);
                float sf = (strength * scaleFactor) - SR_Math.normalizeDistance(ab, minDist, maxDist, strength * scaleFactor, ExponentialInterpolation.Squared); //invert
                a.interpolateToPlane3D(p, sf);
            }

        }

        public class Axis : Align
        {
            public int axis { get; set; }

            public Axis(int _priority, float _strength, int _axis) : this(_priority, _strength, 0, _axis) { }

            public Axis(int _priority, float _strength, float _maxDist, int _axis) : base(_priority, _strength, _maxDist)
            {
                axis = _axis;
            }

            public void interpolateToVector(Plane3D a, Vec3D vec, float factor)
            {
                switch (axis)
                {
                    case 0:
                        a.interpolateToXX(vec, factor);
                        break;
                    case 1:
                        a.interpolateToYY(vec, factor);
                        break;
                    case 2:
                        a.interpolateToZZ(vec, factor);
                        break;
                    default:
                        a.interpolateToXX(vec, factor);
                        break;
                }
            }

            public Vec3D getAxis(Plane3D a)
            {
                switch (axis)
                {
                    case 0:
                        return a.xx;
                    case 1:
                        return a.yy;
                    case 2:
                        return a.zz;
                    default:
                        return a.xx;
                }
            }

        }

        public class AxisToLinks : Axis
        {

            public AxisToLinks(int _priority, float _strength, int _axis) : base(_priority, _strength, _axis) { }

            public override void run(PlaneAgent a)
            {
                foreach (Link l in a.getLinks())
                {
                    interpolateToVector(a, l.getDir(), strength*scaleFactor);
                }
            }
        }

        public class AxisToVelocity : Axis
        {

            public AxisToVelocity(int _priority, float _strength, int _axis) : base(_priority, _strength, _axis) { }

            public override void run(PlaneAgent a)
            {
                interpolateToVector(a, a.getVel(), strength*scaleFactor);
            }
        }


        public class AxisToNearLinks : Axis
        {
            public bool useParent { get; set; }

            public AxisToNearLinks(int _priority, float _maxDist, float _strength, bool _useParent) : this(_priority, _strength, _maxDist, 0, _useParent) { }

            public AxisToNearLinks(int _priority, float _maxDist, float _strength, int _axis, bool _useParent) : base(_priority, _strength, _maxDist, _axis)
            {
                useParent = _useParent;
            }

            public override void test(PlaneAgent a, Plane3D p)
            {
                if (p is Node) test(a, (Node)p);
            }

            public void test(PlaneAgent a, Node p)
            {
                if (useParent || p.parent != a.parent)
                {
                    foreach (Link l in p.parent.getLinks())
                    {
                        Vec3D ab = l.closestPt(a).sub(a);
                        float f = SR_Math.normalizeDistance(ab, 0, maxDist, strength * scaleFactor, ExponentialInterpolation.Squared);
                        interpolateToVector(a, l.getDir(), f);
                    }
                }
            }
        }

        public class AxisToBestFit : Axis
        {

            private SortedList closestPts;
            private Vec3D n;

            public AxisToBestFit(int _priority, float _strength, float _maxDist) : this(_priority, _strength, _maxDist, 2) { }

            public AxisToBestFit(int _priority, float _strength, float _maxDist, int _axis) : base(_priority,_strength,_maxDist,_axis)
            {
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
                    if (getAxis(a).angleBetween(n) > (float)Math.PI / 2) n.invert();
                    interpolateToVector(a, n, strength * scaleFactor);
                }
                reset();
            }
        }

    }
}
