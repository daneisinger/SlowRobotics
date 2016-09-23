using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.Behaviours
{
    public interface Behaviour : IComparable<Behaviour>
    {
        /// <summary>
        /// Default agent behaviour method
        /// </summary>
        /// <param name="a"></param>
        void run(Agent a);
        /// <summary>
        /// Behaviour to run per neighbour b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        void run(Agent a, Agent b);

        int getPriority();

    }
}
