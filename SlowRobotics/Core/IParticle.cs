using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Core
{
    public interface IParticle
    {

        void addInertia(float modifier);
        void addForce(Vec3D force);
    }
}
