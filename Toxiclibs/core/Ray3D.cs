using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toxiclibs.core
{
    public class Ray3D : Vec3D
    {

    public Vec3D dir;

    public Ray3D() :base()
    {
        dir = Vec3D.Y_AXIS.copy();
    }

    public Ray3D(float x, float y, float z, ReadonlyVec3D d) :base(x, y, z)
    {
        dir = d.getNormalized();
    }

    public Ray3D(ReadonlyVec3D o, ReadonlyVec3D d) :this(o.getX(), o.getY(), o.getZ(), d)
    {
    }

    /**
     * Returns a copy of the ray's direction vector.
     * 
     * @return vector
     */
    public Vec3D getDirection()
    {
        return dir.copy();
    }

    /**
     * Calculates the distance between the given point and the infinite line
     * coinciding with this ray.
     * 
     * @param p
     * @return distance
     */
    public float getDistanceToPoint(Vec3D p)
    {
        Vec3D sp = p.sub(this);
        return sp.distanceTo(dir.scale(sp.dot(dir)));
    }

    /**
     * Returns the point at the given distance on the ray. The distance can be
     * any real number.
     * 
     * @param dist
     * @return vector
     */
    public Vec3D getPointAtDistance(float dist)
    {
        return add(dir.scale(dist));
    }

    /**
     * Uses a normalized copy of the given vector as the ray direction.
     * 
     * @param d
     *            new direction
     * @return itself
     */
    public Ray3D setDirection(ReadonlyVec3D d)
    {
        dir.set(d).normalize();
        return this;
    }

    public Ray3D setNormalizedDirection(ReadonlyVec3D d)
    {
        dir.set(d);
        return this;
    }

    /**
     * Converts the ray into a 3D Line segment with its start point coinciding
     * with the ray origin and its other end point at the given distance along
     * the ray.
     * 
     * @param dist
     *            end point distance
     * @return line segment
     */
    public Line3D toLine3DWithPointAtDistance(float dist)
    {
        return new Line3D(this, getPointAtDistance(dist));
    }

    public String toString()
    {
        return "origin: " + base.toString() + " dir: " + dir;
    }
}
}
