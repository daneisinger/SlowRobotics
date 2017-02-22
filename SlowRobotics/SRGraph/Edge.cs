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

        public bool replaceNode(INode<T> replaceThis, INode<T> withThat)
        {
            if (a == replaceThis)
            {
                a = withThat;
                return true;
            }else if (b == replaceThis)
            {
                b = withThat;
                return true;
            }
            return false;
        }

        public IEnumerable<INode<T>> getNaked()
        {
            if (a.Naked) yield return a;
            if (b.Naked) yield return b;
        }

        public IEnumerable<Edge<T>> split(INode<T> at)
        {
            //note - does change Node edges
            //TODO - add flag for nodes to update / cleanup

            yield return new Edge<T>(a, at);
            yield return new Edge<T>(at, b);
        }
    }

    public class Spring : Edge<SRParticle>
    {
        public float l { get; set; }
        public float s { get; set; }
        public string tag { get; set; }

        public Spring(SRParticle _start, SRParticle _end) : this(new Node<SRParticle>(_start), new Node<SRParticle>(_end)) { }

        public Spring(Edge<SRParticle> edge) : this(edge.a, edge.b) { }

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

        public Vec3D pointAt(float param)
        {
            Vec3D ab = b.Geometry.sub(a.Geometry);
            ab.scaleSelf(param);
            return a.Geometry.add(ab);
        }

        public float angleBetween(Spring other, bool flip)
        {

            Vec3D ab = a.Geometry.sub(b.Geometry);
            Vec3D abo = other.a.Geometry.sub(other.b.Geometry);
            if (flip) abo.invert();
            return ab.angleBetween(abo, true);
        }

        public Vec3D closestPoint(Vec3D p)
        {
            Vec3D dir = b.Geometry.sub(a.Geometry);
            float t = p.sub(a.Geometry).dot(dir) / dir.magSquared();
            if (t > 1) t = 1;
            if (t < 0) t = 0;
            return a.Geometry.add(dir.scaleSelf(t));
        }

        public static void closestPoints(Spring l1, Spring l2, out Vec3D a, out Vec3D b)
        {

            //TODO - sort out this mess
            b = closestOnB(l1, l2);
            a = closestOnB(l2, l1);
        }

        private static Vec3D closestOnB(Spring l1, Spring l2)
        {
            // Algorithm is ported from the C algorithm of Paul Bourke
            Vec3D p1 = l1.a.Geometry;
            Vec3D p2 = l1.b.Geometry;
            Vec3D p3 = l2.a.Geometry;
            Vec3D p4 = l2.b.Geometry;
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
    }
}
