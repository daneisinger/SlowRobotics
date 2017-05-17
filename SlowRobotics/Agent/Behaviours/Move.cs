using SlowRobotics.Core;
using SlowRobotics.Field;
using SlowRobotics.SRGraph;
using SlowRobotics.SRMath;
using SlowRobotics.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Agent.Behaviours
{
    /// <summary>
    /// Parent move class defines base properties for movement behaviours 
    /// </summary>
    public class Move : ScaledBehaviour<SRParticle>
    {
        
        public float strength { get; set; }
        public float minDist { get; set; }
        public float maxDist { get; set; }
        public FalloffStrategy falloff { get; set; }

        public Move(int _priority, float _strength, float _maxDist) : this(_priority, _strength, 0, _maxDist) { }

        public Move(int _priority, float _strength, float _minDist, float _maxDist) : base(_priority)
        {
            strength = _strength;
            maxDist = _maxDist;
            minDist = _minDist;
            falloff = new InverseFalloffStrategy();
        }

        /// <summary>
        /// Moves a particle in one of its axes
        /// </summary>
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

        /// <summary>
        /// Moves a particle using the tensor of a field at the particles location
        /// </summary>
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

        /// <summary>
        /// Interaction behaviour that moves the particle away from all neighbours. Optionally
        /// projects neighbours to the particle plane for 2d repulsion
        /// </summary>
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
                    Vec3D ab = (!inXY) ? p.sub(b_v) : getProjectedAB(p, p.zz, b_v);
                    float radiusSum = b_v.radius + p.radius;
                    float repelDist = maxDist > radiusSum ? maxDist : radiusSum;
                    force.addSelf(falloff.getForce(ab, minDist, repelDist, strength * scaleFactor, interpolator));
                }
            }

            public Vec3D getProjectedAB(Vec3D a, Vec3D normal, Vec3D b)
            {
                ToxiPlane tp = new ToxiPlane(a, normal);
                Vec3D op = tp.getProjectedPoint(b);
                return a.sub(op);
            }

            public override void runOn(SRParticle p)
            {
                p.addForce(force);
                reset();
            }
        }

        /// <summary>
        /// Interaction behaviour that moves the particle toward all neighbours. Optionally
        /// projects neighbours to the particle plane for 2d attraction
        /// </summary>
        public class Together  : Apart
        {

            public Together(int _priority, float _strength, float _minDist, float _maxDist, bool _inXY) : base(_priority, _strength, _minDist, _maxDist, _inXY)
            {
                falloff = new NoFalloffStrategy();
            }

            public override void interactWith(SRParticle p, object b)
            {
                SRParticle b_v = b as SRParticle;
                if (b_v != null)
                {
                    Vec3D ab = (!inXY) ? p.sub(b_v) : getProjectedAB(p, p.zz, b_v);
                    force.addSelf(falloff.getForce(ab.getInverted(), minDist, maxDist, strength * scaleFactor, interpolator));
                }
            }
        }

        /// <summary>
        /// Interaction behaviour that moves the particle towards the closest point on a line
        /// </summary>
        public class PointToLine : Apart
        {
            public PointToLine(int _priority, float _strength, float _minDist, float _maxDist) : base(_priority, _strength, _minDist, _maxDist, false){ }

            public override void interactWith(SRParticle p, object b)
            {
                ILine b_l = b as ILine;
                if (b_l != null)
                {

                    Vec3D cPt = b_l.closestPoint(p);

                    force.addSelf(falloff.getForce(cPt.sub(p), minDist, maxDist, strength * scaleFactor, interpolator));

                   // force.addSelf(calcForce(p, cPt.sub(p), minDist, maxDist, strength * scaleFactor, ExponentialInterpolation.Squared));

                }
            }
        }

        /// <summary>
        /// Interaction behaviour that attempts to cast the particle to a line (e.g. SRLinearParticle) and then
        /// moves towards closest points between neighbouring lines
        /// </summary>
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
                        //force.addSelf(calcForce(p, cPt_a.sub(cPt_b), minDist, maxDist, strength * scaleFactor, ExponentialInterpolation.Squared));
                        force.addSelf(falloff.getForce(cPt_a.sub(cPt_b), minDist, maxDist, strength * scaleFactor, interpolator));
                    }
                }
            }
        }

        /// <summary>
        /// Interaction behaviour that moves a particle towards closest points on all graph edges
        /// </summary>
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
                                    force.addSelf(falloff.getForce(dir, minDist, maxDist, strength * scaleFactor, interpolator));
                                   // force.addSelf(calcForce(a, dir, minDist, maxDist, strength * scaleFactor, ExponentialInterpolation.Squared));
                                }
                            }
                        }
                        else
                        {
                            //attract to closest points on lines
                            Vec3D cPt = b_s.closestPoint(a);
                            force.addSelf(falloff.getForce(cPt.sub(a), minDist, maxDist, strength * scaleFactor, interpolator));
                            //force.addSelf(calcForce(a, cPt.sub(a), minDist, maxDist, strength * scaleFactor, ExponentialInterpolation.Squared));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Interaction behaviour that moves to towards only the closest point in the neighbour list.
        /// This is an alternative to using the FilterClosest behaviour to modify the entire neighbour list.
        /// </summary>
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
                    force = falloff.getForce(closestPt.sub(p), minDist, maxDist, strength * scaleFactor, interpolator);
                   // force = calcForce(p, closestPt.sub(p), minDist, maxDist, strength * scaleFactor, ExponentialInterpolation.Squared);
                    p.addForce(force);
                }
                closestPt = null;
                md = 1000000;
                reset();
            }
        }

        /// <summary>
        /// Interaction behaviour that moves a particle in its z axis towards the plane of neighbouring particles
        /// </summary>
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
                        float f = MathUtils.map(d, 0, maxDist, 1, 0);
                        float sf = interpolator.interpolate(0, strength, f);
                        Vec3D zt = p.zz.scale(sf * scaleFactor);
                        float angle = ab.angleBetween(p.zz, true);
                        if (angle > (float)Math.PI / 2) zt.invert();
                        p.addForce(zt);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Brownian (random) motion. Optional 2d motion.
    /// </summary>
    public class Brownian : ScaledBehaviour<SRParticle>
    {

        public float strength;
        public bool inXY;

        public Brownian(int _priority, float _strength, bool _inXY) : base(_priority)
        {
            strength = _strength;
            inXY = _inXY;
        }

        public override void runOn(SRParticle v)
        {
            Vec3D randomVector = new Vec3D((float)ThreadedRandom.RandDouble() - 0.5f, 
                (float)ThreadedRandom.RandDouble() - 0.5f, 
                inXY?0: (float)ThreadedRandom.RandDouble() - 0.5f);

            randomVector.scaleSelf(strength);
            v.addForce(randomVector);
        }


    }
}
