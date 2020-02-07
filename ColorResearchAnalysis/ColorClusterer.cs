using Colors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorResearchAnalysis
{
    public class ColorClusterer<T> where T : Color3d, new()
    {
        int MaxAllowedDistance = 32;
        Bitmap image;
        
        Color?[,] pixelMap;
        Dictionary<Color, int> clusters;

        /// <param name="frame"></param>
        /// <param name="MaxAllowedDistance">Maximum distance of colors in one cluster. If 0, default value will be used.</param>
        public ColorClusterer(ColorsBitmap<T> image, int MaxAllowedDistance = 0)
        {
            this.image = image.GetRgbBitmap();

            if (MaxAllowedDistance > 0)
                this.MaxAllowedDistance = MaxAllowedDistance;
        }

        public Bitmap ClustersToBitmap()
        {
            var bitmap = new Bitmap(pixelMap.GetLength(0), pixelMap.GetLength(1));

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Color color = pixelMap[x, y].Value;
                    color = Color.FromArgb(
                        Math.Min(255, color.R + MaxAllowedDistance / 2),
                        Math.Min(255, color.G + MaxAllowedDistance / 2),
                        Math.Min(255, color.B + MaxAllowedDistance / 2)
                    );

                    bitmap.SetPixel(x, y, color);
                }
            }

            return bitmap;
        }

        public unsafe void LoadClusters()
        {
            pixelMap = new Color?[image.Width, image.Height];
            clusters = new Dictionary<Color, int>();

            var imageSizeRect = new Rectangle(0, 0, image.Width, image.Height);

            // Lock the bitmap's bits.
            var bitmapData = image.LockBits(imageSizeRect, System.Drawing.Imaging.ImageLockMode.ReadOnly, image.PixelFormat);
            int bytesPerColor = bitmapData.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppArgb ? 4 : 3;

            byte* p = (byte*)bitmapData.Scan0.ToPointer();

            for (int y = 0; y < image.Height; y++)
            {
                byte* row = &p[y * bitmapData.Stride];

                for (int i = 0; i <= bitmapData.Stride - bytesPerColor; i += bytesPerColor)
                {
                    byte Rcolor = row[i + 2];
                    byte Gcolor = row[i + 1];
                    byte Bcolor = row[i];

                    var pixel = Color.FromArgb(Rcolor, Gcolor, Bcolor);
                    var cluster = AddToCluster(pixel);
                    pixelMap[i / bytesPerColor, y] = cluster;
                }
            }

            // Unlock the bits.
            image.UnlockBits(bitmapData);
        }

        private Color AddToCluster(Color color)
        {
            // get color of cluster
            int r = color.R / MaxAllowedDistance * MaxAllowedDistance;
            int g = color.G / MaxAllowedDistance * MaxAllowedDistance;
            int b = color.B / MaxAllowedDistance * MaxAllowedDistance;

            var cluster = Color.FromArgb(r, g, b);

            // update dictionary
            bool found = clusters.TryGetValue(cluster, out int pixelsInCluster);

            if (!found)
                pixelsInCluster = 0;

            clusters[cluster] = ++pixelsInCluster;

            return cluster;
        }
    }
}
