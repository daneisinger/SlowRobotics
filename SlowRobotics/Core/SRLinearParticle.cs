using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toxiclibs.core;

namespace SlowRobotics.Core
{
    public struct Impulse
    {
        public Vec3D pos { get; }
        public Vec3D dir { get; }
        public Impulse(Vec3D pos, Vec3D dir)
        {
            this.pos = pos;
            this.dir = dir;
        }
    }

    public class SRLinearParticle : SRParticle
    {
        public double length;
        private List<Impulse> impulses;

        public SRLinearParticle(Plane3D _p) : base(_p) {
            impulses = new List<Impulse>();

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
            impulses.Add(new Impulse(pos, force));
        }

        public void reset()
        {
            impulses = new List<Impulse>();

        }

        public override IEnumerable<Impulse> getImpulse()
        {
            foreach (Impulse i in impulses) yield return i;
        }
    }
}
