using SlowRobotics.Core;
using SlowRobotics.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.Agent
{
    public interface Behaviour : IComparable<Behaviour>
    {

        /// <summary>
        /// Method to update agent
        /// </summary>
        /// <param name="a"></param>
        void run(Agent a);

        /// <summary>
        /// Method to test for conditions before updating
        /// </summary>
        /// <param name="a"></param>
        /// <param name="p"></param>
        void test(Agent a, Plane3D p);

        /// <summary>
        /// Return priority of this behaviour
        /// </summary>
        /// <returns></returns>
        int getPriority();

    }
}
