using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Field
{
    public class FieldData
    {

        public Vec3D vectorData { get; set; }
        public WeightedPlane3D planeData { get; set; }
        public Color colourData { get; set; }
        public float numberData { get; set; }

        public FieldData()
        {
            vectorData = null;
            planeData = null;
            colourData = new Color();
            numberData = 0;
        }

        public bool hasVectorData() { return vectorData != null; }
        public bool hasPlaneData() { return planeData != null; }
        public bool hasColourData() { return !colourData.IsEmpty; }
        public bool hasNumberData() { return numberData != 0; }
    }
}
