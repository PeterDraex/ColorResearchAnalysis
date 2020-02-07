using Colors;
using System;

namespace GenerateHistogramsRankingModels
{
    /// <summary>
    /// Uses Euclidean distance in 5D to rank images
    /// </summary>
    /// <typeparam name="ColorType">Color space for measuring distance</typeparam>
    class L25dRankingModel<ColorType> : MinAvgDistanceRankingModel where ColorType : Color3d, new()
    {
        public L25dRankingModel(TargetFinder targetFinder) : base(targetFinder)
        {
        }

        public override string Name => "RM:L25D " + targetFinder.Name;

        public override double Distance(ColorRgb queryCircle, Color3d targetColor, int distX, int distY)
        {
            var queryCircleConverted = queryCircle.ConvertTo<ColorType>();
            var targetColorConverted = targetColor.ConvertTo<ColorType>();

            var dist = distX * distX + distY * distY;

            for (int i = 0; i < queryCircle.Spectrums.Length; i++)
            {
                var spectDiff = targetColorConverted.Spectrums[i] - queryCircleConverted.Spectrums[i];
                dist += spectDiff * spectDiff;
            }

            return Math.Sqrt(dist);
        }
    }
}