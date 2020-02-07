using ColorResearchAnalysis;
using Colors;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateHistogramsRankingModels
{
    public class RankingModelsComparer
    {
        public Picture.Answer Query { get; set; }

        public delegate double RankingModelDistance(Color3d queryCircle, Color3d targetColor, int distX, int distY);
        RankingModelDistance rmDistances;
        private string currentDir;
        private string[] filesInCurrentDir;

        public RankingModelsComparer(string folderToSeachIn)
        {
            currentDir = folderToSeachIn;
            filesInCurrentDir = Directory.GetFiles(currentDir, "*.jpg");
        }

        public void AddRankingModel(RankingModelDistance rankingModelDistance)
        {
            rmDistances += rankingModelDistance;
        }

        ConcurrentDictionary<string, double> ranks = new ConcurrentDictionary<string, double>(); // key: filename, val: avgDist of circles from target

        public ConcurrentDictionary<string, double> PerformSearch()
        {
            Parallel.ForEach(filesInCurrentDir, RankPicture);

            return ranks;
        }

        public void RankPicture(string filename)
        {
            var bitmap = new ColorsBitmap<ColorRgb>(filename);
            double totalDist = 0;

            foreach (var circle in Query.Circles)
            {
                if(circle.X >= bitmap.Width || circle.Y >= bitmap.Height)
                {
                    // this happens, but why?? how could this error happen?
                    // are the data mixed up in some way?

                    return;
                }

                var targetPoint = circle.FindMeantPixel(bitmap);
                var targetColor = bitmap.GetPixel(targetPoint.X, targetPoint.Y);

                var distX = circle.X - targetPoint.X;
                var distY = circle.Y - targetPoint.Y;

                // TODO: make use of ALL methods in the delegate, not just the last one
                var lastRmDist = rmDistances(circle.Color, targetColor, distX, distY);
                totalDist += lastRmDist;
            }

            double avgDist = totalDist / Query.Circles.Length;
            ranks.TryAdd(Path.GetFileName(filename), avgDist);
        }
    }
}
