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
            //lateUpdate = true;
        }

        public override void run(IAgent<object> a)
        {

            Vec3D n = a.getData() as Vec3D;
            if (n != null)
            {
                a.neighbours = pts.Search(n, radius);
                a.neighbours.Remove(n);
            }

        }
    }
}
