using Colors;

namespace GenerateHistogramsRankingModels
{
    public class VoidPrep<ColorType> : ImageWarehouse.Preprocessor where ColorType : Colors.Color3d, new()
    {
        public override string Name { get => "Unchanged" + typeof(ColorType).Name; }

        public override unsafe Color3d[,] PreprocessRaw(int Width, int Height, byte* BitmapPointer, int BytesPerPixel)
        {
            Colors.Color3d[,] image = new Colors.Color3d[Width, Height];
            var stride = Width * BytesPerPixel;

            // Read bitmap and insert it to image
            for (int y = 0; y < Height; y++)
            {
                byte* row = &BitmapPointer[y * stride];

                for (int i = 0; i <= stride - BytesPerPixel; i += BytesPerPixel)
                {
                    byte Rcolor = row[i + 2];
                    byte Gcolor = row[i + 1];
                    byte Bcolor = row[i];

                    var pixel = new ColorType();
                    pixel.SetFromRgb(Rcolor, Gcolor, Bcolor);

                    image[i / BytesPerPixel, y] = pixel;
                }
            }

            return image;
        }
    }
}
