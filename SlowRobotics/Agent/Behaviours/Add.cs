
using SlowRobotics.Core;
using SlowRobotics.Spatial;
using SlowRobotics.SRGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Agent.Behaviours
{
    public class Add
    {

        public class PointToWorld : Behaviour<Vec3D>
        {
            public int frequency { get; set; }
            public ISearchable pts {get;set;}
            int c;

            public PointToWorld(int _priority, ISearchable _pts, int _frequency) :base(_priority)
            {
                pts = _pts;
                frequency = _frequency;
                c = 0;
            }

            public override void runOn(Vec3D a)
            {
                if (c++ % frequency == 0)pts.Add(a); 
            }
        }

        public class Extend : ScaledBehaviour<Graph<SRParticle,Spring>>
        {
            public ISearchable pts { get; set; }
            public AgentList pop { get; set; }
            public Vec3D offset { get; set; }
            public List<IBehaviour> behaviours { get; set; }

            public int frequency { get; set; }
            public int ctr;
            public float stiffness { get; set; }

            public Extend(int _priority, int _frequency, Vec3D _offset, float _stiffness,List<IBehaviour> _behaviours, ISearchable _pts) : base(_priority)
            {
                pop = new AgentList();
                pts = _pts;
                offset = _offset;
                frequency = _frequency;
                ctr = 0;
                behaviours = _behaviours;
                stiffness = _stiffness;
                lateUpdate = true;
            }

            public override void runOn(Graph<SRParticle,Spring> graph)
            {
                if (ctr++ % frequency == 0)
                {
                    //get head particle
                    SRParticle p = graph.parent;
                    SRParticle clone = new SRParticle(p.add(offset));
                    //point nodes to new particle
                    graph.replaceGeometry(p, clone);
                    //get the last node
                    INode<SRParticle> lastNode;
                    if(graph.getNodeAt(clone, out lastNode))
                    {
                        //make new connection if possible
                        Spring newConnection = new Spring(lastNode, new Node<SRParticle>(p));
                        graph.insert(newConnection);
                        
                    }
                    //make new agent
                    AgentT<SRParticle> a = new AgentT<SRParticle>(clone);
                    a.addBehaviours(behaviours);
                    //add to world -------------------------------------------TODO - use graph to store things instead
                    pop.add(a);
                }
            }
        }
        
        public class Split : ScaledBehaviour<Graph<SRParticle, Spring>>
        {
            public ISearchable pts { get; set; }
            public AgentList pop { get; set; }
            public float maxLength { get; set; }
            public List<IBehaviour> behaviours { get; set; }

            public Split(int _priority, List<IBehaviour> _behaviours, float _maxLength, ISearchable _pts) : base(_priority)
            {
                pop = new AgentList();
                pts = _pts;
                behaviours = _behaviours;
                maxLength = _maxLength;
                lateUpdate = true;
            }

            public override void onAdd()
            {
                //pop = new AgentList(); //delete pop when added to agent
            }

            public override void runOn(Graph<SRParticle, Spring> graph)
            {
                List<Spring> toRemove = new List<Spring>();
                //get springs
                foreach (Spring s in graph.Edges)
                {
                    if (s.tag != "brace")
                    {
                        //check length
                        if (s.getLength() > maxLength)
                        {

                            //split the edge
                            Vec3D p = s.pointAt(0.5f);
                            SRParticle mid = new SRParticle(new Plane3D(p));

                            AgentT<SRParticle> a = new AgentT<SRParticle>(mid);
                            a.addBehaviours(behaviours);
                            pop.add(a);
                            
                            Spring am = new Spring(s.a.Geometry, mid);
                            Spring mb = new Spring(mid, s.b.Geometry);

                            //add two new edges
                            graph.insert(am);
                            graph.insert(mb);
                            //remove the edge
                            toRemove.Add(s);
                        }

                    }
                    toRemove.ForEach(x => graph.removeEdge(x));
                }
            }
        }

        public class Brace : ScaledBehaviour<Graph<SRParticle, Spring>>
        {

            public float stiffness { get; set; }

            public Brace(int _priority, float _stiffness) : base(_priority)
            {
                stiffness = _stiffness;
                lateUpdate = true;
            }

            public override void runOn(Graph<SRParticle, Spring> graph)
            {

                //loop through nodes
                foreach (INode<SRParticle> n in graph.Nodes) {
                    if (n.Tag != "b")
                    {
                        //if node has two edges
                        List<Spring> edges = graph.getEdgesFor(n.Geometry).Where(x => x.tag != "brace").ToList();
                        if (edges.Count == 2)
                        {
                            Spring s1 = edges[0];
                            Spring s2 = edges[1];

                            Spring newConnection = new Spring(s1.Other(n), s2.Other(n));
                            float rL = s1.l + s2.l;
                            //get rest length

                            newConnection.l = rL;
                            newConnection.tag = "brace";
                            newConnection.s = stiffness;
                            graph.insert(newConnection);
                            n.Tag = "b";
                        }
                    }
                }
            }
        }
    }
}
