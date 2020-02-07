using ColorResearchAnalysis;
using Colors;
using System;

namespace GenerateHistogramsRankingModels
{
    public class TargetFinderL23d : TargetFinder
    {
        public override string Name => $"TF:L23D SD:{SearchDiameterWidthPercentage}%";

        public TargetFinderL23d(int searchDiameterWidthPercentage, int minimumColorDist) : base(searchDiameterWidthPercentage, minimumColorDist) { }

        public override double Distance(Color3d circleColor, Color3d pixelColor, int distX, int distY)
        {
            if (circleColor.GetType() != pixelColor.GetType())
            {
                throw new Exception("Can't compare colors in different color spaces");
            }

            double colorDist = 0;

            for (int i = 0; i < circleColor.Spectrums.Length; i++)
            {
                var dist = circleColor.Spectrums[i] - pixelColor.Spectrums[i];
                colorDist += dist * dist;
            }

            return Math.Sqrt(colorDist);
        }
    }
}
