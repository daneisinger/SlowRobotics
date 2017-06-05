using SlowRobotics.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toxiclibs.core;
using SlowRobotics.SRMath;

namespace SlowRobotics.Core
{
    /// <summary>
    /// Structure for handling impulses (force + location)
    /// </summary>
    public struct Impulse
    {
        public Vec3D pos { get; }
        public Vec3D dir { get; }
        public bool torqueOnly { get; }
        public Impulse(Vec3D pos, Vec3D dir, bool torqueOnly)
        {
            this.pos = pos;
            this.dir = dir;
            this.torqueOnly = torqueOnly;
        }
    }

    /// <summary>
    /// Extension of SRParticle to add linear dimension and impulses
    /// </summary>
    public class SRLinearParticle : SRParticle, ILine
    {
        public double length = 1;
        private List<Impulse> impulses;
        public Vec3D Torque { get; set; }

        /// <summary>
        /// Default constructor with length 1
        /// </summary>
        /// <param name="_p"></param>
        public SRLinearParticle(Plane3D _p) : base(_p) {
            impulses = new List<Impulse>();
            Torque = new Vec3D();
        }

        /// <summary>
        /// Gets position of start of line
        /// </summary>
        public Vec3D start
        {
            get
            {
                return pointAt(-1f);
            }
            set { }
        }

        /// <summary>
        /// Gets position of end of line
        /// </summary>
        public Vec3D end
        {
            get
            {
                return pointAt(1f);
            }
            set { }
        }

        /// <summary>
        /// returns point at normalized parameter
        /// </summary>
        /// <param name="f">Parameter</param>
        /// <returns></returns>
        public Vec3D pointAt(float f)
        {
            return add(zz.scale(f * (float)length));
        }

        /// <summary>
        /// Aplies a force at the centre of the particle
        /// </summary>
        /// <param name="force"></param>
        public override void addForce(Vec3D force)
        {
            addForce(this, force);
        }

        /// <summary>
        /// Applies a force at a parameter value between -1 and 1
        /// </summary>
        /// <param name="param">Parameter</param>
        /// <param name="force">Force to apply</param>
        public void addForce (float param, Vec3D force)
        {
            addForce(this.add(this.zz.scale(-Math.Abs(param))), force);
        }

        /// <summary>
        /// Applies a force at a given location
        /// </summary>
        /// <param name="pos">Position</param>
        /// <param name="force">Force</param>
        public void addForce(Vec3D pos, Vec3D force)
        {
            addForce(pos, force, false);
        }

        /// <summary>
        /// Applies a torque only (rotational) force at a given location
        /// </summary>
        /// <param name="pos">Position</param>
        /// <param name="force">Force</param>
        /// <param name="torqueOnly">Only apply a rotational force</param>
        public void addForce(Vec3D pos, Vec3D force, bool torqueOnly)
        {
            impulses.Add(new Impulse(pos, force, torqueOnly));
        }

        /// <summary>
        /// Resets all parameters of the LinearParticle
        /// </summary>
        public override void reset()
        {
            impulses = new List<Impulse>();
            Torque = new Vec3D();
            accel = new Vec3D();
            inertia = 0.99f;
        }

        /// <summary>
        /// Integrates impulses into particle acceleration and torque
        /// </summary>
        /// <param name="impulses"></param>
        public void ApplyImpulses(IEnumerable<Impulse> impulses)
        {
            foreach (Impulse i in impulses)
            {

                if (!i.torqueOnly) accel.addSelf(i.dir); //integrate force

                Vec3D ab = i.pos.sub(this);
                float d = ab.magnitude();
                if (d > 0)
                {
                    Vec3D crossAb = ab.cross(i.dir);
                    float a = ab.angleBetween(ab.add(i.dir), true);

                    if (!float.IsNaN(a)) Torque.addSelf(crossAb.scale(a)); //integrate torque
                }
            }
        }

        /// <summary>
        /// Integrates accel and torque into particle position and orientation
        /// </summary>
        /// <param name="dt"></param>
        public override void integrate(float dt)
        {
            ApplyImpulses(impulses);
            base.integrate(dt);

            Matrix4x4 t = new Matrix4x4();
            if (Torque.magSquared() > 0)
            {
                Quaternion rot = Quaternion.createFromAxisAngle(Torque, Torque.magnitude() * 1f * dt);
                rot.normalize();
                t = rot.toMatrix4x4();
                transform(t); //transform this plane
            }
            
        }

        /// <summary>
        /// Returns a list of impulses currently applied to the particle
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<Impulse> getImpulse()
        {
            foreach (Impulse i in impulses) yield return i;
        }

        /// <summary>
        /// Gets the closest point on the line
        /// </summary>
        /// <param name="p">Point to search from</param>
        /// <returns></returns>
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

        /// <summary>
        /// gets the closest point between two lines
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Vec3D closestPoint(ILine other)
        {
            return MathUtils.closestPoint(this, other);
        }

        public override IParticle duplicate()
        {
            SRLinearParticle copy = new SRLinearParticle(this);
            copy.copyAttributes(this);
            copy.length = length;
            return copy;
        }
    }
}
