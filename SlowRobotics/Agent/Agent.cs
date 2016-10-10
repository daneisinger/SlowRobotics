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
    public interface Agent
    {
        PriorityQueue<Behaviour> behaviours { get; set; }

        World world { get; set; }

        void step(float damping);

        void addBehaviour(Behaviour b);


    }
}
