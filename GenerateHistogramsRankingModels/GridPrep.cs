using Colors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GenerateHistogramsRankingModels
{
    public class GridPrep<ColorType> : ImageWarehouse.Preprocessor where ColorType : Colors.Color3d, new()
    {
        public override string Name { get => "Grid" + typeof(ColorType).Name + $" W:{GridWidth} T:{ColorTolerance}"; }

        public int GridWidth { get; private set; }
        public int ColorTolerance { get; private set; }

        public GridPrep(int gridWidth, int colorTolerance)
        {
            this.GridWidth = gridWidth;
            this.ColorTolerance = colorTolerance;
        }

        public override unsafe Color3d[,] PreprocessRaw(int Width, int Height, byte* BitmapPointer, int BytesPerPixel)
        {
            var dominantColors = DetermineDominantColors(
                Width,
                Height,
                BitmapPointer,
                BytesPerPixel
            );

            int gridHeight = (int)Math.Round(GridWidth * ((double)Height / Width));
            var grid = MakeGridFromMostDominantColors(dominantColors, gridHeight);
            
            return grid;
        }

        protected struct ClusterData
        {
            public ColorType AverageColor;
            public int TotalPixels;
        }

        protected unsafe Dictionary<ColorType, ClusterData>[,] DetermineDominantColors(int Width, int Height, byte* BitmapPointer, int BytesPerPixel)
        {
            int GridHeight = (int)Math.Round(GridWidth * ((double)Height / Width));
            var stride = Width * BytesPerPixel;

            var dominantColors = new Dictionary<ColorType /* clusterized color */, Dictionary<ColorType /* actual color */, int /* pixel count */>>[GridWidth, GridHeight];
            
            for (int y = 0; y < Height; y++)
            {
                byte* row = &BitmapPointer[y * stride];

                for (int i = 0; i <= stride - BytesPerPixel; i += BytesPerPixel)
                {
                    byte Rcolor = row[i + 2];
                    byte Gcolor = row[i + 1];
                    byte Bcolor = row[i];

                    var pixel = new ColorType();
                    pixel.SetFromRgb(Rcolor, Gcolor, Bcolor);

                    int x = i / BytesPerPixel;
                    int gX = (int)(x / ((double)Width / GridWidth));
                    int gY = (int)(y / ((double)Height / GridHeight));

                    ColorType clusterizedColorKey = GetClusterizedColor(pixel);

                    if (dominantColors[gX, gY] == null)
                        dominantColors[gX, gY] = new Dictionary<ColorType, Dictionary<ColorType, int>>();

                    if (! dominantColors[gX, gY].ContainsKey(clusterizedColorKey))
                        dominantColors[gX, gY][clusterizedColorKey] = new Dictionary<ColorType, int>();

                    if (!dominantColors[gX, gY][clusterizedColorKey].ContainsKey(pixel))
                        dominantColors[gX, gY][clusterizedColorKey][pixel] = 0;

                    dominantColors[gX, gY][clusterizedColorKey][pixel]++;
                }
            }

            var resultingClusters = new Dictionary<ColorType, ClusterData>[GridWidth, GridHeight];

            for (int x = 0; x < GridWidth; x++)
            {
                for (int y = 0; y < GridHeight; y++)
                {
                    resultingClusters[x, y] = new Dictionary<ColorType, ClusterData>();

                    foreach (var clusterizedColor in dominantColors[x, y])
                    {
                        int totalPixelsInCluster = 0;
                        ColorType avgColor = new ColorType();

                        foreach (var actualColor in clusterizedColor.Value)
                        {
                            totalPixelsInCluster += actualColor.Value;

                            for (int i = 0; i < actualColor.Key.Spectrums.Length; i++)
                            {
                                avgColor.Spectrums[i] += actualColor.Key.Spectrums[i];
                            }
                        }

                        for (int i = 0; i < 3; i++)
                        {
                            avgColor.Spectrums[i] /= totalPixelsInCluster;
                        }

                        resultingClusters[x, y].Add(clusterizedColor.Key, new ClusterData() { TotalPixels = totalPixelsInCluster, AverageColor = avgColor });
                    }
                }
            }

            return resultingClusters;
        }

        protected Color3d[,] MakeGridFromMostDominantColors(Dictionary<ColorType, ClusterData>[,] dominantColors, int gridHeight)
        {
            Colors.Color3d[,] grid = new Colors.Color3d[GridWidth, gridHeight];

            for (int x = 0; x < GridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    grid[x, y] = dominantColors[x, y].Aggregate((l, r) => l.Value.TotalPixels > r.Value.TotalPixels ? l : r).Value.AverageColor;
                }
            }

            return grid;
        }

        private ColorType GetClusterizedColor(ColorType color)
        {
            var c = new ColorType();
            c.Set(
                color.Spectrums[0] / ColorTolerance * ColorTolerance + ColorTolerance / 2,
                color.Spectrums[1] / ColorTolerance * ColorTolerance + ColorTolerance / 2,
                color.Spectrums[2] / ColorTolerance * ColorTolerance + ColorTolerance / 2
            );

            return c;
        }
    }
}
