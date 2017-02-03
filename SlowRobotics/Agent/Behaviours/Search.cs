using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Agent.Behaviours
{
    public class Search : ScaledBehaviour<Vec3D>, IWorldBehaviour
    {

        //TODO - should be a search points function and a search agents function

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
        public IWorld world { get; set; }

        public Search(int _priority, float _radius, SearchMethod _method, IWorld _world) : this(_priority, _radius,_radius,_method,_world) { }
        public Search(int _priority, float _dynamicRadius, float _staticRadius, SearchMethod _method, IWorld _world) : base(_priority)
        {
            method = _method;
            dynamicRadius = _dynamicRadius;
            staticRadius = _staticRadius;
            world = _world;
        }

        public override void run(IAgentT<object> a)
        {
            Vec3D n = (Vec3D)a.getData();
            a.neighbours = (new List<Vec3D>());
            switch (method.m)
            {
                case (0):
                    a.addNeighbours(searchDynamic(n, dynamicRadius));
                    break;
                case (1):
                    a.addNeighbours(searchStatic(n, dynamicRadius));
                    break;
                case (2):
                    a.addNeighbours(searchAll(n, dynamicRadius, staticRadius));
                    break;
            }
        }

        public List<Vec3D> searchDynamic(Vec3D a, float radius)
        {
            List<Vec3D> n = world.search(a, radius * scaleFactor,0);
            n.Remove(a);
            return n;
        }

        public List<Vec3D> searchStatic(Vec3D a, float radius)
        {
            List<Vec3D> n = world.search(a, radius * scaleFactor,1);
            n.Remove(a);
            return n;
        }

        public List<Vec3D> searchAll(Vec3D a, float dynamicRadius, float staticRadius)
        {
            List<Vec3D> n = world.search(a, dynamicRadius * scaleFactor,2);
            n.AddRange(searchStatic(a, staticRadius * scaleFactor));
            n.Remove(a);
            return n;
        }
    }
}
