﻿using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Behaviours.TrailBehaviours
{
    public class AddForceToTrailBehaviour : TrailBehaviour
    {
        Vec3D force;

        public AddForceToTrailBehaviour(int _priority, Vec3D _force) : base(_priority)
        {
            force = _force;
        }

        override
        public void run(Agent a, int i)
        {
            Link l = a.trail[i];
            l.a.addForce(force);
        }

    }
}
