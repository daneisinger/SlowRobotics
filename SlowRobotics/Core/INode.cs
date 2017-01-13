using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.Core
{
    public interface INode
    {
        List<Link> getLinks();
        bool hasLinks();
        void connect(Link l);
        bool disconnect(Link l);

        Node parent { get; set; }
    }
}
