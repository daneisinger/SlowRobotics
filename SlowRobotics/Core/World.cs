using SlowRobotics.Agent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Core
{
    public class World : IWorld
    {

        private AgentList pop;

        public Plane3DOctree dynamicTree; //this octree is rebuilt every n frames
        public Plane3DOctree staticTree;  //this octree is not rebuilt
        public float bounds;

        public World(float _bounds)
        {

            pop = new AgentList();
            bounds = _bounds;
            dynamicTree = new Plane3DOctree(new Vec3D(-bounds, -bounds, -bounds), bounds * 2);
            staticTree = new Plane3DOctree(new Vec3D(-bounds, -bounds, -bounds), bounds * 2);

        }

        public void addAgent(IAgent a)
        {
            pop.add(a);
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
            //add any new agents
            pop.populate();

            int steps = (int)(1 / damping);

            for (int i = 0; i < steps; i++)
            {
                foreach (IAgent a in pop.getRandomizedAgents()) a.step(damping);
            }

            cleanup();
            pop.flush();
        }

        public void cleanup()
        {
            //rebuild octrees
            dynamicTree = new Plane3DOctree(new Vec3D(-bounds, -bounds, -bounds), bounds * 2);

            foreach (IAgent a in pop.getAgents())
            {
                Vec3D p = a.getPos();
                if (p != null)
                {
                    if (a.getDeltaForStep() > 0)
                    {
                        addPoint(p, true); //add to dynamic octree
                    }
                    else {
                        pop.remove(a);
                        addPoint(p, false); //add to static octree
                    }
                }
            }
        }
    }
}
