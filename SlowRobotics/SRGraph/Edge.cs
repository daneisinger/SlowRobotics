using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.SRGraph
{
    public class Edge<T> : IEdge<T>
    {
        public INode<T> a { get; set; }
        public INode<T> b { get; set; }


        public Edge(INode<T> _start, INode<T> _end)
        {
            a = _start;
            b = _end;
        }

        public void cleanup()
        {
            a.remove(this);
            b.remove(this);
        }

        public INode<T> Other (INode<T> toThis)
        {
            if (a == toThis) return b;
            if (b == toThis) return a;
            return null;
        }
    }

    public class Spring : Edge<SRParticle>
    {
        public float l { get; set; }
        public float s { get; set; }
        public string tag { get; set; }

        public Spring(SRParticle _start, SRParticle _end) : this(new Node<SRParticle>(_start), new Node<SRParticle>(_end)) { }

        public Spring(INode<SRParticle> _start, INode<SRParticle> _end) : base(_start, _end)
        {
            updateLength();
            s = 0.08f;
            tag = "";
        }

        public void updateLength()
        {
            l = a.Geometry.distanceTo(b.Geometry);
        }

        public Vec3D getDir()
        {
            return b.Geometry.sub(a.Geometry).normalize();
        }

        public float getLength()
        {
            return a.Geometry.distanceTo(b.Geometry);
        }

        /*

    //From old link class

    public LegacyLink replaceNode(LegacyNode oldN, LegacyNode newN)
        {
            if(oldN== a)
            {
                return new LegacyLink(newN, b);
            }else if(oldN== b)
            {
                return new LegacyLink(a, newN);
            }
            return null;
        }

        public bool getNaked(out LegacyNode n)
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

        public void split(LegacyNode splitPt)
        {
            LegacyLink a_s = new LegacyLink(splitPt, a);
            LegacyLink b_s = new LegacyLink(splitPt, b);

            splitPt.connect(a_s);
            splitPt.connect(b_s);

            a.replaceLink(this, a_s);
            b.replaceLink(this, b_s);

            
        }

        public float angleBetween(LegacyLink other, bool flip)
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

        public static void closestPtBetweenLinks(LegacyLink l1, LegacyLink l2, ref Vec3D a, ref Vec3D b)
        {

            //TODO - sort out this mess
            b = closestOnB(l1, l2);
            a = closestOnB(l2, l1);
        }

        private static Vec3D closestOnB(LegacyLink l1, LegacyLink l2)
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

    */
    }
}
