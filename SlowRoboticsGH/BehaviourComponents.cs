using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Drawing;
using SlowRobotics.Agent.Behaviours;
using Grasshopper.Kernel.Parameters;
using SlowRobotics.Rhino.IO;
using SlowRobotics.Agent;
using SlowRobotics.Field;
using SlowRobotics.Core;
using Toxiclibs.core;
using SlowRobotics.Spatial;

namespace SlowRoboticsGH
{

    public class RebuildComponent : GH_Component
    {
        public RebuildComponent() : base("Rebuild Tree", "Rebuild", "Rebuilds spatial structure with new objects", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{71a0c2d9-bcde-4d03-b079-245205106639}");
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
            pManager.AddParameter(new AgentListParameter(), "Agents", "A", "Try and add objects from these agents", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public RebuildTree rebuildtree = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_AgentList pop = null;
            int priority = 5;

            if (!DA.GetData(0, ref pop)) { return; }
            if (!DA.GetData(1, ref priority)) { return; }

            if (rebuildtree != null)
            {

                if(pop.Value!=null)rebuildtree.pop = pop.Value;
                rebuildtree.priority = priority;

            }
            else
            {
               if(pop.Value!= null) rebuildtree = new RebuildTree(priority, pop.Value);
            }

            DA.SetData(0, rebuildtree);

        }
    }

    public class IntegrateComponent : GH_Component
    {
        public IntegrateComponent() : base("Integrate", "Integrate", "Integrates particle accelleration and velocity", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{e2df1927-29ad-4fe0-bd86-b651aff77b92}");
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
            pManager.AddNumberParameter("Damping", "D", "Particle Damping", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public IntegrateBehaviour integrate = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double damping = 0.1;
            int priority = 5;

            if (!DA.GetData(0, ref damping)) { return; }
            if (!DA.GetData(1, ref priority)) { return; }

            if (integrate != null)
            {

                integrate.damping = (float)damping;
                integrate.priority = priority;

            }
            else
            {
                integrate = new IntegrateBehaviour(priority, (float)damping);

            }
            DA.SetData(0, integrate);

        }
    }
    public class SpringComponent : GH_Component
    {
        public SpringComponent() : base("Springs", "Spring", "Adds hookes law springs to links", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{c9a35ba6-b679-446b-860e-d28244cd6360}");
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
            pManager.AddNumberParameter("Damping", "D", "Spring Damping", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Hookes", "H", "Update Both Particles", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public SpringBehaviour spring = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double damping = 0.1;
            bool verlet = true;
            int priority = 5;

            if (!DA.GetData(0, ref damping)) { return; }
            if (!DA.GetData(1, ref verlet)) { return; }
            if (!DA.GetData(2, ref priority)) { return; }

            if (spring!=null)
            {
                
                spring.damping = (float)damping;
                spring.verlet = verlet;
                spring.priority = priority;
                
            }
            else
            {
                spring = new SpringBehaviour(priority, (float)damping, verlet);
                
            }
            DA.SetData(0,spring);

        }
    }

    public class CohereInZAxisComponent : GH_Component
    {
        public CohereInZAxisComponent() : base("Attract in Z", "AttractZ", "Cohere with neighbours by moving in ZAxis (interaction behaviour)", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("{a2d895cf-bf77-4ec1-955d-dbb5151c4884}");
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
            pManager.AddNumberParameter("Strength", "S", "Cohere Strength", GH_ParamAccess.item);
            pManager.AddNumberParameter("Maximum Distance", "Mx", "Maximum distance for effect", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Move.TogetherInZ cZAxis = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double strength = 0.005;
            double maxDist = 7;
            int priority = 5;

            if (!DA.GetData(0, ref strength)) { return; }
            if (!DA.GetData(1, ref maxDist)) { return; }
            if (!DA.GetData(2, ref priority)) { return; }

            if (cZAxis != null)
            {

                cZAxis.strength = (float)strength;
                cZAxis.maxDist = (float) maxDist;
                cZAxis.priority = priority;

            }
            else
            {
                cZAxis = new Move.TogetherInZ(priority, (float)strength, (float)maxDist);
                
            }

            DA.SetData(0, cZAxis);
        }
    }
   
    //TODO - too much overlap between this and the duplicate behaviour
    public class AddLinkComponent : GH_Component
    {
        public AddLinkComponent() : base("Add link", "AddLink", "Duplicate an agent and create a braced link", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        public override Guid ComponentGuid => new Guid("{2962c6c8-c190-4a81-835f-b3b489349199}");
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
            pManager.AddParameter(new BehaviourParameter(), "New Behaviours", "B", "Behaviours for duplicated agent", GH_ParamAccess.list);
            pManager.AddVectorParameter("Offset Vector", "V", "Offset with this vector before duplicating node", GH_ParamAccess.item);
            pManager.AddNumberParameter("Stiffness", "S", "Link Stiffness", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Add Frequency", "F", "Add a link every n steps", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);
            pManager.AddParameter(new AgentListParameter(), "Population",  "P", "Population of Agents", GH_ParamAccess.item);
            pManager.AddGenericParameter("Structure", "S", "Spatial structure to add new points to", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Add.Extend addLink = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<GH_Behaviour> behaviours = new List<GH_Behaviour>();
            Vector3d offset = Vector3d.Unset;
            double stiffness = 0.1;
            int freq = 1;
            int priority = 5;
            GH_AgentList pop = null;
            ISearchable pts = null;
            //Object b = null;

            if (!DA.GetDataList(0, behaviours)) { return; }
            if (!DA.GetData(1, ref offset)) { return; }
            if (!DA.GetData(2, ref stiffness)) { return; }
            if (!DA.GetData(3, ref freq)) { return; }
            if (!DA.GetData(4, ref priority)) { return; }
            if (!DA.GetData(5, ref pop)) { return; }
            if (!DA.GetData(6, ref pts)) { return; }
            if (addLink != null)
            {
                addLink.behaviours = (behaviours.ConvertAll(b => { return b.Value; }));
                addLink.stiffness = (float)stiffness;

                addLink.offset = IO.ToVec3D(offset);
                addLink.frequency = freq;

                addLink.priority = priority;
                addLink.pop = pop.Value;
                addLink.pts = pts;
            }
            else
            {
                addLink = new Add.Extend(priority, freq, IO.ToVec3D(offset), (float)stiffness,  behaviours.ConvertAll(b => { return b.Value; }), pop.Value,pts);
                
            }
            DA.SetData(0, addLink);
        }
    }

    public class FrictionComponent : GH_Component
    {
        public FrictionComponent() : base("Friction", "Friction", "Add inertia from nearby particles (interaction behaviour)", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("{e75e117d-6662-49d2-a06e-e38a09f9a4a6}");
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
            pManager.AddNumberParameter("Maximum Distance", "Mx", "Maximum distance for friction effect", GH_ParamAccess.item);
            pManager.AddNumberParameter("Friction Coefficient", "F", "Maximum inertia modifier", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Arrest.Friction friction = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double maxDist = 10;
            double frictionCof = -0.1;
            int priority = 5;

            if (!DA.GetData(0, ref maxDist)) { return; }
            if (!DA.GetData(1, ref frictionCof)) { return; }
            if (!DA.GetData(2, ref priority)) { return; }

            if (friction != null)
            {

                friction.maxDist = (float)maxDist;
                friction.frictionCof = (float)frictionCof;
                friction.priority = priority;

            }
            else
            {
                friction = new Arrest.Friction(priority, (float)maxDist, (float)frictionCof);
                
            }
            DA.SetData(0, friction);
        }
    }

    public class FreezeComponent : GH_Component
    {
        public FreezeComponent() : base("Freeze", "Freeze", "Freeze agents with low inertia and velocity", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{1cf1d232-9f49-40d3-97e5-02dd4dc769cf}");
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
            pManager.AddNumberParameter("Minimum Inertia", "I", "Minimum Inertia value before locking", GH_ParamAccess.item);
            pManager.AddNumberParameter("Speed Multiplier", "S", "Effect of speed on inertia", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Minimum Age", "A", "Minimum age value before locking", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Arrest.Freeze iLock = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double minInertia = 0;
            double speedFactor = 4;
            int minAge = 0;
            int priority = 5;

            if (!DA.GetData(0, ref minInertia)) { return; }
            if (!DA.GetData(1, ref speedFactor)) { return; }
            if (!DA.GetData(2, ref minAge)) { return; }
            if (!DA.GetData(3, ref priority)) { return; }

            if (iLock != null)
            {

                iLock.minInertia = (float)minInertia;
                iLock.speedFactor = (float)speedFactor;
                iLock.minAge = minAge;
                iLock.priority = priority;

            }
            else
            {
                iLock = new Arrest.Freeze(priority, (float)minInertia, (float)speedFactor, minAge);
                
            }
            DA.SetData(0, iLock);
        }
    }

    public class ArrestGroundBehaviour : GH_Component
    {
        public ArrestGroundBehaviour() : base("Freeze ground", "Ground", "Freeze agents below Z value", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{94d9995e-bb05-4172-9e9d-a4d40d3cbfbb}");
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
            pManager.AddNumberParameter("Minimum Z", "Z", "Minimum Z value before locking", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Arrest.Z zLock = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double minZ = 0;
            int priority = 5;

            if (!DA.GetData(0, ref minZ)) { return; }
            if (!DA.GetData(1, ref priority)) { return; }

            if (zLock != null)
            {

                zLock.minZ = (float) minZ;
                zLock.priority = priority;

            }
            else
            {
                zLock = new Arrest.Z(priority, (float)minZ);
                
            }
            DA.SetData(0, zLock);
        }
    }

    public class InteractComponent : GH_Component
    {

        public InteractComponent() : base("Interact", "Interact", "Interact with neighbouring nodes", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{9327e8c7-a048-49af-aaaa-3c29bd5201c5}");
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
            pManager.AddParameter(new BehaviourParameter(), "Behaviours", "B", "Behaviours for interacting with other nodes", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Interact interact = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {

            List<GH_Behaviour> behaviours = new List<GH_Behaviour>();
            int priority = 5;

            if (!DA.GetDataList(0, behaviours)) { return; }
            if (!DA.GetData(1, ref priority)) { return; }
            
            if (interact != null)
            {
                interact.setBehaviours(behaviours.ConvertAll(b=> { return b.Value; }));
                interact.priority = priority;
            }
            else
            {
                interact = new Interact(priority, behaviours.ConvertAll(b => { return b.Value; }));
            }
            DA.SetData(0, interact);
        }
    }

    public class FilterClosestNeighbourComponent : GH_Component
    {

        public FilterClosestNeighbourComponent() : base("Filter Closest", "FilterClosest", "Filter out all but closest neighbour", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{fb1100ba-1d65-4bd6-99f2-f3f0b14422b3}");
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
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public FilterClosest filter = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {

            int priority = 5;

            if (!DA.GetData(1, ref priority)) { return; }

            if (filter != null)
            {
                filter.priority = priority;
            }
            else
            {
                filter = new FilterClosest(priority);
            }
            DA.SetData(0, filter);
        }
    }

    public class FilterParentComponent : GH_Component
    {

        public FilterParentComponent() : base("Filter Parents", "FilterParent", "Filter out neighbours with same parent", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{42fa9ca5-e6b5-4633-b343-73325baf86ae}");
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
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public FilterParents filter = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {

            int priority = 5;

            if (!DA.GetData(1, ref priority)) { return; }

            if (filter != null)
            {
                filter.priority = priority;
            }
            else
            {
                filter = new FilterParents(priority);
            }
            DA.SetData(0, filter);
        }
    }

    public class NewtonComponent : GH_Component
    {
        public NewtonComponent() : base("Newton", "Newton", "Move particle with a force", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{8974b28e-b8d7-4783-9b9f-3d4adc85e36a}");
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
            pManager.AddVectorParameter("Force", "F", "Newton Force", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Newton newton = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Vector3d force = Vector3d.Unset;
            int priority = 5;

            if (!DA.GetData(0, ref force)) { return; }
            if (!DA.GetData(1, ref priority)) { return; }

            if (newton != null)
            {
                newton.force = IO.ToVec3D(force);
                newton.priority = priority;
            }
            else
            {
                newton = new Newton(priority, IO.ToVec3D(force));

            }
            DA.SetData(0, newton);
        }
    }

    public class TraverseFieldComponent : GH_Component
    {
        public TraverseFieldComponent() : base("Traverse field", "TraverseField", "Move an agent through a field", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        public override Guid ComponentGuid => new Guid("{f21c5bee-d4b2-47f6-875c-1ce463657a02}");
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
            pManager.AddGenericParameter("Field", "F", "Field to evaluate", GH_ParamAccess.item);
            pManager.AddNumberParameter("Strength", "S", "Strength of effect", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Move.InField fieldf = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IField field = null;
            double strength = 0;
            int priority = 5;

            if (!DA.GetData(0, ref field)) { return; }
            if (!DA.GetData(1, ref strength)) { return; }
            if (!DA.GetData(2, ref priority)) { return; }

            if (fieldf != null)
            {
                fieldf.field = field;
                fieldf.strength = (float)strength;
                fieldf.priority = priority;
            }
            else
            {
                fieldf = new Move.InField(priority, field, (float)strength);

            }
            DA.SetData(0, fieldf);
        }
    }

    public class AlignFieldComponent : GH_Component
    {
        public AlignFieldComponent() : base("Align to field", "AlignField", "Align an agent with a field", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        public override Guid ComponentGuid => new Guid("{66de0eb8-09c2-4fc7-bd61-724e7d70442e}");
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
            pManager.AddGenericParameter("Field", "F", "Field to evaluate", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Axis", "A", "Axis of field plane", GH_ParamAccess.item);
            pManager.AddNumberParameter("Strength", "S", "Strength of effect", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Align.AxisToField fieldf = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            IField field = null;
            int axis = 0;
            double strength = 0;
            int priority = 5;

            if (!DA.GetData(0, ref field)) { return; }
            if (!DA.GetData(1, ref axis)) { return; }
            if (!DA.GetData(2, ref strength)) { return; }
            if (!DA.GetData(3, ref priority)) { return; }
            
            if (fieldf != null)
            {
                fieldf.field = field;
                fieldf.axis = axis;
                fieldf.strength = (float)strength;
                fieldf.priority = priority;
            }
            else
            {
                fieldf = new Align.AxisToField(priority, field, (float)strength, axis);
                
            }
            DA.SetData(0, fieldf);
        }
    }

    public class MoveInAxisComponent : GH_Component
    {
        public MoveInAxisComponent() : base("Move in axis", "MoveAxis", "Move an agent with one of its axes", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{e8e65855-a4ec-46b2-b9e3-ee4fb426353f}");
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
            pManager.AddIntegerParameter("Axis", "A", "Axis of field plane", GH_ParamAccess.item);
            pManager.AddNumberParameter("Strength", "S", "Strength of effect", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Move.InAxis move = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int axis = 0;
            double strength = 0;
            int priority = 5;

            if (!DA.GetData(0, ref axis)) { return; }
            if (!DA.GetData(1, ref strength)) { return; }
            if (!DA.GetData(2, ref priority)) { return; }

            if (move != null)
            {
                move.axis = axis;
                move.strength = (float)strength;
                move.priority = priority;
            }
            else
            {
                move = new Move.InAxis(priority,  (float)strength, axis);

            }
            DA.SetData(0, move);
        }
    }

   
    public class AlignAxisToVelocityComponent : GH_Component
    {

        public AlignAxisToVelocityComponent() : base("Align to velocity", "AlignVelocity", "Align Plane with velocity", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{f8540ae4-0554-427f-9c2f-4017d060285e}");
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
            pManager.AddNumberParameter("Strength", "S", "Strength of effect", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Axis", "A", "Axis to align", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);

            Param_Integer param = pManager[1] as Param_Integer;
            param.AddNamedValue("X Axis", 0);
            param.AddNamedValue("Y Axis", 1);
            param.AddNamedValue("Z Axis", 2);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Align.AxisToVelocity aVel = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double strength = 0.1;
            int axis = 0;
            int priority = 5;

            if (!DA.GetData(0, ref strength)) { return; }
            if (!DA.GetData(1, ref axis)) { return; }
            if (!DA.GetData(2, ref priority)) { return; }

            if (aVel != null)
            {
                aVel.strength = (float)strength;
                aVel.axis = axis;
                aVel.priority = priority;

            }
            else
            {
                aVel = new Align.AxisToVelocity(priority, (float)strength, axis);
                
            }
            DA.SetData(0, aVel);
        }
    }

    public class AlignPlanesComponent : GH_Component
    {

        public AlignPlanesComponent() : base("Align to planes", "AlignPlanes", "Align Plane with neighbours (interaction behaviour)", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("{d6ff6e8b-5e96-44f8-a7c8-71069f9d7bf5}");
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
            pManager.AddNumberParameter("Strength", "S", "Strength of effect", GH_ParamAccess.item);
            pManager.AddNumberParameter("Max Distance", "Mx", "Maximum Attraction Distance", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Align.Planes alignPlanes = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double strength = 0.1;
            double maxDist = 10;
            int priority = 5;

            if (!DA.GetData(0, ref strength)) { return; }
            if (!DA.GetData(1, ref maxDist)) { return; }
            if (!DA.GetData(2, ref priority)) { return; }

            if (alignPlanes != null)
            {
                alignPlanes.strength = (float)strength;
                alignPlanes.maxDist = (float)maxDist;
                alignPlanes.priority = priority;
            }
            else
            {
                alignPlanes = new Align.Planes(priority, (float)maxDist, (float)strength);
                
            }
            DA.SetData(0, alignPlanes);
        }
    }

    public class AlignZZToBestFitComponent : GH_Component
    {

        public AlignZZToBestFitComponent() : base("Align to best fit plane", "AlignBestFit", "Align Plane Z Axis to best fit plane of nearest neighbours (interaction behaviour)", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("{3e0578d3-7cd5-4764-9c4d-04aa7bf0013a}");
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
            pManager.AddNumberParameter("Strength", "S", "Strength of effect", GH_ParamAccess.item);
            pManager.AddNumberParameter("Max Distance", "Mx", "Maximum Attraction Distance", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Align.AxisTo3PtTri bestFit = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double strength = 0.04;
            double maxDist = 10;
            int priority = 5;

            if (!DA.GetData(0, ref strength)) { return; }
            if (!DA.GetData(1, ref maxDist)) { return; }
            if (!DA.GetData(2, ref priority)) { return; }

            if (bestFit != null)
            {
                bestFit.strength = (float)strength;
                bestFit.maxDist = (float)maxDist;
                bestFit.priority = priority;

            }
            else
            {
                bestFit = new Align.AxisTo3PtTri(priority, (float)maxDist, (float)strength);
                
            }
            DA.SetData(0,bestFit);
        }
    }

    public class LineToLineComponent : GH_Component
    {

        public LineToLineComponent() : base("Line To Line", "LineLine", "Attract lines to lines (interaction behaviour)", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("{07895539-0e74-40a0-b6f1-794a89e94f26}");
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
            pManager.AddNumberParameter("Strength", "S", "Attraction Strength", GH_ParamAccess.item);
            pManager.AddNumberParameter("Min Distance", "Mn", "Minimum Attraction Distance", GH_ParamAccess.item);
            pManager.AddNumberParameter("Max Distance", "Mx", "Maximum Attraction Distance", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Move.LineToLine attract = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double strength = 0.1;
            double maxDist = 10;
            double minDist = 1;
            int priority = 5;

            if (!DA.GetData(0, ref strength)) { return; }
            if (!DA.GetData(1, ref minDist)) { return; }
            if (!DA.GetData(2, ref maxDist)) { return; }
            if (!DA.GetData(3, ref priority)) { return; }

            if (attract != null)
            {
                attract.strength = (float)strength;
                attract.minDist = (float)minDist;
                attract.maxDist = (float)maxDist;
                attract.priority = priority;
            }
            else
            {
                attract = new Move.LineToLine(priority, (float)minDist, (float)maxDist, (float)strength);
            }
            DA.SetData(0, attract);
        }
    }

    public class PointToLineComponent : GH_Component
    {

        public PointToLineComponent() : base("Point To Line", "PointLine", "Attract particles to lines (interaction behaviour)", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("{099cc560-1b5b-4b94-9443-9326029e270b}");
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
            pManager.AddNumberParameter("Strength", "S", "Attraction Strength", GH_ParamAccess.item);
            pManager.AddNumberParameter("Min Distance", "Mn", "Minimum Attraction Distance", GH_ParamAccess.item);
            pManager.AddNumberParameter("Max Distance", "Mx", "Maximum Attraction Distance", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Move.PointToLine attract = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double strength = 0.1;
            double maxDist = 10;
            double minDist = 1;
            int priority = 5;

            if (!DA.GetData(0, ref strength)) { return; }
            if (!DA.GetData(1, ref minDist)) { return; }
            if (!DA.GetData(2, ref maxDist)) { return; }
            if (!DA.GetData(3, ref priority)) { return; }

            if (attract != null)
            {
                attract.strength = (float)strength;
                attract.minDist = (float)minDist;
                attract.maxDist = (float)maxDist;
                attract.priority = priority;
            }
            else
            {
                attract = new Move.PointToLine(priority, (float)minDist, (float)maxDist, (float)strength);
            }
            DA.SetData(0, attract);
        }
    }

    public class AttractComponent: GH_Component
    {

        public AttractComponent() : base("Attract", "Attract", "Attract Agents (interaction behaviour)", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("{d3e8a92e-16f3-4de0-98aa-73ce5d948723}");
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
            pManager.AddNumberParameter("Strength", "S", "Attraction Strength", GH_ParamAccess.item);
            pManager.AddNumberParameter("Min Distance", "Mn", "Minimum Attraction Distance", GH_ParamAccess.item);
            pManager.AddNumberParameter("Max Distance", "Mx", "Maximum Attraction Distance", GH_ParamAccess.item);
            pManager.AddBooleanParameter("In XY", "XY", "Attract only in Plane XY", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Move.Together attract = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool inXY = false;
            double strength = 0.1;
            double maxDist = 10;
            double minDist = 1;
            int priority = 5;

            if (!DA.GetData(0, ref strength)) { return; }
            if (!DA.GetData(1, ref minDist)) { return; }
            if (!DA.GetData(2, ref maxDist)) { return; }
            if (!DA.GetData(3, ref inXY)) { return; }
            if (!DA.GetData(4, ref priority)) { return; }

            if (attract != null)
            {
                attract.strength = (float)strength;
                attract.minDist = (float)minDist;
                attract.maxDist = (float)maxDist;
                attract.inXY = inXY;
                attract.priority = priority;
            }
            else
            {
                if (!inXY)
                {
                    attract = new Move.Together(priority, (float)minDist, (float)maxDist, (float)strength, inXY);
                }
                else
                {
                    attract = new Move.Together(priority, (float)minDist, (float)maxDist, (float)strength, inXY);
                }
            }
            DA.SetData(0, attract);
        }
    }

    public class RepelComponent : GH_Component
    {
       

        public RepelComponent() : base("Repel", "Repel", "Repel Agents (interaction behaviour)", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public override Guid ComponentGuid => new Guid("{38563a34-1be4-4c68-bd94-cb4b2e15a4f2}");
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
            pManager.AddNumberParameter("Strength", "S", "Seperation Strength", GH_ParamAccess.item);
            pManager.AddNumberParameter("Min Distance", "Mn", "Minimum Separation Distance", GH_ParamAccess.item);
            pManager.AddNumberParameter("Max Distance", "Mx", "Maximum Separation Distance", GH_ParamAccess.item);
            pManager.AddBooleanParameter("In XY", "XY", "Separate only in Plane XY", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Move.Apart separate = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool inXY = false;
            double strength = 0.1;
            double maxDist = 10;
            double minDist = 1;
            int priority = 5;

            if (!DA.GetData(0, ref strength)) { return; }
            if (!DA.GetData(1, ref minDist)) { return; }
            if (!DA.GetData(2, ref maxDist)) { return; }
            if (!DA.GetData(3, ref inXY)) { return; }
            if (!DA.GetData(4, ref priority)) { return; }

            if (separate != null) {

                separate.inXY = inXY;
                separate.strength = (float)strength;
                separate.minDist = (float)minDist;
                separate.maxDist = (float)maxDist;
                separate.priority = priority;

            }
            else
            {
                separate = new Move.Apart(priority, (float)strength, (float)minDist, (float)maxDist, inXY);
            }
            DA.SetData(0, separate);
        }
    }

    public class SearchComponent : GH_Component
    {
        public SearchComponent() : base("Search", "Search", "Search Nodes", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.primary;
        public override Guid ComponentGuid => new Guid("{0979e24f-914b-46cc-986a-6638ffedba71}");
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

            pManager.AddNumberParameter("Radius", "R", "Search Radius", GH_ParamAccess.item);
            pManager.AddGenericParameter("Structure", "S", "Spatial structure to search", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Search searchBehaviour = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double radius = 10;
            ISearchable pts = null;
            int priority = 5;

            if (!DA.GetData(0, ref radius)) { return; }
            if (!DA.GetData(1, ref pts)) { return; }
            if (!DA.GetData(2, ref priority)) { return; }


            if (searchBehaviour != null)
            {
                searchBehaviour.radius = (float)radius;
                searchBehaviour.pts = pts;
                searchBehaviour.priority = priority;
            }
            else
            {
                searchBehaviour = new Search(priority, (float)radius, pts);
            } 
            DA.SetData(0, searchBehaviour);
        }
    }

    public class ScaleBehaviourByDistanceToBoxComponent : GH_Component
    {
        public ScaleBehaviourByDistanceToBoxComponent() : base("Scale box", "ScaleBox", "Scales a list of behaviours by distance to a box", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.quinary;
        public override Guid ComponentGuid => new Guid("{6e557a31-193a-4f9a-8d94-9de59f74c03d}");
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

            pManager.AddBoxParameter("Box", "Bx", "Box to scale behaviours", GH_ParamAccess.item);
            pManager.AddParameter(new BehaviourParameter(), "Behaviours", "Be", "Behaviours to scale", GH_ParamAccess.list);
            pManager.AddNumberParameter("Maximum Distance", "Mx", "Maximum distance from box for scaling effect", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour priority", GH_ParamAccess.item);

        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Scale.ByDistToBoundingBox scaleBox = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<GH_Behaviour> ghBehaviours = new List<GH_Behaviour>();
            Box b = Box.Unset;
            double maxDist = 10;
            int priority = 0;

            if (!DA.GetData(0, ref b)) { return; }
            if (!DA.GetDataList(1, ghBehaviours)) { return; }
            if (!DA.GetData(2, ref maxDist)) { return; }
            if (!DA.GetData(3, ref priority)) { return; }

            List<IScaledBehaviour> behaviours = new List<IScaledBehaviour>();
            foreach (GH_Behaviour _b in ghBehaviours) if (_b.Value is IScaledBehaviour)behaviours.Add((IScaledBehaviour)_b.Value);

            AABB bounds = IO.ToAABB(b);

            if (scaleBox != null)
            {
                scaleBox.box = bounds;
                scaleBox.behaviours = behaviours;
                scaleBox.maxDist = (float)maxDist;
                scaleBox.priority = priority;
            }
            else
            {
                scaleBox = new Scale.ByDistToBoundingBox(priority,behaviours,bounds,(float)maxDist);
            }
            DA.SetData(0, scaleBox);
        }
    }

    public class LeaveTraceComponent : GH_Component
    {
        public LeaveTraceComponent() : base("Leave trace", "Leave Trace", "Creates a static copy of the agent plane", "SlowRobotics", "Behaviours") { }
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
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

            pManager.AddIntegerParameter("Frequency", "F", "Save frequency", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Priority", "P", "Behaviour Priority", GH_ParamAccess.item);
            pManager.AddGenericParameter("Structure", "S", "Structure to store point", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new BehaviourParameter(), "Behaviour", "B", "Behaviour", GH_ParamAccess.item);
        }

        public Add.PointToWorld addBehaviour = null;

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            int frequency = 2;
            int priority = 0;
            ISearchable pts = null;

            if (!DA.GetData(0, ref frequency)) { return; }
            if (!DA.GetData(1, ref priority)) { return; }
            if (!DA.GetData(2, ref pts)) { return; }

            if (addBehaviour != null)
            {
                addBehaviour.frequency = frequency;
                addBehaviour.priority = priority;
                addBehaviour.pts = pts;
            }
            else
            {
                addBehaviour = new Add.PointToWorld(priority,pts,frequency);
            }
            DA.SetData(0, addBehaviour);
        }
    }
    

}
