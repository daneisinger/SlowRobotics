using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Utils
{
    public static class SR_Math
    {

        public static float constrain(float v, float min, float max)
        {
            if (v < min)
            {
                return min;
            }
            else if (v > max)
            {
                return max;
            }
            else return v;
        }

        public static float map(float v, float min, float max, float min2, float max2)
        {
            return min2 + ((v / (max - min)) * (max2 - min2));
        }

        public static float lerp(float min, float max, float f)
        {
            return min + (max - min) * f;
        }

        public static float lerp(int min, int max, float f)
        {
            return lerp((float)min, (float)max, f);
        }

        public static float approxDistance(Vec3D source, Vec3D target)
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

        public static Vec3D averageVectors(List<Vec3D> _pts)
        {
            return new Vec3D(
            _pts.Average(x => x.x),
            _pts.Average(x => x.y),
            _pts.Average(x => x.z));
        }

        public static float normalizeDistance(Vec3D ab, float minDist, float maxDist, float maxForce, InterpolateStrategy interpolator)
        {
            float dist = ab.magnitude();
            float sf = 0;
            if (dist > minDist && dist < maxDist)
            {
                float f = (dist - minDist) / (maxDist - minDist);
                sf = interpolator.interpolate(0, maxForce, f);
            }
            return sf;
        }
    }
}
