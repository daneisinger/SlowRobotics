using SlowRobotics.SRMath;
using System.Collections.Generic;

using Toxiclibs.core;

namespace SlowRobotics.Agent.Behaviours
{
    /// <summary>
    /// Parent scale behaviour. The scale behaviour takes a collection of ScaledBehaviours
    /// and calls the scale() function on each before calling the runOn function. This is useful 
    /// for implementing custom constraints and falloff effects.
    /// </summary>
    public class Scale : ScaledBehaviour<Vec3D>
    {
        public List<IScaledBehaviour> behaviours { get; set; }

        public Scale(int _priority, List<IScaledBehaviour> _behaviours) : base(_priority)
        {
            behaviours = _behaviours;
        }

        /// <summary>
        /// Default scaling
        /// </summary>
        /// <param name="a"></param>
        public override void run(IAgent<object> a)
        {
            foreach (IScaledBehaviour b in behaviours) b.scale(1);
        }

        /// <summary>
        /// Scales a behaviour by distance to the closest point in a given point collection
        /// </summary>
        public class ByClosestPoint : Scale
        {

            public List<Vec3D> pts { get; set; }
            public float maxDist { get; set; }

            public ByClosestPoint(int _priority, List<IScaledBehaviour> _behaviours, List<Vec3D> _pts, float _maxDist):base(_priority,_behaviours)
            {
                pts = _pts;
                maxDist = _maxDist;
            }

            public override void run(IAgent<object> a)
            {
                IAgent<Vec3D> typedAgent = (IAgent<Vec3D>)a; //cast to generic
                float f = getFactor(typedAgent.getData(), pts);
                foreach (IScaledBehaviour b in behaviours)
                {
                    b.scale(f);
                }
            }

            public virtual float getFactor(Vec3D a, List<Vec3D> _pts)
            {
                Vec3D cPt = MathUtils.getClosestN(a, _pts, 1)[0];
                float f = a.distanceTo(cPt);
                return f > maxDist ? 1 : f / maxDist;
            }
        }

        /// <summary>
        /// Scales a behaviour by averaged distance to all points in a collection
        /// </summary>
        public class ByClosestPoints : ByClosestPoint
        {
            public int numClosest { get; set; }

            public ByClosestPoints(int _priority, List<IScaledBehaviour> _behaviours, List<Vec3D> _pts, float _maxDist, int _numClosest) : base(_priority, _behaviours, _pts, _maxDist)
            {
                numClosest = _numClosest;
            }

            public override float getFactor(Vec3D a, List<Vec3D> _pts)
            {
                Vec3D avg = MathUtils.averageVectors(MathUtils.getClosestN(a, _pts, numClosest));
                float f = a.distanceTo(avg);
                return f > maxDist ? 1 : f / maxDist;
            }
        }

        /// <summary>
        /// Scales a behaviour by distance to a bounding box. Points within the box are unaffected.
        /// </summary>
        public class ByDistToBoundingBox : Scale
        {
            public float maxDist { get; set; }
            public AABB box { get; set; }

            public ByDistToBoundingBox(int _priority, List<IScaledBehaviour> _behaviours, AABB _box, float _maxDist) : base(_priority, _behaviours)
            {
                maxDist = _maxDist;
                box = _box;
            }

            public override void run(IAgent<object> a)
            {

                Vec3D v = a.getData() as Vec3D;
                if (v != null)
                {
                    float f = getFactor(v, box);
                    foreach (IScaledBehaviour b in behaviours)
                    {
                        b.scale(f);
                    }
                }
            }

            public float getFactor(Vec3D a, AABB _box)
            {
                if (_box.containsPoint(a)) return 1;
                Vec3D p = _box.closestPointOnAABB(a);
                float f = a.distanceTo(p);
                return (f < maxDist) ? 1-(f / maxDist): 0.0f;
            }
        }
    }
}
