using KDTree;
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

    public class Plane3DKDTree : ISearchable
    {
        private KDTree<Vec3D> tree;

        public Plane3DKDTree()
        {
            tree = new KDTree<Vec3D>(3);
        }

        public List<Vec3D> Search(Vec3D pt, float radius)
        {
            return tree.NearestNeighbors(new double[] { pt.x, pt.y, pt.z }, 1024, radius * radius).ToList();
        }

        public void Add(Vec3D pt)
        {
            tree.AddPoint(new double[] { pt.x, pt.y, pt.z }, pt);
        }

        public void Update(IEnumerable<Vec3D> pts)
        {
            tree = new KDTree<Vec3D>(3);
            foreach (Vec3D v in pts)
            {
                Add(v);
            }
        }

        public void AddAll(List<Vec3D> pts)
        {
            foreach (Vec3D v in pts)
            {
                Add(v);
            }
        }
    }
}
