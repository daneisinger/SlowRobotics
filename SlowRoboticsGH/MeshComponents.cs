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
using SlowRobotics.Rhino.MeshTools;


namespace SlowRoboticsGH
{
    public class MeshPipeComponent : GH_Component
    {
        public MeshPipeComponent() : base("Mesh Pipe", "MeshPipe", "Create a mesh by piping a curve", "SlowRobotics", "Mesh") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{06a11782-d367-4cdd-98c6-b24c22bef473}");
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
            pManager.AddCurveParameter("Curve", "C", "Curve to pipe", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Number of Sections", "S", "Divides the curve into n polygons", GH_ParamAccess.item);
            pManager.AddNumberParameter("Radius", "R", "Radius of the pipe polygon", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Number of Sides", "F", "Sides to the pipe polygon", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Mesh", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve curve = null;
            int sections = 1;
            double radius = 1;
            int sides = 1;

            if (!DA.GetData(0, ref curve)) { return; }
            if (!DA.GetData(1, ref sections)) { return; }
            if (!DA.GetData(2, ref radius)) { return; }
            if (!DA.GetData(3, ref sides)) { return; }

            DA.SetData(0, Mesher.pipeCurve(curve, sections, (float)radius, sides));
        }
    }

    public class MeshPolylineSectionsComponent : GH_Component
    {
        public MeshPolylineSectionsComponent() : base("Mesh Loft", "MeshLoft", "Create a mesh by lofting polyline sections", "SlowRobotics", "Mesh") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{99aafd2f-df87-4795-9596-448274e5a107}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.meshPipe;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Polylines", "P", "Polylines to loft", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Mesh", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> curves = new List<Curve>();

            if (!DA.GetDataList(0, curves)) { return; }

            List<Polyline> polylines = new List<Polyline>();
            foreach (Curve c in curves)
            {
                Polyline p;
                if (c.TryGetPolyline(out p)) polylines.Add(p);
            }

            DA.SetData(0, Mesher.buildClosedMeshFromPolylineSections(polylines));
        }
    }
}
