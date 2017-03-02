using Grasshopper.Kernel;
using SlowRobotics.Agent;
using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SlowRoboticsGH
{
    /*
    public class GHBehaviour : IBehaviour
    {

        public int priority;
        public IAgent currentAgent { get; set; }
        public Plane3D currentNeighbour { get; set; }
        /// <summary>
        /// Empty constructor with default priority of 1
        /// </summary>
        public GHBehaviour() : this(1)
        {

        }
        /// <summary>
        /// Create new behaviour with a given priority
        /// </summary>
        /// <param name="_priority">Behaviour priority, higher runs first</param>
        public GHBehaviour(int _priority)
        {
            priority = _priority;
        }

        public int CompareTo(IBehaviour other)
        {
            if (other.getPriority() > priority) return -1;
            if (other.getPriority() < priority) return 1;
            return 0;
        }

        public int getPriority()
        {
            return priority;
        }


        //TODO - rethink this to be more generic

        /// <summary>
        /// Cast to appropriate behaviour methods
        /// </summary>
        /// <param name="a"></param>
        public virtual void run(IAgent a)
        {
            currentAgent = a;
            if (a is PlaneAgent) run((PlaneAgent)a);
            if (a is LinkMesh) run((LinkMesh)a);
        }

        public virtual void test(IAgent a, Plane3D p)
        {
            currentAgent = a;
            currentNeighbour = p;
            if (a is PlaneAgent) test((PlaneAgent)a, p);
            if (a is LinkMesh) test((LinkMesh)a, p);
        }

        /// <summary>
        /// Function for behaviour to run. Override this function in new behaviours
        /// </summary>
        /// <param name="a">Current agent</param>
        public virtual void run(PlaneAgent a) { }

        public virtual void test(PlaneAgent a, Plane3D p) { }

        public virtual void run(LinkMesh a) { }

        public virtual void test(LinkMesh a, Plane3D p) { }

    }

    public class BehaviourInputsComponent : GH_Component
    {
        public BehaviourInputsComponent() : base("Behaviour Inputs", "BehaviourInputs", "Provides access to the inputs of an agent behaviour (agents, neighbours etc)", "Nursery", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.quinary;
        public override Guid ComponentGuid => new Guid("{03a7d3c3-5494-4654-bb3c-e685b9c3a7a9}");
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

        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new AgentParameter(), "Agent", "A", "Reference to agent running the behaviour", GH_ParamAccess.item);
            pManager.AddParameter(new NodeParameter(), "Neighbour", "N", "Location of neighbour to interact with", GH_ParamAccess.item);
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public GHBehaviour behaviour = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (behaviour == null)
            {
                behaviour = new GHBehaviour();
            }

            DA.SetData(0, behaviour.currentAgent);
            DA.SetData(0, behaviour.currentNeighbour);
        }
    }

    public class BehaviourOutputsComponent : GH_Component
    {
        public BehaviourOutputsComponent() : base("Behaviour Outputs", "BehaviourOutputs", "Provides access to the outputs of an agent behaviour (agents, neighbours etc)", "Nursery", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.quinary;
        public override Guid ComponentGuid => new Guid("{03a7d3c3-5494-4654-bb3c-e685b9c3a7a9}");
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
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
            pManager.AddParameter(new AgentParameter(), "Agent", "A", "Reference to agent running the behaviour", GH_ParamAccess.item);
            pManager.AddParameter(new NodeParameter(), "Neighbour", "N", "Location of neighbour to interact with", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (behaviour == null)
            {
                behaviour = new GHBehaviour();
            }

            DA.SetData(0, behaviour.currentAgent);
            DA.SetData(0, behaviour.currentNeighbour);
        }
    }*/
}
