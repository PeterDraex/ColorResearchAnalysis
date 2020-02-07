using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colors
{
    public class ColorRgb : Color3d
    {
        public byte R { get { return (byte)Spectrums[0]; } set { Spectrums[0] = value; } }
        public byte G { get { return (byte)Spectrums[1]; } set { Spectrums[1] = value; } }
        public byte B { get { return (byte)Spectrums[2]; } set { Spectrums[2] = value; } }

        private static ColorConverter converter = new ColorConverter();

        public static ColorRgb FromHex(string str)
        {
            var color = (Color)converter.ConvertFromString(str);

            return new ColorRgb()
            {
                R = color.R,
                G = color.G,
                B = color.B
            };
        }

        public override void SetFromRgb(byte R, byte G, byte B)
        {
            this.R = R;
            this.G = G;
            this.B = B;
        }

        public override ColorRgb ConvertToRgb()
        {
            return this;
        }

        public static explicit operator Color(ColorRgb col)
        {
            return Color.FromArgb(col.R, col.G, col.B);
        }
    }
}
