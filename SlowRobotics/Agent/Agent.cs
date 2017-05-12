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
        /// <summary>
        /// Queue of behaviours
        /// </summary>
        public PriorityQueue<IBehaviour> behaviours { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Agent()
        {
            behaviours = new PriorityQueue<IBehaviour>();
        }

        /// <summary>
        /// Steps through all behaviours in queue
        /// </summary>
        public abstract void step();

        /// <summary>
        /// Steps through all behaviours with lateUpdate property in the queue
        /// </summary>
        public abstract void lateUpdate();
       
        /// <summary>
        /// Adds a behaviour to the queue
        /// </summary>
        /// <param name="b">Behaviour to add</param>
        public void addBehaviour(IBehaviour b)
        {
            behaviours.Enqueue(b);
            b.onAdd();
        }
        
        /// <summary>
        /// Adds a list of behaviours to the queue
        /// </summary>
        /// <param name="newBehaviours">Behaviours to add</param>
        public void addBehaviours(List<IBehaviour> newBehaviours)
        {
            foreach (IBehaviour b in newBehaviours) addBehaviour(b);
        }

        /// <summary>
        /// Replaces all behaviours in the queue with a new list of behaviours
        /// </summary>
        /// <param name="newBehaviours">New behaviours</param>
        public void setBehaviours(List<IBehaviour> newBehaviours)
        {
            behaviours = new PriorityQueue<IBehaviour>();
            addBehaviours(newBehaviours);
        }
     
        /// <summary>
        /// Gets a list of all behaviours in the queue
        /// </summary>
        /// <returns></returns>
        public List<IBehaviour> getBehaviours()
        {
            return behaviours.getData();
        }

        /// <summary>
        /// Removes all behaviours from the queue
        /// </summary>
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
    public class Agent<T> : Agent, IAgent<T> where T : class
    {

        /// <summary>
        /// List of neighbouring points - typically set with the search behaviour.
        /// </summary>
        public List<Vec3D> neighbours { get; set; }
        private T data;

        /// <summary>
        /// Create an agent that wraps a given data type
        /// </summary>
        /// <param name="_data">Object to wrap</param>
        public Agent(T _data)
        {
            data = _data;
            neighbours = new List<Vec3D>();
        }

        /// <summary>
        /// Gets wrapped data
        /// </summary>
        /// <returns></returns>
        public T getData()
        {
            return data;
        }

        /// <summary>
        /// Steps through all behaviours in the queue
        /// </summary>
        public override void step()
        {
            foreach (IBehaviour b in behaviours.getData())
            {
              if(!b.lateUpdate)b.run(this);
            }
        }

        /// <summary>
        /// Steps through all behaviours with lateupdate property in queue
        /// </summary>
        public override void lateUpdate()
        {
            foreach (IBehaviour b in behaviours.getData())
            {
                if (b.lateUpdate) b.run(this);
            }
        }

        /// <summary>
        /// Returns true if agent has 1 or more neighbours
        /// </summary>
        /// <returns></returns>
        public bool hasNeighbours()
        {
            return neighbours.Count > 0;
        }

        /// <summary>
        /// Inserts points in the neighbour list
        /// </summary>
        /// <param name="n"></param>
        public void addNeighbours(List<Vec3D> n)
        {
            neighbours.AddRange(n);
        }
    }
}
