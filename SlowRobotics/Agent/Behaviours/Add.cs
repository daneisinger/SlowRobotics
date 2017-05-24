
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
    /// <summary>
    /// Parent class for behaviours that add objects to the simulation
    /// </summary>
    public class Add
    {
        /// <summary>
        /// Vec3D Behaviour: Inserts a point in a given searchable point collection
        /// </summary>
        public class InsertPoint : Behaviour<Vec3D>
        {
            public int frequency { get; set; }
            public ISearchable pts {get;set;}
            int c;

            /// <summary>
            /// Default constructor
            /// </summary>
            /// <param name="_priority">Behaviour priority</param>
            /// <param name="_pts">Point collection to add point to</param>
            /// <param name="_frequency">Add every n steps</param>
            public InsertPoint(int _priority, ISearchable _pts, int _frequency) :base(_priority)
            {
                pts = _pts;
                frequency = _frequency;
                c = 0;
            }

            /// <summary>
            /// Adds p to point collection.
            /// </summary>
            /// <param name="a"></param>
            public override void runOn(Vec3D p)
            {
                if (c++ % frequency == 0)pts.Add(p); 
            }
        }

        /// <summary>
        /// Graph<SRParticle,Spring> Behaviour: Extends a graph by duplicating the parent particle
        /// of the graph, moving this particle with a given offset and creating a new spring between the
        /// parent and the duplicate. The duplicate is then wrapped in an Agent with the given behaviours and added to the AgentList parameter  
        /// for subsequent simulation. This behaviour is used when the parent particle is running other behaviours to simulate extrusions,
        /// cloth manipulation, bending etc.
        /// </summary>
        public class Extend : ScaledBehaviour<Graph<SRParticle,Spring>>
        {
            public AgentList pop { get; set; }
            public Vec3D offset { get; set; }
            public List<IBehaviour> behaviours { get; set; }
            public int frequency { get; set; }
            public float stiffness { get; set; }

            /// <summary>
            /// Default constructor
            /// </summary>
            /// <param name="_priority">Behaviour priority</param>
            /// <param name="_frequency">Extend the graph every n steps</param>
            /// <param name="_offset">Vector defining how much to displace the duplicate of the parent particle</param>
            /// <param name="_stiffness">Stiffness of extension spring</param>
            /// <param name="_behaviours">Collection of behaviours to add to the duplicated particle</param>
            public Extend(int _priority, int _frequency, Vec3D _offset, float _stiffness, List<IBehaviour> _behaviours) : base(_priority)
            {
                pop = new AgentList();
                offset = _offset;
                frequency = _frequency;
                behaviours = _behaviours;
                stiffness = _stiffness;
                lateUpdate = true;
            }

            public override void runOn(Graph<SRParticle,Spring> graph)
            {
                SRParticle p = graph.parent;
                if (p.age % frequency == 0)
                {
                    //get head particle
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
                        //make new agent
                        Agent<SRParticle> a = new Agent<SRParticle>(clone);
                        a.addBehaviours(behaviours);
                        //add to world -------------------------------------------TODO - use graph to store things instead
                        pop.add(a);
                    }
                }
            }
        }

        /// <summary>
        /// Graph<SRParticle,Spring> Behaviour: Splits all springs in a graph if they are over a 
        /// given length by inserting a new particle at their midpoint and creating two new springs.
        /// The new particle is wrapped in an Agent with given behaviours and added to the pop parameter for subsequent simulation.
        /// </summary>
        public class Split : ScaledBehaviour<Graph<SRParticle, Spring>>
        {
            public AgentList pop { get; set; }
            public float maxLength { get; set; }
            public List<IBehaviour> behaviours { get; set; }

            /// <summary>
            /// Default constructor
            /// </summary>
            /// <param name="_priority">Behaviour priority</param>
            /// <param name="_behaviours">Behaviours for new agent</param>
            /// <param name="_maxLength">Maximum length of spring before splitting</param>
            public Split(int _priority, List<IBehaviour> _behaviours, float _maxLength) : base(_priority)
            {
                pop = new AgentList();
                behaviours = _behaviours;
                maxLength = _maxLength;
                lateUpdate = true;
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

                            Agent<SRParticle> a = new Agent<SRParticle>(mid);
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

        /// <summary>
        /// Graph<SRParticle,Spring> Behaviour: Braces a graph by iterating through all nodes, getting connecting edges and adding a new
        /// spring between end points. Nodes that have been braced are assigned the "b" tag for speedup - implement a custom brace behaviour
        /// if the node tag property is required elsewhere in your model.
        /// </summary>
        public class Brace : ScaledBehaviour<Graph<SRParticle, Spring>>
        {

            public float stiffness { get; set; }

            /// <summary>
            /// Default constructor
            /// </summary>
            /// <param name="_priority">Behaviour priority</param>
            /// <param name="_stiffness">Stiffness of bracing springs</param>
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
