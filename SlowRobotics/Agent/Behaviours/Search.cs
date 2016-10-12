using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Agent.Behaviours
{
    public class Search : ScaledAgentBehaviour
    {
        public struct SearchMethod
        {

            public static readonly SearchMethod Dynamic = new SearchMethod(0);
            public static readonly SearchMethod Static = new SearchMethod(1);
            public static readonly SearchMethod All = new SearchMethod(2);

            public int m
            {
                get; set;
            }

            private SearchMethod(int _m)
            {
                m = _m;
            }

        }

        public SearchMethod method;
        public float dynamicRadius;
        public float staticRadius;

        public Search(int _priority, float _radius, SearchMethod _method) : base(_priority)
        {
            method = _method;
            dynamicRadius = _radius;
            staticRadius = dynamicRadius;
        }
        public Search(int _priority, float _dynamicRadius, float _staticRadius, SearchMethod _method) : base(_priority)
        {
            method = _method;
            dynamicRadius = _dynamicRadius;
            staticRadius = _staticRadius;
        }

        public override void run(PlaneAgent a)
        {
            a.setNeighbours(new List<Vec3D>());
            switch (method.m)
            {
                case (0):
                    a.addNeighbours(searchDynamic(a, dynamicRadius));
                    break;
                case (1):
                    a.addNeighbours(searchStatic(a, dynamicRadius));
                    break;
                case (2):
                    a.addNeighbours(searchAll(a, dynamicRadius, staticRadius));
                    break;
            }
        }

        public List<Vec3D> searchDynamic(PlaneAgent a, float radius)
        {
            List<Vec3D> n = a.world.getDynamicPoints(a, radius * scaleFactor);
            n.Remove(a);
            return n;
        }

        public List<Vec3D> searchStatic(PlaneAgent a, float radius)
        {
            List<Vec3D> n = a.world.getStaticPoints(a, radius * scaleFactor);
            n.Remove(a);
            return n;
        }

        public List<Vec3D> searchAll(PlaneAgent a, float dynamicRadius, float staticRadius)
        {
            List<Vec3D> n = a.world.getDynamicPoints(a, dynamicRadius * scaleFactor);
            n.AddRange(searchStatic(a, staticRadius * scaleFactor));
            n.Remove(a);
            return n;
        }
    }
}
