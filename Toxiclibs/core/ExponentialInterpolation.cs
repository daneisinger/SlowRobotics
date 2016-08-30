using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toxiclibs.core
{
    public class ExponentialInterpolation : InterpolateStrategy
    {

    private float exponent;

    /**
     * Default constructor uses square parabola (exp=2)
     */
    public ExponentialInterpolation() : this(2)
    {
    }

    /**
     * @param exp
     *            curve exponent
     */
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
