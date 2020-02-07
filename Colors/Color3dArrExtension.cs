using Colors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colors
{
    public static class Extension
    {
        public static Color3d GetAveragePixel(this Color3d[,] img, int x, int y)
        {
            int startX = Math.Max(0, x - 1);
            int endX = Math.Min(x + 1, img.GetLength(0) - 1);
            int startY = Math.Max(0, y - 1);
            int endY = Math.Min(y + 1, img.GetLength(1) - 1);
            int pixels = (endX - startX + 1) * (endY - startY + 1);

            var avgValues = new int[] { 0, 0, 0 };

            for (int iX = startX; iX <= endX; iX++)
            {
                for (int iY = startY; iY <= endY; iY++)
                {
                    var pixel = img[iX, iY];

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

            var avgColor = img[x, y].GetCopy();
            avgColor.Set(avgValues);

            return avgColor;
        }
    }
}
