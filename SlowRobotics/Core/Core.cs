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

        public static void run(List<AgentList> pop, float damping)
        {
            foreach (AgentList p in pop) run(p, damping);
        }

        public static void run(AgentList pop, float damping)
        {

           // System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
           // stopwatch.Start();

            //add any new agents
            pop.populate();

            int steps = (int)(1 / damping);

            for (int i = 0; i < steps; i++)
            {
                List<IAgent> agents = pop.getRandomizedAgents();
                Parallel.ForEach(Partitioner.Create(0, pop.Count), range =>
                {
                    for (int index = range.Item1; index < range.Item2; index++)
                    {
                        agents[index].step(damping);
                    }
                });
            }
            pop.flush();

          //  stopwatch.Stop();
          //  if ((OnUpdate != null)) OnUpdate(this, new UpdateEventArgs(this.GetType().ToString(), stopwatch.ElapsedMilliseconds));

        }

    }
}
