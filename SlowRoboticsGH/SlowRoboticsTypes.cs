using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using SlowRobotics.Agent;


using SlowRobotics.Core;
using SlowRobotics.Field;
using SlowRobotics.Rhino.IO;
using SlowRobotics.SRGraph;
using SlowRobotics.SRMath;
using SlowRobotics.Utils;
using SlowRobotics.Voxels;
using System.Collections.Generic;
using Rhino.Geometry;
using System;
using Toxiclibs.core;

namespace SlowRoboticsGH
{
    public class GH_Behaviour : GH_Goo<IBehaviour>
    {
        public GH_Behaviour() { this.Value = null; }
        public GH_Behaviour(GH_Behaviour goo) { this.Value = goo.Value; }
        public GH_Behaviour(IBehaviour native) { this.Value = native; }
        public override IGH_Goo Duplicate() => new GH_Behaviour(this);
        public override bool IsValid => true;
        public override string TypeName => "Behaviour";
        public override string TypeDescription => "Behaviour";
        public override string ToString() => this.Value.ToString();
        public override object ScriptVariable() => Value;
        public override bool CastFrom(object source)
        {
            if (source is IBehaviour)
            {
                Value = source as IBehaviour;
                return true;
            }
            if (source is GH_Behaviour)
            {
                Value = ((GH_Behaviour)source).Value;
                return true;
            }
            return false;
        }
    }

    public class GH_FieldElement : GH_Goo<IFieldElement>
    {
        public GH_FieldElement() { this.Value = null; }
        public GH_FieldElement(GH_FieldElement goo) { this.Value = goo.Value; }
        public GH_FieldElement(IFieldElement native) { this.Value = native; }
        public override IGH_Goo Duplicate() => new GH_FieldElement(this);
        public override bool IsValid => true;
        public override string TypeName => "FieldElement";
        public override string TypeDescription => "FieldElement";
        public override string ToString() => this.Value.ToString();
        public override object ScriptVariable() => Value;

        public override bool CastFrom(object source)
        {
            if (source is IFieldElement)
            {
                Value = source as IFieldElement;
                return true;
            }
            if (source is GH_FieldElement)
            {
                Value = ((GH_FieldElement)source).Value;
                return true;
            }
            return false;
        }
    }

    public class GH_Field : GH_Goo<IField>
    {
        public GH_Field() { this.Value = null; }
        public GH_Field(GH_Field goo) { this.Value = goo.Value; }
        public GH_Field(IField native) { this.Value = native; }

        public override IGH_Goo Duplicate() => new GH_Field(this);
        public override bool IsValid => true;
        public override string TypeName => "Field";
        public override string TypeDescription => "Field";
        public override string ToString() => this.Value.ToString();
        public override object ScriptVariable() => Value;

        public override bool CastFrom(object source)
        {
            if (source is IField)
            {
                Value = source as IField;
                return true;
            }
            if (source is GH_Field)
            {
                Value = ((GH_Field)source).Value;
                return true;
            }
            return false;
        }
    }

    public class GH_Spring : GH_Goo<Spring>, IGH_PreviewData
    {
        public GH_Spring() { this.Value = null; }
        public GH_Spring(GH_Spring goo) { this.Value = goo.Value; }
        public GH_Spring(Spring native) { this.Value = native; }

        public override IGH_Goo Duplicate() => new GH_Spring(this);
        public override bool IsValid => true;
        public override string TypeName => "Spring";
        public override string TypeDescription => "Spring";
        public override string ToString() => this.Value.ToString();
        public override object ScriptVariable() => Value;

        public BoundingBox ClippingBox
        {
            get
            {
                return new BoundingBox();
            }
        }

        public override bool CastFrom(object source)
        {
            if (source is Spring)
            {
                Value = source as Spring;
                return true;
            }
            if (source is GH_Spring)
            {
                Value = ((GH_Spring)source).Value;
                return true;
            }
            return false;
        }

        public override bool CastTo<Q>(ref Q target)
        {
            if (typeof(Q) == typeof(Line))
            {
                target = (Q)(object)Value.toLine();
                return true;
            }
            if (typeof(Q) == typeof(GH_Line))
            {
                target = (Q)(object)new GH_Line(Value.toLine());
                return true;
            }

            return base.CastTo<Q>(ref target);
        }

        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
            ILine l = m_value;
            args.Pipeline.DrawLine(l.toLine(), System.Drawing.Color.Red, 1);
        }

        public void DrawViewportMeshes(GH_PreviewMeshArgs args)
        {
            // DrawViewportMeshes(args);
        }
    }

    public class GH_String : GH_Goo<SRString>
    {
        public GH_String() { this.Value = null; }
        public GH_String(GH_String goo) { this.Value = goo.Value; }
        public GH_String(SRString native) { this.Value = native; }
        //  public GH_Plane3D(Plane native) { this.Value = new Plane3D(new Vec3D()); }
        public override IGH_Goo Duplicate() => new GH_String(this);
        public override bool IsValid => true;
        public override string TypeName => "NString";
        public override string TypeDescription => "Nursery String";
        public override string ToString() => this.Value.ToString();
        public override object ScriptVariable() => Value;

        public override bool CastFrom(object source)
        {
            if (source is GH_String)
            {
                Value = ((GH_String)source).Value;
                return true;
            }
            if (source is SRString)
            {
                Value = source as SRString;
                return true;
            }
            if (source is string)
            {
                string str;
                if(GH_Convert.ToString(source, out str, GH_Conversion.Both))
                {
                    Value = new SRString(str);
                    return true;
                }
                
                return false;
            }

            // last ditch effor to cast to string
            string tryCast;
            if(GH_Convert.ToString(source, out tryCast, GH_Conversion.Both))
            {
                Value = new SRString(tryCast);
                return true;
            }
            return false;
        }
    }

    public class GH_Falloff : GH_Goo<FalloffStrategy>
    {
        public GH_Falloff() { this.Value = null; }
        public GH_Falloff(GH_Falloff goo) { this.Value = goo.Value; }
        public GH_Falloff(FalloffStrategy native) { this.Value = native; }
        public override IGH_Goo Duplicate() => new GH_Falloff(this);
        public override bool IsValid => true;
        public override string TypeName => "Falloff";
        public override string TypeDescription => "Falloff Strategy";
        public override string ToString() => this.Value.ToString();
        public override object ScriptVariable() => Value;

        //todo - do casts

        public override bool CastFrom(object source)
        {
            if (source is GH_Falloff)
            {
                Value = ((GH_Falloff)source).Value;
                return true;
            }
            if (source is FalloffStrategy)
            {
                Value = source as FalloffStrategy;
                return true;
            }
           
            return false;
        }

    }

    public class GH_Plane3D : GH_Goo<Plane3D>
    {
        public GH_Plane3D() { this.Value = null; }
        public GH_Plane3D(GH_Plane3D goo) { this.Value = goo.Value; }
        public GH_Plane3D(Plane3D native) { this.Value = native; }
      //  public GH_Plane3D(Plane native) { this.Value = new Plane3D(new Vec3D()); }
        public override IGH_Goo Duplicate() => new GH_Plane3D(this);
        public override bool IsValid => true;
        public override string TypeName => "Plane3D";
        public override string TypeDescription => "Plane3D";
        public override string ToString() => this.Value.ToString();
        public override object ScriptVariable() => Value;

        //todo - do casts

        public override bool CastFrom(object source)
        {
            if (source is GH_Plane)
            {
                Value = ((GH_Plane)source).Value.ToPlane3D();
                return true;
            }
            if (source is Plane3D)
            {
                Value = source as Plane3D;
                return true;
            }
            if (source is GH_Plane3D)
            {
                Value = ((GH_Plane3D)source).Value;
                return true;
            }
            if (source is GH_Point)
            {
                Value = new Plane3D(((GH_Point)source).Value.ToVec3D());
                return true;
            }
            return false;
        }

        //test castTo override
        public override bool CastTo<Q>(ref Q target)
        {
            if (typeof(Q) == typeof(Plane3D))
            {
                target = (Q)(object)Value;
                return true;
            }
            if (typeof(Q) == typeof(GH_Plane))
            {
                target = (Q)(object)new GH_Plane(Value.ToPlane());
                return true;
            }
            if (typeof(Q) == typeof(Plane))
            {
                target = (Q)(object)Value.ToPlane();
                return true;
            }
            return base.CastTo<Q>(ref target);
        }
    }

    public class GH_Particle : GH_Goo<IParticle>, IGH_PreviewData
    {
        public GH_Particle() { this.Value = null; }
        public GH_Particle(GH_Particle goo) { this.Value = goo.Value; }
        public GH_Particle(IParticle native) { this.Value = native; }

        public override IGH_Goo Duplicate() => new GH_Particle(this);
        public override bool IsValid => true;
        public override string TypeName => "Node";
        public override string TypeDescription => "Node";

        public BoundingBox ClippingBox
        {
            get
            {
                IParticle particle = m_value;
                Vec3D d = particle.getExtents();
                Vec3D p = particle.get();
                return new BoundingBox(p.x, p.y, p.z, p.x + d.x, p.y+d.y, p.z+d.z);
            }
        }

        public override string ToString() => this.Value.ToString();
        public override object ScriptVariable() => Value;

        public override bool CastFrom(object source)
        {
            if(source is IParticle)
            {
                Value = (IParticle)source;
                return true;
            }
            else if (source is Plane3D)
            {
                Value = new SRParticle((Plane3D)source);
                return true;
            }
            else if (source is GH_Plane3D)
            {
                Value = new SRParticle(((GH_Plane3D)source).Value);
                return true;
            }

            else if (source is GH_Plane)
            {
                SRParticle p = new SRParticle(((GH_Plane)source).Value.ToPlane3D());
                Value = p;
                return true;
            }
            else if (source is GH_Line)
            {
                Line l = ((GH_Line)source).Value;
                double length = l.Length / 2;
                Plane pln = Plane.Unset;
                NurbsCurve n = l.ToNurbsCurve();
                double lp = 0;
                n.NormalizedLengthParameter(length, out lp);
                n.PerpendicularFrameAt(lp, out pln);
                SRLinearParticle p = new SRLinearParticle(pln.ToPlane3D());
                p.length = (float)length;
                Value = p;
                return true;
            }
            if (source is Plane)
            {
                Value = new SRParticle(((Plane)source).ToPlane3D());
                return true;
            }
            if (source is GH_Point)
            {
                Value = new SRParticle(new Plane3D(((GH_Point)source).Value.ToVec3D()));
                return true;
            }
            return false;
        }

        //test castTo override
        public override bool CastTo<Q>(ref Q target)
        {
            if (typeof(Q) == typeof(Plane))
            {
                target = (Q)(object)Value.get().ToPlane();
                return true;
            }
            if (typeof(Q) == typeof(GH_Plane))
            {
                target = (Q)(object)new GH_Plane(Value.get().ToPlane());
                return true;
            }
            return base.CastTo<Q>(ref target);
        }

        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
            IParticle p = m_value;
            Vec3D d = p.getExtents();
            Plane3D pln = p.get();
            Point3d pt = pln.ToPoint3d();
            Point3d px = pln.xx.ToPoint3d();
            Point3d py = pln.yy.ToPoint3d();
            args.Pipeline.DrawLine(pt, pt + px, System.Drawing.Color.Red, 1);
            args.Pipeline.DrawLine(pt, pt + py, System.Drawing.Color.Blue, 1);

        }

        public void DrawViewportMeshes(GH_PreviewMeshArgs args)
        {
           // DrawViewportMeshes(args);
        }
    }

    public class GH_Body : GH_Goo<SRBody>, IGH_PreviewData
    {
        public GH_Body() { this.Value = null; }
        public GH_Body(GH_Body goo) { this.Value = goo.Value; }
        public GH_Body(SRBody native) { this.Value = native; }

        public override IGH_Goo Duplicate() => new GH_Body(this);
        public override bool IsValid => true;
        public override string TypeName => "Node";
        public override string TypeDescription => "Node";

        public BoundingBox ClippingBox
        {
            get
            {
                SRBody particle = m_value;
                Vec3D d = particle.getExtents();
                Vec3D p = particle.get();
                return new BoundingBox(p.x, p.y, p.z, p.x + d.x, p.y + d.y, p.z + d.z);
            }
        }

        public override string ToString() => this.Value.ToString();
        public override object ScriptVariable() => Value;

        public override bool CastFrom(object source)
        {
            if (source is SRBody)
            {
                Value = (SRBody)source;
                return true;
            }
            if (source is GH_Graph)
            {
                Value = SRBody.CreateFromGraph(((GH_Graph)source).Value,true);
                return true;
            }
            if (source is Graph<SRParticle,Spring>)
            {
                Value = SRBody.CreateFromGraph((Graph<SRParticle, Spring>)source, true);
                return true;
            }

            else if (source is Plane3D)
            {
                Value = new SRBody((Plane3D)source);
                return true;
            }
            else if (source is GH_Plane3D)
            {
                Value = new SRBody(((GH_Plane3D)source).Value);
                return true;
            }

            else if (source is GH_Plane)
            {
                SRBody p = new SRBody(((GH_Plane)source).Value.ToPlane3D());
                Value = p;
                return true;
            }

            if (source is Plane)
            {
                Value = new SRBody(((Plane)source).ToPlane3D());
                return true;
            }
            if (source is GH_Point)
            {
                Value = new SRBody(new Plane3D(((GH_Point)source).Value.ToVec3D()));
                return true;
            }
            return false;
        }

        //test castTo override
        public override bool CastTo<Q>(ref Q target)
        {
            if (typeof(Q) == typeof(Plane))
            {
                target = (Q)(object)Value.get().ToPlane();
                return true;
            }
            if (typeof(Q) == typeof(GH_Plane))
            {
                target = (Q)(object)new GH_Plane(Value.get().ToPlane());
                return true;
            }
            return base.CastTo<Q>(ref target);
        }

        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
            SRBody body = m_value;
            foreach (SRParticle p in body.pts)
            {
                Vec3D d = p.xx.add(p.yy).add(p.zz);
                Point3d pt = p.ToPoint3d();
                Point3d px = p.xx.ToPoint3d();
                Point3d py = p.yy.ToPoint3d();
                args.Pipeline.DrawLine(pt, pt + px, System.Drawing.Color.Red, 1);
                args.Pipeline.DrawLine(pt, pt + py, System.Drawing.Color.Blue, 1);
            }
        }

        public void DrawViewportMeshes(GH_PreviewMeshArgs args)
        {
            // DrawViewportMeshes(args);
        }
    }

    public class GH_Graph : GH_Goo<Graph<SRParticle, Spring>>, IGH_PreviewData { 

        public GH_Graph() { this.Value = null; }
        public GH_Graph(GH_Graph goo) { this.Value = goo.Value; }
        public GH_Graph(Graph<SRParticle, Spring> native) { this.Value = native; }

        public override IGH_Goo Duplicate() => new GH_Graph(this);
        public override bool IsValid => true;
        public override string TypeName => "Spring Graph";
        public override string TypeDescription => "Spring Graph";

        public BoundingBox ClippingBox
        {
            get
            {
                return new BoundingBox();
            }
        }

        public override string ToString() => this.Value.ToString();
        public override object ScriptVariable() => Value;

        public override bool CastFrom(object source)
        {
            if (source is Graph<SRParticle, Spring>)
            {
                Value = source as Graph<SRParticle, Spring>;
                return true;
            }
            if (source is GH_Graph)
            {
                Value = ((GH_Graph)source).Value;
                return true;
            }
            if (source is GH_Particle)
            {
                Graph<SRParticle, Spring> graph = new Graph<SRParticle, Spring>();
                SRParticle p = (SRParticle)((GH_Particle)source).Value;
                if (p !=null)
                {
                    graph.insert(p);
                    graph.parent = p;
                }
                Value = graph;
                return true;
            }
            if (source is Mesh)
            {
                Value = SRConvert.MeshToGraph((Mesh)source,0.08f);
                return true;
            }
            if (source is GH_GeometricGoo<Curve>)
            {
                Graph<SRParticle, Spring> graph = new Graph<SRParticle, Spring>();
                SRConvert.CurveToGraph(((GH_GeometricGoo<Curve>)source).Value,0.08f,ref graph);
                Value = graph;
                return true;
            }

            return false;
        }

        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
            Graph<SRParticle, Spring> graph = m_value;

            foreach (Spring li in graph.Edges)
            {
                if (li.tag == "")
                {
                    Vec3D aa = li.a.Geometry;
                    Vec3D bb = li.b.Geometry;
                    Line l = new Line(new Point3d(aa.x, aa.y, aa.z), new Point3d(bb.x, bb.y, bb.z));
                    args.Pipeline.DrawLine(l, System.Drawing.Color.Green, 1);
                }
            }

            foreach (SRParticle p in graph.Geometry)
            {
                Vec3D d = p.xx.add(p.yy).add(p.zz);
                Point3d pt = p.ToPoint3d();
                Point3d px = p.xx.ToPoint3d();
                Point3d py = p.yy.ToPoint3d();
                args.Pipeline.DrawLine(pt, pt + px, System.Drawing.Color.Red, 1);
                args.Pipeline.DrawLine(pt, pt + py, System.Drawing.Color.Blue, 1);
            }

        }


        public void DrawViewportMeshes(GH_PreviewMeshArgs args)
        {
            
            //throw new NotImplementedException();
        }
    }

    public class GH_Agent : GH_Goo<IAgent>
    {
        public GH_Agent() { this.Value = null; }
        public GH_Agent(GH_Agent goo) { this.Value = goo.Value; }
        public GH_Agent(IAgent native) { this.Value = native; }

        public override IGH_Goo Duplicate() => new GH_Agent(this);
        public override bool IsValid => true;
        public override string TypeName => "Agent";
        public override string TypeDescription => "Agent";
        public override string ToString() => this.Value.ToString();
        public override object ScriptVariable() => Value;

        public override bool CastFrom(object source)
        {
            if (source is IAgent)
            {
                Value = source as IAgent;
                return true;
            }
           
            if (source is GH_Agent)
            {
                Value = ((GH_Agent)source).Value;
                return true;
            }

            if (source is IGH_Goo)
            {
                object o = ((IGH_Goo)source).ScriptVariable();
                Value = new Agent<object>(o);
                return true;
            }
            
            if(source is GH_ObjectWrapper)
            {
                GH_ObjectWrapper wrapper = (GH_ObjectWrapper)source;

                Value = new Agent<object>(wrapper.Value);
                return true;
            }
            Value = new Agent<object>(source);
            return true;
        }

    }

    public class GH_AgentList : GH_Goo<AgentList>
    {
        public GH_AgentList() { this.Value = null; }
        public GH_AgentList(GH_AgentList goo) { this.Value = goo.Value; }
        public GH_AgentList(AgentList native) { this.Value = native; }

        public override IGH_Goo Duplicate() => new GH_AgentList(this);
        public override bool IsValid => true;
        public override string TypeName => "AgentList";
        public override string TypeDescription => "AgentList";
        public override string ToString() => this.Value.ToString();
        public override object ScriptVariable() => Value;

        public BoundingBox ClippingBox
        {
            get
            {
                return new BoundingBox();
            }
        }

        public override bool CastFrom(object source)
        {
            if (source is AgentList)
            {
                Value = source as AgentList;
                return true;
            }

            if (source is GH_AgentList)
            {
                Value = ((GH_AgentList)source).Value;
                return true;
            }

            if (source is GH_Agent)
            {
                Value = new AgentList(((GH_Agent)source).Value);

                return true;
            }
            if (source is IEnumerable<GH_Agent>)
            {
                AgentList l = new AgentList();
                foreach(GH_Agent a in (IEnumerable<GH_Agent>)source)
                {
                    l.add(a.Value);
                }
                Value = l;

                return true;
            }
            if (source is IEnumerable<IAgent>)
            {
                AgentList l = new AgentList();
                l.addAll((IEnumerable<IAgent>)source);
                Value = l;

                return true;
            }
            if (source is IAgent)
            {
                Value = new AgentList((IAgent)source);
                return true;
            }

            return false;
        }

    }

    public class GH_VoxelGrid : GH_Goo<IVoxelGrid> 
    {
        public GH_VoxelGrid() { this.Value = null; }
        public GH_VoxelGrid(GH_VoxelGrid goo) { this.Value = goo.Value; }
        public GH_VoxelGrid(IVoxelGrid native) { this.Value = native; }
        public GH_VoxelGrid(int num) { this.Value = new FloatGrid(num, num, num, new float[] { 0, 0, 0 }, new float []{num,num,num }); }

        public override IGH_Goo Duplicate() => new GH_VoxelGrid(this);
        public override bool IsValid => true;
        public override string TypeName => "VoxelGrid";
        public override string TypeDescription => "VoxelGrid";
        public override string ToString() => this.Value.ToString();
        public override object ScriptVariable() => Value;
        public override bool CastFrom(object source)
        {
            if (source is IVoxelGrid)
            {
                Value = source as IVoxelGrid;
                return true;
            }
            if (source is GH_VoxelGrid)
            {
                Value = ((GH_VoxelGrid)source).Value;
                return true;
            }
            return false;
        }
    }

}
