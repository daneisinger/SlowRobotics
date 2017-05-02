using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Core
{
    public interface IParticle
    {
        string tag { get; set; }
        float mass { get; set; }
        float radius { get; set; }

        Plane3D get();

        Vec3D getExtents();
        void step(float dt);

        
    }
}
