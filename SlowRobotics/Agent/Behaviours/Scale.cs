using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Agent.Behaviours
{
    public class Scale : ScaledAgentBehaviour
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
        public override void run(IAgent a)
        {
            foreach (IScaledBehaviour b in behaviours) b.scale(1);
        }

        private static float approxDistance(Vec3D source, Vec3D target)
        {
            float dx = target.x - source.x;
            float dz = target.y - source.y;
            float dy = target.z - source.z;

            return (dx * dx) + (dy * dy) + (dz * dz);
        }

        public static List<Vec3D> getClosestN(Vec3D a, List<Vec3D> _pts, int numClosest)
        {
            return _pts.Where(point => point != a).
                      OrderBy(point => approxDistance(a, point)).Take(numClosest).ToList();
        }

        public static Vec3D averageVectors (List<Vec3D> _pts)
        {
            return new Vec3D(
            _pts.Average(x => x.x),
            _pts.Average(x => x.y),
            _pts.Average(x => x.z));
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

            public override void run(IAgent a)
            {
                float f = getFactor(a.getPos(), pts);
                foreach (IScaledBehaviour b in behaviours)
                {
                    b.scale(f);
                }
            }

            public virtual float getFactor(Vec3D a, List<Vec3D> _pts)
            {
                Vec3D cPt = Scale.getClosestN(a, _pts, 1)[0];
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
                Vec3D avg = Scale.averageVectors(Scale.getClosestN(a, _pts, numClosest));
                float f = a.distanceTo(avg);
                return f > maxDist ? 1 : f / maxDist;
            }
        }
    }
}
