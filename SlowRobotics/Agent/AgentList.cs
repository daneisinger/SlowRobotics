using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.Agent
{
    /// <summary>
    /// Collection of agents
    /// </summary>
    public class AgentList
    {

        public Dictionary<IAgent, object> pop { get; set; }
        private List<IAgent> addBuffer;
        private List<IAgent> removeBuffer;

        /// <summary>
        /// Default constructor
        /// </summary>
        public AgentList() : this(new Dictionary<IAgent, object>()) { }

        /// <summary>
        /// Wraps an agent in an AgentList for passing to the core
        /// </summary>
        /// <param name="a">Agent to wrap</param>
        public AgentList(IAgent a) : this(new Dictionary<IAgent, object>())
        {
            add(a);
            populate();
        }

        /// <summary>
        /// Creates an agent list from a dictionary of agents and associated object data
        /// </summary>
        /// <param name="agents">Agents to add</param>
        public AgentList(Dictionary<IAgent, object> agents)
        {
            pop = agents;
            removeBuffer = new List<IAgent>();
            addBuffer = new List<IAgent>();
        }

        /// <summary>
        /// Gets all objects of a given type
        /// </summary>
        /// <param name="T">Type of data to get</param>
        /// <returns>Object collection</returns>
        public IEnumerable<object> getByType(Type T)
        {
            return pop.Values.Where(t => t.GetType()==T);
        }

        /// <summary>
        /// Adds agents to the buffer - use populate() to add to the population
        /// </summary>
        /// <param name="agents">Agents to add</param>
        public void addAll(IEnumerable<IAgent> agents)
        {
            foreach (IAgent a in agents) add(a);
        }

        /// <summary>
        /// Adds an agent to the buffer - use populate() to add to the population
        /// </summary>
        /// <param name="a">Agent to add</param>
        public void add(IAgent a)
        {
            addBuffer.Add(a);
        }

        /// <summary>
        /// Adds an agent to the population and maintains dicionary associating agents with wrapped data types
        /// </summary>
        /// <param name="a">Agent to add</param>
        private void insert(IAgent a)
        {
            //dont add duplicate agents, check for GH nulls
            if (a!=null && !pop.ContainsKey(a))
            {
                IAgent<object> ao = (IAgent<object>)a;
                object data = null;
                if (ao != null) data = ao.getData();
                pop.Add(a, data);
            }
        }

        /// <summary>
        /// Puts an agent in the trash - use flush() to remove agents in the trash from the population.
        /// </summary>
        /// <param name="a">Agent to remove</param>
        public void remove(IAgent a)
        {
            removeBuffer.Add(a);
        }

        /// <summary>
        /// Returns a randomized collection of all agents 
        /// </summary>
        /// <returns></returns>
        public List<IAgent> getRandomizedAgents()
        {
            if (Count == 0) return new List<IAgent>();
            Random r = new Random();
            return pop.Keys.OrderBy(n => r.Next()).ToList();
        }

        /// <summary>
        /// Returns agents in default order
        /// </summary>
        /// <returns></returns>
        public List<IAgent> getAgents()
        {
            return pop.Keys.ToList();
        }

        /// <summary>
        /// Trys to get data for a given un-typed agent
        /// </summary>
        /// <typeparam name="T">Type to get</typeparam>
        /// <param name="a">Agent to return data for</param>
        /// <param name="data">Variable to store data</param>
        /// <returns>Returns true if able to get data of requested type, false if not</returns>
        public bool getDataFor<T>(IAgent a, out T data) where T : class
        {
            data = pop[a] as T;
            return data != null;
        }

        /// <summary>
        /// Checks if list contains a given agent
        /// </summary>
        /// <param name="a">Agent to search for</param>
        /// <returns></returns>
        public bool contains(IAgent a)
        {
            return pop.ContainsKey(a);
        }

        /// <summary>
        /// Adds all agents in the buffer to the population
        /// </summary>
        public void populate()
        {
            foreach(IAgent a in addBuffer) insert(a);
            addBuffer = new List<IAgent>();
        }

        /// <summary>
        /// Removes all agents in the trash
        /// </summary>
        public void flush()
        {
            foreach (IAgent a in removeBuffer)
            {
                pop.Remove(a);
            }
            removeBuffer = new List<IAgent>();
        }

        /// <summary>
        /// Gets the number of agents in the population
        /// </summary>
        public int Count
        {
            get
            {
                return pop.Count;
            }
        }
    }
}
