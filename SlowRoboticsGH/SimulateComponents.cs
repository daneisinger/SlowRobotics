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

    public class CreatePopulationComponent :GH_Component
    {
        public CreatePopulationComponent() : base("Create Population", "CreatePop", "Creates an agent list for simulation", "Nursery", "Simulation") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{de35d78e-dc9a-4e2a-ae4c-708ee7ec823c}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new AgentParameter(), "Agents", "P", "Agents to add to population", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new AgentListParameter(), "Agents", "P", "Population", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<GH_Agent> pop = new List<GH_Agent>();

            if (!DA.GetDataList(0, pop)) { return; }

            AgentList list = new AgentList();
            foreach (GH_Agent a in pop) list.add(a.Value);
            list.populate();

            DA.SetData(0, list);
        }
    }

    public class SimulateFieldComponent : GH_Component
    {
        public SimulateFieldComponent() : base("Simulate Field", "SimField", "Traces lines through a field", "Nursery", "Field") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{995ccc70-edc1-44a3-b380-e1f33bb3c2cb}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Plane3DParameter(),"Planes", "P", "Starting planes", GH_ParamAccess.item);
            pManager.AddParameter(new FieldParameter(),"Field", "F", "Field to simulate", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Simulation steps", "S", "Number of simulation steps", GH_ParamAccess.item,100);
            pManager.AddNumberParameter("Strength", "St", "Strength Multiplier", GH_ParamAccess.item,1);
            pManager.AddBooleanParameter("Normalize", "N", "Normalize field strength", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddPlaneParameter("Planes", "P", "Field planes", GH_ParamAccess.list);
        }
        protected override void SolveInstance (IGH_DataAccess DA)
        {

            Plane3D plane = null;
            GH_Field f = null;
            int steps = 1;
            double strength = 1;
            bool normalize = false;

            if (!DA.GetData(0, ref plane)) { return; }
            if (!DA.GetData(1, ref f)) { return; }
            if (!DA.GetData(2, ref steps)) { return; }
            if (!DA.GetData(3, ref strength)) { return; }
            if (!DA.GetData(4, ref normalize)) { return; }

            Plane3D init = new Plane3D(plane);
            List<Plane> planes = new List<Plane>();
            
            for (int i = 0; i < steps; i++)
            {
                FieldData data = f.Value.evaluate(init);
                Vec3D dir = new Vec3D();
                //TODO implement number data without flipping vectors
                
                if (data.hasPlaneData()) dir.addSelf(data.planeData.wx);
                if (data.hasVectorData()) dir.addSelf(data.vectorData);

                if(normalize) dir.normalizeTo((float)strength);
                init.addSelf(dir);
                init.interpolateToXX(dir, 1);
                planes.Add(init.ToPlane());
            }
            
            DA.SetDataList(0, planes);
        }
    }

    public class SimulateComponent : GH_Component
    {
        public SimulateComponent() : base("Simulate", "Sim", "Updates all agents", "Nursery", "Simulation") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{6d564eab-11d8-4dd7-af01-f7cfc5d435e7}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new AgentListParameter(),"Agents", "P", "Population to simulate", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Solver steps", "S", "Steps per update", GH_ParamAccess.item,1);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new AgentListParameter(), "Agents", "P", "Population", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<GH_AgentList> list = new List<GH_AgentList>();
            int steps = 1;

            if (!DA.GetDataList(0, list)){ return; }
            if (!DA.GetData(1, ref steps)) { return; } 

            foreach(GH_AgentList l in list)Core.run(l.Value, 1 / (float)steps);


            DA.SetDataList(0, list);
        }
    }
    
    public class FixParticlesComponent : GH_Component
    {
        public FixParticlesComponent() : base("Fix Particles", "FixParticles", "Fix particles by proximity to a list of points", "Nursery", "Simulation") { }
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
            IAgentT<Vec3D> typedAgent = (IAgentT<Vec3D>)agent;
            if (typedAgent != null)
            {
                foreach (Point3d p in pts)
                {
                    if (typedAgent.getData().distanceTo(new Vec3D((float)p.X, (float)p.Y, (float)p.Z)) < 1)
                    {
                        agent.removeBehaviours();
                        if (agent is SlowRobotics.Core.SRParticle) ((SlowRobotics.Core.SRParticle)agent).f = true;
                        return;
                    }
                }
            }
            else
            {
                throw new TypeAccessException("Incorrect agent type, try implementing IAgentT<Vec3D>");
            }
        }
    }

    public class SetBehaviours : GH_Component
    {
        public SetBehaviours() : base("Set Behaviours", "SetBehaviours", "Set agent behaviours", "Nursery", "Agent") { }
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

            List<IAgent> addTo = new List<IAgent>();


            if (wrapper.Value is GH_AgentList)
            {
                AgentList agents = ((GH_AgentList)wrapper.Value).Value;
                addTo = agents.getAgents();
            }
            else if (wrapper.Value is AgentList)
            {
                AgentList agents = (AgentList)wrapper.Value;
                addTo = agents.getAgents();
            }
            else if (wrapper.Value is List<IAgent>) addTo = (List<IAgent>)wrapper.Value;
            else if (wrapper.Value is IAgent) ((IAgent)wrapper.Value).setBehaviours(behaviours.ConvertAll(b => { return b.Value; }));
            else if (wrapper.Value is List<GH_Agent>)
            {
                List<GH_Agent> agents = (List<GH_Agent>)wrapper.Value;
                foreach (GH_Agent a in agents) a.Value.setBehaviours(behaviours.ConvertAll(b => { return b.Value; }));
            }
            else if (wrapper.Value is GH_Agent) ((GH_Agent)wrapper.Value).Value.setBehaviours(behaviours.ConvertAll(b => { return b.Value; }));

            foreach (IAgent a in addTo)
            {
                a.setBehaviours(behaviours.ConvertAll(b => { return b.Value; }));
            }

            DA.SetData(0, wrapper.Value);
        }
    }

    public class AddBehaviours : GH_Component
    {
        public AddBehaviours() : base("Add Behaviours", "AddBehaviours", "Add behaviours to agents", "Nursery", "Agent") { }
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
                    a.addBehaviours(behaviours.ConvertAll(b => { return b.Value; }));
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
                    a.Value.addBehaviours(behaviours.ConvertAll(b => { return b.Value; }));
                }
            }
            else if (wrapper.Value is GH_Agent)
            {
                ((GH_Agent)wrapper.Value).Value.addBehaviours(behaviours.ConvertAll(b => { return b.Value; }));
            }

           
            DA.SetData(0, wrapper.Value);
        }
    }
}
