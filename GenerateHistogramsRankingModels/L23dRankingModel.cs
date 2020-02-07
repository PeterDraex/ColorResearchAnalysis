using Colors;
using System;

namespace GenerateHistogramsRankingModels
{
    /// <summary>
    /// Uses Euclidean distance in 4D to rank images
    /// </summary>
    /// <typeparam name="ColorType">Color space for measuring distance</typeparam>
    public class L23dRankingModel<ColorType> : MinAvgDistanceRankingModel where ColorType : Color3d, new()
    {
        public L23dRankingModel(TargetFinder targetFinder) : base(targetFinder)
        {
        }

        public override string Name => "RM:L23D " + targetFinder.Name;

        public override double Distance(ColorRgb queryCircle, Color3d targetColor, int distX, int distY)
        {
            var queryCircleConverted = queryCircle.ConvertTo<ColorType>();
            var targetColorConverted = targetColor.ConvertTo<ColorType>();

            return targetColorConverted.Get4dDistanceL2(queryCircleConverted, 0);
        }
    }
}