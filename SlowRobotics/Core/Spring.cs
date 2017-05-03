using SlowRobotics.SRGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toxiclibs.core;

namespace SlowRobotics.Core
{
    public class Spring : Line3D, IEdge<SRParticle>
    {
        public INode<SRParticle> a { get; set; }
        public INode<SRParticle> b { get; set; }

        public float l { get; set; }
        public float s { get; set; }
        public string tag { get; set; }

        public Spring(SRParticle _start, SRParticle _end) : this(new Node<SRParticle>(_start), new Node<SRParticle>(_end)) { }

        public Spring(Edge<SRParticle> edge) : this(edge.a, edge.b) { }

        public Spring(INode<SRParticle> _start, INode<SRParticle> _end) : base(_start.Geometry, _end.Geometry)
        {
            a = _start;
            b = _end;

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

        public new float getLength()
        {
            return a.Geometry.distanceTo(b.Geometry);
        }

        public static void closestPoints(Line3D l1, Line3D l2, out Vec3D a, out Vec3D b)
        {

            //TODO - sort out this mess
            b = l1.closestPoint(l2);
            a = l2.closestPoint(l1);
        }

        public void cleanup()
        {
            a.remove(this);
            b.remove(this);
        }

        public INode<SRParticle> Other(INode<SRParticle> toThis)
        {
            if (a == toThis) return b;
            if (b == toThis) return a;
            return null;
        }

        public INode<SRParticle> Common (IEdge<SRParticle> toThis)
        {
            if (a == toThis.a || a == toThis.b) return a;
            if (b == toThis.b || b == toThis.b) return b;
            return null;
        }

        public bool replaceNode(INode<SRParticle> replaceThis, INode<SRParticle> withThat)
        {
            if (a == replaceThis)
            {
                a = withThat;
                return true;
            }
            else if (b == replaceThis)
            {
                b = withThat;
                return true;
            }
            return false;
        }

    }
}
