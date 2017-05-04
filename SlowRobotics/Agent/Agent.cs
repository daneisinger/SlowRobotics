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
    /// Abstract agent implementation
    /// </summary>
    public abstract class Agent : IAgent 
    {
        
        public PriorityQueue<IBehaviour> behaviours { get; set; }

        public Agent()
        {
            behaviours = new PriorityQueue<IBehaviour>();
        }

        public abstract void step(float damping);
        public abstract void lateUpdate(float damping);
       
        public void addBehaviour(IBehaviour b)
        {
            behaviours.Enqueue(b);
            b.onAdd();
        }
        
        public void addBehaviours(List<IBehaviour> newBehaviours)
        {
            foreach (IBehaviour b in newBehaviours) addBehaviour(b);
        }

        public void setBehaviours(List<IBehaviour> newBehaviours)
        {
            behaviours = new PriorityQueue<IBehaviour>();
            addBehaviours(newBehaviours);
        }
     

        public List<IBehaviour> getBehaviours()
        {
            return behaviours.getData();
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

        public List<Vec3D> neighbours { get; set; }
        private T data;

        public AgentT(T _data)
        {
            data = _data;
            neighbours = new List<Vec3D>();
        }

        public T getData()
        {
            return data;
        }

        public override void step(float damping)
        {
            foreach (IBehaviour b in behaviours.getData())
            {
              if(!b.lateUpdate)b.run(this);
            }
        }

        public override void lateUpdate(float damping)
        {
            foreach (IBehaviour b in behaviours.getData())
            {
                if (b.lateUpdate) b.run(this);
            }
        }

        public bool hasNeighbours()
        {
            return neighbours.Count > 0;
        }

        public void addNeighbours(List<Vec3D> n)
        {
            neighbours.AddRange(n);
        }
    }
}
