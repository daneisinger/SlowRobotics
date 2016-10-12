using SlowRobotics.Core;
using SlowRobotics.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Agent.Behaviours
{
    public class Attract : ScaledAgentBehaviour
    {
        
        public float strength { get; set; }
        public float minDist { get; set; }
        public float maxDist { get; set; }

        public Attract(int _priority, float _strength, float _maxDist) : this(_priority, _strength, 0, _maxDist) { }

        public Attract(int _priority, float _strength, float _minDist, float _maxDist) : base(_priority)
        {
            strength = _strength;
            maxDist = _maxDist;
            minDist = _minDist;
        }

        public override void test(PlaneAgent a, Plane3D p)
        {
            a.addForce(attract(a, p, minDist, maxDist, strength*scaleFactor, ExponentialInterpolation.Squared));
        }

        public Vec3D attract(Vec3D a, Vec3D b, float minDist, float maxDist, float maxForce, InterpolateStrategy interpolator)
        {
            Vec3D ab = b.sub(a);
            float d = ab.magnitude();
            float f = maxForce - SR_Math.normalizeDistance(ab, minDist, maxDist, maxForce, interpolator);
            return (d > minDist && d < maxDist) ? ab.normalizeTo(f) : new Vec3D();
        }


        public class InXY  : Attract
        {
            public InXY(int _priority, float _strength, float _minDist, float _maxDist) : base(_priority, _strength, _minDist, _maxDist) { }

            public override void test(PlaneAgent a, Plane3D p)
            {
                ToxiPlane tp = new ToxiPlane(a, a.zz);
                Vec3D op = tp.getProjectedPoint(p);
                a.addForce(attract(a, op, minDist, maxDist, strength * scaleFactor, ExponentialInterpolation.Squared));
            }
        }

        public class InZAxis : Attract
        {

            public InZAxis(int _priority, float _strength, float _maxDist) : base(_priority,_strength, _maxDist) {}

            public override void test(PlaneAgent a, Plane3D p)
            {
                Vec3D toPlane3D = p.sub(a);
                float d = toPlane3D.magnitude();
                if (d > minDist && d < maxDist)
                {
                    float ratio = 1-(d / maxDist); //NOTE! I inverted this ratio. Seems to have a pretty dramatic effect
                    float f = ExponentialInterpolation.Squared.interpolate(0, strength * scaleFactor, ratio);
                    Vec3D zt = a.zz.scale(f);
                    float ab = toPlane3D.angleBetween(a.zz, true);
                    if (ab > (float)Math.PI / 2) zt.invert();
                    a.addForce(zt);
                }
            }
        }

        public class ToNearestLink : Attract
        {

            public LinkMesh parent { get; set; }

            public ToNearestLink(int _priority, LinkMesh _parent, float _strength, float _maxDist) : base(_priority, _strength, _maxDist)
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
            List<LinkMesh> used;

            public void reset()
            {
                closestL = null;
                closestO = null;
                targetA = null;
                targetB = null;
                a_l = new Vec3D();
                b_l = new Vec3D();
                minD = 1000;
                used = new List<LinkMesh>();
            }

            public override void test(LinkMesh a, Plane3D p)
            {
                if (p is Node) test(a, (Node)p);
            }

            public void test(LinkMesh lm, Node p)
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

            public override void test(PlaneAgent a, Plane3D p)
            {
                if (p is Node) test(a, (Node)p);
            }

            public void test(PlaneAgent a, Node p)
            {
                if (!used.Contains(p.parent))
                {
                    foreach (Link ll in p.parent.getLinks())
                    {
                        Vec3D pt = ll.closestPt(a);
                        float d = a.distanceTo(pt);
                        if (d < minD && d > 0)
                        {
                            targetA = a.copy();
                            targetB = b_l.copy();
                            minD = d;
                            closestL = null;
                            closestO = ll;
                        }
                    }
                }
            }

            public override void run(LinkMesh a)
            {
                if (targetA != null && minD > 0.2)
                {
                    a.addForce(attract(a, targetB, 0.25f, maxDist, strength, ExponentialInterpolation.Squared));

                    if (minD < 0.5)
                    {
                       // if (targetA.distanceTo(closestL.a) > 2 && targetA.distanceTo(closestL.b) > 2) split(a, closestL, targetA);
                    }
                }

                reset();
            }

            public override void run(PlaneAgent a)
            {
                if (targetA != null && minD > 0.2)
                {
                    a.addForce(attract(a, targetB, 0.25f, maxDist, strength, ExponentialInterpolation.Squared));
                    if (minD < 0.5)
                    {
                        //if (targetA.distanceTo(closestL.a) > 2 && targetA.distanceTo(closestL.b) > 2) split(a, closestL, targetA);
                    }
                }

                reset();
            }

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
            }
        }

    }
}
