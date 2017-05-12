using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toxiclibs.core;

namespace SlowRobotics.Spatial
{
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

        public IEnumerable<Vec3D> Search(Vec3D pt, float radius)
        {
            return allPts.Where(p => p.distanceTo(pt) < radius);
        }

        public void Update(IEnumerable<Vec3D> pts)
        {
            allPts = pts.ToList();
        }
    }
}
