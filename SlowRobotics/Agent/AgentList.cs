﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.Agent
{
    public class AgentList 
    {

        //handles adding and removing of agents from a collection
        private Dictionary<IAgent, object> pop { get; set; }
        private Dictionary<IAgent, object> addBuffer;
        private List<IAgent> removeBuffer;

        public AgentList() : this(new Dictionary<IAgent,object>()) { }

        public AgentList(Dictionary<IAgent,object> agents) 
        {
            pop = agents;
            removeBuffer = new List<IAgent>();
            addBuffer = new Dictionary<IAgent,object>();
        }

        public void add(IAgent a, object o)
        {
            addBuffer.Add(a,o);
        }

        public void remove(IAgent a)
        {
            removeBuffer.Add(a);
        }

        public List<IAgent> getRandomizedAgents()
        {
            Random r = new Random();
            return pop.Keys.OrderBy(n => r.Next()).ToList();
        }

        public List<IAgent> getAgents()
        {
            return pop.Keys.ToList();
        }

        public bool getDataFor<T>(IAgent a, out T data) where T : class
        {
            data = pop[a] as T;
            return data != null;
        }

        public void populate()
        {
            foreach (KeyValuePair<IAgent,object> a in addBuffer)
            {
                pop.Add(a.Key,a.Value);
            }
            addBuffer = new Dictionary<IAgent,object>();
        }

        public void flush()
        {
            foreach (IAgent a in removeBuffer)
            {
                pop.Remove(a);
            }
            removeBuffer = new List<IAgent>();
        }

        public int Count
        {
            get
            {
                return pop.Count;
            }
        }
    }
}