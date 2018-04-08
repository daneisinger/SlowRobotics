using SlowRobotics.Core;
using SlowRobotics.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Agent.Behaviours
{
    /// <summary>
    /// The interaction behaviour stores a collection of additional behaviours that define
    /// how an agent should interact with objects in its neighbour collection. The behaviour 
    /// iterates over the neighbour collection of an agent and calls
    /// the interactWith function of each behaviour, passing the current neighbour as an argument.
    /// After this loop is completed, a second loop iterates over all behaviours and calls the  run method. 
    /// This is useful if you want to use the interactWith loop to check for some condition (closest point,
    /// first neighbour with a property, minimum angles etc) before then operating on that specific neighbour 
    /// and modifying the agent or agent data in the main run method.
    /// </summary>
    public class Interact : Behaviour
    {
        private PriorityQueue<IBehaviour> behaviours;

        public Interact(int _priority, List<IBehaviour> _interactionBehaviours) : base(_priority)
        {
            behaviours = new PriorityQueue<IBehaviour>();
            foreach (IBehaviour b in _interactionBehaviours) behaviours.Enqueue(b);
        }

        public override void onAdd() { }

        public override void interact(IAgent<object> a, object b)
        {
            throw new NotImplementedException();
        }

        public override void run(IAgent<object> a)
        {

            if (a.hasNeighbours())
            {
                // Testing step - loop through all neighbours
                // used for closest point search etc.

                //TODO - should loop through agents
                foreach (Vec3D p in a.neighbours)
                {
                    //TODO - neighbour search doesnt return IAgent
                    foreach (IBehaviour b in behaviours.getData()) b.interact(a, p);
                }
                //update the agent using test data
                foreach (IBehaviour b in behaviours.getData()) b.run(a);
            }
        }

        public void setBehaviours(List<IBehaviour> newBehaviours)
        {
            behaviours = new PriorityQueue<IBehaviour>();
            foreach (IBehaviour b in newBehaviours) behaviours.Enqueue(b);
        }

        public List<IBehaviour> getBehaviours()
        {
            return behaviours.getData();
        }

        /// <summary>
        /// Gets runtime of behaviour for a given agent in milliseconds
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public override string debug(IAgent<object> a)
        {
            string log = "";
            stopWatch.Reset();
            stopWatch.Start();
            if (a.hasNeighbours())
            {
                // Testing step - loop through all neighbours
                // used for closest point search etc.

                //TODO - should loop through agents
                foreach (Vec3D p in a.neighbours)
                {
                    //TODO - neighbour search doesnt return IAgent
                    foreach (IBehaviour b in behaviours.getData()) b.interact(a, p);
                }
                //update the agent using test data
                foreach (IBehaviour b in behaviours.getData()) log= log+b.debug(a)+ "\r\n";
            }
            stopWatch.Stop();
            return "Name: " + ToString() + "\r\n" + log + "Run Time: "+stopWatch.ElapsedMilliseconds.ToString();
        }
    }
}
