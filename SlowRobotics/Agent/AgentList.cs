using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.Agent
{
    public class AgentList 
    {

        //handles adding and removing of agents from a collection
        public Dictionary<IAgent, object> pop { get; set; }
        private Dictionary<IAgent, object> addBuffer;
        private List<IAgent> removeBuffer;

        public AgentList() : this(new Dictionary<IAgent,object>()) { }

        public AgentList(IAgent a) : this(new Dictionary<IAgent, object>())
        {
            add(a);
            populate();
        }

        public AgentList(Dictionary<IAgent,object> agents) 
        {
            pop = agents;
            removeBuffer = new List<IAgent>();
            addBuffer = new Dictionary<IAgent,object>();
        }

        public void addAll(IEnumerable<IAgent> agents)
        {
            foreach (IAgent a in agents) add(a);
        }

        public void add(IAgent a)
        {

            //try and get data
            AgentT<object> ao = (AgentT<object>)a;
            object data = null;
            if (ao != null) data = ao.getData();
            add(a, data);
        }

        public void removeAgent(IAgent a)
        {
            remove(a);
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
            if (Count == 0) return new List<IAgent>();
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

        public bool contains(IAgent a)
        {
            return pop.ContainsKey(a);
        }

        public void populate()
        {
            foreach (KeyValuePair<IAgent,object> a in addBuffer)
            {
                if(a.Key!=null && a.Value!=null) pop.Add(a.Key,a.Value);
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
