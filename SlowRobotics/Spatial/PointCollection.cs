using SlowRobotics.SRMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toxiclibs.core;

namespace SlowRobotics.Spatial
{
    /// <summary>
    /// Brute force nearest neighbour search
    /// </summary>
    public class PointCollection : ISearchable
    {
        public List<Vec3D> allPts;

        public PointCollection()
        {
            allPts = new List<Vec3D>();
        }

        public void Add(Vec3D pt)
        {
            allPts.Add(pt);
        }

        public IEnumerable<Vec3D> Search(Vec3D pt, float radius, int maxPoints)
        {
            List<Vec3D> pts = allPts.Where(p => p.distanceTo(pt) < radius).ToList();
            pts.Sort(delegate (Vec3D x, Vec3D y)
            {
                return x.sub(pt).CompareTo(y.sub(pt));
            });
            return pts.Take(maxPoints);
        }

        public void Update(IEnumerable<Vec3D> pts)
        {
            allPts = pts.ToList();
        }
    }
}
