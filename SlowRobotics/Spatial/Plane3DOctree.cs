using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Spatial
{
    public class Plane3DOctree : ISearchable
    {
        private PointOctree tree;
        private Vec3D origin;
        private float extents;

        public Plane3DOctree(Vec3D o, float d) {
            tree = new PointOctree(o, d);
            origin = o;
            extents = d;
        }

        public List<Vec3D> Search(Vec3D pt, float radius)
        {
            return tree.getPointsWithinSphere(pt, radius);
        }

        public void Add(Vec3D pt)
        {
            tree.addPoint(pt);
        }

        public void Update(IEnumerable<Vec3D> pts)
        {
            tree = new PointOctree(origin, extents);
            tree.addAll(pts.ToList());
        }

        public void AddAll(List<Vec3D> pts)
        {
            tree.addAll(pts);
        }
    }
}
