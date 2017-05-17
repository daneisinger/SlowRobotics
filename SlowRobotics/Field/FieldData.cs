using SlowRobotics.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Field
{
    /// <summary>
    /// The FieldData is a container class for storing and checking field information.
    /// It containts vector data, plane3d data, colour data and number data.
    /// </summary>
    public class FieldData
    {

        public Vec3D vectorData { get; set; }
        public WeightedPlane3D planeData { get; set; }
        public Color colourData { get; set; }
        public float numberData { get; set; }

        /// <summary>
        /// Default (empty) constructor
        /// </summary>
        public FieldData()
        {
            vectorData = null;
            planeData = null;
            colourData = new Color();
            numberData = 0;
        }

        /// <summary>
        /// Checks whether any Vector data has been set
        /// </summary>
        /// <returns></returns>
        public bool hasVectorData() { return vectorData != null; }
        /// <summary>
        /// Checks whether any Plane3D data has been set
        /// </summary>
        /// <returns></returns>
        public bool hasPlaneData() { return planeData != null; }
        /// <summary>
        /// Checks whether any Colour data has been set
        /// </summary>
        /// <returns></returns>
        public bool hasColourData() { return !colourData.IsEmpty; }
        /// <summary>
        /// Checks whether any numerical data has been set
        /// </summary>
        /// <returns></returns>
        public bool hasNumberData() { return numberData != 0; }
    }
}
