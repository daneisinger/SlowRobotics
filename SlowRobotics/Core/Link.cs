using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Core
{
    public class Link
    {
        public Node a;
        public Node b;
        public float l;
        public float stiffness { get; set; }

        public Link(Node _a, Node _b, float _l)
        {
            a = _a;
            b = _b;
            l = _l;
            stiffness = 0.08f;
        }

        public Link(Node _a, Node _b)
        {
            a = _a;
            b = _b;
            updateLength();
            stiffness = 0.08f;
        }
        
        public bool replaceNode(Node oldN, Node newN)
        {
            if(oldN== a)
            {
                a = newN;
                updateLength();
                return true;
            }else if(oldN== b)
            {
                b = newN;
                updateLength();
                return true;
            }
            return false;
        }

        public float angleBetween(Link other, bool flip)
        {
            Vec3D ab = a.sub(b);
            Vec3D abo = other.a.sub(other.b);
            if (flip) abo.invert();
            return ab.angleBetween(abo, true);
        }

        public Vec3D closestPt(Vec3D p)
        {
            Vec3D dir = b.sub(a);
            float t = p.sub(a).dot(dir) / dir.magSquared();
            if (t > 1) t = 1;
            if (t < 0) t = 0;
            return a.add(dir.scaleSelf(t));
        }

        public bool closestPtBetweenLinks(Link l1, Link l2, Vec3D a, Vec3D b)
        {
            // Algorithm is ported from the C algorithm of 
            // Paul Bourke at http://local.wasp.uwa.edu.au/~pbourke/geometry/lineline3d/
            Vec3D resultSegmentPoint1 = new Vec3D();
            Vec3D resultSegmentPoint2 = new Vec3D();

            Vec3D p1 = l1.a;
            Vec3D p2 = l1.b;
            Vec3D p3 = l2.a;
            Vec3D p4 = l2.b;
            Vec3D p13 = p1.sub(p3);
            Vec3D p43 = p4.sub(p3);

            if (p43.magSquared() < Math.E)
            {
                return false;
            }
            Vec3D p21 = p2.sub(p1);
            if (p21.magSquared() < Math.E)
            {
                return false;
            }

            double d1343 = p13.x * (double)p43.x + (double)p13.y * p43.y + (double)p13.z * p43.z;
            double d4321 = p43.x * (double)p21.x + (double)p43.y * p21.y + (double)p43.z * p21.z;
            double d1321 = p13.x * (double)p21.x + (double)p13.y * p21.y + (double)p13.z * p21.z;
            double d4343 = p43.x * (double)p43.x + (double)p43.y * p43.y + (double)p43.z * p43.z;
            double d2121 = p21.x * (double)p21.x + (double)p21.y * p21.y + (double)p21.z * p21.z;

            double denom = d2121 * d4343 - d4321 * d4321;
            if (Math.Abs(denom) < Math.E)
            {
                return false;
            }
            double numer = d1343 * d4321 - d1321 * d4343;

            float mua = Math.Max(Math.Min((float)(numer / denom),1),0);
            float mub = Math.Max(Math.Min((float)((d1343 + d4321 * (mua)) / d4343), 1),0);

            // if((mua==0 && mub != 0) || (mua == 1 && mub != 1))return false;
            a.x = (p1.x + mua * p21.x);
            a.y = (p1.y + mua * p21.y);
            a.z = (p1.z + mua * p21.z);
            b.x = (p3.x + mub * p43.x);
            b.y = (p3.y + mub * p43.y);
            b.z = (p3.z + mub * p43.z);
            return true;
        }

        public void updateLength()
        {
            l = a.distanceTo(b);
        }

        public Vec3D getDir()
        {
            return b.sub(a).normalize();
        }

        public float getLength()
        {
            return a.distanceTo(b);
        }

        public Node tryGetOther(Node test)
        {
            if (a == test) return b;
            return a;
        }
    }
}
