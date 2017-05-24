using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlowRobotics.Utils
{
    public class SRWrapper
    {
        public object data {
            get; set;
        }

        public Dictionary<string, object> properties
        {
            get; set;
        }

        public SRWrapper(object _data, Dictionary<string,object> _properties)
        {
            data = _data;
            properties = _properties;
        }
    }
}
