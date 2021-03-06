﻿using KDTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toxiclibs.core;

namespace SlowRobotics.Spatial
{
    /// <summary>
    /// Wrapper for KDTree class that implementes ISearchable interface
    /// </summary>
    public class Plane3DKDTree : ISearchable
    {
        private KDTree<Vec3D> tree;
        private List<Vec3D> ptRefs; //TODO - rewrite kdtree to give access to point collection

        public Plane3DKDTree()
        {
            tree = new KDTree<Vec3D>(3);
            ptRefs = new List<Vec3D>();
        }

        public IEnumerable<Vec3D> Collection
        {
            get
            {
                return ptRefs;
            }
        }

        public IEnumerable<Vec3D> Search(Vec3D pt, float radius, int maxPoints)
        {
            NearestNeighbour<Vec3D> n = tree.NearestNeighbors(new double[] { pt.x, pt.y, pt.z }, maxPoints, radius * radius);
            return n.ToList();
        }

        public void Add(Vec3D pt)
        {
            tree.AddPoint(new double[] { pt.x, pt.y, pt.z }, pt);
            ptRefs.Add(pt);
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
