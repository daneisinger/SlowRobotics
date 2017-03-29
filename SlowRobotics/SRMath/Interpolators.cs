using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toxiclibs.core;

namespace SlowRobotics.SRMath
{
    public class LinearInterpolation : InterpolateStrategy
    {

        public LinearInterpolation()
        {
        }

        public double interpolate(double a, double b, double f)
        {
            return a + (b - a) * f;
        }

        public float interpolate(float a, float b, float f)
        {
            return a + (b - a) * f;
        }

    }

    public class ExponentialInterpolation : InterpolateStrategy
    {

        private float exponent;
        public readonly static ExponentialInterpolation Squared = new ExponentialInterpolation(2);

        public ExponentialInterpolation() : this(2)
        {
        }

        public ExponentialInterpolation(float exp)
        {
            this.exponent = exp;
        }

        public double interpolate(double a, double b, double f)
        {
            return a + (b - a) * Math.Pow(f, exponent);
        }

        public float interpolate(float a, float b, float f)
        {
            return a + (b - a) * (float)Math.Pow(f, exponent);
        }

    }
}
