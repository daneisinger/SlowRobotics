using SlowRobotics.Core;
using SlowRobotics.Field;
using SlowRobotics.SRMath;
using SlowRobotics.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Agent.Behaviours
{
    public abstract class Align : ScaledBehaviour<Plane3D>
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

            public override void interactWith(Plane3D a_p, object b)
            {
                Plane3D b_p = b as Plane3D;
                if (b_p != null) {
                    Vec3D ab = b_p.sub(a_p);
                    float d = ab.magnitude();
                    if (d > minDist && d < maxDist)
                    {
                        float f = MathUtils.map(d, minDist, maxDist, 1, 0);
                        float sf = interpolator.interpolate(0, strength, f);
                        a_p.interpolateToPlane3D(b_p, sf * scaleFactor);
                    }
                }
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

        public class AxisToField : Axis
        {
            public IField field { get; set; }
            public AxisToField(int _priority, IField _field, float _strength, int _axis) : base(_priority, _strength, _axis)
            {
                field = _field;
            }

            public override void runOn(Plane3D a_p)
            {
                FieldData d = field.evaluate(a_p);
                Vec3D dir = new Vec3D();

                if (d.hasPlaneData()) dir.addSelf(getAxis(d.planeData));
                if (d.hasVectorData()) dir.addSelf(d.vectorData); 

                interpolateToVector(a_p, dir, strength * scaleFactor);
            }

            public Vec3D getAxis(WeightedPlane3D a)
            {
                switch (axis)
                {
                    case 0:
                        return a.wx;
                    case 1:
                        return a.wy;
                    case 2:
                        return a.wz;
                    default:
                        return a.wx;
                }
            }
        }

       /*

        //TODO - implement using graph

        public class AxisToLinks : Axis
        {

            public AxisToLinks(int _priority, float _strength, int _axis) : base(_priority, _strength, _axis) { }

            public override void runOn(Particle a_p)
            {
                foreach (LegacyLink l in a_p.getLinks())
                    {
                        interpolateToVector(a_p, l.getDir(), strength * scaleFactor);
                    }
            }
        }*/
        
        public class AxisToVelocity : Axis
        {

            public AxisToVelocity(int _priority, float _strength, int _axis) : base(_priority, _strength, _axis) { }

            public override void runOn(Plane3D a_p)
            {
                SRParticle p = (SRParticle)a_p; //TODO - WITHOUT CASTS
                if(p!=null) interpolateToVector(a_p, p.getVel(), strength * scaleFactor);
            }
        }

       /*

            //TODO - USE GRAPH

        public class AxisToNearLinks : Axis
        {
            public bool useParent { get; set; }

            public AxisToNearLinks(int _priority, float _maxDist, float _strength, bool _useParent) : this(_priority, _strength, _maxDist, 0, _useParent) { }

            public AxisToNearLinks(int _priority, float _maxDist, float _strength, int _axis, bool _useParent) : base(_priority, _strength, _maxDist, _axis)
            {
                useParent = _useParent;
            }

            public override void interactWith(Particle a_n, object b)
            {
                LegacyNode b_n = b as LegacyNode;
                if (b_n != null)
                {
                    if (useParent || b_n.parent != a_n.parent)
                    {
                        foreach (LegacyLink l in b_n.parent.getLinks())
                        {
                            Vec3D ab = l.closestPt(a_n).sub(a_n);
                            float d = ab.magnitude();

                            if (d > minDist && d < maxDist)
                            {
                                float f = SR_Math.map(d, minDist, maxDist, 1, 0);
                                float sf = ExponentialInterpolation.Squared.interpolate(0, strength, f);
                                interpolateToVector(a_n, l.getDir(), f);
                            }
                        }
                    }
                }
            }
        }
      */

        public class AxisTo3PtTri : Axis
        {

            private SortedList closestPts;
            private Vec3D n;

            public AxisTo3PtTri(int _priority, float _strength, float _maxDist) : this(_priority, _strength, _maxDist, 2) { }

            public AxisTo3PtTri(int _priority, float _strength, float _maxDist, int _axis) : base(_priority,_strength,_maxDist,_axis)
            {
                reset();
            }

            public override void reset()
            {
                closestPts = new SortedList();
                n = new Vec3D();
                scaleFactor = 1;
            }

            public override void interactWith(Plane3D a_v, object b)
            {
                Vec3D b_v = b as Vec3D;
                if (b_v != null)
                {
                    float d = b_v.distanceTo(a_v);
                    if (d > 0 && !closestPts.ContainsKey(d)) closestPts.Add(d, b_v);
                }
            }

            public override void runOn(Plane3D a_p)
            {
                if (closestPts.Count >= 3)
                {
                    Triangle3D tri = new Triangle3D((Vec3D)closestPts.GetValueList()[0], (Vec3D)closestPts.GetValueList()[1], (Vec3D)closestPts.GetValueList()[2]);
                    n = tri.computeNormal();
                    if (getAxis(a_p).angleBetween(n) > (float)Math.PI / 2) n.invert();
                    interpolateToVector(a_p, n, strength * scaleFactor);
                }
                    reset();
            }
        }

        public class AxisToBestFitPlane : Axis
        {

            public AxisToBestFitPlane(int _priority, float _strength, float _maxDist) : this(_priority, _strength, _maxDist, 2) { }

            public AxisToBestFitPlane(int _priority, float _strength, float _maxDist, int _axis) : base(_priority, _strength, _maxDist, _axis)
            {
                reset();
            }

            public override void run(IAgentT<object> a)
            {
                
                Plane3D a_p = a.getData() as Plane3D;
                if (a_p != null && a.neighbours.Count >= 3)
                {
                    Vec3D centroid = new Vec3D();

                    Vec3D n = MathUtils.getPlaneNormal(a.neighbours, out centroid);
                    if (n != null)
                    {
                        if (getAxis(a_p).angleBetween(n) > (float)Math.PI / 2) n.invert();
                        interpolateToVector(a_p, n, strength * scaleFactor);
                    }
                }
                reset();
            }
        }
    }
}
