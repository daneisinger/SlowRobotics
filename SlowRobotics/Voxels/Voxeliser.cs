using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Voxels
{
    public class Voxeliser
    {
        VoxelGrid grid;

        public Voxeliser(VoxelGrid _grid)
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
            foreach(Point3d pt in points)
            {
                grid.setWithRadius((float)pt.X, (float)pt.Y, (float)pt.Z, val, maxRad);
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
                    if (distSquared < maxRadSquared) voxelVal += val*(1-(distSquared / maxRadSquared));
                }
            }
            return voxelVal;
        }

        public void voxelisePointBlur(List<Point3d> points, int blurIterations, float val)
        {
            foreach(Point3d pt in points)
            {
                grid.setValue((float) pt.X, (float) pt.Y, (float)pt.Z, val);
            }
            for(int i=0;i< blurIterations;i++) grid.blur();
        }
    }
}
