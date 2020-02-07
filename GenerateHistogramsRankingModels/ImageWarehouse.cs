using ColorResearchAnalysis;
using Colors;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GenerateHistogramsRankingModels
{
    public unsafe class ImageWarehouse
    {
        public string SourceDir;
        public string[] SourceFiles;

        public SQLiteConnection DbConn { get; }

        public ImageWarehouse(string sourceDir, SQLiteConnection DbConn)
        {
            SourceDir = sourceDir;
            SourceFiles = Directory.GetFiles(sourceDir, "*.jpg");
            this.DbConn = DbConn;
        }

        public class Image
        {
            public string SourceFile { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public Preprocessor Preprocessor { get; set; }
            public Colors.Color3d[,] PreprocessedImage;
        }

        public abstract class Preprocessor
        {
            /// <summary>
            /// Every preprocessor will generate a new data set of preprocessed images.
            /// Ranking models will then be compared on each dataset separately.
            /// </summary>
            /// 
            /// Preprocessing can include clustering, resizing, etc.
            /// 
            /// Every image from the data source will be preprocessed
            /// before ranking models will be compared on them.
            ///
            /// <param name="BitmapPointer">Pointer to the first byte of the bitmap</param>
            /// <returns>Preprocessed image</returns>
            public virtual Colors.Color3d[,] PreprocessRaw(int Width, int Height, byte* BitmapPointer, int BytesPerColor)
            {
                throw new NotImplementedException();
            }

            public abstract string Name { get; }
            public int IndexInContainer { get; set; }

            public virtual Color3d[,] Preprocess(Bitmap bitmap)
            {
                var imageSizeRect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

                // Lock the bitmap's bits.
                var bitmapData = bitmap.LockBits(imageSizeRect, System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);
                int bytesPerColor = bitmapData.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppArgb ? 4 : 3;

                byte* bitmapPointer = (byte*)bitmapData.Scan0.ToPointer();

                var preprocessedImage = PreprocessRaw(bitmap.Width, bitmap.Height, bitmapPointer, bytesPerColor);

                // Unlock the bits.
                bitmap.UnlockBits(bitmapData);

                return preprocessedImage;
            }
        }

        #region Pre-processing
        
        public List<Preprocessor> Preprocessors = new List<Preprocessor>();
        public ConcurrentBag<Image>[] PreprocessedDatasets;

        /// <summary>
        /// Parameter: image filename
        /// </summary>
        public event Action<string> OnImagePreprocessed = s => { };

        public void PreprocessImages()
        {
            PreprocessedDatasets = new ConcurrentBag<Image>[Preprocessors.Count];
            Histograms = new int[PreprocessedDatasets.Length, RankingModels.Count, SourceFiles.Length];

            for (int i = 0; i < PreprocessedDatasets.Length; i++)
            {
                PreprocessedDatasets[i] = new ConcurrentBag<Image>();
            }

            Parallel.ForEach(SourceFiles, sourceImageFilename =>
            {
                // Open file as bitmap
                var bitmap = new System.Drawing.Bitmap(sourceImageFilename);

                // Preprocess with each preprocessor
                for (int i = 0; i < Preprocessors.Count; i++)
                {
                    var preprocessedImage = Preprocessors[i].Preprocess(bitmap);

                    // Create image
                    var image = new Image()
                    {
                        Preprocessor = Preprocessors[i],
                        SourceFile = sourceImageFilename,
                        Width = bitmap.Width,
                        Height = bitmap.Height,
                        PreprocessedImage = preprocessedImage,
                    };

                    // Add to appropriate dataset
                    PreprocessedDatasets[i].Add(image);
                }

                OnImagePreprocessed(sourceImageFilename);
            });
        }
        #endregion

        #region Comparing ranking models
        public abstract class RankingModel
        {
            public abstract List<Image> Sort(Picture.Answer query, ConcurrentBag<Image> imageDataset);
            public abstract string Name { get; }
            public int IndexInContainer { get; set; }
        }

        public List<RankingModel> RankingModels = new List<RankingModel>();

        /// <summary>
        /// Parameters: datasetIndex, rankModIndex, query, rank
        /// </summary>
        public event Action<int, int, Picture.Answer, int> OnOneRankingFinished = (w, x, y, z) => { };

        public int RunSingleRankingModel(Picture picture, Picture.Answer query, int datasetIndex, int rankModIndex)
        {
            var searchResults = RankingModels[rankModIndex].Sort(query, PreprocessedDatasets[datasetIndex]);
            var rank = searchResults.FindIndex(i => Path.GetFileName(i.SourceFile) == Path.GetFileName(picture.Filename));

            Histograms[datasetIndex, rankModIndex, rank]++;
            OnOneRankingFinished(datasetIndex, rankModIndex, query, rank);

            return rank;
        }

        // Histograms[datasetIndex,rankModIndex,rank] = count of queries that were on position [rank] in the search results
        public int[,,] Histograms; 

        public void RunRankingModels(Picture picture, Picture.Answer query)
        {
            for (int datasetIndex = 0; datasetIndex < PreprocessedDatasets.Length; datasetIndex++)
            {
                for (int rankModIndex = 0; rankModIndex < RankingModels.Count; rankModIndex++)
                {
                    RunSingleRankingModel(picture, query, datasetIndex, rankModIndex);
                }
            }
        }

        public void RunRankingModels()
        {
            foreach (var picFilename in SourceFiles)
            {
                var pic = new Picture(DbConn);
                pic.Load(picFilename);

                foreach (var query in pic.Answers)
                {
                    RunRankingModels(pic, query);
                }
            }
        }
        #endregion
    }
}
