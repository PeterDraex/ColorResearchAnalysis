using ColorResearchAnalysis;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Drawing;
using System.IO;

namespace GenerateHistogramsRankingModels
{
    class RandomRankingModel : ImageWarehouse.RankingModel
    {
        public override string Name { get => "Random"; }
        public override List<ImageWarehouse.Image> Sort(Picture.Answer query, ConcurrentBag<ImageWarehouse.Image> imageDataset)
        {
            var outputList = new List<ImageWarehouse.Image>(imageDataset.Count);

            foreach (var img in imageDataset)
            {
                outputList.Add(img);
            }

            DebugSave(outputList);
            return outputList;
        }

        private void DebugSave(List<ImageWarehouse.Image> outputList)
        {
            foreach (var img in outputList)
            {
                var bitmap = new Bitmap(img.PreprocessedImage.GetLength(0), img.PreprocessedImage.GetLength(1));

                for (int x = 0; x < bitmap.Width; x++)
                {
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        var pixColor = img.PreprocessedImage[x, y].ConvertToRgb();
                        var drawingColor = Color.FromArgb(pixColor.R, pixColor.G, pixColor.B);

                        bitmap.SetPixel(x, y, drawingColor);
                    }
                }

                string preprocessorName = img.Preprocessor.Name.Replace(" ", "").Replace(":", "");
                Directory.CreateDirectory(".\\Downscaling-images");
                bitmap.Save($@".\Downscaling-images\{Path.GetFileNameWithoutExtension(img.SourceFile)}-{preprocessorName}{Path.GetExtension(img.SourceFile)}");
            }
        }
    }
}
