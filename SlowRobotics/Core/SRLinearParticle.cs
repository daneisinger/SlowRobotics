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
        public double length;

        public SRLinearParticle(float _x, float _y, float _z) : this(new Vec3D(_x, _y, _z)) { }
        public SRLinearParticle(Vec3D _o) : base(_o) { }
        public SRLinearParticle(Plane3D _p) : base(_p) { }


    }
}
