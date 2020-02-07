using Colors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ColorResearchAnalysis
{
    public partial class FormViewResults : PictureBrowserForm
    {
        private Picture.Answer.Circle.CircleFilter circleFilter = (c, i) => true;
        private Action AnnotatePicture;
        public override PictureBox MainPictureBox => pictureBox;
        
        public FormViewResults()
        {
            InitializeComponent();

            AnnotatePicture = _annotatePicture<ColorRgb>;
        }        

        private void _annotatePicture<T>() where T : Color3d, new()
        {
            var bitmapForDrawing = (Bitmap)MainPictureBox.Image;
            var bitmapForSearching = new ColorsBitmap<T>(new Bitmap(MainPictureBox.Image));

            var p = new Picture(Program.DbConn);
            p.Load(CurrentPicturePath, circleFilter);

            foreach (var ans in p.Answers)
            {
                ans.Draw(
                    bitmapToDraw: bitmapForDrawing,
                    bitmapToSearchTarget: bitmapForSearching,
                    showRadius: checkBoxShowRadius.Checked,
                    showTarget: checkBoxTarget.Checked,
                    showOrderOfCircles: checkBoxNumbers.Checked
                );
            }
        }

        public override void ShowPicture()
        {
            base.ShowPicture();
            AnnotatePicture();
        }

        private void checkBoxClustered_CheckedChanged(object sender, EventArgs e)
        {
            ShowPicture();
        }

        //private void trackBarClusterDistance_Scroll(object sender, EventArgs e)
        //{
        //    textBoxClusterDistance.Text = trackBarClusterDistance.Value.ToString(); 
        //    ShowPicture();
        //}

        //private void textBoxClusterDistance_TextChanged(object sender, EventArgs e)
        //{
        //    trackBarClusterDistance.Value = int.Parse(textBoxClusterDistance.Text);
        //    ShowPicture();
        //}

        private void checkBoxShowRadius_CheckedChanged(object sender, EventArgs e)
        {
            ShowPicture();
        }

        private void checkBoxTarget_CheckedChanged(object sender, EventArgs e)
        {
            ShowPicture();
        }
        
        private void buttonCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetImage(pictureBox.Image);
        }

        private void comboBoxFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxFilter.SelectedItem)
            {
                case "1st circle":
                    circleFilter = (c, i) => c.EntryOrder == 1;
                    break;
                case "2nd circle":
                    circleFilter = (c, i) => c.EntryOrder == 2;
                    break;
                case "3rd circle":
                    circleFilter = (c, i) => c.EntryOrder == 3;
                    break;
                case "4th circle":
                    circleFilter = (c, i) => c.EntryOrder == 4;
                    break;
                case "5th circle":
                    circleFilter = (c, i) => c.EntryOrder == 5;
                    break;
                case "Everything":
                    circleFilter = (c, i) => true;
                    break;
                default:
                    throw new ArgumentException();
            }

            ShowPicture();
        }

        private void checkBoxNumbers_CheckedChanged(object sender, EventArgs e)
        {
            ShowPicture();
        }

        private void comboBoxColorSpace_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxColorSpace.SelectedItem)
            {
                case "RGB":
                    AnnotatePicture = _annotatePicture<ColorRgb>;
                    break;
                case "Lab":
                    AnnotatePicture = _annotatePicture<ColorLab>;
                    break;
                default:
                    throw new ArgumentException();
            }

            ShowPicture();
        }

        private void numericUpDownMinimumColorDist_ValueChanged(object sender, EventArgs e)
        {
            Picture.Answer.MinimumColorDist = (int)numericUpDownMinimumColorDist.Value;
            ShowPicture();
        }

        private void numericUpDownSearchDiameter_ValueChanged(object sender, EventArgs e)
        {
            Picture.Answer.SearchDiameter = (int)numericUpDownSearchDiameter.Value;
            ShowPicture();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        //private void DrawDeepHistogram(string filename)
        //{
        //    const int WIDTH = 1200;
        //    const int HEIGHT = 200;

        //    if (DeepHistogram == null)
        //        LoadDeepHistogram();

        //    var bitmap = new System.Drawing.Bitmap(WIDTH, HEIGHT);
        //    var g = System.Drawing.Graphics.FromImage(bitmap);

        //    //g.Clear(System.Drawing.Color.Beige);
        //    var hBrush = new System.Drawing.Drawing2D.HatchBrush(
        //       System.Drawing.Drawing2D.HatchStyle.Horizontal,
        //       System.Drawing.Color.Red,
        //       System.Drawing.Color.FromArgb(255, 128, 255, 255)
        //    );
        //    g.FillRectangle(hBrush, 0, 0, WIDTH, HEIGHT);

        //    int barCount = (int)Math.Pow(HistogramConfig<T>.INTERVALS_PER_SPECTRUM, (new T()).ComponentCount);
        //    int widthPerBar = WIDTH / barCount;
        //    int maxValue = getHistogramsMaxValue();
        //    int drawnBars = 0;

        //    for (int i = 0; i < HistogramConfig<T>.INTERVALS_PER_SPECTRUM; i++)
        //    {
        //        for (int j = 0; j < HistogramConfig<T>.INTERVALS_PER_SPECTRUM; j++)
        //        {
        //            for (int k = 0; k < HistogramConfig<T>.INTERVALS_PER_SPECTRUM; k++)
        //            {
        //                int barHeight = (int)((double)HEIGHT / maxValue * DeepHistogram[i, j, k]);
        //                int offsetX = drawnBars++ * widthPerBar;
        //                int offsetY = HEIGHT - barHeight;

        //                var brushForHistogram = MakeBrushForInterval(i, j, k);

        //                g.FillRectangle(brushForHistogram, offsetX, offsetY, widthPerBar, barHeight);
        //            }
        //        }
        //    }

        //    var font = new System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif, 10);
        //    var brush = System.Drawing.Brushes.Black;
        //    var point = new System.Drawing.Point(5, 5);

        //    var entropy = Math.Round(GetEntropy(), 3);

        //    g.DrawString(string.Format("Entropy: {0}", entropy), font, brush, point);

        //    bitmap.Save(filename);
        //}
    }
}
