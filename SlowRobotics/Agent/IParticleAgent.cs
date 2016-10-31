using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Agent
{
    /// <summary>
    /// Interface for agents that move in continuous space
    /// e.g. boid systems
    /// </summary>
    public interface IParticleAgent : IAgent
    {

        Particle getParticle();
        void addForce(Vec3D force);
    }
}
