using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlowRobotics.Utils
{
    /// <summary>
    /// Wrapper for string to be used with behaviours
    /// </summary>
    public class SRString
    {
        public String s { get; set; }

        public SRString (String _s)
        {
            s = _s;
        }
        public SRString()
        {
            s = "";
        }

    }
}
