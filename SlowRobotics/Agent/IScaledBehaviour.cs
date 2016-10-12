using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.Agent
{
    public interface IScaledBehaviour : IBehaviour
    {

        float scaleFactor { get; set; }

        void scale(float factor);

    }
}
