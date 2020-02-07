using ColorResearchAnalysis;
using System;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Colors;

namespace GenerateHistogramsRankingModels
{
    static partial class Program
    {
        public static SQLiteConnection DbConn = new SQLiteConnection(Config.DbConnectionString);
        public static ImageWarehouse imageWarehouse;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var folders = new[] { "dataset", "../dataset", "../../dataset", "../../../dataset" };
            //var folders = new[] { "dataset-min", "../dataset-min", "../../dataset-min", "../../../dataset-min" };

            string sourceDir = folders.FirstOrDefault(f => Directory.Exists(f));

            if(sourceDir != null)
            {
                imageWarehouse = imageWarehouseFactory(sourceDir);

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FormHistogramsRM());
            } else
            {
                MessageBox.Show("Dataset folder not found");
            }            
        }

        static unsafe ImageWarehouse imageWarehouseFactory(string sourceDir)
        {
            var iw = new ImageWarehouse(sourceDir, DbConn);

            iw.Preprocessors.Add(new GridPrep<ColorLab>(24, 10));
            iw.Preprocessors.Add(new ThumbnailPrep<ColorLab>(24));
            iw.Preprocessors.Add(new VoidPrep<ColorLab>());
            iw.Preprocessors.Add(new GridDifferentNeighborPrep<ColorLab>(24, 10));

            iw.RankingModels.Add(new L23dRankingModel<ColorLab>(new TargetFinderL23d(3, 0)));
            iw.RankingModels.Add(new L24dRankingModel<ColorLab>(new TargetFinderL24d(3, 0)));

            return iw;
        }

        public static int CountQueries()
        {
            int count = 0;

            foreach (var picFilename in imageWarehouse.SourceFiles)
            {
                var pic = new Picture(DbConn);
                pic.Load(picFilename);

                count += pic.Answers.Length;
            }

            return count;
        }
    }
}
