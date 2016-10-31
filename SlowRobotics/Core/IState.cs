using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Core
{
    /// <summary>
    /// Interface for storing states in the world
    /// </summary>
    public interface IState
    {
        Vec3D getPos();
        IState copyState();
    }
}
