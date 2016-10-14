using SlowRobotics.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Field.Elements
{
    public class BitmapFieldElement : DistanceFieldElement
    {
        public Bitmap img { get; set; }
        public float scale { get; set; }

        public BitmapFieldElement(Bitmap _img, Vec3D _location, float _scale, float _weight, float _maxDist, float _attenuation) : base(_location,_weight,_maxDist,_attenuation)
        {
            img = _img;
            scale = _scale;
        }

        public override float getWeight(Vec3D loc)
        {
            float d = SR_Math.constrain(location.distanceTo(loc), 1, maxDist);
            return ((d < maxDist) ? (weight * (1 / (float)Math.Pow(d, attenuation))) : 0);
        }

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

        public bool inBoundsXY(Vec3D imgLoc)
        {
            return (imgLoc.x >= 0 && imgLoc.y >= 0 && imgLoc.x < img.Width * scale && imgLoc.y < img.Height * scale);
        }

        public Color getPixelAt(Vec3D imgLoc)
        {
            return img.GetPixel((int)imgLoc.x, (int)imgLoc.y);
        }
    }
}
