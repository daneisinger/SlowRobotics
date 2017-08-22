using SlowRobotics.SRMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Core
{
    /// <summary>
    /// Plane implementation. Adds x,y,z axis to Toxiclibs Vec3D class. Provides methods for transforming planes while
    /// maintaining normalized and perpendicular axis vectors.
    /// </summary>
    public class Plane3D : Vec3D
    {

        public Vec3D xx;
        public Vec3D zz;
        public Vec3D yy;

        /// <summary>
        /// Default constructor creates a plane at the origin
        /// </summary>
        public Plane3D() : this(new Vec3D()) { }

        /// <summary>
        /// Constructs a plane at a given origin with x axis aligned to world x
        /// </summary>
        /// <param name="_origin">Plane Origin</param>
        public Plane3D(Vec3D _origin) : this(_origin, new Vec3D(1, 0, 0))
        {
        }

        /// <summary>
        /// Constructs a plane at a given origin and with a given x axis. Y axis is determined by
        /// crossing xaxis with world z vector
        /// </summary>
        /// <param name="_origin">Plane origin</param>
        /// <param name="_x">X Axis</param>
        protected Plane3D(Vec3D _origin, Vec3D _x) : this(_origin, _x, _x.cross(new Vec3D(0, 0, 1)))
        {
        }

        /// <summary>
        /// Constructs a plane from origin, x and y axis
        /// </summary>
        /// <param name="_origin">Plane origin</param>
        /// <param name="_x">X Axis</param>
        /// <param name="_y">Y Axis</param>
        public Plane3D(Vec3D _origin, Vec3D _x, Vec3D _y) :base(_origin)
        {
            xx = _x.getNormalized();
            yy = _y.getNormalized();
            zz = xx.cross(yy);
            zz.normalize();
        }

        /// <summary>
        /// Creates a copy of a plane
        /// </summary>
        /// <param name="_p"></param>
        public Plane3D(Plane3D _p) :base(_p)
        {
            xx = _p.xx.copy().normalize();
            zz = _p.zz.copy().normalize();
            yy = _p.yy.copy().normalize();
        }
        
        public bool Equals(Plane3D vv)
        {
            try
            {
                return (base.Equals(vv) && zz.x == vv.zz.x && zz.y == vv.zz.y && zz.z == vv.zz.z);
            }
            catch (NullReferenceException e)
            {
                return false;
            }
        }

        /// <summary>
        /// Linearly interpolates this plane to a target plane by a given interpolation factor
        /// </summary>
        /// <param name="p">Target plane</param>
        /// <param name="factor">Interpolation factor</param>
        public void interpolateToPlane3D(Plane3D p, float factor)
        {
            Quaternion interpToZ = Quaternion.getAlignmentQuat(p.zz, zz);
            Quaternion interpToX = Quaternion.getAlignmentQuat(p.xx, xx);
            Quaternion transformMatrixZ = new Quaternion().interpolateToSelf(interpToZ, factor);
            Quaternion transformMatrixX = new Quaternion().interpolateToSelf(interpToX, factor);
            transform(transformMatrixZ.toMatrix4x4());
            transform(transformMatrixX.toMatrix4x4());
        }

        /// <summary>
        /// Linearly interpolates plane z axis to a given guide vector
        /// </summary>
        /// <param name="dir">Guide vector</param>
        /// <param name="amt">Interpolation factor</param>
        public void interpolateToZZ(Vec3D dir, float amt)
        {
            Quaternion interpTo = Quaternion.getAlignmentQuat(dir, zz);
            Quaternion transformMatrix = new Quaternion().interpolateToSelf(interpTo, amt);
            transform(transformMatrix.toMatrix4x4());
        }

        /// <summary>
        /// Linearly interpolates plane x axis to a given guide vector
        /// </summary>
        /// <param name="dir">Guide vector</param>
        /// <param name="amt">Interpolation factor</param>
        /// <returns></returns>
        public Matrix4x4 interpolateToXX(Vec3D dir, float amt)
        {
            Quaternion interpTo = Quaternion.getAlignmentQuat(dir, xx);
            Quaternion transformMatrix = new Quaternion().interpolateToSelf(interpTo, amt);
            transform(transformMatrix.toMatrix4x4());
            return transformMatrix.toMatrix4x4();
        }

        /// <summary>
        /// Linearly interpolates plane y axis to a given guide vector
        /// </summary>
        /// <param name="dir">Guide vector</param>
        /// <param name="amt">Interpolation factor</param>
        /// <returns></returns>
        public Matrix4x4 interpolateToYY(Vec3D dir, float amt)
        {
            Quaternion interpTo = Quaternion.getAlignmentQuat(dir, yy);
            Quaternion transformMatrix = new Quaternion().interpolateToSelf(interpTo, amt);
            transform(transformMatrix.toMatrix4x4());
            return transformMatrix.toMatrix4x4();
        }

        /// <summary>
        /// Rotate the plane around its x axis
        /// </summary>
        /// <param name="r"></param>
        public void rotateNormal(float r)
        {
            zz.rotateAroundAxis(xx, r);
            yy.rotateAroundAxis(xx, r);
        }

        /// <summary>
        /// Get plane in matrix format
        /// </summary>
        /// <returns></returns>
        public Matrix4x4 getMatrix()
        {
            Matrix4x4 j = new Matrix4x4(
            xx.x, xx.y, xx.z, 0,
            yy.x, yy.y, yy.z, 0,
            zz.x, zz.y, zz.z, 0,
            0, 0, 0, 1);

            return j;
        }

        /// <summary>
        /// Transform plane with a given matrix
        /// </summary>
        /// <param name="t">Transformation matrix</param>
        public virtual void transform(Matrix4x4 t)
        {
            
            xx = t.applyTo(xx).normalize();
            zz = t.applyTo(zz).normalize();
            yy = t.applyTo(yy).normalize();
        }

        /// <summary>
        /// transform plane origin with a given matrix
        /// </summary>
        /// <param name="t">Transformation matrix</param>
        /// <returns></returns>
        public Vec3D transformOrigin(Matrix4x4 t)
        {
            Vec3D to = t.applyTo(copy());
            return sub(to);
        }

        /// <summary>
        /// Copy a plane
        /// </summary>
        /// <param name="p">Plane to copy</param>
        public void set(Plane3D p)
        {
            set((Vec3D)p);
            xx = p.xx.copy();
            yy = p.yy.copy();
            zz = p.zz.copy();
        }

        public override string ToString()
        {
            return string.Format("Plane3D:O {0},{1},{2}  Z {3},{4},{5}", x, y, z,zz.x,zz.y,zz.z);
        }
    }
}
