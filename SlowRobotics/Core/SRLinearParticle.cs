using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toxiclibs.core;

namespace SlowRobotics.Core
{
    public class SRLinearParticle : SRParticle
    {

        public struct Force
        {
            Vec3D pos { get; }
            Vec3D dir { get; }
            public Force(Vec3D pos,Vec3D dir)
            {
                this.pos = pos;
                this.dir = dir;
            }
        }

        public double length;
        private List<Force> forces;

        public SRLinearParticle(float _x, float _y, float _z) : this(new Vec3D(_x, _y, _z)) { }
        public SRLinearParticle(Vec3D _o) : base(_o) { }
        public SRLinearParticle(Plane3D _p) : base(_p) { }

        public override void addForce(Vec3D force)
        {
            addForce(this, force);
        }

        public void addForce(Vec3D pos, Vec3D force)
        {
            forces.Add(new Force(pos, force));
        }

        public List<Force> Forces
        {
            get
            {
                return forces;
            }
        }

        public override void step(float dt)
        {
            //reset?
        }
    }
}
