using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.Core
{
    public interface IParticle
    {

        void step(float dt);
    }
}
