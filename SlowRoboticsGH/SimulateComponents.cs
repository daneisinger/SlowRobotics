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
    public class SimulateFieldComponent : GH_Component
    {
        public SimulateFieldComponent() : base("Simulate Field", "SimField", "Traces lines through a field", "Nursery", "Simulation") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{995ccc70-edc1-44a3-b380-e1f33bb3c2cb}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.SimulateField;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Plane3DParameter(),"Planes", "P", "Starting planes", GH_ParamAccess.item);
            pManager.AddParameter(new FieldParameter(),"Field", "F", "Field to simulate", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Simulation steps", "S", "Number of simulation steps", GH_ParamAccess.item,100);
            pManager.AddNumberParameter("Limit", "L", "Field weight limit", GH_ParamAccess.item,1);
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
            double limit = 1;
            bool normalize = false;

            if (!DA.GetData(0, ref plane)) { return; }
            if (!DA.GetData(1, ref f)) { return; }
            if (!DA.GetData(2, ref steps)) { return; }
            if (!DA.GetData(3, ref limit)) { return; }
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

                if (normalize)
                {
                    dir.normalizeTo((float)limit);
                }
                else
                {
                    dir.limit((float)limit);
                }
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
        protected override System.Drawing.Bitmap Icon => Properties.Resources.Simulate;

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

            foreach(GH_AgentList l in list)Core.run(l.Value, steps);


            DA.SetDataList(0, list);
        }
    }
    
    public class FixParticlesComponent : GH_Component
    {
        public FixParticlesComponent() : base("Fix Particles", "FixParticles", "Fix particles by proximity to a list of points", "Nursery", "Utilities") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("{e90f5cb9-f76d-4ec9-bb73-c93b3210cc40}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.FixParticles;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new ParticleParameter(),"Particle", "P", "Particle to fix", GH_ParamAccess.item);
            pManager.AddPointParameter("Points", "P", "Fix Points", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new ParticleParameter(), "Particles", "P", "Particles", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {

            List<Point3d> points = new List<Point3d>();
            GH_Particle ghP = null; 

            if (!DA.GetData(0, ref ghP)) { return; }
            if (!DA.GetDataList(1, points)) { return; }

            //make a copy
            IParticle part = ghP.Value.duplicate();
            foreach (Point3d p in points)
            {
                if (part.get().distanceTo(new Vec3D((float)p.X, (float)p.Y, (float)p.Z)) < 1)
                {
                    part.f = true;
                    break;
                }
            }
            
            DA.SetData(0, part);
        }

        public void testPt(IParticle part, List<Point3d> pts)
        {

            foreach (Point3d p in pts)
            {
                if (part.get().distanceTo(new Vec3D((float)p.X, (float)p.Y, (float)p.Z)) < 1)
                {
                    part.f = true;
                    return;
                }
            }
        }
    }

    public class SetBehaviours : GH_Component
    {
        public SetBehaviours() : base("Set Behaviours", "SetBehaviours", "Set agent behaviours", "Nursery", "Simulation") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{8aecd428-5f62-4003-939a-baf18729a08f}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.SetBehaviour;

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
}
