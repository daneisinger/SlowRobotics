using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Agent.Behaviours
{
    public class Newton : PlaneAgentBehaviour
    {
        public Vec3D force { get; set; }

        public Newton(int _priority, Vec3D _force) : base(_priority)
        {
            force = _force;
        }

        public override void run(PlaneAgent a)
        {
            a.addForce(force);
        }

    }
}
