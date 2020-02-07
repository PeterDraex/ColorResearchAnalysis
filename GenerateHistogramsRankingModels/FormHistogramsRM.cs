using ColorResearchAnalysis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace GenerateHistogramsRankingModels
{
    public partial class FormHistogramsRM : Form
    {
        public FormHistogramsRM()
        {
            InitializeComponent();
        }

        private class SearchConfig
        {
            public readonly int PreprocessorIndex;
            public readonly int RankingModelIndex;
            public readonly double AverageRank;

            public ImageWarehouse.Preprocessor Preprocessor => Program.imageWarehouse.Preprocessors[PreprocessorIndex];
            public ImageWarehouse.RankingModel RankingModel => Program.imageWarehouse.RankingModels[RankingModelIndex];

            public SearchConfig(int DatasetIndex, int RankingModelIndex)
            {
                this.PreprocessorIndex = DatasetIndex;
                this.RankingModelIndex = RankingModelIndex;

                int totalQueries = 0;
                int totalRanks = 0;

                for (int i = 0; i < Program.imageWarehouse.SourceFiles.Length; i++)
                {
                    int val = Program.imageWarehouse.Histograms[DatasetIndex, RankingModelIndex, i];

                    totalQueries += val;
                    totalRanks += i * val;
                }

                AverageRank = (double)totalRanks / totalQueries;
            }

            public override string ToString()
            {
                var preprocName = Program.imageWarehouse.Preprocessors[PreprocessorIndex].Name;
                var rankingName = Program.imageWarehouse.RankingModels[RankingModelIndex].Name;

                return $"{Math.Round(AverageRank, 2):00.00} {preprocName} {rankingName}";
            }
        }

        private void FormHistogramsRM_Load(object sender, EventArgs e)
        {
            Program.imageWarehouse.OnImagePreprocessed += imageFilename =>
            {
                progressBar1.BeginInvoke((MethodInvoker)(() =>
                {
                    progressBar1.Value++;
                }));
            };

            Program.imageWarehouse.OnOneRankingFinished += ImageWarehouse_OnOneRankingFinished;

            //nUPDatasetIndex.Maximum = Program.imageWarehouse.Preprocessors.Count - 1;
            //nUPRankModelIndex.Maximum = Program.imageWarehouse.RankingModels.Count - 1;
        }

        private void ImageWarehouse_OnOneRankingFinished(int datasetIndex, int rankModelIndex, Picture.Answer query, int rank)
        {
            if(progressBar2.IsHandleCreated)
                progressBar2.BeginInvoke((MethodInvoker)(() =>
                {
                    progressBar2.Value++;
                }));
        }

        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            progressBar1.Maximum = Program.imageWarehouse.SourceFiles.Length;
            progressBar1.Value = 0;
            buttonGenerate.Enabled = false;
            
            Task.Factory.StartNew(() =>
            {
                Program.imageWarehouse.PreprocessImages();
            }).ContinueWith(a =>
            {
                progressBar2.Invoke((MethodInvoker) (() => {
                    progressBar2.Maximum =
                        Program.imageWarehouse.Preprocessors.Count *
                        Program.imageWarehouse.RankingModels.Count *
                        Program.CountQueries();

                    progressBar2.Value = 0;
                }));
                
                Program.imageWarehouse.RunRankingModels();
            }).ContinueWith(a => {
                buttonGenerate.BeginInvoke((MethodInvoker)(() => { buttonGenerate.Enabled = true; }));
                DisplayResults();
            });
        }

        //Chart chartHist;

        //private void DrawHistogram()
        //{
        //    chartHist = new Chart
        //    {
        //        Location = new Point(0, buttonGenerate.Height + 10),
        //        Name = "chartHist",
        //        Size = new Size(440, 280),
        //        Text = "chart"
        //    };

        //    chartHist.ChartAreas.Add(new ChartArea());

        //    //chartHist.ChartAreas[0].AxisX.Minimum = min;
        //    //chartHist.ChartAreas[0].AxisX.Maximum = max;
        //    //chartHist.ChartAreas[0].AxisX.Interval = (max - min) / 10;

        //    var serie = chartHist.Series.Add("hist");
        //    serie.ChartType = SeriesChartType.Column;
        //    serie.Color = Color.Black;

        //    for (int i = 0; i < histogram.Length; i++)
        //    {
        //        serie.Points.AddXY(i, histogram[i]);
        //    }

        //    BeginInvoke((MethodInvoker)(() =>
        //    {
        //        Controls.Add(chartHist);
        //    }));            
        //}

        //private void CompareModelsOnPicture(string picFilename)
        //{
        //    var pic = new Picture(Program.DbConn);
        //    pic.Load(picFilename, c => true);

        //    foreach (var answer in pic.Answers)
        //    {
        //        var rmc = new RankingModelsComparer(Path.GetDirectoryName(picFilename))
        //        {
        //            Query = answer
        //        };

        //        rmc.AddRankingModel((queryC, targetC, distX, distY) => {
        //            var distFromPoint = (int)Math.Sqrt(distX * distX + distY * distY);
        //            return targetC.Get4dDistanceL2(queryC.ConvertToRgb(), distFromPoint);
        //        });

        //        var results = rmc.PerformSearch();
        //        var picDist = results[Path.GetFileName(picFilename)];
        //        var rank = results.Count(kvp => kvp.Value <= picDist);

        //        histogram[rank]++;
        //    }

        //    progressBar1.BeginInvoke((MethodInvoker)(() => {
        //        progressBar1.Value++;
        //    }));
        //}

        //int[] histogram;

        private List<SearchConfig> listSearchConfigs;

        private void DisplayResults()
        {
            listSearchConfigs = new List<SearchConfig>();
            var sw = new StreamWriter($"results-{DateTime.UtcNow.Ticks}.txt");

            for (int i = 0; i < Program.imageWarehouse.Preprocessors.Count; i++)
            {
                for (int j = 0; j < Program.imageWarehouse.RankingModels.Count; j++)
                {
                    var conf = new SearchConfig(i, j);
                    listSearchConfigs.Add(conf);
                    sw.WriteLine(conf.ToString());
                }
            }

            sw.Close();
            listSearchConfigs.Sort((sc1, sc2) => sc1.AverageRank < sc2.AverageRank ? -1 : 1  );


            lbResults.Invoke((MethodInvoker)(() => {
                lbResults.Items.Clear();

                foreach (var sc in listSearchConfigs)
                {
                    lbResults.Items.Add(sc);
                }
            }));
        }

        private void lbResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbResults.SelectedItem == null)
                return;

            chartHistogram.Series.Clear();

            chartHistogram.ChartAreas[0].AxisX.Minimum = 0;
            chartHistogram.ChartAreas[0].AxisX.Maximum = Program.imageWarehouse.SourceFiles.Length;
            chartHistogram.ChartAreas[0].AxisX.Interval = Program.imageWarehouse.SourceFiles.Length / 10;

            SearchConfig selectedSearchConfig = (SearchConfig)lbResults.SelectedItem;

            string preprocName = Program.imageWarehouse.Preprocessors[selectedSearchConfig.PreprocessorIndex].Name;

            var serie = chartHistogram.Series.Add(selectedSearchConfig.ToString());

            serie.ChartType = SeriesChartType.Column;
            serie.Color = Color.Brown;

            int totalRanks = 0;
            int totalQueries = 0;

            for (int i = 0; i < Program.imageWarehouse.SourceFiles.Length; i++)
            {
                int val = Program.imageWarehouse.Histograms[selectedSearchConfig.PreprocessorIndex, selectedSearchConfig.RankingModelIndex, i];
                serie.Points.AddXY(i, val);

                totalQueries += val;
                totalRanks += i * val;
            }

            lAvgRank.Text = ((double)totalRanks / totalQueries).ToString();
        }
    }
}
