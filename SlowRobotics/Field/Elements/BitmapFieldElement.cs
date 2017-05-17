using SlowRobotics.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Toxiclibs.core;
using SlowRobotics.SRMath;

namespace SlowRobotics.Field.Elements
{

    /// <summary>
    /// Creates a vector field from bitmap RGB values.
    /// </summary>
    public class BitmapFieldElement : DistanceFieldElement
    {
        public Bitmap img { get; set; }
        public float scale { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="_img">Location of bitmap to generate field</param>
        /// <param name="_location">Point defining 0,0 corner of bitmap in world XY space</param>
        /// <param name="_scale">Scale factor for bitmap</param>
        /// <param name="_weight">Field weight</param>
        /// <param name="_maxDist">Maximum distance for field</param>
        /// <param name="_attenuation">Field attenuation</param>
        public BitmapFieldElement(Bitmap _img, Vec3D _location, float _scale, float _weight, float _maxDist, float _attenuation) : base(_location,_weight,_maxDist,_attenuation)
        {
            img = _img;
            scale = _scale;
        }

        /// <summary>
        /// Gets the field strength at a given point
        /// </summary>
        /// <param name="loc">Point to sample</param>
        /// <returns></returns>
        public override float getWeight(Vec3D loc)
        {
            float d = MathUtils.constrain(location.distanceTo(loc), 1, maxDist);
            return ((d < maxDist) ? (weight * (1 / (float)Math.Pow(d, attenuation))) : 0);
        }

        /// <summary>
        /// Integrates the bitmap field tensor at a given point into fielddata
        /// </summary>
        /// <param name="d">FieldData to integrate</param>
        /// <param name="loc">Sample point</param>
        public override void integrate(ref FieldData d, Vec3D loc)
        {
            Vec3D pixelLoc = loc.sub(location).scale(scale);
            
            if (inBoundsXY(pixelLoc))
            {
                Color c = getPixelAt(pixelLoc);

                Vec3D scaledVal = new Vec3D(c.R, c.G, c.B);

                if (d.hasVectorData())
                {
                    d.vectorData.addSelf(scaledVal);
                }
                else {
                    d.vectorData = scaledVal;
                }

                d.colourData = c; // overwrite existing colours WIP
            }
        }

        /// <summary>
        /// Tests whether a point is inside of the  pixel array
        /// </summary>
        /// <param name="imgLoc">Point in image space</param>
        /// <returns></returns>
        public bool inBoundsXY(Vec3D imgLoc)
        {
            return (imgLoc.x >= 0 && imgLoc.y >= 0 && imgLoc.x < img.Width * scale && imgLoc.y < img.Height * scale);
        }

        /// <summary>
        /// Gets the colour of a pixel at a sample point
        /// </summary>
        /// <param name="imgLoc">Sample point</param>
        /// <returns></returns>
        public Color getPixelAt(Vec3D imgLoc)
        {
            return img.GetPixel((int)imgLoc.x, (int)imgLoc.y);
        }
    }
}
