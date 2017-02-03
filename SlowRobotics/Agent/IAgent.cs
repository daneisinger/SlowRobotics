using SlowRobotics.Agent.Behaviours;
using SlowRobotics.Core;
using SlowRobotics.SRGraph;
using SlowRobotics.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Agent
{
    /// <summary>
    /// Agent interface - provides collections of neighbouring points and
    /// queue of behaviours
    /// </summary>
    public interface IAgent
    {
        PriorityQueue<IBehaviour> behaviours { get; set; }

        void step(float damping);

        void addBehaviour(IBehaviour b);
        void addBehaviours(List<IBehaviour> behaviours);
        void setBehaviours(List<IBehaviour> behaviours);
        void removeBehaviours();
    }

    /// <summary>
    /// Generic extension - provides generic data to behaviours
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAgentT<out T> : IAgent 
    {
        //THIS SHOULD REALLY BE A LIST OF AGENTS NOT VEC3D!

        List<Vec3D> neighbours { get; set; }
        bool hasNeighbours();
        void addNeighbours(List<Vec3D> neighbours);

        T getData();
    }
}
