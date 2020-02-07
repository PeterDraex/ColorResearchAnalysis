using Colors;
using System;

namespace GenerateHistogramsRankingModels
{
    /// <summary>
    /// Uses Euclidean distance in 4D to rank images
    /// </summary>
    /// <typeparam name="ColorType">Color space for measuring distance</typeparam>
    class L24dRankingModel<ColorType> : MinAvgDistanceRankingModel where ColorType : Color3d, new()
    {
        public L24dRankingModel(TargetFinder targetFinder) : base(targetFinder)
        {
        }

        public override string Name => "RM:L24D " + targetFinder.Name;

        public override double Distance(ColorRgb queryCircle, Color3d targetColor, int distX, int distY)
        {
            var queryCircleConverted = queryCircle.ConvertTo<ColorType>();
            var targetColorConverted = targetColor.ConvertTo<ColorType>();

            var distFromPoint = (int)Math.Sqrt(distX * distX + distY * distY);
            return targetColorConverted.Get4dDistanceL2(queryCircleConverted, distFromPoint);
        }
    }
}