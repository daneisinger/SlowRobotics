using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toxiclibs.core;

namespace SlowRobotics.SRMath
{
    /// <summary>
    /// Convenience class for common math functions
    /// </summary>
    public static class MathUtils
    {

        /// <summary>
        /// Constrain a value to a bounds
        /// </summary>
        /// <param name="v">Value to constrain</param>
        /// <param name="min">Bound minimum</param>
        /// <param name="max">Bound maximum</param>
        /// <returns></returns>
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

        /// <summary>
        /// Map a value to a domain
        /// </summary>
        /// <param name="v">Value to map</param>
        /// <param name="min">Reference domain minimum</param>
        /// <param name="max">Reference domain maximum</param>
        /// <param name="min2">Target domain minimum</param>
        /// <param name="max2">Taget domain maximum</param>
        /// <returns></returns>
        public static float map(float v, float min, float max, float min2, float max2)
        {
            return min2 + ((v / (max - min)) * (max2 - min2));
        }

        /// <summary>
        /// Returns the value at a parameter within a domain
        /// </summary>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <param name="f">parameter</param>
        /// <returns></returns>
        public static float lerp(float min, float max, float f)
        {
            return min + (max - min) * f;
        }

        public static float lerp(int min, int max, float f)
        {
            return lerp((float)min, (float)max, f);
        }

        /// <summary>
        /// Returns squared distance between two points
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static float approxDistance(Vec3D source, Vec3D target)
        {
            float dx = target.x - source.x;
            float dz = target.y - source.y;
            float dy = target.z - source.z;

            return (dx * dx) + (dy * dy) + (dz * dz);
        }

        /// <summary>
        /// Returns closest n points in a collection
        /// </summary>
        /// <param name="a">Point to search from</param>
        /// <param name="_pts">Points to search</param>
        /// <param name="numClosest">Number of points to return</param>
        /// <returns></returns>
        public static List<Vec3D> getClosestN(Vec3D a, List<Vec3D> _pts, int numClosest)
        {
            return _pts.Where(point => point != a).
                      OrderBy(point => approxDistance(a, point)).Take(numClosest).ToList();
        }

        /// <summary>
        /// Gets average position from a list of points
        /// </summary>
        /// <param name="_pts">Points to average</param>
        /// <returns></returns>
        public static Vec3D averageVectors(List<Vec3D> _pts)
        {
            return new Vec3D(
            _pts.Average(x => x.x),
            _pts.Average(x => x.y),
            _pts.Average(x => x.z));
        }

        /// <summary>
        /// Gets plane of best fit for a collection of points
        /// </summary>
        /// <param name="planeVerts">Collection of points</param>
        /// <param name="centroid">Plane origin</param>
        /// <returns></returns>
        public static Vec3D getPlaneNormal(List<Vec3D> planeVerts, out Vec3D centroid)
        {
            centroid = averageVectors(planeVerts);
            double[,] dataMat = new double[3, planeVerts.Count];
            int i = 0;

            foreach (Vec3D v in planeVerts)
            {
                dataMat[0, i] = v.x - centroid.x;
                dataMat[1, i] = v.y - centroid.y;
                dataMat[2, i] = v.z - centroid.z;
                i++;
            }

            double[] w = new double[3];
            double[,] u = new double[3, 3];
            double[,] t = new double[3, 3];

            bool a = alglib.svd.rmatrixsvd(dataMat, 3, planeVerts.Count, 1, 0, 2, ref w, ref u, ref t);

            return (a) ? new Vec3D((float)u[2, 0], (float)u[2, 1], (float)u[2, 2]) : null;

        }

        /// <summary>
        /// Returns the point on line 1 that is closest to line 2
        /// </summary>
        /// <param name="l1">First line</param>
        /// <param name="l2">Second line</param>
        /// <returns></returns>
        public static Vec3D closestPoint(ILine l1, ILine l2)
        {
            // Algorithm is ported from the C algorithm of Paul Bourke
            Vec3D p1 = l1.start;
            Vec3D p2 = l1.end;
            Vec3D p3 = l2.start;
            Vec3D p4 = l2.end;
            Vec3D p21 = p2.sub(p1);
            Vec3D p13 = p1.sub(p3);
            Vec3D p43 = p4.sub(p3);
            double d1343 = p13.x * (double)p43.x + (double)p13.y * p43.y + (double)p13.z * p43.z;
            double d4321 = p43.x * (double)p21.x + (double)p43.y * p21.y + (double)p43.z * p21.z;
            double d1321 = p13.x * (double)p21.x + (double)p13.y * p21.y + (double)p13.z * p21.z;
            double d4343 = p43.x * (double)p43.x + (double)p43.y * p43.y + (double)p43.z * p43.z;
            double d2121 = p21.x * (double)p21.x + (double)p21.y * p21.y + (double)p21.z * p21.z;

            double denom = d2121 * d4343 - d4321 * d4321;
            double numer = d1343 * d4321 - d1321 * d4343;

            float mua = Math.Max(Math.Min((float)(numer / denom), 1), 0);
            float mub = Math.Max(Math.Min((float)((d1343 + d4321 * (mua)) / d4343), 1), 0);
            return l2.pointAt(mub);
        }
    }
}
