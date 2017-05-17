using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toxiclibs.core
{
    //Implementation of Toxiclibs core classes in C#
    //src here: https://bitbucket.org/postspectacular/toxiclibs/src/

    public class Vec3D : IComparable<ReadonlyVec3D>, ReadonlyVec3D {
        
        public class Axis
        {

            public static readonly Axis X = new Axis(X_AXIS);
            public static readonly Axis Y = new Axis(Y_AXIS);
            public static readonly Axis Z = new Axis(Z_AXIS);

            private ReadonlyVec3D vector;

            private Axis(ReadonlyVec3D v)
            {
                this.vector = v;
            }
            public ReadonlyVec3D getVector()
            {
                return vector;
            }

        }


    /** Defines positive X axis. */
    public static readonly ReadonlyVec3D X_AXIS = new Vec3D(1, 0, 0);

    /** Defines positive Y axis. */
    public static readonly ReadonlyVec3D Y_AXIS = new Vec3D(0, 1, 0);

    /** Defines positive Z axis. */
    public static readonly ReadonlyVec3D Z_AXIS = new Vec3D(0, 0, 1);

    /** Defines the zero vector. */
    public static readonly ReadonlyVec3D ZERO = new Vec3D();

        /**
         * Defines vector with all coords set to float.MinValue. Useful for
         * bounding box operations.
         */
        public static readonly ReadonlyVec3D MIN_VALUE = new Vec3D(float.MinValue,
                float.MinValue, float.MinValue);

    /**
     * Defines vector with all coords set to float.MaxValue. Useful for
     * bounding box operations.
     */
    public static readonly ReadonlyVec3D MAX_VALUE = new Vec3D(float.MaxValue,
            float.MaxValue, float.MaxValue);

    public static readonly ReadonlyVec3D NEG_MAX_VALUE = new Vec3D(
                -float.MaxValue, -float.MaxValue, -float.MaxValue);

    /**
     * Creates a new vector from the given angle in the XY plane. The Z
     * component of the vector will be zero.
     * 
     * The resulting vector for theta=0 is equal to the positive X axis.
     * 
     * @param theta
     *            the theta
     * 
     * @return new vector in the XY plane
     */
    public static Vec3D fromXYTheta(float theta)
    {
        return new Vec3D((float)Math.Cos(theta), (float)Math.Sin(theta), 0);
    }

    /**
     * Creates a new vector from the given angle in the XZ plane. The Y
     * component of the vector will be zero.
     * 
     * The resulting vector for theta=0 is equal to the positive X axis.
     * 
     * @param theta
     *            the theta
     * 
     * @return new vector in the XZ plane
     */
    public static Vec3D fromXZTheta(float theta)
    {
        return new Vec3D((float)Math.Cos(theta), 0, (float)Math.Sin(theta));
    }

    /**
     * Creates a new vector from the given angle in the YZ plane. The X
     * component of the vector will be zero.
     * 
     * The resulting vector for theta=0 is equal to the positive Y axis.
     * 
     * @param theta
     *            the theta
     * 
     * @return new vector in the YZ plane
     */
    public static Vec3D fromYZTheta(float theta)
    {
        return new Vec3D(0, (float)Math.Cos(theta), (float)Math.Sin(theta));
    }

    /**
     * Constructs a new vector consisting of the largest components of both
     * vectors.
     * 
     * @param b
     *            the b
     * @param a
     *            the a
     * 
     * @return result as new vector
     */
    public static Vec3D max(ReadonlyVec3D a, ReadonlyVec3D b)
    {
        return new Vec3D(Math.Max(a.getX(), b.getX()), Math.Max(a.getY(),
                b.getY()), Math.Max(a.getZ(), b.getZ()));
    }

    /**
     * Constructs a new vector consisting of the smallest components of both
     * vectors.
     * 
     * @param b
     *            comparing vector
     * @param a
     *            the a
     * 
     * @return result as new vector
     */
    public static Vec3D min(ReadonlyVec3D a, ReadonlyVec3D b)
    {
        return new Vec3D(Math.Min(a.getX(), b.getX()), Math.Min(a.getY(),
                b.getY()), Math.Min(a.getZ(), b.getZ()));
    }

    /**
     * Static factory method. Creates a new random unit vector using the Random
     * implementation set as default for the {@link MathUtils} class.
     * 
     * @return a new random normalized unit vector.
     */
    public static Vec3D randomVector()
    {
        return randomVector(new Random());
    }

    /**
     * Static factory method. Creates a new random unit vector using the given
     * Random generator instance. I recommend to have a look at the
     * https://uncommons-maths.dev.java.net library for a good choice of
     * reliable and high quality random number generators.
     * 
     * @param rnd
     *            the rnd
     * 
     * @return a new random normalized unit vector.
     */
    public static Vec3D randomVector(Random rnd)
    {
        Vec3D v = new Vec3D((float)rnd.NextDouble() * 2 - 1, (float)rnd.NextDouble() * 2 - 1,
                (float) rnd.NextDouble() * 2 - 1);
        return v.normalize();
    }

        /** X coordinate. */
        public float x { get; set; }

        /** Y coordinate. */
        public float y { get; set; }

        /** Z coordinate. */
        public float z { get; set; }

    /**
     * Creates a new zero vector.
     */
    public Vec3D()
    {
    }

    /**
     * Creates a new vector with the given coordinates.
     * 
     * @param x
     *            the x
     * @param y
     *            the y
     * @param z
     *            the z
     */
    public Vec3D(float _x, float _y, float _z)
    {
        this.x = _x;
        this.y = _y;
        this.z = _z;
    }

    public Vec3D(float[] v)
    {
        x = v[0];
        y = v[1];
        z = v[2];
    }

        /**
         * Creates a new vector with the coordinates of the given vector.
         * 
         * @param v
         *            vector to be copied
         */
        public Vec3D(ReadonlyVec3D v)
    {
        this.x = v.getX();
        this.y = v.getY();
        this.z = v.getZ();
    }

        public static Vec3D operator +(Vec3D c1, Vec3D c2)
        {
            return c1.add(c2);
        }
        public static Vec3D operator -(Vec3D c1, Vec3D c2)
        {
            return c1.sub(c2);
        }
        public static Vec3D operator *(Vec3D c1, float c2)
        {
            return c1.scale(c2);
        }
        public static Vec3D operator /(Vec3D c1, float c2)
        {
            return c1.scale(1/c2);
        }

        public Vec3D copy()
        {
            return new Vec3D(this);
        }

        /**
         * Abs.
         * 
         * @return the vec3 d
         */
        public Vec3D abs()
    {
        x = Math.Abs(x);
        y = Math.Abs(y);
        z = Math.Abs(z);
        return this;
    }

    public Vec3D add(float a, float b, float c)
    {
        return new Vec3D(x + a, y + b, z + c);
    }

    public Vec3D add(ReadonlyVec3D v)
    {
        return new Vec3D(x + v.getX(), y + v.getY(), z + v.getZ());
    }

    public Vec3D add(Vec3D v)
    {
        return new Vec3D(x + v.x, y + v.y, z + v.z);
    }

    /**
     * Adds vector {a,b,c} and overrides coordinates with result.
     * 
     * @param a
     *            X coordinate
     * @param b
     *            Y coordinate
     * @param c
     *            Z coordinate
     * 
     * @return itself
     */
    public Vec3D addSelf(float a, float b, float c)
    {
        x += a;
        y += b;
        z += c;
        return this;
    }

    public  Vec3D addSelf(ReadonlyVec3D v)
    {
        x += v.getX();
        y += v.getY();
        z += v.getZ();
        return this;
    }

    /**
     * Adds vector v and overrides coordinates with result.
     * 
     * @param v
     *            vector to add
     * 
     * @return itself
     */
    public Vec3D addSelf(Vec3D v)
    {
        x += v.x;
        y += v.y;
        z += v.z;
        return this;
    }

    public float angleBetween(ReadonlyVec3D v)
    {
        return (float)Math.Acos(dot(v));
    }

    public float angleBetween(ReadonlyVec3D v, bool forceNormalize)
    {
        float theta;
        if (forceNormalize)
        {
            theta = getNormalized().dot(v.getNormalized());
        }
        else {
            theta = dot(v);
        }
        return (float)Math.Acos(theta);
    }

    /**
     * Sets all vector components to 0.
     * 
     * @return itself
     */
    public ReadonlyVec3D clear()
    {
        x = y = z = 0;
        return this;
    }

    public int CompareTo(ReadonlyVec3D v)
    {
        if (x == v.getX() && y == v.getY() && z == v.getZ())
        {
            return 0;
        }
        float a = magSquared();
        float b = v.magSquared();
        if (a < b)
        {
            return -1;
        }
        return +1;
    }

    /**
     * Forcefully fits the vector in the given AABB.
     * 
     * @param box
     *            the box
     * 
     * @return itself
     */
    
    public Vec3D constrain(AABB box)
    {
        return constrain(box.getMin(), box.getMax());
    }

        /**
         * Forcefully fits the vector in the given AABB specified by the 2 given
         * points.
         * 
         * @param min
         * @param max
         * @return itself
         */

        public Vec3D constrain(Vec3D min, Vec3D max)
    {
        x = Math.Min(Math.Max(x, min.x), max.x);
        y = Math.Min(Math.Max(y, min.y), max.y);
        z = Math.Min(Math.Max(z, min.z), max.z);
        return this;
    }

    public Vec3D copgetY()
    {
        return new Vec3D(this);
    }

    public Vec3D cross(ReadonlyVec3D v)
    {
        return new Vec3D(y * v.getZ() - v.getY() * z, z * v.getX() - v.getZ() * x, x
                * v.getY() - v.getX() * y);
    }

    public Vec3D cross(Vec3D v)
    {
        return new Vec3D(y * v.z - v.y * z, z * v.x - v.z * x, x * v.y - v.x
                * y);
    }

    public Vec3D crossInto(ReadonlyVec3D v, Vec3D result)
    {
        float vx = v.getX();
        float vy = v.getY();
        float vz = v.getZ();
        result.x = y * vz - vy * z;
        result.y = z * vx - vz * x;
        result.z = x * vy - vx * y;
        return result;
    }

    /**
     * Calculates cross-product with vector v. The resulting vector is
     * perpendicular to both the current and supplied vector and overrides the
     * current.
     * 
     * @param v
     *            the v
     * 
     * @return itself
     */
    public Vec3D crossSelf(Vec3D v)
    {
        float cx = y * v.z - v.y * z;
        float cy = z * v.x - v.z * x;
        z = x * v.y - v.x * y;
        y = cy;
        x = cx;
        return this;
    }

    public float distanceTo(ReadonlyVec3D v)
    {
        if (v != null)
        {
             float dx = x - v.getX();
             float dy = y - v.getY();
             float dz = z - v.getZ();
            return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }
        else {
            return float.NaN;
        }
    }

    public float distanceToSquared(ReadonlyVec3D v)
    {
        if (v != null)
        {
             float dx = x - v.getX();
             float dy = y - v.getY();
             float dz = z - v.getZ();
            return dx * dx + dy * dy + dz * dz;
        }
        else {
            return float.NaN;
        }
    }

    public  float dot(ReadonlyVec3D v)
    {
        return x * v.getX() + y * v.getY() + z * v.getZ();
    }

    public float dot(Vec3D v)
    {
        return x * v.x + y * v.y + z * v.z;
    }

    /// <summary>
    /// NOTE - maybe need to implement custom hashcode methods on a case by case basis when extending vec3d
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
            string hcode = "" + x + " " + y + " " + z;
            return hcode.GetHashCode();
    }

    public override bool Equals(Object v)
    {
        try
        {
            ReadonlyVec3D vv = (ReadonlyVec3D)v;
            return (x == vv.getX() && y == vv.getY() && z == vv.getZ());
        }
        catch (NullReferenceException e)
        {
            return false;
        }
        catch (InvalidCastException e)
        {
            return false;
        }
    }

    public bool Equals(ReadonlyVec3D v)
    {
        try
        {
            return (x == v.getX() && y == v.getY() && z == v.getZ());
        }
        catch (NullReferenceException e)
        {
            return false;
        }
    }

    public bool equalsWithTolerance(ReadonlyVec3D v, float tolerance)
    {
        try
        {
            float diff = x - v.getX();
            if (float.IsNaN(diff))
            {
                return false;
            }
            if ((diff < 0 ? -diff : diff) > tolerance)
            {
                return false;
            }
            diff = y - v.getY();
            if (float.IsNaN(diff))
            {
                return false;
            }
            if ((diff < 0 ? -diff : diff) > tolerance)
            {
                return false;
            }
            diff = z - v.getZ();
            if (float.IsNaN(diff))
            {
                return false;
            }
            if ((diff < 0 ? -diff : diff) > tolerance)
            {
                return false;
            }
            return true;
        }
        catch (NullReferenceException e)
        {
            return false;
        }
    }

    /**
     * Replaces the vector components with integer values of their current
     * values.
     * 
     * @return itself
     */
    public Vec3D floor()
    {
        x = (float) Math.Floor(x);
        y = (float) Math.Floor(y);
        z = (float) Math.Floor(z);
        return this;
    }

    /**
     * Replaces the vector components with the fractional part of their current
     * values.
     * 
     * @return itself
     */
    public Vec3D frac()
    {
        x -= (float)Math.Floor(x);
            y -= (float)Math.Floor(y);
            z -= (float)Math.Floor(z);
            return this;
    }

    public Vec3D getAbs()
    {
        return new Vec3D(this).abs();
    }

    public Vec3D getCartesian()
    {
        return copgetY().toCartesian();
    }

        public Vec3D getConstrained(AABB box)
        {
            return new Vec3D(this).constrain(box);
        }

        /**
         * Identifies the closest cartesian axis to this vector. If at leat two
         * vector components are equal, no unique decision can be made and the
         * method returns null.
         * 
         * @return Axis enum or null
         */
        public Axis getClosestAxis()
    {
        float ax = Math.Abs(x);
        float ay = Math.Abs(y);
        float az = Math.Abs(z);
        if (ax > ay && ax > az)
        {
            return Axis.X;
        }
        if (ay > ax && ay > az)
        {
            return Axis.Y;
        }
        if (az > ax && az > ay)
        {
            return Axis.Z;
        }
        return null;
    }

        /*
    public float getComponent(Axis id)
    {
        switch (id)
        {
            case X:
                return x;
            case Y:
                return y;
            case Z:
                return z;
        }
        throw new ArgumentException();
    }
    */

    public float getComponent(int id)
    {
        switch (id)
        {
            case 0:
                return x;
            case 1:
                return y;
            case 2:
                return z;
        }
        throw new ArgumentException("index must be 0, 1 or 2");
    }


    public  Vec3D getFloored()
    {
        return new Vec3D(this).floor();
    }

    /*
     * (non-Javadoc)
     * 
     * @see toxi.geom.ReadonlyVec3D#getFrac()
     */
    public  Vec3D getFrac()
    {
        return new Vec3D(this).frac();
    }

    /*
     * (non-Javadoc)
     * 
     * @see toxi.geom.ReadonlyVec3D#getInverted()
     */
    public Vec3D getInverted()
    {
        return new Vec3D(-x, -y, -z);
    }

    /*
     * (non-Javadoc)
     * 
     * @see toxi.geom.ReadonlyVec3D#getLimited(float)
     */
    public Vec3D getLimited(float lim)
    {
        if (magSquared() > lim * lim)
        {
            return getNormalizedTo(lim);
        }
        return new Vec3D(this);
    }



    /*
     * (non-Javadoc)
     * 
     * @see toxi.geom.ReadonlyVec3D#getNormalized()
     */
    public  Vec3D getNormalized()
    {
        return new Vec3D(this).normalize();
    }

    /*
     * (non-Javadoc)
     * 
     * @see toxi.geom.ReadonlyVec3D#getNormalizedTo(float)
     */
    public  Vec3D getNormalizedTo(float len)
    {
        return new Vec3D(this).normalizeTo(len);
    }

    /*
     * (non-Javadoc)
     * 
     * @see toxi.geom.ReadonlyVec3D#getReciprocal()
     */
    public  Vec3D getReciprocal()
    {
        return copgetY().reciprocal();
    }

    public  Vec3D getReflected(ReadonlyVec3D normal)
    {
        return copgetY().reflect(normal);
    }

    /*
     * (non-Javadoc)
     * 
     * @see toxi.geom.ReadonlyVec3D#getRotatedAroundAxis(toxi.geom.Vec3D, float)
     */
    public Vec3D getRotatedAroundAxis(ReadonlyVec3D axis, float theta)
    {
        return new Vec3D(this).rotateAroundAxis(axis, theta);
    }

    /*
     * (non-Javadoc)
     * 
     * @see toxi.geom.ReadonlyVec3D#getRotatedX(float)
     */
    public  Vec3D getRotatedX(float theta)
    {
        return new Vec3D(this).rotateX(theta);
    }

    /*
     * (non-Javadoc)
     * 
     * @see toxi.geom.ReadonlyVec3D#getRotatedY(float)
     */
    public  Vec3D getRotatedY(float theta)
    {
        return new Vec3D(this).rotateY(theta);
    }

    /*
     * (non-Javadoc)
     * 
     * @see toxi.geom.ReadonlyVec3D#getRotatedZ(float)
     */
    public  Vec3D getRotatedZ(float theta)
    {
        return new Vec3D(this).rotateZ(theta);
    }

    public Vec3D getRoundedTo(float prec)
    {
        return copgetY().roundTo(prec);
    }

    /*
     * (non-Javadoc)
     * 
     * @see toxi.geom.ReadonlyVec3D#getSignum()
     */
    public  Vec3D getSignum()
    {
        return new Vec3D(this).signum();
    }

    public Vec3D getSpherical()
    {
        return copgetY().toSpherical();
    }

        /**
         * Returns a hash code value based on the data values in this object. Two
         * different Vec3D objects with identical data values (i.e., Vec3D.equals
         * returns true) will return the same hash code value. Two objects with
         * different data members may return the same hash value, although this is
         * not likely.
         * 
         * @return the integer hash code value
         */
        static int floatToIntBits(float f)
        {
            if (f == 0.0f)
            {
                return 0;
            }
            else {
                return BitConverter.ToInt32(BitConverter.GetBytes(f), 0);
            }
        }


        public int hashCode()
    {
        long bits = 1L;
        bits = 31L * bits + floatToIntBits(x);
        bits = 31L * bits + floatToIntBits(y);
        bits = 31L * bits + floatToIntBits(z);
        return (int)(bits ^ (bits >> 32));
    }

    /*
     * (non-Javadoc)
     * 
     * @see toxi.geom.ReadonlyVec3D#headingXgetY()
     */
    public  float headingXY()
    {
        return (float)Math.Atan2(y, x);
    }

    /*
     * (non-Javadoc)
     * 
     * @see toxi.geom.ReadonlyVec3D#headingXgetZ()
     */
    public float headingXZ()
    {
        return (float)Math.Atan2(z, x);
    }

    /*
     * (non-Javadoc)
     * 
     * @see toxi.geom.ReadonlyVec3D#headingYgetZ()
     */
    public float headingYZ()
    {
        return (float)Math.Atan2(y, z);
    }

    public ReadonlyVec3D immutable()
    {
        return this;
    }

    public Vec3D interpolateTo(ReadonlyVec3D v, float f)
    {
        return new Vec3D(x + (v.getX() - x) * f, y + (v.getY() - y) * f, z
                + (v.getZ() - z) * f);
    }

    public Vec3D interpolateTo(ReadonlyVec3D v, float f,
            InterpolateStrategy s)
    {
        return new Vec3D(s.interpolate(x, v.getX(), f),
                s.interpolate(y, v.getY(), f), s.interpolate(z, v.getZ(), f));
    }

    public Vec3D interpolateTo(Vec3D v, float f)
    {
        return new Vec3D(x + (v.x - x) * f, y + (v.y - y) * f, z + (v.z - z)
                * f);
    }

    public Vec3D interpolateTo(Vec3D v, float f, InterpolateStrategy s)
    {
        return new Vec3D(s.interpolate(x, v.x, f), s.interpolate(y, v.y, f),
                s.interpolate(z, v.z, f));
    }

    /**
     * Interpolates the vector towards the given target vector, using linear
     * interpolation.
     * 
     * @param v
     *            target vector
     * @param f
     *            interpolation factor (should be in the range 0..1)
     * 
     * @return itself, result overrides current vector
     */
    public Vec3D interpolateToSelf(ReadonlyVec3D v, float f)
    {
        x += (v.getX() - x) * f;
        y += (v.getY() - y) * f;
        z += (v.getZ() - z) * f;
        return this;
    }

    /**
     * Interpolates the vector towards the given target vector, using the given
     * {@link InterpolateStrategy}.
     * 
     * @param v
     *            target vector
     * @param f
     *            interpolation factor (should be in the range 0..1)
     * @param s
     *            InterpolateStrategy instance
     * 
     * @return itself, result overrides current vector
     */
    public Vec3D interpolateToSelf(ReadonlyVec3D v, float f,
            InterpolateStrategy s)
    {
        x = s.interpolate(x, v.getX(), f);
        y = s.interpolate(y, v.getY(), f);
        z = s.interpolate(z, v.getZ(), f);
        return this;
    }

    /**
     * Scales vector uniformly by factor -1 ( v = -v ), overrides coordinates
     * with result.
     * 
     * @return itself
     */
    public Vec3D invert()
    {
        x *= -1;
        y *= -1;
        z *= -1;
        return this;
    }

    public bool isInAABB(Vec3D boxOrigin, Vec3D boxExtent)
    {
        float w = boxExtent.x;
        if (x < boxOrigin.x - w || x > boxOrigin.x + w)
        {
            return false;
        }
        w = boxExtent.y;
        if (y < boxOrigin.y - w || y > boxOrigin.y + w)
        {
            return false;
        }
        w = boxExtent.z;
        if (z < boxOrigin.z - w || z > boxOrigin.z + w)
        {
            return false;
        }
        return true;
    }

        public bool isInAABB(AABB box)
        {
            Vec3D min = box.getMin();
            Vec3D max = box.getMax();
            if (x < min.x || x > max.x)
            {
                return false;
            }
            if (y < min.y || y > max.y)
            {
                return false;
            }
            if (z < min.z || z > max.z)
            {
                return false;
            }
            return true;
        }

        public bool isMajorAxis(float tol)
    {
        float ax = Math.Abs(x);
        float ay = Math.Abs(y);
        float az = Math.Abs(z);
        float itol = 1 - tol;
        if (ax > itol)
        {
            if (ay < tol)
            {
                return (az < tol);
            }
        }
        else if (ay > itol)
        {
            if (ax < tol)
            {
                return (az < tol);
            }
        }
        else if (az > itol)
        {
            if (ax < tol)
            {
                return (ay < tol);
            }
        }
        return false;
    }

    public bool isZeroVector()
    {
            return Math.Abs(x) < Math.E
                && Math.Abs(y) < Math.E
                && Math.Abs(z) < Math.E;
    }

    /**
     * Add random jitter to the vector in the range -j ... +j using the default
     * {@link Random} generator of {@link MathUtils}.
     * 
     * @param j
     *            the j
     * 
     * @return the vec3 d
     */
    public Vec3D jitter(float j)
    {
        return jitter(j, j, j);
    }

    /**
     * Adds random jitter to the vector in the range -j ... +j using the default
     * {@link Random} generator of {@link MathUtils}.
     * 
     * @param jx
     *            maximum x jitter
     * @param jy
     *            maximum y jitter
     * @param jz
     *            maximum z jitter
     * 
     * @return itself
     */
    public Vec3D jitter(float jx, float jy, float jz)
    {
        return jitter(new Random(), jx, jy, jz);
    }

    public Vec3D jitter(Random rnd, float j)
    {
        return jitter(rnd, j, j, j);
    }

    public Vec3D jitter(Random rnd, float jx, float jy, float jz)
    {
            Random r = new Random();
            x += (float)r.NextDouble() * jx;
            y += (float)r.NextDouble() * jy;
            z += (float)r.NextDouble() * jz;
            return this;
    }

    public Vec3D jitter(Random rnd, Vec3D jitterVec)
    {
        return jitter(rnd, jitterVec.x, jitterVec.y, jitterVec.z);
    }

    /**
     * Adds random jitter to the vector in the range defined by the given vector
     * components and using the default {@link Random} generator of
     * {@link MathUtils}.
     * 
     * @param jitterVec
     *            the jitter vec
     * 
     * @return itself
     */
    public Vec3D jitter(Vec3D jitterVec)
    {
        return jitter(jitterVec.x, jitterVec.y, jitterVec.z);
    }

    /**
     * Limits the vector's magnitude to the length given.
     * 
     * @param lim
     *            new maximum magnitude
     * 
     * @return itself
     */
    public Vec3D limit(float lim)
    {
        if (magSquared() > lim * lim)
        {
            return normalize().scaleSelf(lim);
        }
        return this;
    }

    public float magnitude()
    {
        return (float)Math.Sqrt(x * x + y * y + z * z);
    }

    public float magSquared()
    {
        return x * x + y * y + z * z;
    }

    /**
     * Max self.
     * 
     * @param b
     *            the b
     * 
     * @return the vec3 d
     */
    public Vec3D maxSelf(ReadonlyVec3D b)
    {
        x = Math.Max(x, b.getX());
        y = Math.Max(y, b.getY());
        z = Math.Max(z, b.getZ());
        return this;
    }

    /**
     * Min self.
     * 
     * @param b
     *            the b
     * 
     * @return the vec3 d
     */
    public Vec3D minSelf(ReadonlyVec3D b)
    {
        x = Math.Min(x, b.getX());
        y = Math.Min(y, b.getY());
        z = Math.Min(z, b.getZ());
        return this;
    }

    /**
     * Applies a uniform modulo operation to the vector, using the same base for
     * all components.
     * 
     * @param base
     *            the base
     * 
     * @return itself
     */
    public Vec3D modSelf(float b)
    {
        x %= b;
        y %= b;
        z %= b;
        return this;
    }

    /**
     * Calculates modulo operation for each vector component separately.
     * 
     * @param bx
     *            the bx
     * @param by
     *            the by
     * @param bz
     *            the bz
     * 
     * @return itself
     */

    public Vec3D modSelf(float bx, float by, float bz)
    {
        x %= bx;
        y %= by;
        z %= bz;
        return this;
    }

    /**
     * Normalizes the vector so that its magnitude = 1.
     * 
     * @return itself
     */
    public Vec3D normalize()
    {
        float mag = (float)Math.Sqrt(x * x + y * y + z * z);
        if (mag > 0)
        {
            mag = 1f / mag;
            x *= mag;
            y *= mag;
            z *= mag;
        }
        return this;
    }

    /**
     * Normalizes the vector to the given length.
     * 
     * @param len
     *            desired length
     * @return itself
     */
    public Vec3D normalizeTo(float len)
    {
        float mag = (float)Math.Sqrt(x * x + y * y + z * z);
        if (mag > 0)
        {
            mag = len / mag;
            x *= mag;
            y *= mag;
            z *= mag;
        }
        return this;
    }

    /**
     * Replaces the vector components with their multiplicative inverse.
     * 
     * @return itself
     */
    public Vec3D reciprocal()
    {
        x = 1f / x;
        y = 1f / y;
        z = 1f / z;
        return this;
    }

    public Vec3D reflect(ReadonlyVec3D normal)
    {
        return set(normal.scale(this.dot(normal) * 2).subSelf(this));
    }

    /**
     * Rotates the vector around the giving axis.
     * 
     * @param axis
     *            rotation axis vector
     * @param theta
     *            rotation angle (in radians)
     * 
     * @return itself
     */
    public Vec3D rotateAroundAxis(ReadonlyVec3D axis, float theta)
    {
        float ax = axis.getX();
        float ay = axis.getY();
        float az = axis.getZ();
        float ux = ax * x;
        float uy = ax * y;
        float uz = ax * z;
        float vx = ay * x;
        float vy = ay * y;
        float vz = ay * z;
        float wx = az * x;
        float wy = az * y;
        float wz = az * z;
        double si = Math.Sin(theta);
        double co = Math.Cos(theta);
        float xx = (float)(ax * (ux + vy + wz)
                + (x * (ay * ay + az * az) - ax * (vy + wz)) * co + (-wy + vz)
                * si);
        float yy = (float)(ay * (ux + vy + wz)
                + (y * (ax * ax + az * az) - ay * (ux + wz)) * co + (wx - uz)
                * si);
        float zz = (float)(az * (ux + vy + wz)
                + (z * (ax * ax + ay * ay) - az * (ux + vy)) * co + (-vx + uy)
                * si);
        x = xx;
        y = yy;
        z = zz;
        return this;
    }

    /**
     * Rotates the vector by the given angle around the X axis.
     * 
     * @param theta
     *            the theta
     * 
     * @return itself
     */
    public Vec3D rotateX(float theta)
    {
        float co = (float)Math.Cos(theta);
        float si = (float)Math.Sin(theta);
        float zz = co * z - si * y;
        y = si * z + co * y;
        z = zz;
        return this;
    }

    /**
     * Rotates the vector by the given angle around the Y axis.
     * 
     * @param theta
     *            the theta
     * 
     * @return itself
     */
    public Vec3D rotateY(float theta)
    {
        float co = (float)Math.Cos(theta);
        float si = (float)Math.Sin(theta);
        float xx = co * x - si * z;
        z = si * x + co * z;
        x = xx;
        return this;
    }

    /**
     * Rotates the vector by the given angle around the Z axis.
     * 
     * @param theta
     *            the theta
     * 
     * @return itself
     */
    public Vec3D rotateZ(float theta)
    {
        float co = (float)Math.Cos(theta);
        float si = (float)Math.Sin(theta);
        float xx = co * x - si * y;
        y = si * x + co * y;
        x = xx;
        return this;
    }
   
    public Vec3D roundTo(float prec)
    {
        
        x = (float)Math.Floor(x / prec + 0.5f) * prec;
        y = (float)Math.Floor(y / prec + 0.5f) * prec;
            z = (float)Math.Floor(z / prec + 0.5f) * prec;
            return this;
    }

    public Vec3D scale(float s)
    {
        return new Vec3D(x * s, y * s, z * s);
    }

    public Vec3D scale(float a, float b, float c)
    {
        return new Vec3D(x * a, y * b, z * c);
    }

    public Vec3D scale(ReadonlyVec3D s)
    {
        return new Vec3D(x * s.getX(), y * s.getY(), z * s.getZ());
    }

    public Vec3D scale(Vec3D s)
    {
        return new Vec3D(x * s.x, y * s.y, z * s.z);
    }

    /**
     * Scales vector uniformly and overrides coordinates with result.
     * 
     * @param s
     *            scale factor
     * 
     * @return itself
     */
    public Vec3D scaleSelf(float s)
    {
        x *= s;
        y *= s;
        z *= s;
        return this;
    }

    /**
     * Scales vector non-uniformly by vector {a,b,c} and overrides coordinates
     * with result.
     * 
     * @param a
     *            scale factor for X coordinate
     * @param b
     *            scale factor for Y coordinate
     * @param c
     *            scale factor for Z coordinate
     * 
     * @return itself
     */
    public Vec3D scaleSelf(float a, float b, float c)
    {
        x *= a;
        y *= b;
        z *= c;
        return this;
    }

    public Vec3D scaleSelf(ReadonlyVec3D s)
    {
        x *= s.getX();
        y *= s.getY();
        z *= s.getZ();
        return this;
    }

    /**
     * Scales vector non-uniformly by vector v and overrides coordinates with
     * result.
     * 
     * @param s
     *            scale vector
     * 
     * @return itself
     */

    public Vec3D scaleSelf(Vec3D s)
    {
        x *= s.x;
        y *= s.y;
        z *= s.z;
        return this;
    }

    /**
     * Overrides coordinates with the given values.
     * 
     * @param x
     *            the x
     * @param y
     *            the y
     * @param z
     *            the z
     * 
     * @return itself
     */
    public Vec3D set(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        return this;
    }

    public Vec3D set(ReadonlyVec3D v)
    {
        x = v.getX();
        y = v.getY();
        z = v.getZ();
        return this;
    }

    /**
     * Overrides coordinates with the ones of the given vector.
     * 
     * @param v
     *            vector to be copied
     * 
     * @return itself
     */
    public Vec3D set(Vec3D v)
    {
        x = v.x;
        y = v.y;
        z = v.z;
        return this;
    }

        /*
    public Vec3D setComponent(Axis id, float val)
    {
        switch (id)
        {
            case X:
                x = val;
                break;
            case Y:
                y = val;
                break;
            case Z:
                z = val;
                break;
        }
        return this;
    }
    */

    public Vec3D setComponent(int id, float val)
    {
        switch (id)
        {
            case 0:
                x = val;
                break;
            case 1:
                y = val;
                break;
            case 2:
                z = val;
                break;
        }
        return this;
    }

    public Vec3D setX(float x)
    {
        this.x = x;
        return this;
    }

    /**
     * Overrides XY coordinates with the ones of the given 2D vector.
     * 
     * @param v
     *            2D vector
     * 
     * @return itself
     */
     /*
    public Vec3D setXY(Vec2D v)
    {
        x = v.x;
        y = v.y;
        return this;
    }*/

    public Vec3D setY(float y)
    {
        this.y = y;
        return this;
    }

    public Vec3D setZ(float z)
    {
        this.z = z;
        return this;
    }

    public Vec3D shuffle(int iterations)
    {
            Random r = new Random();
        float t;
        for (int i = 0; i < iterations; i++)
        {
            switch ((int) Math.Round((r.NextDouble()*3)))
            {
                case 0:
                    t = x;
                    x = y;
                    y = t;
                    break;
                case 1:
                    t = x;
                    x = z;
                    z = t;
                    break;
                case 2:
                    t = y;
                    y = z;
                    z = t;
                    break;
            }
        }
        return this;
    }

    /**
     * Replaces all vector components with the signum of their original values.
     * In other words if a components value was negative its new value will be
     * -1, if zero => 0, if positive => +1
     * 
     * @return itself
     */
    public Vec3D signum()
    {
        x = (x < 0 ? -1 : x == 0 ? 0 : 1);
        y = (y < 0 ? -1 : y == 0 ? 0 : 1);
        z = (z < 0 ? -1 : z == 0 ? 0 : 1);
        return this;
    }

    /**
     * Rounds the vector to the closest major axis. Assumes the vector is
     * normalized.
     * 
     * @return itself
     */
    public Vec3D snapToAxis()
    {
        if (Math.Abs(x) < 0.5f)
        {
            x = 0;
        }
        else {
            x = x < 0 ? -1 : 1;
            y = z = 0;
        }
        if (Math.Abs(y) < 0.5f)
        {
            y = 0;
        }
        else {
            y = y < 0 ? -1 : 1;
            x = z = 0;
        }
        if (Math.Abs(z) < 0.5f)
        {
            z = 0;
        }
        else {
            z = z < 0 ? -1 : 1;
            x = y = 0;
        }
        return this;
    }

    public Vec3D sub(float a, float b, float c)
    {
        return new Vec3D(x - a, y - b, z - c);
    }

    public Vec3D sub(ReadonlyVec3D v)
    {
        return new Vec3D(x - v.getX(), y - v.getY(), z - v.getZ());
    }

    public Vec3D sub(Vec3D v)
    {
        return new Vec3D(x - v.x, y - v.y, z - v.z);
    }

    /**
     * Subtracts vector {a,b,c} and overrides coordinates with result.
     * 
     * @param a
     *            X coordinate
     * @param b
     *            Y coordinate
     * @param c
     *            Z coordinate
     * 
     * @return itself
     */
    public Vec3D subSelf(float a, float b, float c)
    {
        x -= a;
        y -= b;
        z -= c;
        return this;
    }

    public Vec3D subSelf(ReadonlyVec3D v)
    {
        x -= v.getX();
        y -= v.getY();
        z -= v.getZ();
        return this;
    }

    /**
     * Subtracts vector v and overrides coordinates with result.
     * 
     * @param v
     *            vector to be subtracted
     * 
     * @return itself
     */
    public Vec3D subSelf(Vec3D v)
    {
        x -= v.x;
        y -= v.y;
        z -= v.z;
        return this;
    }
    
        /*
    public Vec2D to2DXgetY()
    {
        return new Vec2D(x, y);
    }

    public Vec2D to2DXgetZ()
    {
        return new Vec2D(x, z);
    }

    public Vec2D to2DYgetZ()
    {
        return new Vec2D(y, z);
    }

    public Vec4D to4D()
    {
        return new Vec4D(x, y, z, 1);
    }

    public Vec4D to4D(float w)
    {
        return new Vec4D(x, y, z, w);
    }*/

    public float[] toArray()
    {
        return new float[] {
                    x, y, z
            };
    }

    public float[] toArray4(float w)
    {
        return new float[] {
                    x, y, z, w
            };
    }

    public Vec3D toCartesian()
    {
        float a = (float)(x * Math.Cos(z));
        float xx = (float)(a * Math.Cos(y));
        float yy = (float)(x * Math.Sin(z));
        float zz = (float)(a * Math.Sin(y));
        x = xx;
        y = yy;
        z = zz;
        return this;
    }

    public Vec3D toSpherical()
    {
        float xx = Math.Abs(x) <= Math.E ? (float)Math.E : x;
        float zz = z;

        float radius = (float)Math.Sqrt((xx * xx) + (y * y) + (zz * zz));
        z = (float)Math.Asin(y / radius);
        y = (float)Math.Atan(zz / xx) + (xx < 0.0 ? (float)Math.PI : 0);
        x = radius;
        return this;
    }

    public String toString()
    {
        StringBuilder sb = new StringBuilder(48);
        sb.Append("{x:").Append(x).Append(", y:").Append(y).Append(", z:")
                .Append(z).Append("}");
        return sb.ToString();
    }
        public float getX()
        {
            return x;
        }

        public float getY()
        {
            return y;
        }

        public float getZ()
        {
            return z;
        }
    }
    }