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

        void step(float damping);

        float getDeltaForStep();

        void addBehaviour(IBehaviour b);
        void addBehaviours(List<IBehaviour> behaviours);
        void setBehaviours(List<IBehaviour> behaviours);

    }
}
