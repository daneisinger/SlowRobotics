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
    public class SimulateWorldComponent : GH_Component
    {
        public SimulateWorldComponent() : base("Simulate World", "SimWorld", "Updates all particles and links in the world", "SlowRobotics", "Simulation") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{6d564eab-11d8-4dd7-af01-f7cfc5d435e7}");
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
            pManager.AddParameter(new WorldParameter(), "World", "W", "World to simulate", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new WorldParameter(), "World", "w", "World state", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_World world = null;

            if (!DA.GetData(0, ref world)) { return; }

            world.Value.run();

            DA.SetData(0, world);

        }
    }


    public class AddToWorldComponent : GH_Component
    {
        public AddToWorldComponent() : base("Add Nodes To World", "AddWorld", "Add wrapped list of nodes to a world", "SlowRobotics", "Agent") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{25adadd2-b209-4f16-9f3b-75c5e38ec22c}");
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
                    else if (wrapper.Value is Node)
                    {
                        addAgent((Node)wrapper.Value, world.Value, false);
                    }
                    else if (wrapper.Value is LinkMesh)
                    {
                        addAgent((LinkMesh)wrapper.Value, world.Value, false);
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

    public class AddBehavioursToAgents : GH_Component
    {
        public AddBehavioursToAgents() : base("Add Behaviours", "AddBehaviours", "Add behaviours to agents", "SlowRobotics", "Agent") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{674aab77-4e92-4260-b23d-01656da24a08}");
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
                    ((IAgent)a).setBehaviours(behaviours.ConvertAll(b => { return b.Value; }));
                }
            }
            else if (wrapper.Value is IAgent)
            {
                ((IAgent)wrapper.Value).setBehaviours(behaviours.ConvertAll(b => { return b.Value; }));
            }

            DA.SetData(0, wrapper.Value);
        }
    }
}
