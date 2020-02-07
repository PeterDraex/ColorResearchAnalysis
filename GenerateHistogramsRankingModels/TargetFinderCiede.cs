using ColorResearchAnalysis;
using Colors;
using System;

namespace GenerateHistogramsRankingModels
{
    class TargetFinderCiede : TargetFinder
    {
        public override string Name => $"TF:Ciede2000 SD:{SearchDiameterWidthPercentage}%";

        public TargetFinderCiede(int searchDiameterWidthPercentage, int minimumColorDist) : base(searchDiameterWidthPercentage, minimumColorDist) { }

        public override double Distance(Color3d circleColor, Color3d pixelColor, int distX, int distY)
        {
            if (circleColor.GetType() != pixelColor.GetType())
            {
                throw new Exception("Can't compare colors in different color spaces");
            }

            var circleColorRgb = circleColor.ConvertTo<ColorRgb>();
            var pixelColorRgb = pixelColor.ConvertTo<ColorRgb>();

            var mineCircleColor = new ColorMine.ColorSpaces.Rgb
            {
                R = circleColorRgb.R,
                G = circleColorRgb.G,
                B = circleColorRgb.B
            };

            var minePixelColor = new ColorMine.ColorSpaces.Rgb
            {
                R = pixelColorRgb.R,
                G = pixelColorRgb.G,
                B = pixelColorRgb.B
            };

            var ciede2000diff = mineCircleColor.Compare(minePixelColor, new ColorMine.ColorSpaces.Comparisons.CieDe2000Comparison());
            int distFromPoint = (int)Math.Sqrt(distX * distX + distY * distY);

            return ciede2000diff + distFromPoint;
        }
    }
}
