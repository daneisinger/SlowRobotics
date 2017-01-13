using SlowRobotics.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Agent
{

    /// <summary>
    /// Abstract agent implementation
    /// </summary>
    public abstract class Agent : IAgent
    {
        public List<Vec3D> neighbours { get; set; }
        public PriorityQueue<IBehaviour> behaviours { get; set; }

        public Agent()
        {
            behaviours = new PriorityQueue<IBehaviour>();
            neighbours = new List<Vec3D>();
        }

        public abstract void step(float damping);
        public abstract float getDeltaForStep();

        public bool hasNeighbours()
        {
            return neighbours.Count > 0;
        }

        public void addNeighbours(List<Vec3D> n)
        {
            neighbours.AddRange(n);
        }

        public void addBehaviour(IBehaviour b)
        {
            behaviours.Enqueue(b);
        }

        public void addBehaviours(List<IBehaviour> newBehaviours)
        {
            foreach (IBehaviour b in newBehaviours) behaviours.Enqueue(b);
        }

        public List<IBehaviour> getBehaviours()
        {
            return behaviours.getData();
        }

        public void setBehaviours(List<IBehaviour> newBehaviours)
        {
            behaviours = new PriorityQueue<IBehaviour>();
            foreach (IBehaviour b in newBehaviours) behaviours.Enqueue(b);
        }

        public void removeBehaviours()
        {
            behaviours = new PriorityQueue<IBehaviour>();
        }
    }

    /// <summary>
    /// Generic class for wrapping a single object type in an agent. 
    /// Note - this generic implementation does not handle delta calculations
    /// or perform any object updates - these must be performed using behaviours
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AgentT<T> : Agent, IAgentT<T> where T : class
    {

        private T data;

        public AgentT(T _data)
        {
            data = _data;
        }

        public T getData()
        {
            return data;
        }

        public override float getDeltaForStep()
        {
            return 1;
        }

        public override void step(float damping)
        {
            foreach (IBehaviour b in behaviours.getData())
            {
                b.run(this);
            }
        }
    }
}
