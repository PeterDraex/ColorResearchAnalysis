using System.Drawing;
using ColorResearchAnalysis;
using Colors;
using System;

namespace GenerateHistogramsRankingModels
{
    public abstract class TargetFinder : MinAvgDistanceRankingModel.TargetFinder
    {
        protected readonly int SearchDiameterWidthPercentage;
        protected readonly int MinimumColorDist;

        public TargetFinder(int searchDiameterWidthPercentage, int minimumColorDist)
        {
            SearchDiameterWidthPercentage = searchDiameterWidthPercentage;
            MinimumColorDist = minimumColorDist;
        }

        public override Point FindTarget(Picture.Answer.Circle circle, ImageWarehouse.Image img)
        {
            int relativeX = (int)(circle.X / ((double)img.Width / img.PreprocessedImage.GetLength(0)));
            int relativeY = (int)(circle.Y / ((double)img.Height / img.PreprocessedImage.GetLength(1)));

            int SearchDiameter = (int)Math.Round(img.Width * (SearchDiameterWidthPercentage / 100.0));
            int beginX = Math.Max(0, relativeX - SearchDiameter / 2);
            int beginY = Math.Max(0, relativeY - SearchDiameter / 2);

            Point closestPoint = new Point(relativeX, relativeY);
            double closestColorDistance = double.MaxValue;

            var circleColor = img.PreprocessedImage[0, 0].GetCopy();
            circleColor.SetFromRgb(circle.Color.R, circle.Color.G, circle.Color.B);

            for (int iX = 0; iX < SearchDiameter; iX++)
            {
                for (int iY = 0; iY < SearchDiameter; iY++)
                {
                    int currentPointX = beginX + iX;
                    int currentPointY = beginY + iY;

                    if (currentPointX < 0 || currentPointY < 0 || currentPointX >= img.PreprocessedImage.GetLength(0) || currentPointY >= img.PreprocessedImage.GetLength(1))
                        continue;

                    int distX = relativeX - currentPointX;
                    int distY = relativeY - currentPointY;
                    int distFromPoint = (int)Math.Sqrt(distX * distX + distY * distY);

                    if (distFromPoint <= SearchDiameter / 2)
                    {
                        var pixelColor = img.PreprocessedImage.GetAveragePixel(currentPointX, currentPointY);
                        double colorDist = Distance(circleColor, pixelColor, currentPointX, currentPointY);

                        if (colorDist > MinimumColorDist && colorDist < closestColorDistance)
                        {
                            closestColorDistance = colorDist;
                            closestPoint = new Point(currentPointX, currentPointY);
                        }
                    }
                }
            }

            return closestPoint;
        }

        public abstract double Distance(Color3d circleColor, Color3d pixelColor, int distX, int distY);
    }
}
