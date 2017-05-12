using SlowRobotics.SRGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toxiclibs.core;

namespace SlowRobotics.Core
{
    /// <summary>
    /// Vertlet Spring implementation. Can be used as an edge in a Graph<SRParticle,Spring> class.
    /// </summary>
    public class Spring : Line3D, IEdge<SRParticle>
    {
        public INode<SRParticle> a { get; set; }
        public INode<SRParticle> b { get; set; }

        public float l { get; set; }
        public float s { get; set; }
        public string tag { get; set; }

        /// <summary>
        /// Default constructor with stiffness 0.08 and rest length equal to distance between two particles
        /// </summary>
        /// <param name="_start">Start of spring</param>
        /// <param name="_end">End of spring</param>
        public Spring(SRParticle _start, SRParticle _end) : this(new Node<SRParticle>(_start), new Node<SRParticle>(_end)) { }

        /// <summary>
        /// Creates a spring from an Edge object
        /// </summary>
        /// <param name="edge"></param>
        public Spring(Edge<SRParticle> edge) : this(edge.a, edge.b) { }

        /// <summary>
        /// Creates a spring between two Nodes with stiffness 0.08 and length equal to distance between two particles
        /// </summary>
        /// <param name="_start"></param>
        /// <param name="_end"></param>
        public Spring(INode<SRParticle> _start, INode<SRParticle> _end) : base(_start.Geometry, _end.Geometry)
        {
            a = _start;
            b = _end;

            updateLength();
            s = 0.08f;
            tag = "";
        }

        /// <summary>
        /// Updates spring rest length to be equal to distance between start and end of spring
        /// </summary>
        public void updateLength()
        {
            l = a.Geometry.distanceTo(b.Geometry);
        }

        /// <summary>
        /// Gets direction of the spring
        /// </summary>
        /// <returns></returns>
        public Vec3D getDir()
        {
            return b.Geometry.sub(a.Geometry).normalize();
        }

        /// <summary>
        /// Gets current length of spring
        /// </summary>
        /// <returns></returns>
        public new float getLength()
        {
            return a.Geometry.distanceTo(b.Geometry);
        }

        /// <summary>
        /// Static method for finding closest points between two lines
        /// </summary>
        /// <param name="l1">First line</param>
        /// <param name="l2">Second line</param>
        /// <param name="a">Closest point on first line</param>
        /// <param name="b">Closest point on second line</param>
        public static void closestPoints(Line3D l1, Line3D l2, out Vec3D a, out Vec3D b)
        {

            //TODO - sort out this mess
            b = l1.closestPoint(l2);
            a = l2.closestPoint(l1);
        }

        /// <summary>
        /// Deletes reference to this spring from both nodes
        /// </summary>
        public void cleanup()
        {
            a.remove(this);
            b.remove(this);
        }

        /// <summary>
        /// Gets the other end of the spring
        /// </summary>
        /// <param name="toThis">Node to test against</param>
        /// <returns></returns>
        public INode<SRParticle> Other(INode<SRParticle> toThis)
        {
            if (a == toThis) return b;
            if (b == toThis) return a;
            return null;
        }

        /// <summary>
        /// Gets a common node between this spring and some other edge if it exists
        /// </summary>
        /// <param name="toThis">Edge to find common node</param>
        /// <returns></returns>
        public INode<SRParticle> Common (IEdge<SRParticle> toThis)
        {
            if (a == toThis.a || a == toThis.b) return a;
            if (b == toThis.b || b == toThis.b) return b;
            return null;
        }

        /// <summary>
        /// Replaces a node
        /// </summary>
        /// <param name="replaceThis">Node to replace</param>
        /// <param name="withThat">Replacement</param>
        /// <returns></returns>
        public bool replaceNode(INode<SRParticle> replaceThis, INode<SRParticle> withThat)
        {
            if (a == replaceThis)
            {
                a = withThat;
                start = withThat.Geometry;
                return true;
            }
            else if (b == replaceThis)
            {
                b = withThat;
                end = withThat.Geometry;
                return true;
            }
            return false;
        }

    }
}
