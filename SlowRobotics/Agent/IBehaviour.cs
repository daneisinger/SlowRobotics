using SlowRobotics.Core;
using SlowRobotics.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.Agent
{
    public interface IBehaviour : IComparable<IBehaviour>
    {

        /// <summary>
        /// Method to update agent
        /// </summary>
        /// <param name="a"></param>
        void run(IAgent a);

        /// <summary>
        /// Method to interact with other agents
        /// </summary>
        /// <param name="a"></param>
        /// <param name="p"></param>
        void interact(IAgent a, IAgent b);

        /// <summary>
        /// Return priority of this behaviour
        /// </summary>
        /// <returns></returns>
        int getPriority();

    }
}
