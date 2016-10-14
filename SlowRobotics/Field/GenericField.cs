using SlowRobotics.Core;
using SlowRobotics.Field.Elements;
using SlowRobotics.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Field
{
    public class GenericField : IField
    {
        public List<IFieldElement> field { get; set; }
        public Vec3D min { get; set; }
        public AABB bounds { get; set; }

        public GenericField()
        {
            field = new List<IFieldElement>();
        }

        public GenericField(List<Plane3D> fieldPts) :this()
        {

            List<PlaneFieldElement> fieldElements = fieldPts.Select(p => new PlaneFieldElement(p)).ToList();
            field.AddRange(fieldElements);
            updateBounds();
        }

        public GenericField(List<IFieldElement> fieldElements):this()
        {
            field = fieldElements.ToList();
            updateBounds();
        }

        public void insertElement(IFieldElement e)
        {
            field.Add(e);
        }

        public void updateBounds()
        {
            bounds = AABB.getBoundingBox(field.Select(p => p.location).ToList());
        }

        public FieldData evaluate(Vec3D loc)
        {
            FieldData d = new FieldData();
            for (int i = 0; i < field.Count; i++)
            {
                field[i].integrate(ref d, loc);
            }
            return d;
        }
    }
}
