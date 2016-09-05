using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Core
{
    public class SimpleWorld : World
    {

        List<Node> pop;
        public Plane3DOctree dynamicTree;
        public Plane3DOctree staticTree;
        float bounds;

        public SimpleWorld(float _bounds)
        {
            pop = new List<Node>();
            bounds = _bounds;
            dynamicTree = new Plane3DOctree(new Vec3D(-bounds, -bounds, -bounds), bounds * 2);
            staticTree = new Plane3DOctree(new Vec3D(-bounds, -bounds, -bounds), bounds * 2);
        }

        
        public List<Node> getPop()
        {
            return pop;
        }

        /// <summary>
        /// Add a plane to the octree
        /// </summary>
        /// <param name="p"></param>
        public void addDynamic(Node p)
        {
            pop.Add(p);
            dynamicTree.addPoint(p);
        }
        /// <summary>
        /// Remove a plane from the octree
        /// </summary>
        /// <param name="p"></param>
        public bool removeDynamic(Node p)
        {
            pop.Remove(p);
            return dynamicTree.remove(p);
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

        public void addStatic(Node p)
        {
            pop.Add(p);
            staticTree.addPoint(p);
        }

        public bool removeStatic(Node p)
        {
            pop.Remove(p);
            return staticTree.remove(p);
        }


        public void run()
        {
            Random r = new Random();
            foreach (Node a in pop.OrderBy(n => r.Next())) a.update();
            //rebuild octrees
            dynamicTree = new Plane3DOctree(new Vec3D(-bounds, -bounds, -bounds), bounds * 2);
            foreach (Node a in pop)
            {
                if(a is Particle) dynamicTree.addPoint(a);
            }
        }
    }
}
