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

        public static void run(List<AgentList> pop, float damping)
        {
            foreach (AgentList p in pop) run(p, damping);
        }

        public static void run(AgentList pop, float damping)
        {
            if (pop != null)
            {
                System.Threading.ThreadPool.SetMaxThreads(maxThreads, maxThreads);

                //add any new agents
                pop.populate();

                    int steps = (int)(1 / damping);

                    for (int i = 0; i < steps; i++)
                    {

                        int numChunks = (int)Math.Ceiling(pop.Count / (double)maxThreads);
                        int runningThreads = 0;

                        /*
                        Parallel.ForEach(Partitioner.Create(0, pop.Count), range =>
                        {
                            for (int index = range.Item1; index < range.Item2; index++)
                            {
                                agents[index].step(damping);
                            }
                        });*/
                        List<IAgent> agents = pop.getRandomizedAgents();
                        foreach (IAgent a in agents)
                        {
                            System.Threading.Interlocked.Increment(ref runningThreads);
                            System.Threading.ThreadPool.QueueUserWorkItem(delegate
                            {
                                a.step(damping);
                                System.Threading.Interlocked.Decrement(ref runningThreads);
                            });
                        }

                        while (runningThreads > 0) { } // wait for threads to finish
                    }
                    pop.flush();
                }
            }
        }
}
