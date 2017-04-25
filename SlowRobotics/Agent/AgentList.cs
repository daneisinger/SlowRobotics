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
        private List<IAgent> addBuffer;
        private List<IAgent> removeBuffer;

        public AgentList() : this(new Dictionary<IAgent, object>()) { }

        public AgentList(IAgent a) : this(new Dictionary<IAgent, object>())
        {
            add(a);
            populate();
        }

        public AgentList(Dictionary<IAgent, object> agents)
        {
            pop = agents;
            removeBuffer = new List<IAgent>();
            addBuffer = new List<IAgent>();
        }

        public IEnumerable<object> getByType(Type T)
        {
            return pop.Values.Where(t => t.GetType()==T);
        }

        public void addAll(IEnumerable<IAgent> agents)
        {
            foreach (IAgent a in agents) add(a);
        }

        public void add(IAgent a)
        {
            addBuffer.Add(a);
        }

        private void insert(IAgent a)
        {
            //dont add duplicate agents, check for GH nulls
            if (a!=null && !pop.ContainsKey(a))
            {
                IAgentT<object> ao = (IAgentT<object>)a;
                object data = null;
                if (ao != null) data = ao.getData();
                pop.Add(a, data);
            }
        }

        public void removeAgent(IAgent a)
        {
            remove(a);
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
            foreach(IAgent a in addBuffer) insert(a);
            addBuffer = new List<IAgent>();
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
