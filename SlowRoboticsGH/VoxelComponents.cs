using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using SlowRobotics.Voxels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRoboticsGH
{
    public class CreateVoxelGridComponent : GH_Component
    {
        public CreateVoxelGridComponent() : base("Create Voxel Grid", "CreateVoxels", "Create Voxel grid from dimensions and bounds", "Nursery", "Voxels") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{76684a60-8999-40f5-8966-c20728c3198f}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter( "Width", "W", "Width of grid (x axis)", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Height", "H", "Height of grid (y axis)", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Depth", "D", "Width of grid (z axis)", GH_ParamAccess.item);
            pManager.AddBoxParameter("Bounds", "B", "Location in model space", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Data Type", "T", "Type of data", GH_ParamAccess.item);

            Param_Integer param = pManager[4] as Param_Integer;
            param.AddNamedValue("Byte", 0);
            param.AddNamedValue("Float", 1);
            param.AddNamedValue("Colour", 2);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new VoxelGridParameter(), "Voxel Grid", "V", "Voxel Grid", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int w = 0;
            int h = 0;
            int d = 0;
            Box b = Box.Unset;
            int t = 1;

            if (!DA.GetData (0, ref w)) { return; }
            if (!DA.GetData(1, ref h)) { return; }
            if (!DA.GetData(2, ref d)) { return; }
            if (!DA.GetData(3, ref b)) { return; }
            if (!DA.GetData(4, ref t)) { return; }

            VoxelGrid grid = null;
            Point3d pmn = b.PointAt(0, 0, 0);
            Point3d pmx = b.PointAt(1, 1, 1);
            float[] min = new float[] {(float)pmn.X,(float)pmn.Y,(float)pmn.Z };
            float[] max = new float[] { (float)pmx.X, (float)pmx.Y, (float)pmx.Z };
            switch (t)
            {
                case (0):
                    grid = new ByteGrid(w, h, d, min, max);
                    break;
                case (1):
                    grid = new FloatGrid(w, h, d, min, max);
                    break;
                case (2):
                    grid = new ColourGrid(w, h, d, min, max);
                    break;
                default:
                    grid = new FloatGrid(w, h, d, min, max);
                    break;
            }

            DA.SetData(0, grid);

        }
    }
}
