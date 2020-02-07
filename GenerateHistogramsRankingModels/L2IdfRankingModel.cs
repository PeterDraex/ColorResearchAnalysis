using Colors;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GenerateHistogramsRankingModels
{
    class L2IdfRankingModel<ColorType> : L24dRankingModel<ColorType> where ColorType : Color3d, new()
    {
        int[,,] colorOccurences = null;
        int totalPixelsInDataset;
        object colorOccLock = new object();

        public L2IdfRankingModel(TargetFinder targetFinder) : base(targetFinder)
        {
        }

        private void LoadColorOccurences()
        {
            if(colorOccurences == null)
            {
                lock(colorOccLock)
                {
                    if(colorOccurences == null)
                    {
                        var fs = File.Open("ColorOccurences.bin", FileMode.Open);
                        var bf = new BinaryFormatter();
                        colorOccurences = (int[,,])bf.Deserialize(fs, headers =>
                        {
                            totalPixelsInDataset = (int)headers[0].Value;
                            return new object();
                        });
                    }
                }
            }
        }

        private double CalculateIdf(Color3d color3d, int clusterMaxDiffInOneColor = 30)
        {
            var color = color3d.ConvertTo<ColorRgb>();

            int minR = Math.Max(color.R - clusterMaxDiffInOneColor, 0);
            int minG = Math.Max(color.G - clusterMaxDiffInOneColor, 0);
            int minB = Math.Max(color.B - clusterMaxDiffInOneColor, 0);
            int maxR = Math.Min(color.R + clusterMaxDiffInOneColor, 255);
            int maxG = Math.Min(color.G + clusterMaxDiffInOneColor, 255);
            int maxB = Math.Min(color.B + clusterMaxDiffInOneColor, 255);

            int totalOfThisColor = 1;

            for (int r = minR; r < maxR; r++)
            {
                for (int g = minG; g < maxG; g++)
                {
                    for (int b = minB; b < maxB; b++)
                    {
                        totalOfThisColor += colorOccurences[r, g, b];
                    }
                }
            }

            return Math.Log(totalPixelsInDataset / (double)totalOfThisColor);
        }

        public override double Distance(ColorRgb queryCircle, Color3d targetColor, int distX, int distY)
        {
            LoadColorOccurences();

            var l2dist = base.Distance(queryCircle, targetColor, distX, distY);
            return l2dist * CalculateIdf(queryCircle);
        }
    }
}