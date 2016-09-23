using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Core
{
    public class Particle : Node
    {
        protected Vec3D accel = new Vec3D();
        protected Vec3D vel = new Vec3D();
        float spd = 1;
        float accLimit = 1;
        public int age = 0;
        protected bool f = false; //TODO remove this system for fixing and just use nodes instead
        private float inertia = 1;

        public Particle(float _x, float _y, float _z) : base(_x, _y, _z) { }
        public Particle(Vec3D _o) : base(_o) { }
        public Particle(Node _n) : base(_n) { }

        public bool inBounds(int extents)
        {
            if (x < -extents || x > extents || y < -extents || y > extents || z < -extents || z > extents) return false;
            return true;
        }

        public float constrain(float v, float min, float max)
        {
            if (v < min)
            {
                return min;
            }
            else if (v > max)
            {
                return max;
            }
            else return v;
        }

        public float map(float v, float min, float max, float min2, float max2)
        {
            return min2 + ((v / (max - min)) * (max2 - min2));
        }

        public float lerp(float min, float max, float f)
        {
            return min + (max - min) * f;
        }

        public float lerp(int min, int max, float f)
        {
            return lerp((float)min, (float)max, f);
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
                update();
            }
        }

        public override void update()
        {
            update(1);
        }

        public void update(float damping)
        {
            // accel.limit(10);
            if (!f)
            {
                accel.limit(accLimit);
                vel.addSelf(accel);
                vel.limit(spd);
                vel.scaleSelf(damping * inertia);
                addSelf(vel);

                age++;

            }
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

        public void Lock(){
	  		f = true;
	  	}

    public bool locked()
    {
        return f;
    }
}
}
