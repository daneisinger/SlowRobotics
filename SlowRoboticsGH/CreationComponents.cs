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

namespace SlowRoboticsGH
{
    public class CreateAgentsComponent : GH_Component
    {
        public CreateAgentsComponent() : base("Create Agents", "CreateAgents", "Create Agents from a collection of planes", "SlowRobotics", "Agent") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{93c48701-8b2f-43ed-8a6a-fb82ebdf3737}");
        // protected override System.Drawing.Bitmap Icon => Properties.Resources.iconCommand;
        protected override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Planes", "P", "Agent Planes", GH_ParamAccess.list);
            pManager.AddParameter(new BehaviourParameter(), "Behaviours", "B", "Behaviours for agents", GH_ParamAccess.list);
            pManager.AddParameter(new WorldParameter(), "World", "W", "World to contain agents", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Plane> planes = new List<Plane>();
            List<GH_Behaviour> behaviours = new List<GH_Behaviour>();
            GH_World world = null;

            if (!DA.GetDataList(0,  planes)) { return; }
            if (!DA.GetDataList(1,  behaviours)) { return; }
            if (!DA.GetData(2, ref world)) { return; }

            List<Behaviour> agentBehaviours = (behaviours.ConvertAll(b => { return b.Value; }));
            List<Agent> agents = new List<Agent>();
            foreach(Plane3D p in planes.ConvertAll(x => { return IO.ConvertToPlane3D(x); }))
            {
                Agent a = new PlaneAgent(p, world.Value);
                foreach (Behaviour b in agentBehaviours) a.addBehaviour(b);
                agents.Add(a);
            }

            DA.SetData(0, new GH_ObjectWrapper(agents));

        }
    }
}
