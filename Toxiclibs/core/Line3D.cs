using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toxiclibs.core
{
    public class Line3D : ILine
    {
        public enum IntersectionType
        {
            NON_INTERSECTING,
            INTERSECTING
        }

        public class LineIntersection
        {

            

            private IntersectionType type;
            private Line3D line;
            private float[] coeff;

            public LineIntersection(IntersectionType type) : this(type, null, 0, 0)
            {
            }

            public LineIntersection(IntersectionType type, Line3D line, float mua, float mub)
            {
                this.type = type;
                this.line = line;
                this.coeff = new float[] {
                    mua, mub
            };
            }

            public float[] getCoefficients()
            {
                return coeff;
            }

            public float getLength()
            {
                return line.getLength();
            }

            /**
             * @return the pos
             */
            public Line3D getLine()
            {
                return line.copy();
            }

            /**
             * @return the type
             */
            public IntersectionType getType()
            {
                return type;
            }

            public bool isIntersectionInside()
            {
                return type == IntersectionType.INTERSECTING && coeff[0] >= 0 && coeff[0] <= 1
                        && coeff[1] >= 0 && coeff[1] <= 1;
            }

            public String toString()
            {
                return "type: " + type + " line: " + line;
            }
        }

        /**
         * Splits the line between A and B into segments of the given length,
         * starting at point A. The tweened points are added to the given result
         * list. The last point added is B itself and hence it is likely that the
         * last segment has a shorter length than the step length requested. The
         * first point (A) can be omitted and not be added to the list if so
         * desired.
         * 
         * @param a
         *            start point
         * @param b
         *            end point (always added to results)
         * @param stepLength
         *            desired distance between points
         * @param segments
         *            existing array list for results (or a new list, if null)
         * @param addFirst
         *            false, if A is NOT to be added to results
         * @return list of result vectors
         */
        public static List<Vec3D> splitIntoSegments(Vec3D a, Vec3D b,
                float stepLength, List<Vec3D> segments, bool addFirst)
        {
            if (segments == null)
            {
                segments = new List<Vec3D>();
            }
            if (addFirst)
            {
                segments.Add(a.copy());
            }
            float dist = a.distanceTo(b);
            if (dist > stepLength)
            {
                Vec3D pos = a.copy();
                Vec3D step = b.sub(a).limit(stepLength);
                while (dist > stepLength)
                {
                    pos.addSelf(step);
                    segments.Add(pos.copy());
                    dist -= stepLength;
                }
            }
            segments.Add(b.copy());
            return segments;
        }

        public Vec3D start { get; set; }
        public Vec3D end { get; set; }

        public Line3D(float x1, float y1, float z1, float x2, float y2, float z2)
        {
            start = new Vec3D(x1, y1, z1);
            end = new Vec3D(x2, y2, z2);
        }

        public Line3D(ReadonlyVec3D a, ReadonlyVec3D b)
        {
            start = a.copy();
            end = b.copy();
        }

        public Line3D(Vec3D a, Vec3D b)
        {
            start = a;
            end = b;
        }

        /**
         * Calculates the line segment that is the shortest route between this line
         * and the given one. Also calculates the coefficients where the end points
         * of this new line lie on the existing ones. If these coefficients are
         * within the 0.0 .. 1.0 interval the endpoints of the intersection line are
         * within the given line segments, if not then the intersection line is
         * outside.
         * 
         * <p>
         * Code based on original by Paul Bourke:<br/>
         * http://local.wasp.uwa.edu.au/~pbourke/geometry/lineline3d/
         * </p>
         */

        public Vec3D closestPoint(ILine l2)
        {
            // Algorithm is ported from the C algorithm of Paul Bourke
            Vec3D p1 = start;
            Vec3D p2 = end;
            Vec3D p3 = l2.start;
            Vec3D p4 = l2.end;
            Vec3D p21 = p2.sub(p1);
            Vec3D p13 = p1.sub(p3);
            Vec3D p43 = p4.sub(p3);
            double d1343 = p13.x * (double)p43.x + (double)p13.y * p43.y + (double)p13.z * p43.z;
            double d4321 = p43.x * (double)p21.x + (double)p43.y * p21.y + (double)p43.z * p21.z;
            double d1321 = p13.x * (double)p21.x + (double)p13.y * p21.y + (double)p13.z * p21.z;
            double d4343 = p43.x * (double)p43.x + (double)p43.y * p43.y + (double)p43.z * p43.z;
            double d2121 = p21.x * (double)p21.x + (double)p21.y * p21.y + (double)p21.z * p21.z;

            double denom = d2121 * d4343 - d4321 * d4321;
            double numer = d1343 * d4321 - d1321 * d4343;

            float mua = Math.Max(Math.Min((float)(numer / denom), 1), 0);
            float mub = Math.Max(Math.Min((float)((d1343 + d4321 * (mua)) / d4343), 1), 0);
            return l2.pointAt(mub);

        }

        public Vec3D pointAt(float param)
        {
            Vec3D ab = end.sub(start);
            ab.scaleSelf(param);
            return start.add(ab);
        }

        /**
         * Computes the closest point on this line to the given one.
         * 
         * @param p
         *            point to check against
         * @return closest point on the line
         */

        public Vec3D closestPoint(ReadonlyVec3D p)
        {
            Vec3D v = end.sub(start);
            float t = p.sub(start).dot(v) / v.magSquared();
            // Check to see if t is beyond the extents of the line segment
            if (t < 0.0f)
            {
                return start.copy();
            }
            else if (t > 1.0f)
            {
                return end.copy();
            }
            // Return the point between 'a' and 'b'
            return start.add(v.scaleSelf(t));
        }

        public Line3D copy()
        {
            return new Line3D(start.copy(), end.copy());
        }

        public bool equals(Object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (this == obj)
            {
                return true;
            }
            if (!(obj is Line3D)) {
                return false;
            }
            Line3D l = (Line3D)obj;
            return (start.Equals(l.start) || start.Equals(l.end))
                    && (end.Equals(l.end) || end.Equals(l.start));
        }

        /**
         * Returns the line's axis-aligned bounding box.
         * 
         * @return aabb
         * @see toxi.geom.AABB
         */
        public AABB getBounds()
        {
            return AABB.fromMinMax(start, end);
        }

        public Vec3D getDirection()
        {
            return end.sub(start).normalize();
        }

        public float getLength()
        {
            return start.distanceTo(end);
        }

        public float getLengthSquared()
        {
            return start.distanceToSquared(end);
        }

        public Vec3D getMidPoint()
        {
            return start.add(end).scaleSelf(0.5f);
        }

        public Vec3D getNormal()
        {
            return end.cross(start);
        }

        public bool hasEndPoint(Vec3D p)
        {
            return start.Equals(p) || end.Equals(p);
        }

        /**
         * Computes a hash code ignoring the directionality of the line.
         * 
         * @return hash code
         * 
         * @see java.lang.Object#hashCode()
         * @see #hashCodeWithDirection()
         */
        public int hashCode()
        {
            return start.hashCode() + end.hashCode();
        }

        /**
         * Computes the hash code for this instance taking directionality into
         * account. A->B will produce a different hash code than B->A. If
         * directionality is not required or desired use the default
         * {@link #hashCode()} method.
         * 
         * @return hash code
         * 
         * @see #hashCode()
         */
        public int hashCodeWithDirection()
        {
            long bits = 1L;
            bits = 31L * bits + start.hashCode();
            bits = 31L * bits + end.hashCode();
            return (int)(bits ^ (bits >> 32));
        }

        public Line3D offsetAndGrowBy(float offset, float scale, Vec3D refVec)
        {
            Vec3D m = getMidPoint();
            Vec3D d = getDirection();
            Vec3D n = start.cross(d).normalize();
            if (refVec != null && m.sub(refVec).dot(n) < 0) {
                n.invert();
            }
            n.normalizeTo(offset);
            start.addSelf(n);
            end.addSelf(n);
            d.scaleSelf(scale);
            start.subSelf(d);
            end.addSelf(d);
            return this;
        }

        public Line3D scaleLength(float scale)
        {
            float delta = (1 - scale) * 0.5f;
            Vec3D newA = start.interpolateTo(end, delta);
            end.interpolateToSelf(start, delta);
            start.set(newA);
            return this;
        }

        public Line3D set(ReadonlyVec3D a, ReadonlyVec3D b)
        {
            this.start = a.copy();
            this.end = b.copy();
            return this;
        }

        public Line3D set(Vec3D a, Vec3D b)
        {
            this.start = a;
            this.end = b;
            return this;
        }

        public List<Vec3D> splitIntoSegments(List<Vec3D> segments,
                float stepLength, bool addFirst)
        {
            return splitIntoSegments(start, end, stepLength, segments, addFirst);
        }



        public float angleBetween(Line3D other, bool flip)
        {

            Vec3D ab = start.sub(end);
            Vec3D abo = other.start.sub(other.end);
            if (flip) abo.invert();
            return ab.angleBetween(abo, true);
        }
        /*
        public Ray3D toRay3D()
        {
            return new Ray3D(a.copy(), getDirection());
        }*/

        public override string ToString()
        {
            return start.ToString() + " -> " + end.ToString();
        }
    }
}
