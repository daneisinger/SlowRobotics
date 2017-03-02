using SlowRobotics.SRGraph;
using SlowRobotics.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Core
{
    public class SRBody : SRParticle
    {
        public List<SRParticle> pts;
        public Vec3D LinearVelocity;
        public Vec3D AngularVelocity;
        public Vec3D Torque;

        public SRBody(Plane3D _centreOfMass) : base(_centreOfMass)
    {
            pts = new List<SRParticle>();

            AngularVelocity = new Vec3D();
            LinearVelocity = new Vec3D();
            Torque = new Vec3D();
        }

        public Plane3D calculateCoM()
        {
            Vec3D origin = SRMath.averageVectors(pts.Select(pt => (Vec3D)pt).ToList());
            Vec3D xx = SRMath.averageVectors(pts.Select(pt => pt.xx).ToList());
            Vec3D yy = SRMath.averageVectors(pts.Select(pt => pt.yy).ToList());
            return new Plane3D(origin, xx, yy);
        }

        public void setCoM(Plane3D p)
        {
            set(p);
        }

        public void insertPoint(SRParticle _pt)
        {
            pts.Add(_pt);
        }

        public void insertPoints(IEnumerable<SRParticle> _pts)
        {
            pts.AddRange(_pts);
        }

        private void sumForces()
        {
            foreach (SRParticle p in pts)
            {
                ApplyImpulses(p.getImpulse());
            }
        }

        //adds to accel and torque
        public void ApplyImpulses(IEnumerable<Impulse> impulses)
        {
            foreach (Impulse i in impulses)
            {

                if (!i.torqueOnly)
                    accel.addSelf(i.dir); //integrate force

                Vec3D ab = i.pos.sub(this);
                float d = ab.magnitude();

                Vec3D crossAb = ab.cross(i.dir);
                float a = ab.angleBetween(ab.add(i.dir), true);

                if (!float.IsNaN(a)) Torque.addSelf(crossAb.scale(a)); //integrate torque
            }
        }

        public override void transform(Matrix4x4 t)
        {
            //transform plane
            xx = t.applyTo(xx).normalize();
            zz = t.applyTo(zz).normalize();
            yy = t.applyTo(yy).normalize();

            //transform body
            foreach (SRParticle p in pts)
            {
                p.transform(t);
                Vec3D localP = p.sub(this);
                t.applyToSelf(localP);
                p.set(localP.addSelf(this));
                p.reset();
            }
        }

        public override void step(float dt)
        {
            sumForces(); //sets torque and accel based on forces on particles

            //add linear velocity as impulse
            Vec3D impulse = accel.scale(dt);
            addSelf(impulse);
            foreach (SRParticle p in pts) p.addSelf(impulse);

            Matrix4x4 t = new Matrix4x4();

            //add angular velocity as impulse
            Quaternion rot = Quaternion.createFromAxisAngle(Torque, Torque.magnitude() * 1f * dt);
            rot.normalize();
            t = rot.toMatrix4x4();

            transform(t); //transform this plane

            Torque = new Vec3D();
            accel = new Vec3D();

            age++;
        }

        public static SRBody CreateFromGraph(Graph<SRParticle, Spring> graph)
        {
            SRBody body = new SRBody(new Plane3D()); //create temporary centre of mass
            int num = graph.Geometry.Count;
            if (num == 0) return body;

            foreach (SRParticle p in graph.Geometry)
            {
                p.f = true; //don't move particles
                body.insertPoint(p); //add to body
            }
            body.set(SRMath.averageVectors(graph.Geometry.ConvertAll(x => (Vec3D)x)));

            return body;
        }
    }
}
