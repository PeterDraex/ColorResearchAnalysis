using System.Collections.Concurrent;
using System.Collections.Generic;
using ColorResearchAnalysis;
using System.Threading.Tasks;
using Colors;
using System.Linq;
using System;

namespace GenerateHistogramsRankingModels
{
    public abstract class MinAvgDistanceRankingModel : ImageWarehouse.RankingModel
    {
        protected readonly TargetFinder targetFinder;

        /// <summary>
        /// Given a circle and a picture, returns which pixel user had in mind when drawing the circle
        /// </summary>
        public abstract class TargetFinder
        {
            public abstract string Name { get; }
            public abstract System.Drawing.Point FindTarget(Picture.Answer.Circle circle, ImageWarehouse.Image image);
        }

        public MinAvgDistanceRankingModel(TargetFinder targetFinder)
        {
            this.targetFinder = targetFinder;
        }
        
        public override List<ImageWarehouse.Image> Sort(Picture.Answer Query, ConcurrentBag<ImageWarehouse.Image> imageDataset)
        {
            var results = new ConcurrentBag<(double, ImageWarehouse.Image)>();

            Parallel.ForEach(imageDataset, img => {
                double totalDist = 0;

                foreach (var circle in Query.Circles)
                {
                    int relativeX = (int)(circle.X / ((double)img.Width / img.PreprocessedImage.GetLength(0)));
                    int relativeY = (int)(circle.Y / ((double)img.Height / img.PreprocessedImage.GetLength(1)));

                    if (relativeX >= img.PreprocessedImage.GetLength(0) || relativeY >= img.PreprocessedImage.GetLength(1))
                    {
                        // this happens, but why?? how could this error happen?
                        // are the data mixed up in some way?

                        return;
                    }

                    var targetPoint = targetFinder.FindTarget(circle, img);
                    var targetColor = img.PreprocessedImage[targetPoint.X, targetPoint.Y];

                    var distX = relativeX - targetPoint.X;
                    var distY = relativeY - targetPoint.Y;

                    var lastRmDist = Distance(circle.Color, targetColor, distX, distY);
                    totalDist += lastRmDist;
                }

                double avgDist = totalDist / Query.Circles.Length;

                if(double.IsNaN(avgDist))
                {
                    Console.WriteLine("nananananana");
                }

                if (double.IsInfinity(avgDist))
                    Console.WriteLine("to infinityyyy");

                results.Add((avgDist, img));
            });


            var orderedResults = from r in results
                                 orderby r.Item1
                                 select r;

            var orderedTop100 = orderedResults.Take(100);

            // zkuste k tomu take vypisovat pro kazdy ranking ciselnou hodnotu H, kterou ziskate z prvnich 100 prvku serazenych vysledku(pro velkou DB)
            // H = -k / (sum_{ i = 1..100} ln(x_i / x_100)), kde x je distance podle ktere obrazky tridite
            // 
            // pokud budeme mit tuto hodnotu pro RANK a pak pro RANK bez odebraneho kolecka, tak muzeme sledovat ve 2D, jestli tvori dobra a spatna kolecka klastry nebo ne

            int k = results.Count; // ?? je to správne

            // (sum_{ i = 1..100} ln(x_i / x_100)), kde x je distance podle ktere obrazky tridite
            var lastDistance = orderedTop100.Last().Item1;
            var valueHEnum = from r in orderedTop100 select r.Item1 == 0 || lastDistance == 0 ? 0 : Math.Log(r.Item1 / lastDistance);
            double valueHSum = valueHEnum.Sum();

            return (from r in results orderby r.Item1 select r.Item2).ToList();
        }

        public abstract double Distance(Colors.ColorRgb queryCircle, Colors.Color3d targetColor, int distX, int distY);
    }
}