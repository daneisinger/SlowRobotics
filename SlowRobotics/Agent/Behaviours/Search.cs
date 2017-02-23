using SlowRobotics.Core;
using SlowRobotics.Spatial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Agent.Behaviours
{
    public class Search : ScaledBehaviour<Vec3D>
    {
        
        public ISearchable pts;
        public float radius;

        public Search(int _priority, float _radius,  ISearchable _pts) : base(_priority)
        {
            pts = _pts;
            radius = _radius;
        }

        public override void run(IAgentT<object> a)
        {
            Vec3D n = (Vec3D)a.getData();
            a.neighbours = pts.Search(n,radius);
            a.neighbours.Remove(n);
        }
    }
}
