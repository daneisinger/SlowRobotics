using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.Agent
{
    public interface IWorldBehaviour : IBehaviour
    {
        /// <summary>
        /// Adds world property to behaviour
        /// </summary>
        IWorld world { get; set; }

    }
}
