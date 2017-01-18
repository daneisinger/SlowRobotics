using SlowRobotics.Core;
using SlowRobotics.Field;
using SlowRobotics.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Agent.Behaviours
{
    public class Move : ScaledBehaviour<Particle>
    {
        
        public float strength { get; set; }
        public float minDist { get; set; }
        public float maxDist { get; set; }

        public Move(int _priority, float _strength, float _maxDist) : this(_priority, _strength, 0, _maxDist) { }

        public Move(int _priority, float _strength, float _minDist, float _maxDist) : base(_priority)
        {
            strength = _strength;
            maxDist = _maxDist;
            minDist = _minDist;
        }

        public Vec3D calcForce(Vec3D a, Vec3D ab, float minDist, float maxDist, float maxForce, InterpolateStrategy interpolator)
        {
            float d = ab.magnitude();
            float f = SR_Math.map(d, minDist, maxDist, 1, 0);
            return (d > minDist && d < maxDist) ? ab.normalizeTo(interpolator.interpolate(0, maxForce, f)) : new Vec3D();
        }

        public class InAxis : Move
        {
            public int axis { get; set; }

            public InAxis(int _priority, float _strength, int _axis) : this(_priority, _strength, 0, _axis) { }

            public InAxis(int _priority, float _strength, float _maxDist, int _axis) : base(_priority, _strength, _maxDist)
            {
                axis = _axis;
            }

            public override void runOn(Particle p)
            {
                if (scaleFactor > 0) p.addForce(getAxis(p).scale(strength * scaleFactor));
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

        public class InField : Move
        {
            public IField field { get; set; }
            public InField(int _priority, IField _field, float _strength) : base(_priority, _strength, 1000)
            {
                field = _field;
            }

            public override void runOn(Particle p)
            {
                FieldData d = field.evaluate(p);
                if (d.hasVectorData()) p.addForce(d.vectorData.scale(strength * scaleFactor));
            }
        }

        public class Apart : Move
        {
            public bool inXY { get; set; }

            public Vec3D force;

            public Apart(int _priority, float _strength, float _minDist, float _maxDist, bool _inXY) : base(_priority, _strength, _minDist,_maxDist)
            {
                inXY = _inXY;
                reset();
            }

            public override void reset()
            {
                force = new Vec3D();
                scaleFactor = 1;
            }

            public override void interactWith(Particle p, object b)
            {
                Vec3D b_v = b as Vec3D;
                if(b_v!= null) { 
                    
                    if (!inXY)
                    {
                        force.addSelf(calcForce(p, p.sub(b_v), minDist, maxDist, strength * scaleFactor, ExponentialInterpolation.Squared));
                    }
                    else
                    {
                        ToxiPlane tp = new ToxiPlane(p, p.zz);
                        Vec3D op = tp.getProjectedPoint(b_v);
                        float d = p.distanceTo(b_v);
                        float f = SR_Math.map(d, minDist, maxDist, 1, 0);
                        float sf = ExponentialInterpolation.Squared.interpolate(0, strength, f);
                        if (d > minDist && d < maxDist) force.addSelf(p.sub(op).normalizeTo(sf));
                    }
                }

            }

            public override void runOn(Particle p)
            {
                p.addForce(force);
                reset();
            }
        }

        public class Together  : Apart
        {

            public Together(int _priority, float _strength, float _minDist, float _maxDist, bool _inXY) : base(_priority, _strength, _minDist, _maxDist, _inXY){ }

            public override void interactWith(Particle p, object b)
            {
                Vec3D b_v = b as Vec3D;
                if (b_v != null)
                {
                    if (!inXY)
                    {
                        force.addSelf(calcForce(p, b_v.sub(p), minDist, maxDist, strength * scaleFactor, ExponentialInterpolation.Squared));
                    }
                    else
                    {
                        ToxiPlane tp = new ToxiPlane(p, p.zz);
                        Vec3D op = tp.getProjectedPoint(b_v);
                        float d = p.distanceTo(b_v);
                        float f = SR_Math.map(d, minDist, maxDist, 1, 0);
                        float sf = ExponentialInterpolation.Squared.interpolate(0, strength, f);
                        if (d > minDist && d < maxDist) force.addSelf(op.sub(p).normalizeTo(sf));
                    }
                }
            }
        }

        public class TogetherInZ : Move
        {

            public TogetherInZ(int _priority, float _strength, float _maxDist) : base(_priority,_strength, _maxDist) {}

            public override void interactWith(Particle p, object b)
            {
                Vec3D b_v = b as Vec3D;
                if (b_v != null)
                {
                    Vec3D ab = b_v.sub(p);
                    float d = ab.magnitude();
                    if (d > minDist && d < maxDist)
                    {
                        float f = SR_Math.map(d, 0, maxDist, 1, 0);
                        float sf = ExponentialInterpolation.Squared.interpolate(0, strength, f);
                        Vec3D zt = p.zz.scale(sf * scaleFactor);
                        float angle = ab.angleBetween(p.zz, true);
                        if (angle > (float)Math.PI / 2) zt.invert();
                        p.addForce(zt);
                    }
                }
            }
        }
    }
}
