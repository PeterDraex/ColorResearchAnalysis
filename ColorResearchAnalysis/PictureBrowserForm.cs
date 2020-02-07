using Colors;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorResearchAnalysis
{
    public class PictureBrowserForm : Form
    {
        string currentDir;
        protected string[] filesInCurrentDir;
        int currentFileIndex;

        public string CurrentPicturePath { get { return filesInCurrentDir[currentFileIndex]; } }

        public virtual PictureBox MainPictureBox { get => throw new NotImplementedException(); }

        protected void Form_Load(object sender, EventArgs e)
        {
            var folders = new[] { "dataset", "../dataset", "../../dataset", "../../../dataset" };

            foreach (var folder in folders)
            {
                if (Directory.Exists(folder))
                {
                    OpenFile(Directory.GetFiles(folder, "*.jpg").First());
                    ShowNewPicture();

                    break;
                }
            }
        }

        public virtual void ShowPicture()
        {
            var bitmap = Image.FromFile(CurrentPicturePath);

            MainPictureBox.Image = bitmap;
            MainPictureBox.Width = bitmap.Width;
            MainPictureBox.Height = bitmap.Height;
        }

        public virtual void ShowNewPicture()
        {
            ShowPicture();
        }

        protected void pictureBox_Click(object sender, EventArgs e)
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Files|*.jpg;*.jpeg;*.png";
            fileDialog.FileOk += (sender2, e2) =>
            {
                OpenFile(fileDialog.FileName);
                ShowPicture();
            };

            fileDialog.ShowDialog();
        }

        private void OpenFile(string file)
        {
            currentDir = Path.GetDirectoryName(file);
            filesInCurrentDir = Directory.GetFiles(currentDir, "*.jpg");

            for (int i = 0; i < filesInCurrentDir.Length; i++)
            {
                if (filesInCurrentDir[i] == file)
                {
                    currentFileIndex = i;
                    break;
                }
            }
        }

        protected void buttonNext_Click(object sender, EventArgs e)
        {
            if (currentFileIndex < filesInCurrentDir.Length - 1)
            {
                currentFileIndex++;
                ShowNewPicture();
            }
        }

        protected void buttonPrevious_Click(object sender, EventArgs e)
        {
            if (currentFileIndex > 0)
            {
                currentFileIndex--;
                ShowNewPicture();
            }
        }
    }

}
