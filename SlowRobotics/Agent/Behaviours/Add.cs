
using SlowRobotics.Agent.Types;
using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Agent.Behaviours
{
    public class Add
    {

        public class StateToWorld : Behaviour<IState>, IWorldBehaviour
        {
            public int frequency { get; set; }
            public IWorld world {get;set;}
            int c;

            public StateToWorld(int _priority, IWorld _world, int _frequency) :base(_priority)
            {
                world = _world;
                frequency = _frequency;
                c = 0;
            }

            public override void runOn(IState a)
            {
                    if (c % frequency == 0)
                    {
                        world.addStatic(a); //MIGHT NEED TO COPY STATE
                    }
                    c++;
            }
        }

        public class Extend : ScaledBehaviour<Graph>, IWorldBehaviour
        {
            public IWorld world { get; set; }

            public Vec3D offset { get; set; }
            public List<IBehaviour> behaviours { get; set; }

            public int frequency { get; set; }
            public int ctr;
            public float stiffness { get; set; }
            public float braceStiffness { get; set; }
            public bool dynamic { get; set; }
            public bool tryToBrace { get; set; }

            public Extend(int _priority, bool _tryToBrace, int _frequency, Vec3D _offset, float _stiffness, float _braceStiffness, List<IBehaviour> _behaviours, bool _dynamic, IWorld _world) : base(_priority)
            {
                world = _world;
                offset = _offset;
                frequency = _frequency;
                ctr = 0;
                behaviours = _behaviours;
                stiffness = _stiffness;
                braceStiffness = _braceStiffness;
                dynamic = _dynamic;
                tryToBrace = _tryToBrace;
            }

            public override void runOn(Graph l)
            {
                if (ctr % frequency == 0)
                {
                    //duplicate the last node and make links
                    Node n = new Node(l.parent);
                    n.addSelf(offset);

                    if (dynamic)
                    {
                        //make agent if dynamic
                        Particle p = new Particle(n);

                        l.swapConnections(l.parent, p, tryToBrace, stiffness * scaleFactor, braceStiffness * scaleFactor);

                        ParticleAgent a = new ParticleAgent(p);
                        a.addBehaviours(behaviours);
                        world.addDynamic(p);
                        world.addAgent(a);
                    }
                    else
                    {
                        l.swapConnections(l.parent, n, tryToBrace, stiffness * scaleFactor, braceStiffness * scaleFactor);
                        world.addStatic(n);
                    }
                }
                ctr++;
            }
        }
    }
}
