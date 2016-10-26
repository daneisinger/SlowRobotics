using SlowRobotics.Simulation.Noise;
using SlowRobotics.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toxiclibs.core;

namespace SlowRobotics.Field.Elements
{
    public class NoiseFieldElement : DistanceFieldElement
    {
        public float ns { get; set; }
        public float rs { get; set; }
        public NoiseFieldElement(Vec3D _location, float _weight, float _maxDist, float _attenuation, float _noiseScale, float _rotationScale) :base(_location,_weight,_maxDist,_attenuation)
        {
            ns = _noiseScale;
            rs = _rotationScale;
        }

        public override void integrate(ref FieldData d, Vec3D loc)
        {
            float w = getWeight(loc);

            double p = (PerlinNoise.perlin(loc.x * ns, loc.y * ns, loc.z * ns));
            d.numberData+= (float)p*w;

            Vec3D scaledVal = new Vec3D((float)Math.Sin(p * Math.PI * rs), (float)Math.Cos(p * Math.PI * rs), 0).scale(w);

            if (d.hasVectorData())
            {
                d.vectorData.addSelf(scaledVal);
            }
            else {
                d.vectorData = scaledVal;
            }
        }

    }
}
