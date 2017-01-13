using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Core
{

    //TODO make halfedges for easier splitting

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
        
        public Link replaceNode(Node oldN, Node newN)
        {
            if(oldN== a)
            {
                return new Link(newN, b);
            }else if(oldN== b)
            {
                return new Link(a, newN);
            }
            return null;
        }

        public bool getNaked(out Node n)
        {
            if (a.links.Count <= 1)
            {
                n = a;
                return true;
            }
            if (b.links.Count <= 1)
            {
                n = b;
                return true;
            }
            n = null;
            return false;
        }

        public void split(Node splitPt)
        {
            Link a_s = new Link(splitPt, a);
            Link b_s = new Link(splitPt, b);

            splitPt.connect(a_s);
            splitPt.connect(b_s);

            a.replaceLink(this, a_s);
            b.replaceLink(this, b_s);

            
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

        public Vec3D pointAt(float param)
        {
            Vec3D ab = b.sub(a);
            ab.scaleSelf(param);
            return a.add(ab);
        }

        public static void closestPtBetweenLinks(Link l1, Link l2, ref Vec3D a, ref Vec3D b)
        {

            //TODO - sort out this mess
            b = closestOnB(l1, l2);
            a = closestOnB(l2, l1);
        }

        private static Vec3D closestOnB(Link l1, Link l2)
        {
            // Algorithm is ported from the C algorithm of Paul Bourke
            Vec3D p1 = l1.a;
            Vec3D p2 = l1.b;
            Vec3D p3 = l2.a;
            Vec3D p4 = l2.b;
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

            float mua = Math.Max(Math.Min((float)(numer / denom),1),0);
            float mub = Math.Max(Math.Min((float)((d1343 + d4321 * (mua)) / d4343), 1),0);
            return l2.pointAt(mub);

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
