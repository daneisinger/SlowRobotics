﻿using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using SlowRobotics.Utils;
using SlowRobotics.Voxels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Rhino.VoxelTools
{
    public class Voxeliser
    {
        FloatGrid grid;

        public Voxeliser(FloatGrid _grid)
        {
            grid = _grid;
        }

        public void voxelisePointCharges(List<Point3d> points, float maxRad, float val)
        {
            /*
            PointOctree tree = new PointOctree(new Vec3D(grid.min), (grid.max[0]-grid.min[0])*2);
            foreach(Point3d pt in points) tree.addPoint(new Vec3D((float)pt.X,(float)pt.Y,(float)pt.Z));

            for (int i = 0; i < grid.w; i++)
            {
                for (int j = 0; j < grid.h; j++)
                {
                    for (int k = 0; k < grid.d; k++)
                    {
                        grid.setGridValue(i,j, k, getChargeAt(i, j, k, tree, maxRad, val));
                    }
                }
            }
            */
            foreach (Point3d pt in points)
            {
                int[] gridPt = grid.map((float)pt.X, (float)pt.Y, (float)pt.Z);
                int radV = (int)(maxRad / ((grid.max[0] - grid.min[0]) / grid.w));
                grid.booleanSphere(gridPt[0], gridPt[1], gridPt[2], radV,val, VoxelGridT<float>.Greater);
            }
        }

        public float getChargeAt(int x, int y, int z, PointOctree tree, float maxRad, float val)
        {
            Vec3D pt = new Vec3D(grid.mapWorld(x, y, z));
            float maxRadSquared = maxRad * maxRad;
            float voxelVal = 0;
            List<Vec3D> neighbours = tree.getPointsWithinSphere(pt, maxRad);
            if (neighbours != null)
            {
                foreach (Vec3D n in neighbours)
                {
                    float distSquared = pt.distanceToSquared(n);
                    if (distSquared < maxRadSquared) voxelVal += val * (1 - (distSquared / maxRadSquared));
                }
            }
            return voxelVal;
        }

        public void voxelisePointBlur(List<Point3d> points, int blurIterations, float val)
        {
            foreach (Point3d pt in points)
            {
                grid.setNearest((float)pt.X, (float)pt.Y, (float)pt.Z, val);
            }
            for (int i = 0; i < blurIterations; i++) grid.blur();
        }

        public static ByteGrid readRawFile(string path)
        {
            byte[] data = File.ReadAllBytes(path);
            // byte[] r = new byte[8];
            // Array.Copy(data, 8, r, 0, 8);
            int sx = BitConverter.ToInt32(data, 4);
            int sy = BitConverter.ToInt32(data, 8);
            int sz = BitConverter.ToInt32(data, 12);
            int res = BitConverter.ToInt32(data, 16);

            ByteGrid grid = new ByteGrid(sx, sy, sz, new float[] { 0, 0, 0 }, new float[] { sx, sy, sz });
            grid.Function((int x, int y, int z) => {
                return data[grid.indexAt(x,y, z)+20];
            });
            return grid;
        }

        public static void voxeliseMesh(FloatGrid grid, int wallThickness, float value, Mesh mesh)
        {
            Vector3d v1 = new Vector3d(grid.w * 4, 0, 0);
            for (int z = 1; z < grid.d; z++)
            {
                for (int y = 1; y < grid.h; y++)
                {
                    Point3d s = new Point3d(-(grid.w * 2), y - 1, z - 1);
                    Line lsct = new Line(s, v1);
                    int[] fids;
                    Point3d[] pts = Intersection.MeshLine(mesh, lsct, out fids);
                    List<Point3d> xPts = pts.OrderBy(x => x.X).ToList();

                    if (xPts.Count > 1)
                    {
                        bool f = true;
                        for (int i = 0; i < xPts.Count - 1; i += 1)
                        {

                            int current = (int)SR_Math.constrain((float)xPts[i].X, 0, grid.w - 1);
                            int next = (int)SR_Math.constrain((float)xPts[i + 1].X, 0, grid.w - 1);

                            if (f)
                            {
                                for (int x = current; x <= next; x++)
                                {
                                    grid.set(x, y, z, value);
                                }
                            }
                            f = !f;

                        }
                    }
                }
            }
        }
    }
}
