using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SlowRobotics.Voxels
{
    public class ColourGrid : VoxelGrid<Color>
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
            initGrid(Color.White);
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
                        setValue((float) pp.X, (float)pp.Y, (float) pp.Z,c);
                    }

                }
            }
        }

    }
}
