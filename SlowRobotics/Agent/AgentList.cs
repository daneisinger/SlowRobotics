using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.Agent
{
    public class AgentList 
    {

        //handles adding and removing of agents from a collection
        public List<IAgent> pop { get; set; }
        private List<IAgent> addBuffer;
        private List<IAgent> removeBuffer;

        public AgentList() : this(new List<IAgent>()) { }

        public AgentList(List<IAgent> agents) 
        {
            pop = agents;
            removeBuffer = new List<IAgent>();
            addBuffer = new List<IAgent>();
        }

        public void add(IAgent a)
        {
            addBuffer.Add(a);
        }

        public void remove(IAgent a)
        {
            removeBuffer.Add(a);
        }

        public List<IAgent> getRandomizedAgents()
        {
            Random r = new Random();
            return pop.OrderBy(n => r.Next()).ToList();
        }

        public List<IAgent> getAgents()
        {
            return pop;
        }

        public void populate()
        {
            foreach (IAgent a in addBuffer)
            {
                pop.Add(a);
            }
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
