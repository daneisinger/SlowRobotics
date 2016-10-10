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
}
