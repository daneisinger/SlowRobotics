using SlowRobotics.Rhino.GraphTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toxiclibs.core;

namespace SlowRobotics.Spatial
{
    // Todo - speed improvement - crazy slow for some reason
    public class BinnedCollection : ISearchable
    {
        public Dictionary<string,List<Vec3D>> allPts;
        public float binSize;

        public BinnedCollection() :this(10) { }

        public BinnedCollection(float _binSize)
        {
            binSize = _binSize;
            allPts = new Dictionary<string,List<Vec3D>>();
        }

        public IEnumerable<Vec3D> Collection
        {
            //TODO - implement
            get
            {
                return new List<Vec3D>();
            }
        }

        public void Add(Vec3D pt)
        {
            string key = GraphUtils.makeSpatialKey(pt, binSize);
            if (!allPts.ContainsKey(key)) allPts.Add(key, new List<Vec3D>());
            allPts[key].Add(pt);
        }

        public IEnumerable<Vec3D> Search(Vec3D pt, float radius, int maxPoints)
        {
            //todo - implement max points
            foreach(string s in getBins(pt, radius))
            {
                foreach (Vec3D p in allPts[s])
                {
                   if(p.distanceTo(pt)< radius) yield return p;
                }
            }
        }

        public IEnumerable<string> getBins(Vec3D pt, float radius)
        {
            int num = (int)Math.Floor(radius / binSize);
            if (num == 0) yield return GraphUtils.makeSpatialKey(pt, binSize);

            for(int i = -num; i<= num; i++)
            {
                for (int j = -num; j <= num; j++)
                {
                    for (int k = -num; k <= num; k++)
                    {
                        string key= GraphUtils.makeSpatialKey(pt.add(i * binSize, j * binSize, k * binSize), binSize);
                        if (allPts.ContainsKey(key)) yield return key;
                    }
                }
            }
        }

        public void Update(IEnumerable<Vec3D> pts)
        {
            allPts = new Dictionary<string, List<Vec3D>>();
            foreach (Vec3D pt in pts) Add(pt);
        }

        
    }
}
