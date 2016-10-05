using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Behaviours.TrailBehaviours
{
    public class DuplicateAndDisconnectBehaviour : AgentBehaviour
    {

        Vec3D offset;
        List<Behaviour> behaviours;
        int frequency;
        int ctr;
        float stiffness;
        bool dynamic;

        public DuplicateAndDisconnectBehaviour(int _priority, int _frequency, Vec3D _offset, float _stiffness, List<Behaviour> _behaviours) : this(_priority, _frequency, _offset,_stiffness,_behaviours, true) {}
        public DuplicateAndDisconnectBehaviour(int _priority, int _frequency, Vec3D _offset ) : this(_priority, _frequency, _offset, 0, new List<Behaviour>(), false) { }
        public DuplicateAndDisconnectBehaviour(int _priority, int _frequency, Vec3D _offset, float _stiffness, List<Behaviour> _behaviours, bool _dynamic) : base(_priority)
        {
            offset = _offset;
            frequency = _frequency;
            ctr = 0;
            behaviours = _behaviours;
            stiffness = _stiffness;
            dynamic = _dynamic;
        }

        override
        public void run(Agent a)
        {
            if (ctr % frequency == 0)
            {
                Agent b = new Agent(a, a.locked(), a.world);
                b.addSelf(offset);
                b.parent = a.parent;

                //replace connections and add to new node
                foreach (Link l in a.getLinks())
                {
                    l.replaceNode(a, b);
                    b.connect(l);
                }
                //delete links from a
                a.isolate();

                //create new link
                Link connection = new Link(a, b);
                connection.stiffness = stiffness;
                a.connect(connection);
                b.connect(connection);

                //add behaviours

                if (dynamic)
                {
                    foreach (Behaviour nb in behaviours) b.addBehaviour(nb);
                    a.world.addDynamic(b);//add to world
                }
                else
                {
                    a.world.addStatic(b);
                }
            }
            ctr++;

        }

    }
}
