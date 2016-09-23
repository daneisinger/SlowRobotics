using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Voxels
{
    public class ColourGrid : VoxelGridT<ColourVoxel>
    {
        /// <summary>
        /// Constructs a colour grid of white voxels
        /// </summary>
        /// <param name="_w"></param>
        /// <param name="_h"></param>
        /// <param name="_d"></param>
        /// <param name="_min"></param>
        /// <param name="_max"></param>
        public ColourGrid(int _w, int _h, int _d, float[] _min, float[] _max) : base(_w,_h,_d,_min, _max)
        {
            setAll(ColourVoxel.White);
        }

        public void filter(Color c, float tolerance, bool subtract)
        {
            Function((int x, int y, int z) => {
                ColourVoxel v = getValue(x, y, z);
                bool withinThreshold = compareColours(c, v, tolerance);
                if((withinThreshold && subtract) || (!withinThreshold && !subtract))
                {
                    return ColourVoxel.White;
                }
                return v;
            });
        }
        
        public bool compareColours(Color c, ColourVoxel v, float tolerance)
        {
            float dr = (float)(Math.Abs(c.R - v.R)) / 255;
            float dg = (float)(Math.Abs(c.G - v.G)) / 255;
            float db = (float)(Math.Abs(c.B - v.B)) / 255;
            return (dr < tolerance && db < tolerance && dg < tolerance);
        }

        public void paintHollowCurve(Curve profile, Curve target, int numSteps, float val, float thickness) {
            //reparametrize curves
            profile.Domain = new Interval(0, 1);
            target.Domain = new Interval(0, 1);

            for(int i = 0; i <= numSteps; i++)
            {
                float t = (float)i / numSteps;
                float radius = (float) profile.PointAt(t).Y;
                Point3d loc = target.PointAt(t);
                booleanSphere(loc, radius, ColourVoxel.Red);
            }
            for (int i = 0; i <= numSteps; i++)
            {
                float t = (float)i / numSteps;
                float radius = (float)profile.PointAt(t).Y + thickness;
                Point3d loc = target.PointAt(t);
                booleanSphere(loc, radius,ColourVoxel.White);
            }

        }

        public void booleanSphere(Point3d loc, float rad, ColourVoxel val)
        {

            int[] pt = map((float)loc.X, (float)loc.Y, (float)loc.Z);
            int gridRadius = (int)(rad / ((max[0] - min[0]) / w));

            int radSqr = gridRadius * gridRadius;
            setRegion(pt[0], pt[1], pt[2], val, gridRadius, (int i, int j, int k, ColourVoxel dVal) =>
            {
                int distSqr = (i * i) + (j * j) + (k * k);
                if (distSqr < radSqr) //sphere
                {
                    byte v = getValue(pt[0] + i, pt[1] + j, pt[2] + k).R;
                    if (v == 0)
                    {
                        set(pt[0] + i, pt[1] + j, pt[2] + k, val);
                        return true;
                    }
                }
                return false;
            });
        }

        public void rayMarchImage(Bitmap img, Surface srf, int numSteps, double depthPerStep)
        {
            srf.SetDomain(0, new Interval(0, 1));
            srf.SetDomain(1, new Interval(0, 1));

            for(int i = 0;i<img.Width; i++)
            {
                for(int j =0;j<img.Height; j++)
                {
                    double pu = (double)i / img.Width;
                    double pv = (double)j / img.Height;

                    Color c = img.GetPixel(i, j);
                    
                    Point3d p = srf.PointAt(pu, pv);
                    Vector3d v = srf.NormalAt(pu, pv);

                    for (int k = 0; k< numSteps;k++)
                    {
                        Point3d pp = p + (v * (k* depthPerStep));
                        setNearest((float) pp.X, (float)pp.Y, (float) pp.Z, new ColourVoxel(c));
                    }

                }
            }
        }

    }
}
