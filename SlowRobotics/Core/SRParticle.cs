using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Core
{
    public class SRParticle : Plane3D, IParticle
    {
        public IParticle parent { get; set; }

        protected Vec3D accel = new Vec3D();
        protected Vec3D vel = new Vec3D();
        //public float spd = 1;
        //public float accLimit = 1;

        public int age = 0;
        private float inertia = 1;
        public float mass { get; set; } = 1;

        public string tag { get; set; } = "";
        public bool f { get; set; } = false; //TODO sort out better locking system

        public SRParticle(float _x, float _y, float _z) : this(new Vec3D(_x, _y, _z)) { }
        public SRParticle(Vec3D _o) : base(_o) { }
        public SRParticle(Plane3D _p) : base(_p) { }

        public bool inBounds(int extents)
        {
            if (x < -extents || x > extents || y < -extents || y > extents || z < -extents || z > extents) return false;
            return true;
        }

        public virtual void addForce(Vec3D force)
        {
            if (force.magnitude() > 0.0001) accel.addSelf(force);
        }

        public virtual void step(float dt)
        {
            if (!f && mass>0)
            {
                integrate(dt);
                age++;
            }
            accel = new Vec3D();
            inertia = 0.97f; 
        }

        /// <summary>
        /// integrates acceleration + velocity into particle position
        /// </summary>
        /// <param name="dt"></param>
        public virtual void integrate(float dt)
        {
            vel.addSelf(accel.scale(1 / mass));
            vel.scaleSelf(dt * inertia);
            addSelf(vel);
        }

        public virtual IEnumerable<Impulse> getImpulse()
        {
            yield return new Impulse(this, accel, false);
        }

        public virtual void reset()
        {
            accel = new Vec3D();
        }

        public float getInertia()
        {
            return inertia;
        }

        public float getSpeed()
        {
            return vel.magnitude();
        }

        public Vec3D getVel()
        {
            return vel.copy();
        }
        public Vec3D getAccel()
        {
            return accel.copy();
        }
        public Plane3D get()
        {
            return this;
        }

        public Vec3D getExtents()
        {
            return xx.add(yy).add(zz);
        }

        public float getDeltaForStep()
        {
            return (age>0)?vel.magnitude():1;
        }

        public void scaleInertia(float factor)
        {
            setInertia(inertia *= factor);
        }

        public void addInertia(float mod)
        {
            setInertia(inertia += mod);
        }

        public void setInertia(float val)
        {
            inertia = val;
            if (inertia > 1) inertia = 1;
            if (inertia < 0) inertia = 0;
        }
    }
}
