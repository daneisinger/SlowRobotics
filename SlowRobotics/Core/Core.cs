using SlowRobotics.Agent;
using SlowRobotics.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlowRobotics.Core
{
    /// <summary>
    /// Multithreaded management class that handles updates of agentlist collections.
    /// The core fist calls agentlist populate() functions to add any agents in buffers to
    /// the populations to simulate. It then iterates over all agents in the population 
    /// for a given number of simulation steps and calls 
    /// each agent's step() function. Finally the core calls agentlist flush() functions to remove
    /// any agents flagged to be deleted.
    /// </summary>
    public static class Core
    {
        private static int maxThreads = Environment.ProcessorCount * 4;

        /// <summary>
        /// Updates a list of agents
        /// </summary>
        /// <param name="pop">List of agents to update</param>
        /// <param name="steps">Number of simulation steps</param>
        public static void run(List<IAgent> pop, int steps)
        {
            AgentList list = new AgentList();
            list.addAll(pop);
            run(list, steps);
        }

        /// <summary>
        /// Updates a list of AgentLists
        /// </summary>
        /// <param name="pop">List of AgentLists to update</param>
        /// <param name="steps">Number of simulation steps</param>
        public static void run(List<AgentList> pop, int steps)
        {
            for (int i = 0; i < steps; i++)
            {
                foreach (AgentList p in pop) run(p, 1);
            }
        }

        /// <summary>
        /// Updates a single AgentList
        /// </summary>
        /// <param name="pop">AgentList to update</param>
        /// <param name="steps">Number of simulation steps</param>
        public static void run(AgentList pop, int steps)
        {
            System.Threading.ThreadPool.SetMaxThreads(maxThreads, maxThreads);

            //add any new agents
            pop.populate();

            for (int i = 0; i < steps; i++)
            {
                //-------------------------------------------------------------------start parallel compute loop
                int numChunks = (int)Math.Ceiling(pop.Count / (double)maxThreads);
                int runningThreads = 0;
                //List<IAgent> agents = pop.getRandomizedAgents();
                List<IAgent> agents = pop.getAgents();
                foreach (IAgent a in agents)
                {
                    System.Threading.Interlocked.Increment(ref runningThreads);
                    System.Threading.ThreadPool.QueueUserWorkItem(delegate
                    {
                        a.step();
                        System.Threading.Interlocked.Decrement(ref runningThreads);
                    });
                }
                while (runningThreads > 0) {} // wait for threads to finish

                //-------------------------------------------------------------------late update loop
                //hack: todo implement as event
                foreach (IAgent a in agents) a.lateUpdate();

            }
            pop.flush(); 
            }
        }
}
