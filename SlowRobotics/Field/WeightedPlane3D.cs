using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Field
{
    public class WeightedPlane3D : Plane3D
    {
        public Vec3D wx {get;set;}
        public Vec3D wy { get; set; }
        public Vec3D wz { get; set; }

        public WeightedPlane3D()
        {
            wx = new Vec3D();
            wy = new Vec3D();
            wz = new Vec3D();
        }

        public WeightedPlane3D(Vec3D _wx, Vec3D _wy, Vec3D _wz)
        {
            wx = _wx;
            wy = _wy;
            wz = _wz;
        }

        public WeightedPlane3D(Plane3D p, float w) :base(p)
        {
            wx = xx.scale(w);
            wy = yy.scale(w);
            wz = zz.scale(w);
        }

        public static WeightedPlane3D scale(WeightedPlane3D p, float f)
        {
            return new WeightedPlane3D(
                p.wx.scale(f),
                p.wy.scale(f),
                p.wz.scale(f));
        }

        public void add(WeightedPlane3D b, float w)
        {
            wx.addSelf(b.wx.scale(w));
            wy.addSelf(b.wy.scale(w));
            wz.addSelf(b.wz.scale(w));
        }
    }
}
