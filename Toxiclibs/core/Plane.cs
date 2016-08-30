using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toxiclibs.core
{
    public class ToxiPlane : Vec3D , Shape3D
    {

    /**
     * Classifier constant for {@link Plane#classifyPoint(ReadonlyVec3D, float)}
     */
    public enum Classifier
    {
        FRONT,
        BACK,
        ON_PLANE
    }

    public static ToxiPlane XY = new ToxiPlane(new Vec3D(), Vec3D.Z_AXIS);
    public static ToxiPlane XZ = new ToxiPlane(new Vec3D(), Vec3D.Y_AXIS);
    public static ToxiPlane YZ = new ToxiPlane(new Vec3D(), Vec3D.X_AXIS);

    public Vec3D normal;

    public ToxiPlane() : base()
    {
        normal = Vec3D.Y_AXIS.copy();
    }

    public ToxiPlane(ReadonlyVec3D origin, ReadonlyVec3D norm) : base(origin)
    {
        normal = norm.getNormalized();
    }

    public ToxiPlane(Triangle3D t) : this(t.computeCentroid(), t.computeNormal())
        {
    }

    /**
     * Classifies the relative position of the given point to the plane using
     * the given tolerance.
     * 
     * @return One of the 3 classification types: FRONT, BACK, ON_PLANE
     */
    public Classifier classifyPoint(ReadonlyVec3D p, float tolerance)
    {
        float d = this.sub(p).normalize().dot(normal);
        if (d < -tolerance)
        {
            return Classifier.FRONT;
        }
        else if (d > tolerance)
        {
            return Classifier.BACK;
        }
        return Classifier.ON_PLANE;
    }

    public bool containsPoint(ReadonlyVec3D p)
    {
        return classifyPoint(p, (float)Math.E) == Classifier.ON_PLANE;
    }

    public float getDCoeff()
    {
        return this.dot(normal);
    }

    /**
     * Calculates distance from the plane to point P.
     * 
     * @param p
     * @return distance
     */
    public float getDistanceToPoint(Vec3D p)
    {
        float sn = -normal.dot(p.sub(this));
        float sd = normal.magSquared();
        Vec3D isec = p.add(normal.scale(sn / sd));
        return isec.distanceTo(p);
    }

    /**
     * Calculates the intersection point between plane and ray (line).
     * 
     * @param r
     * @return intersection point or null if ray doesn't intersect plane
     */
     
    public ReadonlyVec3D getIntersectionWithRay(Ray3D r)
    {
        float denom = normal.dot(r.getDirection());
        if (denom > Math.E)
        {
            float u = normal.dot(this.sub(r)) / denom;
            return r.getPointAtDistance(u);
        }
        else {
            return null;
        }
    }
    
    public Vec3D getProjectedPoint(Vec3D p)
    {
        Vec3D dir;
        if (normal.dot(sub(p)) < 0)
        {
            dir = normal.getInverted();
        }
        else {
            dir = normal;
        }
        Vec3D proj = new Ray3D(p, dir)
                .getPointAtDistance(getDistanceToPoint(p));
        return proj;
    }

    /**
     * Calculates the distance of the vector to the given plane in the specified
     * direction. A plane is specified by a 3D point and a normal vector
     * perpendicular to the plane. Normalized directional vectors expected (for
     * rayDir and planeNormal).
     * 
     * @param ray
     *            intersection ray
     * @return distance to plane in world units, -1 if no intersection.
     */
    public float intersectRayDistance(Ray3D ray)
    {
        float d = -normal.dot(this);
        float numer = normal.dot(ray) + d;
        float denom = normal.dot(ray.dir);

        // normal is orthogonal to vector, cant intersect
        if (Math.Abs(denom) < Math.E)
        {
            return -1;
        }

        return -(numer / denom);
    }

    /**
     * Computes the intersection ray between this plane and the given one. If
     * the planes are parallel or coincident the method returns null. If the
     * planes are intersecting, the returned {@link Ray3D} will start at a point
     * lying on both planes and point along the infinite intersection line
     * between them.
     * 
     * Code ported from:
     * http://forums.create.msdn.com/forums/p/39074/234178.aspx#234178
     * 
     * @param plane
     *            intersection partner
     * @return intersection ray or null
     */
    public Ray3D intersectsPlane(ToxiPlane plane)
    {
        float d = getDCoeff();
        float d2 = plane.getDCoeff();

        if (normal.equalsWithTolerance(plane.normal, 0.0001f) || d == d2)
        {
            return null;
        }

        float offDiagonal = normal.dot(plane.normal);
        double det = 1.0 / (1 - offDiagonal * offDiagonal);
        double a = (d - d2 * offDiagonal) * det;
        double b = (d2 - d * offDiagonal) * det;
        Vec3D anchor = normal.scale((float)a).addSelf(
                plane.normal.scale((float)b));
        Vec3D dir = normal.cross(plane.normal);

        return new Ray3D(anchor, dir);
    }

    /**
     * Creates a TriangleMesh representation of the plane as a finite, squared
     * quad of the requested size, centred around the current plane point.
     * 
     * @param size
     *            desired edge length
     * @return mesh
     */
     /*
    public Mesh3D toMesh(float size)
    {
        return toMesh(null, size);
    }

    public Mesh3D toMesh(Mesh3D mesh, float size)
    {
        if (mesh == null)
        {
            mesh = new TriangleMesh("plane", 4, 2);
        }
        ReadonlyVec3D p = equalsWithTolerance(Vec3D.ZERO, 0.01f) ? add(0.01f,
                0.01f, 0.01f) : this;
        size *= 0.5f;
        Vec3D n = p.cross(normal).normalizeTo(size);
        Vec3D m = n.cross(normal).normalizeTo(size);
        Vec3D a = this.add(n).addSelf(m);
        Vec3D b = this.add(n).subSelf(m);
        Vec3D c = this.sub(n).subSelf(m);
        Vec3D d = this.sub(n).addSelf(m);
        mesh.addFace(a, d, b, null, null, null, null);
        mesh.addFace(b, d, c, null, null, null, null);
        return mesh;
    }
    */
    public String toString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("origin: ").Append(base.toString()).Append(" norm: ")
                .Append(normal.toString());
        return sb.ToString();
    }
}
}
