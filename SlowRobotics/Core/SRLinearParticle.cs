﻿using SlowRobotics.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toxiclibs.core;
using SlowRobotics.SRMath;

namespace SlowRobotics.Core
{
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

    public class SRLinearParticle : SRParticle, ILine
    {
        public double length;
        private List<Impulse> impulses;

        public SRLinearParticle(Plane3D _p) : base(_p) {
            impulses = new List<Impulse>();

        }

        public Vec3D start
        {
            get
            {
                return pointAt(-1f);
            }
            set { }
        }

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
        /// <param name="f"></param>
        /// <returns></returns>
        public Vec3D pointAt(float f)
        {
            return add(zz.scale(f * (float)length));
        }

        public override void addForce(Vec3D force)
        {
            addForce(this, force);
        }

        /// <summary>
        /// Adds a force at a parameter value between -1 and 1
        /// </summary>
        /// <param name="param"></param>
        /// <param name="force"></param>
        public void addForce (float param, Vec3D force)
        {
            addForce(this.add(this.zz.scale(-Math.Abs(param))), force);
        }

        public void addForce(Vec3D pos, Vec3D force)
        {
            addForce(pos, force, false);
        }

        public void addForce(Vec3D pos, Vec3D force, bool torqueOnly)
        {
            impulses.Add(new Impulse(pos, force.getLimited(accLimit), torqueOnly));
        }

        public override void reset()
        {
            impulses = new List<Impulse>();

        }
        
        public override Vec3D getAccel()
        {
            foreach(Impulse i in getImpulse()) accel.addSelf(i.dir); //integrate force
            return accel.getLimited(accLimit);
        }

        public override IEnumerable<Impulse> getImpulse()
        {
            foreach (Impulse i in impulses) yield return i;
        }

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

        public Vec3D closestPoint(ILine other)
        {
            return MathUtils.closestPoint(this, other);
        }
    }
}
