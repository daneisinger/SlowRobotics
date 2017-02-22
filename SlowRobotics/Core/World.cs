﻿using SlowRobotics.Agent;
using SlowRobotics.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toxiclibs.core;

namespace SlowRobotics.Core
{
    public class World : IWorld, IBenchmark
    {

        private AgentList pop;

        public Plane3DOctree dynamicTree; //this octree is rebuilt every n frames
        public Plane3DOctree staticTree;  //this octree is not rebuilt
        public float bounds;

        public event EventHandler<UpdateEventArgs> OnUpdate;
        private string txtReport;

        public World(float _bounds)
        {

            pop = new AgentList();
            bounds = _bounds;
            dynamicTree = new Plane3DOctree(new Vec3D(-bounds, -bounds, -bounds), bounds * 2);
            staticTree = new Plane3DOctree(new Vec3D(-bounds, -bounds, -bounds), bounds * 2);

            txtReport = "";
        }

        public void addAgent(IAgent a)
        {
            //try and get some data

            //TODO - fix this up so there is no casting

            AgentT<object> ao = (AgentT<object>)a;

            object data = null;
            if (ao != null) data = ao.getData();
            pop.add(a,data);
        }

        public void removeAgent(IAgent a)
        {
            pop.remove(a);
        }

        public List<IAgent> getPop()
        {
            return pop.getAgents();
        }

        /// <summary>
        /// Add a plane to the octree
        /// </summary>
        /// <param name="p"></param>
        public void addPoint(Vec3D p, bool dynamic)
        {
            if (dynamic)
            {
                dynamicTree.addPoint(p);
            }
            else
            {
                staticTree.addPoint(p);
            }
        }
        /// <summary>
        /// Remove a plane from the octree
        /// </summary>
        /// <param name="p"></param>
        public bool removePoint(Vec3D p)
        {
            return dynamicTree.remove(p) || staticTree.remove(p);
        }

        public List<Vec3D> getPoints()
        {
            List<Vec3D> pts = dynamicTree.getPoints();
            pts.AddRange(staticTree.getPoints());
            return pts;

        }

        public List<Vec3D> search(Vec3D pos, float radius, int type)
        {
            List<Vec3D> pts = new List<Vec3D>();
            switch (type)
            {
                case 0:
                    pts.AddRange(searchDynamic(pos, radius));
                    break;
                case 1:
                    pts.AddRange(searchStatic(pos, radius));
                    break;
                default:
                    pts.AddRange(searchDynamic(pos, radius));
                    pts.AddRange(searchStatic(pos, radius));
                    break;
            }
            return pts;
        }

        private List<Vec3D> searchDynamic(Vec3D pos, float radius)
        {
            return dynamicTree.getPointsWithinSphere(pos, radius);
        }

        private List<Vec3D> searchStatic(Vec3D pos, float radius)
        {
            return staticTree.getPointsWithinSphere(pos, radius);
        }

        public void run()
        {
            run(0.94f);
        }


        public void run(float damping)
        {

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            //add any new agents
            pop.populate();

            int steps = (int)(1 / damping);

            for (int i = 0; i < steps; i++)
            {
                List<IAgent> agents = pop.getRandomizedAgents();

                Parallel.ForEach(Partitioner.Create(0, Count), range =>
                {
                    for (int index = range.Item1; index < range.Item2; index++)
                    {
                        agents[index].step(damping);
                    }
                });

               // foreach (IAgent a in pop.getRandomizedAgents()) a.step(damping);
            }

            cleanup();
            pop.flush();

            stopwatch.Stop();
            if ((OnUpdate != null)) OnUpdate(this, new UpdateEventArgs(this.GetType().ToString(), stopwatch.ElapsedMilliseconds));

        }

        public int Count
        {
            get
            {
                return pop.Count;
            }
        }

        public void cleanup()
        {
            //rebuild octrees
            dynamicTree = new Plane3DOctree(new Vec3D(-bounds, -bounds, -bounds), bounds * 2);

            //need a more elegant way of doing this
            foreach (IAgent a in pop.getAgents())
            {
                Vec3D p;
                if(pop.getDataFor(a, out p))
                { 
                    //TODO - implement better system for handling dynamic points
                    //and removing agents

                    //if (a.getDeltaForStep() > 0)
                  //  {
                        addPoint(p, true); //add to dynamic octree
                   // }
                   // else {
                    //    pop.remove(a);
                     //   addPoint(p, false); //add to static octree
                   // }
                }
            }
        }
    }
}
