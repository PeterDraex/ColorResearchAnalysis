using ColorResearchAnalysis;
using Colors;
using System;

namespace GenerateHistogramsRankingModels
{
    class TargetFinderL25d : TargetFinder
    {
        public override string Name => $"TF:L25D SD:{SearchDiameterWidthPercentage}%";

        public TargetFinderL25d(int searchDiameterWidthPercentage, int minimumColorDist) : base(searchDiameterWidthPercentage, minimumColorDist) { }

        public override double Distance(Color3d circleColor, Color3d pixelColor, int distX, int distY)
        {
            if (circleColor.GetType() != pixelColor.GetType())
            {
                throw new Exception("Can't compare colors in different color spaces");
            }

            double totalDist = 0;

            for (int i = 0; i < circleColor.Spectrums.Length; i++)
            {
                var dist = circleColor.Spectrums[i] - pixelColor.Spectrums[i];
                totalDist += dist * dist;
            }

            totalDist += distX * distX + distY * distY;

            return Math.Sqrt(totalDist);
        }
    }
}
