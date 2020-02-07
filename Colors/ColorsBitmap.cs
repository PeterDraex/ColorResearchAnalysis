using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colors
{
    public class ColorsBitmap<T> where T : Color3d, new()
    {
        public int Width { get; set; }
        public int Height { get; set; }

        private T[,] pixMatrix;
        Bitmap bitmap;

        public ColorsBitmap(string filename)
        {
            bitmap = new Bitmap(filename);
            Initialize(bitmap);            
        }

        public ColorsBitmap(Bitmap bitmap)
        {
            Initialize(bitmap);
        }

        private void Initialize(Bitmap bitmap)
        {
            this.bitmap = bitmap;

            Width = bitmap.Width;
            Height = bitmap.Height;

            pixMatrix = new T[Width, Height];
        }

        public T GetPixel(int x, int y)
        {
            if(pixMatrix[x, y] == null)
            {
                var pixel = bitmap.GetPixel(x, y);

                var color = new T();
                color.SetFromRgb(pixel.R, pixel.G, pixel.B);

                pixMatrix[x, y] = color;
            }

            return pixMatrix[x, y];
        }

        public Bitmap GetRgbBitmap()
        {
            return bitmap;
        }

        public T GetAveragePixel(int x, int y)
        {
            int startX = Math.Max(0, x - 1);
            int endX = Math.Min(x + 1, Width - 1);
            int startY = Math.Max(0, y - 1);
            int endY = Math.Min(y + 1, Height - 1);
            int pixels = (endX - startX + 1) * (endY - startY + 1);

            var avgValues = new int[] { 0, 0, 0 };

            for (int iX = startX; iX <= endX; iX++)
            {
                for (int iY = startY; iY <= endY; iY++)
                {
                    var pixel = GetPixel(iX, iY);

                    for (int i = 0; i < pixel.Spectrums.Length; i++)
                    {
                        avgValues[i] += pixel.Spectrums[i];
                    }
                }
            }

            for (int i = 0; i < avgValues.Length; i++)
            {
                avgValues[i] /= pixels;
            }

            var avgColor = new T();
            avgColor.Set(avgValues);

            return avgColor;
        }
    }
}
