using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SlowRobotics.Voxels
{
    public struct ColourVoxel : IComparable
    {

        public byte R,G,B,A;
        public static readonly ColourVoxel White = new ColourVoxel(255,255,255,255);
        public static readonly ColourVoxel Red = new ColourVoxel(255, 0, 0, 255);
        public static readonly ColourVoxel Black = new ColourVoxel(0, 0, 0, 255);

        public ColourVoxel(byte _r, byte _g, byte _b, byte _a)
        {
            R = _r;
            B = _b;
            G = _g;
            A = _a;
        }
        
        public ColourVoxel(byte _r, byte _g, byte _b) : this(_r, _g, _b, 255) { }
        public ColourVoxel(Color _c) : this(_c.R,_c.G,_c.B,_c.A) { }

        public float GetBrightness()
        {
            return (R + R + B + G + G + G) / 6;
        }

        public int CompareTo(object obj)
        {
            return R.CompareTo(obj);
        }

        public static ColourVoxel operator +(ColourVoxel a, ColourVoxel b)
        {
            byte rr = (byte) (a.R + b.R);
            byte gg = (byte)(a.G + b.G);
            byte bb = (byte)(a.B + b.B);
            byte aa = (byte)(a.A + b.A);
            return new ColourVoxel(rr,gg,bb,aa);
        }
        public static ColourVoxel operator +(ColourVoxel a, float b)
        {
            byte rr = (byte)(a.R + b);
            byte gg = (byte)(a.G + b);
            byte bb = (byte)(a.B + b);
            byte aa = (byte)(a.A + b);
            return new ColourVoxel(rr, gg, bb, aa);
        }
        public static ColourVoxel operator -(ColourVoxel a, ColourVoxel b)
        {
            byte rr = (byte)(a.R - b.R);
            byte gg = (byte)(a.G - b.G);
            byte bb = (byte)(a.B - b.B);
            byte aa = (byte)(a.A - b.A);
            return new ColourVoxel(rr, gg, bb, aa);
        }
        public static ColourVoxel operator -(ColourVoxel a, float b)
        {
            byte rr = (byte)(a.R - b);
            byte gg = (byte)(a.G - b);
            byte bb = (byte)(a.B - b);
            byte aa = (byte)(a.A - b);
            return new ColourVoxel(rr, gg, bb, aa);
        }
        public static ColourVoxel operator *(ColourVoxel a, ColourVoxel b)
        {
            byte rr = (byte)(a.R * b.R);
            byte gg = (byte)(a.G * b.G);
            byte bb = (byte)(a.B * b.B);
            byte aa = (byte)(a.A * b.A);
            return new ColourVoxel(rr, gg, bb, aa);
        }
        public static ColourVoxel operator *(ColourVoxel a, float b)
        {
            byte rr = (byte)(a.R * b);
            byte gg = (byte)(a.G * b);
            byte bb = (byte)(a.B * b);
            byte aa = (byte)(a.A * b);
            return new ColourVoxel(rr, gg, bb, aa);
        }
        /*
        public static ColourVoxel Blend(this ColourVoxel color, ColourVoxel backColor, double amount)
        {
            byte r = (byte)((color.R * amount) + backColor.R * (1 - amount));
            byte g = (byte)((color.G * amount) + backColor.G * (1 - amount));
            byte b = (byte)((color.B * amount) + backColor.B * (1 - amount));
            return new ColourVoxel(r, g, b);
        }*/

    }
}
