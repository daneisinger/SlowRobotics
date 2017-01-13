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

        public class ToNearestLink : Move
        {

            public Graph parent { get; set; }

            public ToNearestLink(int _priority, Graph _parent, float _strength, float _maxDist) : base(_priority, _strength, _maxDist)
            {
                parent = _parent;
                reset();
            }

            Vec3D a_l;
            Vec3D b_l;
            float minD;
            Link closestL;
            Link closestO;
            Vec3D targetA;
            Vec3D targetB;
            List<Graph> used;

            public override void reset()
            {
                closestL = null;
                closestO = null;
                targetA = null;
                targetB = null;
                a_l = new Vec3D();
                b_l = new Vec3D();
                minD = 1000;
                used = new List<Graph>();
                scaleFactor = 1;
            }

            public override void interactWith(Particle a, object b)
            {

                //TODO fix up this crappy code

                /*
                Node b_n = b as Node;
                if (b_n != null)
                {
                    LinkMesh a_lm = a as LinkMesh;
                    if (a_lm != null) interact(a_lm, b_n);

                    PlaneAgent a_p = a as PlaneAgent;
                    if (a_p != null) interact(a_p, b_n);
                    
                }
                */

            }
            /*
            public void interact(LinkMesh lm, Node p)
            {
                
                if (!used.Contains(p.parent))
                {
                    foreach (Link l in lm.getLinks())
                    {
                        foreach (Link ll in p.parent.getLinks())
                        {

                            Link.closestPtBetweenLinks(l, ll, ref a_l, ref b_l);
                            float d = a_l.distanceTo(b_l);

                            if (d < minD && d > 0)
                            {
                                targetA = a_l.copy();
                                targetB = b_l.copy();
                                minD = d;
                                closestL = l;
                                closestO = ll;
                            }
                        }
                    }
                }
            }

            public void interact(PlaneAgent a, Node p)
            {
                if (!used.Contains(p.parent))
                {
                    foreach (Link ll in p.parent.getLinks())
                    {
                        Vec3D pt = ll.closestPt(a);
                        float d = a.distanceTo(pt);
                        if (d < minD && d > 0)
                        {
                            targetA = ((Vec3D)a).copy();
                            targetB = b_l.copy();
                            minD = d;
                            closestL = null;
                            closestO = ll;
                        }
                    }
                }
            }
*/

            public override void runOn(Particle p)
            {
                if (targetA != null && minD > 0.2)
                {
                    p.addForce(calcForce(p, targetB.sub(p), 0.25f, maxDist, strength, ExponentialInterpolation.Squared));
                }

                reset();
            }
            /*
            public void split(PlaneAgent a, Link l, Vec3D pt)
            {
                PlaneAgent aa = new PlaneAgent(new Plane3D(pt), a.world);
                aa.parent = a.parent;

                Node na = l.a;
                Node nb = l.b;

                Link a_s = new Link(aa, na);
                Link b_s = new Link(aa, nb);

                parent.replaceLink(l, a_s);
                parent.replaceLink(l, b_s);

                foreach (IBehaviour b in a.behaviours.getData()) aa.addBehaviour(b);

                a.world.addDynamic(aa);
            }*/
        }

    }
}
