using SlowRobotics.Core;
using SlowRobotics.Spatial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Agent.Behaviours
{
    /// <summary>
    /// Search behaviour that calls the search() function of a given Isearchable 
    /// structure and populates the agent neighbour collection.
    /// </summary>
    public class Search : ScaledBehaviour<Vec3D>
    {
        
        public ISearchable pts;
        public float radius;
        public int num;

        public Search(int _priority, float _radius,  ISearchable _pts, int _num) : base(_priority)
        {
            pts = _pts;
            radius = _radius;
            num = _num;
        }

        public override void run(IAgent<object> a)
        {

            Vec3D n = a.getData() as Vec3D;
            if (n != null)
            {
                a.neighbours = pts.Search(n, radius, num).ToList();
                a.neighbours.Remove(n);
            }

        }
    }
}
