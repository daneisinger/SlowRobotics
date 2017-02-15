using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Core
{
    public class SRParticle : Plane3D, IParticle
    {
        protected Vec3D accel = new Vec3D();
        protected Vec3D vel = new Vec3D();
        public float spd = 1;
        float accLimit = 1;
        public int age = 0;
        private float inertia = 1;
        public IParticle parent { get; set; }

        //TODO need mass property

        public bool f { get; set; } = false; //TODO sort out better locking system

        public SRParticle(float _x, float _y, float _z) : this(new Vec3D(_x, _y, _z)) { }
        public SRParticle(Vec3D _o) : base(_o) { }
        public SRParticle(Plane3D _p) : base(_p) { }

        public bool inBounds(int extents)
        {
            if (x < -extents || x > extents || y < -extents || y > extents || z < -extents || z > extents) return false;
            return true;
        }

        public void addForce(Vec3D force)
        {
            if (force.magnitude() > 0.001) accel.addSelf(force);
        }

        public void addForceAndUpdate(Vec3D force)
        {
            if (force.magnitude() > 0.001)
            {
                accel.addSelf(force);
                step(1);
            }
        }

        public virtual void step(float dt)
        {
            accel.limit(accLimit);
            
            if (!f)
            {
                vel.addSelf(accel);
                vel.limit(spd);
                vel.scaleSelf(dt * inertia);
                addSelf(vel);
                age++;
            }

            accel = new Vec3D();
            inertia = 0.97f; //add default inertia to slow everything down
        }

        public Vec3D getLimitedAccel()
        {
            return accel.getLimited(accLimit);
        }

        public void resetAccel()
        {
            accel = new Vec3D();
        }

        public void setSpeed(float s)
        {
            spd = s;
        }

        public void setAccel(float a)
        {
            accLimit = a;
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
