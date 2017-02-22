using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.Utils
{
    public interface IBenchmark
    {
        event EventHandler<UpdateEventArgs> OnUpdate;
    }

    
}
