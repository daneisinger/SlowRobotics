using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Core
{
    public class SimpleWorld : World
    {

        List<Agent> pop;
        public Plane3DOctree dynamicTree;
        public Plane3DOctree staticTree;
        float bounds;

        public SimpleWorld(float _bounds)
        {
            pop = new List<Agent>();
            bounds = _bounds;
            dynamicTree = new Plane3DOctree(new Vec3D(-bounds, -bounds, -bounds), bounds * 2);
            staticTree = new Plane3DOctree(new Vec3D(-bounds, -bounds, -bounds), bounds * 2);
        }

        
        public List<Agent> getPop()
        {
            return pop;
        }
        /// <summary>
        /// Add an agent to the octree and pop list for updating behaviours
        /// </summary>
        /// <param name="a"></param>
        public void add(Agent a)
        {
            pop.Add(a);
            dynamicTree.addPoint(a);
        }
        /// <summary>
        /// Add a plane to the octree
        /// </summary>
        /// <param name="p"></param>
        public void addDynamic(Plane3D p)
        {
            dynamicTree.addPoint(p);
        }
        /// <summary>
        /// Remove a plane from the octree
        /// </summary>
        /// <param name="p"></param>
        public void removeDynamic(Plane3D p)
        {
            dynamicTree.remove(p);
        }

        public List<Vec3D> getDynamicPoints(Vec3D pos, float radius)
        {
            List<Vec3D> dynamicPts = dynamicTree.getPointsWithinSphere(pos, radius);
            if (dynamicPts == null)
            {
                return new List<Vec3D>();
            }
            else return dynamicPts;
        }
        public List<Vec3D> getStaticPoints(Vec3D pos, float radius)
        {
            List<Vec3D> staticPts = staticTree.getPointsWithinSphere(pos, radius);
            if (staticPts == null)
            {
                return new List<Vec3D>();
            }
            else return staticPts;
        }

        public void addStatic(Plane3D p)
        {
            staticTree.addPoint(p);
        }

        public void removeStatic(Plane3D p)
        {
            staticTree.remove(p);
        }

        public void remove(Agent a)
        {
            pop.Remove(a);
        }

        public void run()
        {
            Random r = new Random();
            foreach (Agent a in pop.OrderBy(n => r.Next())) a.run();
            //rebuild octrees
            dynamicTree = new Plane3DOctree(new Vec3D(-bounds, -bounds, -bounds), bounds * 2);
            foreach (Agent a in pop) dynamicTree.addPoint(a);
            

        }
    }
}
