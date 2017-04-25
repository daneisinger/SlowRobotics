using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toxiclibs.core
{
    //Implementation of Toxiclibs core classes in C#
    //src here: https://bitbucket.org/postspectacular/toxiclibs/src/
    public interface ReadonlyVec3D
    {

        /**
         * Adds vector {a,b,c} and returns result as new vector.
         * 
         * @param a
         *            X coordinate
         * @param b
         *            Y coordinate
         * @param c
         *            Z coordinate
         * 
         * @return result as new vector
         */
         Vec3D add(float a, float b, float c);

         Vec3D add(ReadonlyVec3D v);

        /**
         * Add vector v and returns result as new vector.
         * 
         * @param v
         *            vector to add
         * 
         * @return result as new vector
         */
         Vec3D add(Vec3D v);

        /**
         * Computes the angle between this vector and vector V. This function
         * assumes both vectors are normalized, if this can't be guaranteed, use the
         * alternative implementation {@link #angleBetween(ReadonlyVec3D, bool)}
         * 
         * @param v
         *            vector
         * 
         * @return angle in radians, or NaN if vectors are parallel
         */
         float angleBetween(ReadonlyVec3D v);

        /**
         * Computes the angle between this vector and vector V.
         * 
         * @param v
         *            vector
         * @param forceNormalize
         *            true, if normalized versions of the vectors are to be used
         *            (Note: only copies will be used, original vectors will not be
         *            altered by this method)
         * 
         * @return angle in radians, or NaN if vectors are parallel
         */
         float angleBetween(ReadonlyVec3D v, bool forceNormalize);

        /**
         * Compares the length of the vector with another one.
         * 
         * @param v
         *            vector to compare with
         * 
         * @return -1 if other vector is longer, 0 if both are equal or else +1
         */
         int CompareTo(ReadonlyVec3D v);

        /**
         * Copy.
         * 
         * @return a new independent instance/copy of a given vector
         */
         Vec3D copy();

        /**
         * Calculates cross-product with vector v. The resulting vector is
         * perpendicular to both the current and supplied vector.
         * 
         * @param v
         *            vector to cross
         * 
         * @return cross-product as new vector
         */
         Vec3D cross(ReadonlyVec3D v);

        /**
         * Calculates cross-product with vector v. The resulting vector is
         * perpendicular to both the current and supplied vector and stored in the
         * supplied result vector.
         * 
         * @param v
         *            vector to cross
         * @param result
         *            result vector
         * 
         * @return result vector
         */
         Vec3D crossInto(ReadonlyVec3D v, Vec3D result);

        /**
         * Calculates distance to another vector.
         * 
         * @param v
         *            non-null vector
         * 
         * @return distance or Float.NaN if v=null
         */
         float distanceTo(ReadonlyVec3D v);

        /**
         * Calculates the squared distance to another vector.
         * 
         * @param v
         *            non-null vector
         * 
         * @return distance or NaN if v=null
         * 
         * @see #magSquared()
         */
         float distanceToSquared(ReadonlyVec3D v);

        /**
         * Computes the scalar product (dot product) with the given vector.
         * 
         * @param v
         *            the v
         * 
         * @return dot product
         * 
         * @see <a href="http://en.wikipedia.org/wiki/Dot_product">Wikipedia
         *      entry</a>
         */
         float dot(ReadonlyVec3D v);


        /**
         * Compares this vector with the one given. The vectors are deemed equal if
         * the individual differences of all component values are within the given
         * tolerance.
         * 
         * @param v
         *            the v
         * @param tolerance
         *            the tolerance
         * 
         * @return true, if equal
         */
         bool equalsWithTolerance(ReadonlyVec3D v, float tolerance);

        /**
         * Gets the abs.
         * 
         * @return the abs
         */
         Vec3D getAbs();

        /**
         * Converts the spherical vector back into cartesian coordinates.
         * 
         * @return new vector
         */
         Vec3D getCartesian();

         //Axis getClosestAxis();

         //float getComponent(Axis id);

         float getComponent(int id);

        /**
         * Creates a copy of the vector which forcefully fits in the given AABB.
         * 
         * @param box
         *            the box
         * 
         * @return fitted vector
         */
         Vec3D getConstrained(AABB box);

        /**
         * Creates a new vector whose components are the integer value of their
         * current values.
         * 
         * @return result as new vector
         */
         Vec3D getFloored();

        /**
         * Creates a new vector whose components are the fractional part of their
         * current values.
         * 
         * @return result as new vector
         */
         Vec3D getFrac();

        /**
         * Scales vector uniformly by factor -1 ( v = -v ).
         * 
         * @return result as new vector
         */
         Vec3D getInverted();

        /**
         * Creates a copy of the vector with its magnitude limited to the length
         * given.
         * 
         * @param lim
         *            new maximum magnitude
         * 
         * @return result as new vector
         */
         Vec3D getLimited(float lim);

        /**
         * Produces a new vector with its coordinates passed through the given
         * {@link ScaleMap}.
         * 
         * @param map
         * @return mapped vector
         */
         //Vec3D getMapped(ScaleMap map);

        /**
         * Produces the normalized version as a new vector.
         * 
         * @return new vector
         */
         Vec3D getNormalized();

        /**
         * Produces a new vector normalized to the given length.
         * 
         * @param len
         *            new desired length
         * 
         * @return new vector
         */
         Vec3D getNormalizedTo(float len);

        /**
         * Returns a multiplicative inverse copy of the vector.
         * 
         * @return new vector
         */
         Vec3D getReciprocal();

         Vec3D getReflected(ReadonlyVec3D normal);

        /**
         * Gets the rotated around axis.
         * 
         * @param axis
         *            the axis
         * @param theta
         *            the theta
         * 
         * @return new result vector
         */
         Vec3D getRotatedAroundAxis(ReadonlyVec3D axis, float theta);

        /**
         * Creates a new vector rotated by the given angle around the X axis.
         * 
         * @param theta
         *            the theta
         * 
         * @return rotated vector
         */
         Vec3D getRotatedX(float theta);

        /**
         * Creates a new vector rotated by the given angle around the Y axis.
         * 
         * @param theta
         *            the theta
         * 
         * @return rotated vector
         */
         Vec3D getRotatedY(float theta);

        /**
         * Creates a new vector rotated by the given angle around the Z axis.
         * 
         * @param theta
         *            the theta
         * 
         * @return rotated vector
         */
         Vec3D getRotatedZ(float theta);

        /**
         * Creates a new vector with its coordinates rounded to the given precision
         * (grid alignment).
         * 
         * @param prec
         * @return grid aligned vector
         */
         Vec3D getRoundedTo(float prec);

        /**
         * Creates a new vector in which all components are replaced with the signum
         * of their original values. In other words if a components value was
         * negative its new value will be -1, if zero => 0, if positive => +1
         * 
         * @return result vector
         */
         Vec3D getSignum();

        /**
         * Converts the vector into spherical coordinates. After the conversion the
         * vector components are to be interpreted as:
         * <ul>
         * <li>x = radius</li>
         * <li>y = azimuth</li>
         * <li>z = theta</li>
         * </ul>
         * 
         * @return new vector
         */
         Vec3D getSpherical();

        /**
         * Computes the vector's direction in the XY plane (for example for 2D
         * points). The positive X axis equals 0 degrees.
         * 
         * @return rotation angle
         */
         float headingXY();

        /**
         * Computes the vector's direction in the XZ plane. The positive X axis
         * equals 0 degrees.
         * 
         * @return rotation angle
         */
         float headingXZ();

        /**
         * Computes the vector's direction in the YZ plane. The positive Z axis
         * equals 0 degrees.
         * 
         * @return rotation angle
         */
         float headingYZ();

        /**
         * Interpolates the vector towards the given target vector, using linear
         * interpolation.
         * 
         * @param v
         *            target vector
         * @param f
         *            interpolation factor (should be in the range 0..1)
         * 
         * @return result as new vector
         */
         Vec3D interpolateTo(ReadonlyVec3D v, float f);

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
         * @return result as new vector
         */
         Vec3D interpolateTo(ReadonlyVec3D v, float f, InterpolateStrategy s);

        /**
         * Checks if the point is inside the given AABB.
         * 
         * @param box
         *            bounding box to check
         * 
         * @return true, if point is inside
         */
         bool isInAABB(AABB box);

        /**
         * Checks if the point is inside the given axis-aligned bounding box.
         * 
         * @param boxOrigin
         *            bounding box origin/center
         * @param boxExtent
         *            bounding box extends (half measure)
         * 
         * @return true, if point is inside the box
         */

         bool isInAABB(Vec3D boxOrigin, Vec3D boxExtent);

        /**
         * Checks if the vector is parallel with either the X or Y axis (any
         * direction).
         * 
         * @param tolerance
         * @return true, if parallel within the given tolerance
         */
         bool isMajorAxis(float tolerance);

        /**
         * Checks if vector has a magnitude equals or close to zero (tolerance used
         * is {@link MathUtils#EPS}).
         * 
         * @return true, if zero vector
         */
         bool isZeroVector();

        /**
         * Calculates the magnitude/eucledian length of the vector.
         * 
         * @return vector length
         */
         float magnitude();

        /**
         * Calculates only the squared magnitude/length of the vector. Useful for
         * inverse square law applications and/or for speed reasons or if the real
         * eucledian distance is not required (e.g. sorting).
         * 
         * @return squared magnitude (x^2 + y^2 + z^2)
         */
         float magSquared();

        /**
         * Scales vector uniformly and returns result as new vector.
         * 
         * @param s
         *            scale factor
         * 
         * @return new vector
         */
         Vec3D scale(float s);

        /**
         * Scales vector non-uniformly and returns result as new vector.
         * 
         * @param a
         *            scale factor for X coordinate
         * @param b
         *            scale factor for Y coordinate
         * @param c
         *            scale factor for Z coordinate
         * 
         * @return new vector
         */
         Vec3D scale(float a, float b, float c);

        /**
         * Scales vector non-uniformly by vector v and returns result as new vector.
         * 
         * @param s
         *            scale vector
         * 
         * @return new vector
         */
         Vec3D scale(ReadonlyVec3D s);

        /**
         * Subtracts vector {a,b,c} and returns result as new vector.
         * 
         * @param a
         *            X coordinate
         * @param b
         *            Y coordinate
         * @param c
         *            Z coordinate
         * 
         * @return result as new vector
         */
         Vec3D sub(float a, float b, float c);

        /**
         * Subtracts vector v and returns result as new vector.
         * 
         * @param v
         *            vector to be subtracted
         * 
         * @return result as new vector
         */
         Vec3D sub(ReadonlyVec3D v);

        /**
         * Creates a new 2D vector of the XY components.
         * 
         * @return new vector
         */
       //  Vec2D to2DXY();

        /**
         * Creates a new 2D vector of the XZ components.
         * 
         * @return new vector
         */
       //  Vec2D to2DXZ();

        /**
         * Creates a new 2D vector of the YZ components.
         * 
         * @return new vector
         */
         //Vec2D to2DYZ();

        /**
         * Creates a Vec4D instance of this vector with the w component set to 1.0
         * 
         * @return 4d vector
         */
        // Vec4D to4D();

        /**
         * Creates a Vec4D instance of this vector with it w component set to the
         * given value.
         * 
         * @param w
         * @return weighted 4d vector
         */
         //Vec4D to4D(float w);

         float[] toArray();

         float[] toArray4(float w);
        float getX();
        float getY();
        float getZ();
    }
}
