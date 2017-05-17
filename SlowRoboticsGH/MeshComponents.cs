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
        public MeshPipeComponent() : base("Mesh Pipe", "MeshPipe", "Create a mesh by piping a curve", "Nursery", "Mesh") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{06a11782-d367-4cdd-98c6-b24c22bef473}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Curve to pipe", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Number of Sections", "S", "Divides the curve into n polygons", GH_ParamAccess.item,10);
            pManager.AddNumberParameter("Radius", "R", "Radius of the pipe polygon", GH_ParamAccess.item,1);
            pManager.AddIntegerParameter("Number of Sides", "F", "Sides to the pipe polygon", GH_ParamAccess.item,4);
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
        public MeshPolylineSectionsComponent() : base("Mesh Loft", "MeshLoft", "Create a mesh by lofting polyline sections", "Nursery", "Mesh") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{99aafd2f-df87-4795-9596-448274e5a107}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Polylines", "P", "Polylines to loft", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Cap Start", "Cs", "Mesh the first section", GH_ParamAccess.item, true);
            pManager.AddBooleanParameter("Cap End", "Ce", "Mesh the last section", GH_ParamAccess.item,true);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Mesh", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> curves = new List<Curve>();
            bool cS = true;
            bool cE = true;

            if (!DA.GetDataList(0, curves)) { return; }
            if (!DA.GetData(1, ref cS)) { return; }
            if (!DA.GetData(2, ref cE)) { return; }
            List<Polyline> polylines = new List<Polyline>();
            foreach (Curve c in curves)
            {
                Polyline p;
                if (c.TryGetPolyline(out p)) polylines.Add(p);
            }

            DA.SetData(0, Mesher.buildClosedMeshFromPolylineSections(polylines, cS, cE));
        }
    }

    public class FillMeshComponent : GH_Component
    {
        public FillMeshComponent() : base("Fill Mesh", "FillMesh", "Fill a mesh with maximum n planes that align with the nearest normal on the mesh. Will try n*10 times to fill. Creates planes on surface of mesh if not closed", "Nursery", "Mesh") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{f7c2509a-4b87-49a6-b29d-0bc251a95b28}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Mesh to Populate", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Number", "N", "Number of points to generate", GH_ParamAccess.item, 100);
            pManager.AddNumberParameter("Tolerance", "T", "Acceptable tolerance for pop", GH_ParamAccess.item, 1);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddPlaneParameter("Points", "P", "Points in mesh", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            
            Mesh mesh = null;
            int n = 100;
            double t = 1;
            if (!DA.GetData(0, ref mesh)) { return; }
            if (!DA.GetData(1, ref n)) { return; }
            if (!DA.GetData(2, ref t)) { return; }

            Random r = new Random();
            BoundingBox b = mesh.GetBoundingBox(false);
            List<Plane> pts = new List<Plane>();
            int ctr = 0;
            int i = 0;
            mesh.FaceNormals.ComputeFaceNormals();

            while (ctr < n && i < n*10)
            {
                i += 1;
                Point3d tmp = new Point3d(b.PointAt(r.NextDouble(), r.NextDouble(), r.NextDouble()));
                if (!mesh.IsClosed)
                {
                    Point3d cpt = mesh.ClosestPoint(tmp);
                    MeshPoint mpt = mesh.ClosestMeshPoint(cpt, t);
                    pts.Add(new Plane(mpt.Point, mesh.FaceNormals[mpt.FaceIndex]));
                    ctr += 1;
                }
                else {
                    if (mesh.IsPointInside(tmp, t, false))
                    {
                        Point3d cpt = mesh.ClosestPoint(tmp);
                        MeshPoint mpt = mesh.ClosestMeshPoint(cpt, t);
                        pts.Add(new Plane(tmp, mesh.FaceNormals[mpt.FaceIndex]));
                        ctr += 1;
                    }
                }
            }
            DA.SetDataList(0,pts);
        }
    }

    public class MeshNormalComponent : GH_Component
    {
        public MeshNormalComponent() : base("Mesh Normal", "MeshNormal", "Creates a plane from mesh face normal at a given point", "Nursery", "Mesh") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{d6f19cae-5207-4076-be7f-8d5b24949706}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Mesh to Populate", GH_ParamAccess.item);
            pManager.AddPointParameter("Points", "P", "Plane origins", GH_ParamAccess.list);
            pManager.AddNumberParameter("Max Distance", "Mx", "Maximum distance for mesh closest point", GH_ParamAccess.item, 100);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddPlaneParameter("Planes", "P", "Planes on mesh", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {

            Mesh mesh = null;
            List<Point3d> pts = new List<Point3d>();
            double d = 100;

            if (!DA.GetData(0, ref mesh)) { return; }
            if (!DA.GetDataList(1, pts)) { return; }
            if (!DA.GetData(2, ref d)) { return; }

            mesh.FaceNormals.ComputeFaceNormals();

            List<Plane> faceNormals = new List<Plane>();

            foreach (Point3d p in pts)
            {
                MeshPoint pt = mesh.ClosestMeshPoint(p, d);
                Vector3d f = mesh.FaceNormals[pt.FaceIndex];
                faceNormals.Add(new Plane(p,f));
            }

            DA.SetDataList(0, faceNormals);
        }
    }
}
