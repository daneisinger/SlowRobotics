using Grasshopper.Kernel;
using Grasshopper.Kernel.Geometry;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using SlowRobotics.Agent;
using SlowRobotics.Core;
using SlowRobotics.Rhino.IO;
using SlowRobotics.SRGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRoboticsGH
{
   public static class SR_GH_IO
    {
        public static List<IAgent> getFromWrapper(GH_ObjectWrapper wrapper)
        {
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
            else if (wrapper.Value is IAgent) addTo.Add((IAgent)wrapper.Value);
            else if (wrapper.Value is List<GH_Agent>)
            {
                List<GH_Agent> agents = (List<GH_Agent>)wrapper.Value;
                foreach (GH_Agent a in agents) addTo.Add(a.Value);
            }
            else if (wrapper.Value is GH_Agent) addTo.Add(((GH_Agent)wrapper.Value).Value);

            return addTo;
        }
    }
    public class DrawNeighboursComponent : GH_Component
    {
        public DrawNeighboursComponent() : base("Draw Neighbours", "DrawNeighbours", "Draws lines to points in the agent neighbour list", "Nursery", "Draw") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{2a3d7cd7-f1c0-4be7-ba97-a52975ab1c3f}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Agents", "P", "Agents to draw connections for", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("Neighbours", "N", "Neighbours", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_ObjectWrapper wrapper = null;

            if (!DA.GetData(0, ref wrapper)) { return; }


            List<Line> connections = new List<Line>();

            foreach (IAgent agent in SR_GH_IO.getFromWrapper(wrapper))
            {
                IAgentT<object> a = (IAgentT<object>)agent;
                Vec3D p = a.getData() as Vec3D;
                if (p != null)
                {
                    foreach (Vec3D v in a.neighbours)
                    {
                        connections.Add(new Line(p.ToPoint3d(), v.ToPoint3d()));
                    }
                }
            }
            DA.SetDataList(0, connections);
        }
    }

    public class DrawParticlesComponent : GH_Component
    {
        public DrawParticlesComponent() : base("Draw Planes", "Draw", "Draws any agents that contain plane data", "Nursery", "Draw") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{a21720f3-b133-4e98-8466-60246ba461e6}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Agents", "P", "Agents to draw planes for", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddPlaneParameter("Planes", "P", "Planes", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_ObjectWrapper wrapper = null;

            if (!DA.GetData(0, ref wrapper)) { return; }

            List<GH_Plane> planes = new List<GH_Plane>();

            foreach (IAgent agent in SR_GH_IO.getFromWrapper(wrapper))
            {
                IAgentT<object> a = (IAgentT<object>)agent;
                Plane3D p = a.getData() as Plane3D;
                if (p != null)
                {
                  planes.Add(new GH_Plane(p.ToPlane()));
                }
            }
            DA.SetDataList(0, planes);
        }
    }

    public class DrawGraphComponent : GH_Component
    {
        public DrawGraphComponent() : base("Draw Graph", "Draw", "Draws any agents that contain Graph<ILine> data", "Nursery", "Draw") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{2b489ae4-cfc9-4d8d-b9d2-643df7011c73}");
        protected override System.Drawing.Bitmap Icon => Properties.Resources.createNode;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Agents", "P", "Agents to draw planes for", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("Edges", "E", "Graph Edges", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_ObjectWrapper wrapper = null;

            if (!DA.GetData(0, ref wrapper)) { return; }

            List<GH_Line> lines = new List<GH_Line>();

            foreach (IAgent agent in SR_GH_IO.getFromWrapper(wrapper))
            {
                IAgentT<object> a = (IAgentT<object>)agent;
                Graph<SRParticle,Spring> g = a.getData() as Graph<SRParticle, Spring>;
                if (g != null)
                {
                    foreach(Spring s in g.Edges)
                    {
                        lines.Add(new GH_Line(new Line(s.start.ToPoint3d(), s.end.ToPoint3d())));
                    }
                }
            }
            DA.SetDataList(0, lines);
        }
    }
}
