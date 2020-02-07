using Colors;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GenerateHistogramsRankingModels
{
    public class ThumbnailPrep<ColorType> : ImageWarehouse.Preprocessor where ColorType:Color3d, new()
    {
        public override string Name => "Width" + TargetWidth;

        public int TargetWidth { get; private set; }

        public ThumbnailPrep(int targetWidth)
        {
            this.TargetWidth = targetWidth;
        }

        private Bitmap GetImageHiQualityResized(Image image, int targetWidth, int targetHeight)
        {
            var thumb = new Bitmap(targetWidth, targetHeight);
            using (var g = Graphics.FromImage(thumb))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(image, new Rectangle(0, 0, thumb.Width, thumb.Height));
                return thumb;
            }
        }

        public override unsafe Color3d[,] Preprocess(Bitmap bitmap)
        {
            var targetHeight = (int)(bitmap.Height / ((double)bitmap.Width / TargetWidth));
            var smallBitmap = GetImageHiQualityResized(bitmap, TargetWidth, targetHeight);

            Color3d[,] img = new Color3d[TargetWidth, targetHeight];

            for (int x = 0; x < TargetWidth; x++)
            {
                for (int y = 0; y < targetHeight; y++)
                {
                    var pixel = smallBitmap.GetPixel(x, y);

                    img[x, y] = new ColorType();
                    img[x, y].SetFromRgb(pixel.R, pixel.G, pixel.B);
                }
            }

            return img;
        }
    }
}