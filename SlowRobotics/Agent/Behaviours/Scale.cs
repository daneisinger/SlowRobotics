using SlowRobotics.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Agent.Behaviours
{
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
        public override void run(IAgentT<object> a)
        {
            foreach (IScaledBehaviour b in behaviours) b.scale(1);
        }

        public class ByClosestPoint : Scale
        {

            public List<Vec3D> pts { get; set; }
            public float maxDist { get; set; }

            public ByClosestPoint(int _priority, List<IScaledBehaviour> _behaviours, List<Vec3D> _pts, float _maxDist):base(_priority,_behaviours)
            {
                pts = _pts;
                maxDist = _maxDist;
            }

            public override void run(IAgentT<object> a)
            {
                IAgentT<Vec3D> typedAgent = (IAgentT<Vec3D>)a; //cast to generic
                float f = getFactor(typedAgent.getData(), pts);
                foreach (IScaledBehaviour b in behaviours)
                {
                    b.scale(f);
                }
            }

            public virtual float getFactor(Vec3D a, List<Vec3D> _pts)
            {
                Vec3D cPt = SR_Math.getClosestN(a, _pts, 1)[0];
                float f = a.distanceTo(cPt);
                return f > maxDist ? 1 : f / maxDist;
            }
        }

        public class ByClosestPoints : ByClosestPoint
        {
            public int numClosest { get; set; }

            public ByClosestPoints(int _priority, List<IScaledBehaviour> _behaviours, List<Vec3D> _pts, float _maxDist, int _numClosest) : base(_priority, _behaviours, _pts, _maxDist)
            {
                numClosest = _numClosest;
            }

            public override float getFactor(Vec3D a, List<Vec3D> _pts)
            {
                Vec3D avg = SR_Math.averageVectors(SR_Math.getClosestN(a, _pts, numClosest));
                float f = a.distanceTo(avg);
                return f > maxDist ? 1 : f / maxDist;
            }
        }

        public class ByDistToBoundingBox : Scale
        {
            public float maxDist { get; set; }
            public AABB box { get; set; }

            public ByDistToBoundingBox(int _priority, List<IScaledBehaviour> _behaviours, AABB _box, float _maxDist) : base(_priority, _behaviours)
            {
                maxDist = _maxDist;
                box = _box;
            }

            public override void run(IAgentT<object> a)
            {
                IAgentT<Vec3D> typedAgent = (IAgentT<Vec3D>)a; //cast to generic
                float f = getFactor(typedAgent.getData(), box);
                foreach (IScaledBehaviour b in behaviours)
                {
                    b.scale(f);
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
