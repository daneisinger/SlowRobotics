using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Core
{
    /// <summary>
    /// Particle interface
    /// </summary>
    public interface IParticle
    {
        string tag { get; set; }
        float mass { get; set; }
        float radius { get; set; }
        bool f { get; set; }

        Plane3D get();
        IEnumerable<Impulse> getImpulse();

        Vec3D vel { get; }
        Vec3D accel { get; set; }

        Vec3D getExtents();
        void step(float dt);

        IParticle duplicate();
    }
}
