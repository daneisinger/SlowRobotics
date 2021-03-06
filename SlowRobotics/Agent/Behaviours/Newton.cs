﻿using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Agent.Behaviours
{
    /// <summary>
    /// Applies a given force to the particle.
    /// </summary>
    public class Newton : ScaledBehaviour<SRParticle>
    {
        public Vec3D force { get; set; }

        public Newton(int _priority, Vec3D _force) : base(_priority)
        {
            force = _force;
        }

        public override void runOn(SRParticle p)
        {
            p.addForce(force.scale(scaleFactor));
        }

    }
}
