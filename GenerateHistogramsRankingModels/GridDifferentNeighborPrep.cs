using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colors;

namespace GenerateHistogramsRankingModels
{
    class GridDifferentNeighborPrep<ColorType> : GridPrep<ColorType> where ColorType : Colors.Color3d, new()
    {
        public override string Name { get => "GridDifferentNeighbor" + typeof(ColorType).Name + $" W:{GridWidth} T:{ColorTolerance}"; }

        public GridDifferentNeighborPrep(int gridWidth, int colorTolerance) : base(gridWidth, colorTolerance)
        {
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
            var grid = MakeGridDifferentNeighbor(dominantColors, gridHeight);

            return grid;
        }

        private Color3d[,] MakeGridDifferentNeighbor(Dictionary<ColorType, ClusterData>[,] dominantColors, int gridHeight)
        {
            Colors.Color3d[,] grid = new Colors.Color3d[GridWidth, gridHeight];

            for (int x = 0; x < GridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    var maxClusterBeforeRemoval = GetMaxClusterKvp(dominantColors[x, y]);

                    if (x > 0 && dominantColors[x, y].Count > 0)
                    {
                        var key = GetMaxClusterKvp(dominantColors[x - 1, y]).Key;
                        dominantColors[x, y].Remove(key);

                        if (y > 0 && dominantColors[x, y].Count > 0)
                        {
                            key = GetMaxClusterKvp(dominantColors[x - 1, y - 1]).Key;
                            dominantColors[x, y].Remove(key);
                        }
                    }

                    if (y > 0 && dominantColors[x, y].Count > 0)
                    {
                        var key = GetMaxClusterKvp(dominantColors[x, y - 1]).Key;
                        dominantColors[x, y].Remove(key);
                    }

                    if (dominantColors[x, y].Count == 0)
                    {
                        // need to add the color back, so the next cell could look at the previous cells value
                        dominantColors[x, y].Add(maxClusterBeforeRemoval.Key, maxClusterBeforeRemoval.Value); 
                    }
                    
                    grid[x, y] = GetMaxClusterKvp(dominantColors[x, y]).Value.AverageColor;
                }
            }

            return grid;
        }

        private static KeyValuePair<ColorType, ClusterData> GetMaxClusterKvp(Dictionary<ColorType, ClusterData> dict)
        {
            return dict.Aggregate((l, r) => l.Value.TotalPixels.CompareTo(r.Value.TotalPixels) > 0 ? l : r);
        }
    }
}
