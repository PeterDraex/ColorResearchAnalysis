using ColorResearchAnalysis;
using Colors;
using System;

namespace GenerateHistogramsRankingModels
{
    class TargetFinderL24d : TargetFinder
    {
        public override string Name => $"TF:L24D SD:{SearchDiameterWidthPercentage}%";

        public TargetFinderL24d(int searchDiameterWidthPercentage, int minimumColorDist) : base(searchDiameterWidthPercentage, minimumColorDist) { }

        public override double Distance(Color3d circleColor, Color3d pixelColor, int distX, int distY)
        {
            if (circleColor.GetType() != pixelColor.GetType())
            {
                throw new Exception("Can't compare colors in different color spaces");
            }

            double colorDist = 0;
            int distFromPoint = (int)Math.Sqrt(distX * distX + distY * distY);

            for (int i = 0; i < circleColor.Spectrums.Length; i++)
            {
                var dist = circleColor.Spectrums[i] - pixelColor.Spectrums[i];
                colorDist += dist * dist;
            }

            return Math.Sqrt(colorDist + distFromPoint);
        }
    }
}
