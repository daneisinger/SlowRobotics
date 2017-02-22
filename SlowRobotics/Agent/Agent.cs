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
    public class AgentT<T> : Agent,  IBenchmark, IAgentT<T> where T : class
    {

        public List<Vec3D> neighbours { get; set; }
        private T data;
        public event EventHandler<UpdateEventArgs> OnUpdate;

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
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            foreach (IBehaviour b in behaviours.getData())
            {
                b.run(this);
            }

            stopwatch.Stop();
            if ((OnUpdate != null)) OnUpdate(this, new UpdateEventArgs(this.GetType().ToString(), stopwatch.ElapsedMilliseconds));
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
