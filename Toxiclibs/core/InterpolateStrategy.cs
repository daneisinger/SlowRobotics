using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toxiclibs.core
{
    public interface InterpolateStrategy
    {

        /**
         * Implements an interpolation equation using double precision values.
         * 
         * @param a
         *            current value
         * @param b
         *            target value
         * @param f
         *            normalized interpolation factor (0.0 .. 1.0)
         * @return interpolated value
         */
        double interpolate(double a, double b, double f);

        /**
         * Implements an interpolation equation using float values.
         * 
         * @param a
         *            current value
         * @param b
         *            target value
         * @param f
         *            normalized interpolation factor (0.0 .. 1.0)
         * @return interpolated value
         */
        float interpolate(float a, float b, float f);
    }
}
