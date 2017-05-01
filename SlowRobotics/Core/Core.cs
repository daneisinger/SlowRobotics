using SlowRobotics.Agent;
using SlowRobotics.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlowRobotics.Core
{
    public static class Core
    {
        private static int maxThreads = Environment.ProcessorCount * 4;

        public static void run(List<IAgent> pop, float damping)
        {
            AgentList list = new AgentList();
            list.addAll(pop);
            run(list, damping);
        }

        public static void run(List<AgentList> pop, float damping)
        {
            foreach (AgentList p in pop) run(p, damping);
        }

        public static void run(AgentList pop, float damping)
        {
            System.Threading.ThreadPool.SetMaxThreads(maxThreads, maxThreads);

            //add any new agents
            pop.populate();

            int steps = (int)(1 / damping);

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
                        a.step(damping);
                        System.Threading.Interlocked.Decrement(ref runningThreads);
                    });
                }
                while (runningThreads > 0) {} // wait for threads to finish

                //-------------------------------------------------------------------late update loop
                //hack: todo implement as event
                foreach (IAgent a in agents) a.lateUpdate(damping);

            }
            pop.flush(); 
            }
        }
}
