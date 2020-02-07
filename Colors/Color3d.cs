using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colors
{
    public abstract class Color3d
    {
        public int[] Spectrums { get; protected set; } = new int[3];

        public Color3d(params int[] Spectrums)
        {
            if(Spectrums.Length > 0)
                this.Spectrums = Spectrums;
        }

        public void Set(params int[] Spectrums)
        {
            this.Spectrums = Spectrums;
        }

        public abstract void SetFromRgb(byte R, byte G, byte B);

        public double Get4dDistanceL2(Color3d color, int distFromPoint)
        {
            if(this.GetType() != color.GetType())
            {
                throw new Exception("Can't compare colors in different color spaces");
            }

            double colorDist = 0;

            for (int i = 0; i < Spectrums.Length; i++)
            {
                var dist = Spectrums[i] - color.Spectrums[i];
                colorDist += dist * dist;
            }

            return Math.Sqrt(colorDist + distFromPoint);
        }

        public abstract ColorRgb ConvertToRgb();

        public ColorType ConvertTo<ColorType>() where ColorType : Color3d, new()
        {
            if (typeof(ColorType) == GetType())
            {
                return (ColorType)this;
            }

            var thisInRgb = ConvertToRgb();

            var color = new ColorType();
            color.SetFromRgb(thisInRgb.R, thisInRgb.G, thisInRgb.B);
            return color;
        }
        
        public Color3d GetCopy()
        {
            return (Color3d)MemberwiseClone();
        }

        public override bool Equals(object color2)
        {
            var c2 = (Color3d)color2;

            return
                Spectrums[0] == c2.Spectrums[0] &&
                Spectrums[1] == c2.Spectrums[1] &&
                Spectrums[2] == c2.Spectrums[2];
        }

        public override int GetHashCode()
        {
            return -317622605 + EqualityComparer<int[]>.Default.GetHashCode(Spectrums);
        }
    }
}
