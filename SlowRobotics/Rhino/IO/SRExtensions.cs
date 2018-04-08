using Rhino.Geometry;
using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toxiclibs.core;

namespace SlowRobotics.Rhino.IO
{
    /// <summary>
    /// Extension methods for converting between Nursery and Rhino types
    /// </summary>
    public static class SRExtensions
    {
        private static Point3d _ToPoint3d(Vec3D p)
        {
            return new Point3d(p.x, p.y, p.z);
        }
        private static Vector3d _ToVector3d(Vec3D p)
        {
            return new Vector3d(p.x, p.y, p.z);
        }

        private static Plane _ToPlane(Plane3D p)
        {
            Point3d origin = _ToPoint3d(p);
            Vector3d xx = new Vector3d(p.xx.x, p.xx.y, p.xx.z);
            Vector3d yy = new Vector3d(p.yy.x, p.yy.y, p.yy.z);
            return new Plane(origin, xx, yy);
        }

        public static Plane ToPlane(this SRParticle p)
        {
            return _ToPlane(p);
        }

        public static Point3d ToPoint3d(this SRParticle p)
        {
            return _ToPoint3d(p);
        }

        public static Vector3d ToVector3d(this SRParticle p)
        {
            return _ToVector3d(p);
        }

        public static Plane ToPlane(this Plane3D p)
        {
            return _ToPlane(p);
        }

        public static Point3d ToPoint3d(this Plane3D p)
        {
            return _ToPoint3d(p);
        }

        public static Vector3d ToVector3d(this Plane3D p)
        {
            return _ToVector3d(p);
        }

        public static Point3d ToPoint3d(this Vec3D p)
        {
            return _ToPoint3d(p);
        }

        public static Vector3d ToVector3d(this Vec3D p)
        {
            return _ToVector3d(p);
        }

        public static Line ToLine(this SRLinearParticle l)
        {
            return new Line(l.start.ToPoint3d(), l.end.ToPoint3d());
        }

        public static Line ToLine(this Spring s)
        {
            return new Line(s.a.Geometry.ToPoint3d(), s.b.Geometry.ToPoint3d());
        }

        public static Line ToLine(this ILine l)
        {
            return new Line(l.start.ToPoint3d(), l.end.ToPoint3d());
        }

        public static Line3D ToLine3D(this Line l)
        {
            return new Line3D(l.From.ToVec3D(), l.To.ToVec3D());
        }

        public static Plane3D ToPlane3D(this Plane p)
        {
            return new Plane3D(new Vec3D((float)p.Origin.X, (float)p.Origin.Y, (float)p.Origin.Z),
                         new Vec3D((float)p.XAxis.X, (float)p.XAxis.Y, (float)p.XAxis.Z),
                         new Vec3D((float)p.YAxis.X, (float)p.YAxis.Y, (float)p.YAxis.Z));
        }

        public static Vec3D ToVec3D(this Point3f p)
        {
            return new Vec3D(p.X, p.Y, p.Z);
        }

        public static Vec3D ToVec3D(this Vector3d p)
        {
            return new Vec3D((float)p.X, (float)p.Y, (float)p.Z);
        }

        public static Vec3D ToVec3D(this Point3d p)
        {
            return new Vec3D((float)p.X, (float)p.Y, (float)p.Z);
        }

        public static AABB ToAABB(this Box b)
        {
            Vec3D min = ToVec3D(b.PointAt(0,0,0));
            Vec3D max = ToVec3D(b.PointAt(1,1,1));
            return AABB.fromMinMax(min, max);
        }


    }
}
