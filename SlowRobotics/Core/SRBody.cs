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
                ApplyLinearImpulse(p.getLimitedAccel());
                ApplyAngularImpulse(p,p.getLimitedAccel());
            }
        }

        //adds to accel and torque
        public void ApplyLinearImpulse(Vec3D force)
        {
            accel.addSelf(force);
        }

        public void ApplyAngularImpulse(Vec3D pos, Vec3D force)
        {
            Vec3D ab = pos.sub(this);
            float d = ab.magnitude();

            Vec3D crossAb = ab.cross(force);
            float a = ab.angleBetween(ab.add(force), true);

            if (!float.IsNaN(a)) Torque.addSelf(crossAb.scale(a));
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
                Vec3D localP = p.sub(this);
                t.applyToSelf(localP);
                p.set(localP.addSelf(this));
                p.resetAccel();
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
            body.set(SR_Math.averageVectors(graph.Geometry.ConvertAll(x => (Vec3D)x)));

            return body;
        }
    }
}
