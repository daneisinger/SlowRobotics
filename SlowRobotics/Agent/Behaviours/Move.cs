using SlowRobotics.Core;
using SlowRobotics.Field;
using SlowRobotics.SRGraph;
using SlowRobotics.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Agent.Behaviours
{

    public class Move : ScaledBehaviour<SRParticle>
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
            float f = SRMath.map(d, minDist, maxDist, 1, 0);
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

            public override void runOn(SRParticle p)
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

            public override void runOn(SRParticle p)
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

            public override void interactWith(SRParticle p, object b)
            {
                SRParticle b_v = b as SRParticle;
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
                        float f = SRMath.map(d, minDist, maxDist, 1, 0);
                        float sf = ExponentialInterpolation.Squared.interpolate(0, strength, f);
                        if (d > minDist && d < maxDist) force.addSelf(p.sub(op).normalizeTo(sf));
                    }
                }

            }

            public override void runOn(SRParticle p)
            {
                p.addForce(force);
                reset();
            }
        }

        public class Together  : Apart
        {

            public Together(int _priority, float _strength, float _minDist, float _maxDist, bool _inXY) : base(_priority, _strength, _minDist, _maxDist, _inXY){ }

            public override void interactWith(SRParticle p, object b)
            {
                SRParticle b_v = b as SRParticle;
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
                        float f = SRMath.map(d, minDist, maxDist, 1, 0);
                        float sf = ExponentialInterpolation.Squared.interpolate(0, strength, f);
                        if (d > minDist && d < maxDist) force.addSelf(op.sub(p).normalizeTo(sf));
                    }
                }
            }
        }

        public class PointToLine : Apart
        {
            public PointToLine(int _priority, float _strength, float _minDist, float _maxDist) : base(_priority, _strength, _minDist, _maxDist, false){ }

            public override void interactWith(SRParticle p, object b)
            {
                ILine b_l = b as ILine;
                if (b_l != null)
                {

                    Vec3D cPt = b_l.closestPoint(p);
                    force.addSelf(calcForce(p, cPt.sub(p), minDist, maxDist, strength * scaleFactor, ExponentialInterpolation.Squared));

                }
            }
        }

        public class LineToLine : Apart
        {
            public LineToLine(int _priority, float _strength, float _minDist, float _maxDist) : base(_priority, _strength, _minDist, _maxDist, false) { }

            public override void interactWith(SRParticle p, object b)
            {
                ILine b_l = b as ILine;
                if (b_l != null)
                {
                    ILine a_l = p as ILine;
                    if(a_l != null)
                    {
                        Vec3D cPt_a = a_l.closestPoint(b_l);
                        Vec3D cPt_b = b_l.closestPoint(a_l);
                        force.addSelf(calcForce(p, cPt_a.sub(cPt_b), minDist, maxDist, strength * scaleFactor, ExponentialInterpolation.Squared));
                    }
                }
            }
        }

        public class ToGraphEdges : Apart
        {

            //TODO - use interfaces e.g. Graph<IParticle,ILine>
            Graph<SRParticle, Spring> graph;

            public ToGraphEdges(int _priority, Graph<SRParticle,Spring> _graph, float _strength, float _minDist, float _maxDist) :
                base(_priority, _strength, _minDist, _maxDist, false)
            {
                graph = _graph;
            }

            public override void interactWith(SRParticle a, object b)
            {
                SRParticle b_p = b as SRParticle;
                if (b_p != null)
                {
                    //check if p has any edges 
                    IEnumerable<Spring> a_edges = graph.getEdgesFor(a);

                    //check to see if b is a node in the graph + get edges
                    foreach (Spring b_s in graph.getEdgesFor(b_p))
                    {
                        if (a_edges.Count() != 0)
                        {
                            //average closest points for each edge
                            foreach(Spring a_s in a_edges)
                            {
                                Vec3D cPt_a;
                                Vec3D cPt_b;

                                Spring.closestPoints(a_s, b_s, out cPt_a, out cPt_b);
                                if (cPt_a != null && cPt_b != null)
                                {
                                    Vec3D dir = cPt_b.sub(cPt_a);
                                    force.addSelf(calcForce(a, dir, minDist, maxDist, strength * scaleFactor, ExponentialInterpolation.Squared));
                                }
                            }
                        }
                        else
                        {
                            //attract to closest points on lines
                            Vec3D cPt = b_s.closestPoint(a);
                            force.addSelf(calcForce(a, cPt.sub(a), minDist, maxDist, strength * scaleFactor, ExponentialInterpolation.Squared));
                        }
                    }
                }
            }
        }

        public class ToClosestPoint : Apart
        {
            Vec3D closestPt;
            float md;

            public ToClosestPoint(int _priority, float _strength, float _minDist, float _maxDist, bool _inXY) :
                base(_priority, _strength, _minDist, _maxDist, _inXY)
            {
                closestPt = null;
                md = 10000;
            }

            public override void interactWith(SRParticle p, object b)
            {
                SRParticle b_v = b as SRParticle;
                if (b_v != null)
                {
                    float d = b_v.distanceTo(p);
                    if (d < md)
                    {
                        md = d;
                        closestPt = b_v;
                    }
                }
            }


            public override void runOn(SRParticle p)
            {
                if (closestPt != null)
                {
                    force = calcForce(p, closestPt.sub(p), minDist, maxDist, strength * scaleFactor, ExponentialInterpolation.Squared);
                    p.addForce(force);
                }
                closestPt = null;
                md = 1000000;
                reset();
            }
        }

        public class TogetherInZ : Move
        {

            public TogetherInZ(int _priority, float _strength, float _maxDist) : base(_priority,_strength, _maxDist) {}

            public override void interactWith(SRParticle p, object b)
            {
                Vec3D b_v = b as Vec3D;
                if (b_v != null)
                {
                    Vec3D ab = b_v.sub(p);
                    float d = ab.magnitude();
                    if (d > minDist && d < maxDist)
                    {
                        float f = SRMath.map(d, 0, maxDist, 1, 0);
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
