using SlowRobotics.Core;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toxiclibs.core;

namespace SlowRobotics.Behaviours
{
    public class OrientToBestFitBehaviour : AgentBehaviour
    {
            float maxDist;
            float orientToBestFit;

            public OrientToBestFitBehaviour(int _priority, float _maxDist, float _orientToBestFit) : base(_priority)
            {
                maxDist = _maxDist;
                orientToBestFit = _orientToBestFit;
            }

           override
           public void run(Agent a)
            {
                if (a.neighbours != null)
                {
                SortedList closestPts = new SortedList();
                    Vec3D n = a.zz.copy();
                    for (int i = 0; i < a.neighbours.Count - 1; i++)
                    {

                        Vec3D p = (Vec3D)a.neighbours[i];
                        float d = p.distanceTo(a);
                        if (d > 0 && !closestPts.ContainsKey(d)) closestPts.Add(d, p);
                    }
                    if (closestPts.Count >= 3)
                    {

                        Triangle3D tri = new Triangle3D((Vec3D) closestPts.GetValueList()[0], (Vec3D)closestPts.GetValueList()[1], (Vec3D)closestPts.GetValueList()[2]);
                        n = tri.computeNormal();
                        if (a.zz.angleBetween(n) > (float)Math.PI / 2) n.invert();
                    }
                    a.interpolateToZZ(n, orientToBestFit);
                }
            }
        }
}
