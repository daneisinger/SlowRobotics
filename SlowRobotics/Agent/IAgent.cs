using SlowRobotics.Agent.Behaviours;
using SlowRobotics.Core;
using SlowRobotics.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Agent
{
    public interface IAgent
    {
        PriorityQueue<IBehaviour> behaviours { get; set; }

        IWorld world { get; set; }

        void step(float damping);

        void addBehaviour(IBehaviour b);

        void setBehaviours(List<IBehaviour> behaviours);

        Vec3D getPos();

    }
}
