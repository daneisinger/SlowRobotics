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
    /// <summary>
    /// Basic Field implementation handles adding and removing from a collection of field elements
    /// and evaluating the field at a given sample point to return a FieldData object.
    /// </summary>
    public class Field : IField
    {
        public List<IFieldElement> field { get; set; }
        public Vec3D min { get; set; }
        public AABB bounds { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Field()
        {
            field = new List<IFieldElement>();
        }

        /// <summary>
        /// Convenience constructor creates a collection of PlaneFieldElements and assigns them to the 
        /// field.
        /// </summary>
        /// <param name="fieldPts">Location of field planes</param>
        public Field(List<Plane3D> fieldPts) :this()
        {

            List<PlaneFieldElement> fieldElements = fieldPts.Select(p => new PlaneFieldElement(p)).ToList();
            field.AddRange(fieldElements);
            updateBounds();
        }

        /// <summary>
        /// Creates a field from a collection of field elements
        /// </summary>
        /// <param name="fieldElements">Field elements</param>
        public Field(List<IFieldElement> fieldElements):this()
        {
            field = fieldElements.ToList();
            updateBounds();
        }

        /// <summary>
        /// Adds a FieldElement to the field
        /// </summary>
        /// <param name="e"></param>
        public void insertElement(IFieldElement e)
        {
            field.Add(e);
        }

        /// <summary>
        /// Gets the bounding box of all field elements
        /// </summary>
        public void updateBounds()
        {
            bounds = AABB.getBoundingBox(field.Select(p => p.location).ToList());
        }

        /// <summary>
        /// Evaluates the field at a sample point
        /// </summary>
        /// <param name="loc">Sample point</param>
        /// <returns></returns>
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
