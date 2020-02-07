using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colors
{
    public class ColorLab : Color3d
    {
        public int L { get { return Spectrums[0]; } set { Spectrums[0] = value; } }
        public int a { get { return Spectrums[1]; } set { Spectrums[1] = value; } }
        public int b { get { return Spectrums[2]; } set { Spectrums[2] = value; } }

        static Colourful.Conversion.ColourfulConverter colorfulConverter = new Colourful.Conversion.ColourfulConverter();

        public override ColorRgb ConvertToRgb()
        {
            var colourfulLab = new Colourful.LabColor(L, a, b);
            System.Drawing.Color systemRgb = colorfulConverter.ToRGB(colourfulLab);

            var colorRgb = new ColorRgb();
            colorRgb.SetFromRgb(systemRgb.R, systemRgb.G, systemRgb.B);

            return colorRgb;
        }

        public override void SetFromRgb(byte R, byte G, byte B)
        {
            var colorMineRgb = new ColorMine.ColorSpaces.Rgb
            {
                R = R,
                G = G,
                B = B
            };

            var colorMineLab = colorMineRgb.To<ColorMine.ColorSpaces.Lab>();

            //var colourfulRgb = new Colourful.RGBColor(System.Drawing.Color.FromArgb(R, G, B));
            //var colourfulLab = colorfulConverter.ToLab(colourfulRgb);

            L = (int)colorMineLab.L;
            a = (int)colorMineLab.A;
            b = (int)colorMineLab.B;
        }
    }
}
