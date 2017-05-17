using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Core
{
    /// <summary>
    /// Particle implementation adds acceleration, velocity and mass to Plane3D objects for physics simulations.
    /// Contains a number of convenience properties for tagging particles, assigning radius or length properties for
    /// custom behaviours.
    /// </summary>
    public class SRParticle : Plane3D, IParticle
    {
        public IParticle parent { get; set; }

        public Vec3D accel { get; set; } = new Vec3D();
        public Vec3D vel { get; } = new Vec3D();

        public int age = 0;
        protected float inertia = 1;
        public float mass { get; set; } = 1;
        public float radius { get; set; } = 0;

        public string tag { get; set; } = "";
        public bool f { get; set; } = false; //TODO sort out better locking system

        /// <summary>
        /// Default constuctor creates a particle oriented to the world XY plane with given origin.
        /// </summary>
        /// <param name="_x">X coordinate of particle</param>
        /// <param name="_y">Y coordinate of particle</param>
        /// <param name="_z">Z coordinate of particle</param>
        public SRParticle(float _x, float _y, float _z) : this(new Vec3D(_x, _y, _z)) { }

        /// <summary>
        /// Default constuctor creates a particle oriented to the world XY plane with given origin. 
        /// </summary>
        /// <param name="_o">Origin of particle</param>
        public SRParticle(Vec3D _o) : base(_o) { }

        /// <summary>
        /// Default constructor creates a particle by copying a plane
        /// </summary>
        /// <param name="_p"></param>
        public SRParticle(Plane3D _p) : base(_p) { }

        /// <summary>
        /// Checks whether particle is within a given distance from the origin
        /// </summary>
        /// <param name="extents"></param>
        /// <returns></returns>
        public bool inBounds(int extents)
        {
            if (x < -extents || x > extents || y < -extents || y > extents || z < -extents || z > extents) return false;
            return true;
        }

        /// <summary>
        /// Adds a force to the particle acceleration
        /// </summary>
        /// <param name="force"></param>
        public virtual void addForce(Vec3D force)
        {
            if (force.magnitude() > 0.0001) accel.addSelf(force);
        }

        /// <summary>
        /// Calls the particle integrate method, increments particle age and reset acceleration
        /// </summary>
        /// <param name="dt"></param>
        public virtual void step(float dt)
        {
            if (!f)
            {
                integrate(dt);
                age++;
            }
            reset();
        }

        /// <summary>
        /// integrates acceleration + velocity into particle position
        /// </summary>
        /// <param name="dt"></param>
        public virtual void integrate(float dt)
        {
            if (mass > 0)
            {
                vel.addSelf(accel.scale(1 / mass));
                vel.scaleSelf(dt * inertia);
            }
            else
            {
                //vel = accel
                vel.set(accel.scale((dt*inertia)));
            }
            addSelf(vel);
        }

        /// <summary>
        /// Gets acceleration as an Impulse
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<Impulse> getImpulse()
        {
            yield return new Impulse(this, accel, false);
        }

        /// <summary>
        /// Resets acceleration
        /// </summary>
        public virtual void reset()
        {
            accel = new Vec3D();
            inertia = 0.99f;
        }

        /// <summary>
        /// Gets any inertia applied to the particle 
        /// </summary>
        /// <returns></returns>
        public float getInertia()
        {
            return inertia;
        }

        /// <summary>
        /// Gets the current particle speed
        /// </summary>
        /// <returns></returns>
        public float getSpeed()
        {
            return vel.magnitude();
        }

        /// <summary>
        /// Gets a copy of the particle velocity
        /// </summary>
        /// <returns></returns>
        public Vec3D getVel()
        {
            return vel.copy();
        }

        /// <summary>
        /// Gets a copy of the particle acceleration
        /// </summary>
        /// <returns></returns>
        public Vec3D getAccel()
        {
            return accel.copy();
        }

        /// <summary>
        /// Gets the particle plane
        /// </summary>
        /// <returns></returns>
        public Plane3D get()
        {
            return this;
        }

        /// <summary>
        /// Returns a vector defining the orientation of the particle as xx+yy+zz
        /// </summary>
        /// <returns></returns>
        public Vec3D getExtents()
        {
            return xx.add(yy).add(zz);
        }

        /// <summary>
        /// Gets the speed of the particle if age>0
        /// </summary>
        /// <returns></returns>
        public float getDeltaForStep()
        {
            return (age>0)?vel.magnitude():1;
        }

        /// <summary>
        /// Scales the inertia of the particle by a given factor
        /// </summary>
        /// <param name="factor"></param>
        public void scaleInertia(float factor)
        {
            setInertia(inertia *= factor);
        }

        /// <summary>
        /// Adds the the inertia of the particle
        /// </summary>
        /// <param name="mod"></param>
        public void addInertia(float mod)
        {
            setInertia(inertia += mod);
        }

        /// <summary>
        /// Resets the intertia of the particle
        /// </summary>
        /// <param name="val"></param>
        public void setInertia(float val)
        {
            inertia = val;
            if (inertia > 1) inertia = 1;
            if (inertia < 0) inertia = 0;
        }
    }
}
