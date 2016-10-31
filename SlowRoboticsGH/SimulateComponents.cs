using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Drawing;
using SlowRobotics.Agent.Behaviours;
using Grasshopper.Kernel.Parameters;
using SlowRobotics.Rhino.IO;
using SlowRobotics.Agent;
using SlowRobotics.Core;
using Grasshopper.Kernel.Types;
using SlowRobotics.Field;
using Toxiclibs.core;

namespace SlowRoboticsGH
{
    public class SimulateWorldComponent : GH_Component
    {
        public SimulateWorldComponent() : base("Simulate World", "SimWorld", "Updates all particles and links in the world", "SlowRobotics", "Simulation") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{6d564eab-11d8-4dd7-af01-f7cfc5d435e7}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new WorldParameter(), "World", "W", "World to simulate", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Solver steps", "S", "Steps per update", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new WorldParameter(), "World", "w", "World state", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_World world = null;
            int steps = 1;

            if (!DA.GetData(0, ref world)) { return; }
            if (!DA.GetData(1, ref steps)) { return; }

            world.Value.run(1/(float)steps);

            DA.SetData(0, world);

        }
    }


    public class AddToWorldComponent : GH_Component
    {
        public AddToWorldComponent() : base("Add Nodes To World", "AddWorld", "Add wrapped list of nodes to a world", "SlowRobotics", "Agent") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{25adadd2-b209-4f16-9f3b-75c5e38ec22c}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Agents", "A", "Agents to add", GH_ParamAccess.list);
            pManager.AddParameter(new WorldParameter(), "World", "W", "World to contain agents", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Add Dynamic", "D", "Toggle between adding to dynamic or static trees", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Add Agents", "Add", "Add the agents", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new WorldParameter(), "World", "W", "World", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<GH_ObjectWrapper> wrapperList = new List<GH_ObjectWrapper>();
            GH_World world = null;
            bool dynamic = true;
            bool add = false;

            if (!DA.GetDataList(0, wrapperList)) { return; }
            if (!DA.GetData(1, ref world)) { return; }
            if (!DA.GetData(2, ref dynamic)) { return; }
            if (!DA.GetData(3, ref add)) { return; }

            if (add)
            {

                //TODO - redo all of this casting stuff seems over the top

                foreach (GH_ObjectWrapper wrapper in wrapperList)
                {
                    if (wrapper.Value is List<IAgent>)
                    {
                        List<IAgent> agents = (List<IAgent>)wrapper.Value;
                        foreach (IAgent a in agents)
                        {
                            addAgent((Node)a, world.Value, dynamic);
                        }
                    }
                    else if (wrapper.Value is IAgent)
                    {
                        addAgent((Node)wrapper.Value, world.Value, dynamic);
                    }
                    else if (wrapper.Value is List<GH_Agent>)
                    {
                        List<GH_Agent> agents = (List<GH_Agent>)wrapper.Value;
                        foreach (GH_Agent a in agents)
                        {
                            addAgent((Node)a.Value, world.Value, dynamic);
                        }
                    }
                    else if (wrapper.Value is GH_Agent)
                    {
                        IAgent a = ((GH_Agent)wrapper.Value).Value;
                        addAgent((Node)a, world.Value, dynamic);
                    }
                    else if (wrapper.Value is Node)
                    {
                        addAgent((Node)wrapper.Value, world.Value, false);
                    }
                    else if (wrapper.Value is LinkMesh)
                    {
                        addAgent((LinkMesh)wrapper.Value, world.Value, false);
                    }
                    else if (wrapper.Value is GH_LinkMesh)
                    {
                        addAgent(((GH_LinkMesh)wrapper.Value).Value, world.Value, false);
                    }
                }
            }

            DA.SetData(0, world);
        }

        public void addAgent(Node a, IWorld world, bool dynamic) {
            if (dynamic && a is SlowRobotics.Core.Particle)
            {
                world.addDynamic((SlowRobotics.Core.Particle)a);
            }
            else
            {
                //default to static
                world.addStatic((Node)a);
            }
        }
    }

    /*

        TODO = make this retrieve all nodes for a given parent

    public class RetrieveStatesComponent : GH_Component
    {
        public RetrieveStatesComponent() : base("Retrieve States", "RetrieveStates", "Get any captured agent states", "SlowRobotics", "Agent") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{65722cdd-63e9-4f87-85e4-205b09a56390}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Agents", "A", "Retrieve states from agents", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("States", "S", "States", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_ObjectWrapper wrapper = null;

            if (!DA.GetData(0, ref wrapper)) { return; }

            List<GH_ObjectWrapper> states = new List<GH_ObjectWrapper>();

            if (wrapper.Value is List<IAgent>)
            {
                List<IAgent> agents = (List<IAgent>)wrapper.Value;
                foreach (IAgent a in agents)
                {
                    foreach(IBehaviour cap in a.behaviours.getData())
                    {
                        if (cap is ICaptureBehaviour<Plane3D>)
                        {
                            List<Plane3D> planes = ((ICaptureBehaviour<Plane3D>)cap).get();
                            states.Add(new GH_ObjectWrapper(planes));
                        }
                    }
                }
            }
            else if (wrapper.Value is IAgent)
            {
                foreach (IBehaviour cap in ((IAgent)wrapper.Value).behaviours.getData())
                {
                    if(cap is ICaptureBehaviour<Plane3D>)
                    {
                        List<Plane3D> planes = ((ICaptureBehaviour < Plane3D > )cap).get();
                        states.Add(new GH_ObjectWrapper(planes));
                    }
                    
                }
            }

            DA.SetDataList(0, states);
        }
    }*/

    public class FixParticlesComponent : GH_Component
    {
        public FixParticlesComponent() : base("Fix Particles", "FixParticles", "Fix particles by proximity to a list of points", "SlowRobotics", "Simulation") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("{e90f5cb9-f76d-4ec9-bb73-c93b3210cc40}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Particles", "P", "Particles (agents) to fix", GH_ParamAccess.list);
            pManager.AddPointParameter("Points", "P", "Fix Points", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Agents", "A", "Agents", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> points = new List<Point3d>();
            List<GH_ObjectWrapper> wlist = new List<GH_ObjectWrapper>();

            if (!DA.GetDataList(0, wlist)) { return; }
            if (!DA.GetDataList(1, points)) { return; }

            List<IAgent> outAgents = new List<IAgent>();

            foreach(GH_ObjectWrapper wrapper in wlist)
            {
                if (wrapper.Value is List<IAgent>)
                {
                    List<IAgent> agents = (List<IAgent>)wrapper.Value;
                    foreach (IAgent a in agents)
                    {
                        testPt(a, points);
                        outAgents.Add(a);
                    }
                }
                else if (wrapper.Value is IAgent)
                {
                    testPt((IAgent)wrapper.Value, points);
                    outAgents.Add((IAgent)wrapper.Value);
                }
            }

            DA.SetData(0, new GH_ObjectWrapper(outAgents));
        }

        public void testPt(IAgent agent, List<Point3d> pts)
        {
            if (agent.GetType().IsAssignableFrom(typeof(IStateAgent)))
            {
                IStateAgent a_s = (IStateAgent)agent;
                foreach (Point3d p in pts)
                {
                    if (a_s.getPos().distanceTo(new Vec3D((float)p.X, (float)p.Y, (float)p.Z)) < 1)
                    {
                        agent.setBehaviours(new List<IBehaviour>());
                        if (agent is SlowRobotics.Core.Particle) ((SlowRobotics.Core.Particle)agent).f = true;
                        return;
                    }
                }
            }
        }
    }

    public class ReplaceAgentBehaviours : GH_Component
    {
        public ReplaceAgentBehaviours() : base("Replace Behaviours", "ReplaceBehaviours", "Replace existing agent behaviours", "SlowRobotics", "Agent") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{8aecd428-5f62-4003-939a-baf18729a08f}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Agents", "A", "Agents to modify", GH_ParamAccess.item);
            pManager.AddParameter(new BehaviourParameter(), "Behaviours", "B", "Behaviours for agents", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Agents", "A", "Agents", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_ObjectWrapper wrapper = null;
            List<GH_Behaviour> behaviours = new List<GH_Behaviour>();

            if (!DA.GetData(0, ref wrapper)) { return; }
            if (!DA.GetDataList(1, behaviours)) { return; }

            if (wrapper.Value is List<IAgent>)
            {
                List<IAgent> agents = (List<IAgent>)wrapper.Value;
                foreach (IAgent a in agents)
                {
                    ((IAgent)a).setBehaviours(behaviours.ConvertAll(b => { return b.Value; }));
                }
            }
            else if (wrapper.Value is IAgent)
            {
                ((IAgent)wrapper.Value).setBehaviours(behaviours.ConvertAll(b => { return b.Value; }));
            }
            else if (wrapper.Value is List<GH_Agent>)
            {
                List<GH_Agent> agents = (List<GH_Agent>)wrapper.Value;
                foreach (GH_Agent a in agents)
                {
                    ((GH_Agent)a).Value.setBehaviours(behaviours.ConvertAll(b => { return b.Value; }));
                }
            }
            else if (wrapper.Value is GH_Agent)
            {
                ((GH_Agent)wrapper.Value).Value.setBehaviours(behaviours.ConvertAll(b => { return b.Value; }));
            }

            else if (wrapper.Value is List<GH_LinkMesh>)
            {
                List<GH_LinkMesh> agents = (List<GH_LinkMesh>)wrapper.Value;
                foreach (GH_LinkMesh a in agents)
                {
                    ((GH_LinkMesh)a).Value.setBehaviours(behaviours.ConvertAll(b => { return b.Value; }));
                }
            }
            else if (wrapper.Value is GH_LinkMesh)
            {
                ((GH_LinkMesh)wrapper.Value).Value.setBehaviours(behaviours.ConvertAll(b => { return b.Value; }));
            }
            DA.SetData(0, wrapper.Value);
        }
    }

    public class AddBehavioursToAgents : GH_Component
    {
        public AddBehavioursToAgents() : base("Add Behaviours", "AddBehaviours", "Add behaviours to agents", "SlowRobotics", "Agent") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{674aab77-4e92-4260-b23d-01656da24a08}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Agents", "A", "Agents to modify", GH_ParamAccess.item);
            pManager.AddParameter(new BehaviourParameter(), "Behaviours", "B", "Behaviours for agents", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Agents", "A", "Agents", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_ObjectWrapper wrapper = null;
            List<GH_Behaviour> behaviours = new List<GH_Behaviour>();

            if (!DA.GetData(0, ref wrapper)) { return; }
            if (!DA.GetDataList(1,  behaviours)) { return; }

            if (wrapper.Value is List<IAgent>)
            {
                List<IAgent> agents = (List<IAgent>)wrapper.Value;
                foreach (IAgent a in agents)
                {
                    ((IAgent)a).addBehaviours(behaviours.ConvertAll(b => { return b.Value; }));
                }
            }
            else if (wrapper.Value is IAgent)
            {
                ((IAgent)wrapper.Value).addBehaviours(behaviours.ConvertAll(b => { return b.Value; }));
            }
            else if (wrapper.Value is List<GH_Agent>)
            {
                List<GH_Agent> agents = (List<GH_Agent>)wrapper.Value;
                foreach (GH_Agent a in agents)
                {
                    ((GH_Agent)a).Value.addBehaviours(behaviours.ConvertAll(b => { return b.Value; }));
                }
            }
            else if (wrapper.Value is GH_Agent)
            {
                ((GH_Agent)wrapper.Value).Value.setBehaviours(behaviours.ConvertAll(b => { return b.Value; }));
            }

            else if (wrapper.Value is List<GH_LinkMesh>)
            {
                List<GH_LinkMesh> agents = (List<GH_LinkMesh>)wrapper.Value;
                foreach (GH_LinkMesh a in agents)
                {
                    ((GH_LinkMesh)a).Value.addBehaviours(behaviours.ConvertAll(b => { return b.Value; }));
                }
            }
            else if (wrapper.Value is GH_LinkMesh)
            {
                ((GH_LinkMesh)wrapper.Value).Value.addBehaviours(behaviours.ConvertAll(b => { return b.Value; }));
            }
            DA.SetData(0, wrapper.Value);
        }
    }
}
