using Grasshopper.Kernel;
using Grasshopper.Kernel.Geometry;
using Rhino.Geometry;
using SlowRobotics.Agent;
using SlowRobotics.Core;
using SlowRobotics.Rhino.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRoboticsGH
{
    public class DrawAllPlanesComponent : GH_Component
    {
        public DrawAllPlanesComponent() : base("Draw All Planes", "DrawAll", "Draw all planes in the world", "SlowRobotics", "IO") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{eb1dadf0-fe18-4c52-93cb-c5febf8d402a}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new WorldParameter(), "World", "W", "World to draw", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddPlaneParameter("Planes", "P", "Planes", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_World world = null;

            if (!DA.GetData(0, ref world)) { return; }
            List<Rhino.Geometry.Plane> planes = new List<Rhino.Geometry.Plane>();
            foreach (Vec3D p in world.Value.getPoints())
            {
                if (p is Plane3D) planes.Add(IO.ToPlane((Plane3D)p));
            }

            DA.SetDataList(0, planes);
        }
    }

    public class DrawAgentPlanes : GH_Component
    {
        public DrawAgentPlanes() : base("Draw Agent Planes", "DrawAgents", "Draw all agents in the world", "SlowRobotics", "IO") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{3cf0a1bc-f1f8-4005-8ef5-38aaa276aa9e}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new WorldParameter(), "World", "W", "World to draw", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddPlaneParameter("Planes", "P", "Planes", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_World world = null;

            if (!DA.GetData(0, ref world)) { return; }
            List<Rhino.Geometry.Plane> planes = new List<Rhino.Geometry.Plane>();
            foreach (Vec3D p in world.Value.getPop())
            {
                if (p is Plane3D) planes.Add(IO.ToPlane((Plane3D)p));
            }

            DA.SetDataList(0, planes);
        }
    }

    public class DrawLinksComponent : GH_Component
    {
        public DrawLinksComponent() : base("Draw Links", "DrawLinks", "Draw all links in the world", "SlowRobotics", "IO") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{48b5ead6-b9d5-4b27-9ce9-fc83d3eb6b49}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new WorldParameter(), "World", "W", "World to draw", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("Primary Links", "P", "Primary Links", GH_ParamAccess.list);
            pManager.AddLineParameter("Tertiary Links", "T", "Tertiary Links", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_World world = null;

            if (!DA.GetData(0, ref world)) { return; }

            List<Line> l = new List<Line>();
            List<Line> lb = new List<Line>();

            world.Value.getPop().ForEach(agent => {
                if (agent is LinkMesh)
                {
                    LinkMesh lm = (LinkMesh)agent;
                    List<Link> springs = lm.getLinks();
                    foreach (Link li in springs)
                    {
                        l.Add(new Line(new Point3d(li.a.x, li.a.y, li.a.z), new Point3d(li.b.x, li.b.y, li.b.z)));
                    }
                }
            });

            world.Value.getPop().ForEach(agent => {
                if (agent is LinkMesh)
                {
                    LinkMesh lm = (LinkMesh)agent;
                    List<Link> springs = lm.getTertiaryLinks();
                    foreach (Link li in springs)
                    {
                        lb.Add(new Line(new Point3d(li.a.x, li.a.y, li.a.z), new Point3d(li.b.x, li.b.y, li.b.z)));
                    }
                }
            });

            DA.SetDataList(0, l);
            DA.SetDataList(1, lb);
        }
    }

    public class ToPlaneComponent : GH_Component
    {
        public ToPlaneComponent() : base("Convert to Plane", "ConvertPlane", "Convert to plane if possible", "SlowRobotics", "IO") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{fc0fe70f-db2e-4eed-b45c-4b9fc72df6b7}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Plane3DParameter(), "Plane3D", "P", "Plane to convert", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane", "P", "Plane", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_Plane3D p = null;

            if (!DA.GetData(0, ref p)) { return; }

            DA.SetData(0, IO.ToPlane(p.Value));
        }
    }
}
