using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Core
{
    public class Plane3D : Particle
    {
        public Vec3D xx;
        public Vec3D zz;
        public Vec3D yy;
        int index = -1;

        protected ExponentialInterpolation interp = new ExponentialInterpolation(2);

        //TriangleMesh mesh;

        public Plane3D(Vec3D _origin, Vec3D _x, Vec3D _y) :base(_origin)
        {
            xx = _x.getNormalized();
            yy = _y.getNormalized();
            zz = xx.cross(yy);
            zz.normalize();
        }

        public Plane3D(Vec3D _origin) : this(_origin, new Vec3D(1, 0, 0))
        {

        }

        protected Plane3D(Vec3D _origin, Vec3D _x) : this(_origin, _x, _x.cross(new Vec3D(0, 0, 1)))
        {  
        }

        public Plane3D(Plane3D _j) :base(_j)
        {
            xx = _j.xx.copy().normalize();
            zz = _j.zz.copy().normalize();
            yy = _j.yy.copy().normalize();
        }

        //alignment function

        public void interpolateToPlane3D(Plane3D j, float maxDist, float maxAlign)
        {
            Quaternion interpToZ = Quaternion.getAlignmentQuat(j.zz, zz);
            Quaternion interpToX = Quaternion.getAlignmentQuat(j.xx, xx);

            Vec3D toPlane3D = j.sub(this);
            float ratio = toPlane3D.magnitude() / maxDist;
            float f = interp.interpolate(0, maxAlign, ratio);

            Quaternion transformMatrixZ = new Quaternion().interpolateToSelf(interpToZ, f);
            Quaternion transformMatrixX = new Quaternion().interpolateToSelf(interpToX, f);
            transform(transformMatrixZ.toMatrix4x4());
            transform(transformMatrixX.toMatrix4x4());
        }

        //interpolates the Plane3D towards a vector - I use this to fit the Plane3D to a best fit plane
        //this means if the points already form an approximate surface then they will conform to it, 
        //rather than occasionally resulting in complex noisy surfaces

        public void interpolateToZZ(Vec3D dir, float amt)
        {
            Quaternion interpTo = Quaternion.getAlignmentQuat(dir, zz);
            Quaternion transformMatrix = new Quaternion().interpolateToSelf(interpTo, amt);
            transform(transformMatrix.toMatrix4x4());
        }

        public Matrix4x4 interpolateToXX(Vec3D dir, float amt)
        {
            Quaternion interpTo = Quaternion.getAlignmentQuat(dir, xx);
            Quaternion transformMatrix = new Quaternion().interpolateToSelf(interpTo, amt);
            transform(transformMatrix.toMatrix4x4());
            return transformMatrix.toMatrix4x4();
        }

        public Matrix4x4 interpolateToYY(Vec3D dir, float amt)
        {
            Quaternion interpTo = Quaternion.getAlignmentQuat(dir, yy);
            Quaternion transformMatrix = new Quaternion().interpolateToSelf(interpTo, amt);
            transform(transformMatrix.toMatrix4x4());
            return transformMatrix.toMatrix4x4();
        }

        public void alignOriginWithXX(Plane3D b, float amt)
        {
            Vec3D ab = b.sub(this); //vector between two planes
            Quaternion interpTo = Quaternion.getAlignmentQuat(ab.getNormalized(), b.xx);
            Quaternion transformMatrix = new Quaternion().interpolateToSelf(interpTo, amt);
            transformOrigin(transformMatrix.toMatrix4x4());
        }

        public Vec3D alignOriginWithXX(Vec3D o, Vec3D dir, float amt)
        {
            Vec3D ab = o.sub(this); //vector between two planes
            Quaternion interpTo = Quaternion.getAlignmentQuat(ab.getNormalized(), dir);
            Quaternion transformMatrix = new Quaternion().interpolateToSelf(interpTo, amt);
            return transformOrigin(transformMatrix.toMatrix4x4());
        }

        public void rotateNormal(float r)
        {
            zz.rotateAroundAxis(xx, r);
            yy.rotateAroundAxis(xx, r);
        }


        public Matrix4x4 getMatrix()
        {
            Matrix4x4 j = new Matrix4x4(
            xx.x, xx.y, xx.z, 0,
            yy.x, yy.y, yy.z, 0,
            zz.x, zz.y, zz.z, 0,
            0, 0, 0, 1);

            return j;
        }

        public void transform(Matrix4x4 t)
        {
            xx = t.applyTo(xx).normalize();
            zz = t.applyTo(zz).normalize();
            yy = t.applyTo(yy).normalize();
            //mesh.transform(t);
        }

        public Vec3D transformOrigin(Matrix4x4 t)
        {
            Vec3D to = t.applyTo(copy());
            return sub(to);
        }
    }
}
