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
    /// queue of behaviours. See abstract and generic implementations for
    /// extended documentation of interface methods.
    /// </summary>
    public interface IAgent
    {
        PriorityQueue<IBehaviour> behaviours { get; set; }
        void step();
        void lateUpdate();
        void addBehaviour(IBehaviour b);
        void addBehaviours(List<IBehaviour> behaviours);
        void setBehaviours(List<IBehaviour> behaviours);
        void removeBehaviours();
    }

    /// <summary>
    /// Generic extension - provides generic data to behaviours. See abstract 
    /// and generic implementations for extended documentation of interface methods.
    /// </summary>
    /// <typeparam name="T">Object type for agent data</typeparam>
    public interface IAgent<out T> : IAgent where T :class
    {
        List<Vec3D> neighbours { get; set; }
        bool hasNeighbours();
        void addNeighbours(List<Vec3D> neighbours);
        T getData();
    }
}
