using Colors;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ColorResearchAnalysis
{
    public class Picture
    {
        public Answer[] Answers;
        private SQLiteConnection dbConnection;

        public Picture(SQLiteConnection dbConnection)
        {
            if(dbConnection.State == ConnectionState.Closed)
                dbConnection.Open();

            this.dbConnection = dbConnection;
        }

        public string Filename { get; private set; }

        /// <summary>
        /// Loads data about one picture from DB
        /// </summary>
        /// <param name="filename">Filename of this picture without path</param>
        /// <param name="circleFilter">Filter for circles</param>
        public void Load(string filename, Answer.Circle.CircleFilter circleFilter = null)
        {
            var command = dbConnection.CreateCommand();
            command.CommandText = "SELECT * FROM data WHERE picture = @picture";
            command.Parameters.AddWithValue("picture", Path.GetFileName(filename));
            var reader = command.ExecuteReader();

            var answers = new List<Answer>();

            foreach (IDataRecord row in reader)
            {
                var a = new Answer(row, circleFilter);
                answers.Add(a);
            }

            Filename = filename;
            Answers = answers.ToArray();
        }

        public class Answer
        {
            public static int SearchDiameter = 70;
            public static int MinimumColorDist = 0;

            public enum SexOptions
            {
                Male,
                Female
            }

            public long Id { get; set; }
            public DateTime Time_submitted { get; set; }
            public string Ip_address { get; set; }
            public long Client_id { get; set; }
            public SexOptions Sex { get; set; }
            public long Age { get; set; }
            public long Reload { get; set; }
            public string Picture { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public double Score { get; set; }
            public long Round_num { get; set; }
            public Circle[] Circles { get; set; }
            public string Changes { get; set; }

            public Answer(IDataRecord row, Circle.CircleFilter circleFilter = null)
            {
                string[] res = ((string)row["resolution"]).Split('x');

                Id = (long)row["id"];
                Time_submitted = (DateTime)row["time_submitted"];
                Ip_address = (string)row["ip_address"];
                Client_id = (long)row["client_id"];
                Sex = (string)row["sex"] == "female" ? SexOptions.Female : SexOptions.Male;
                Age = (long)row["age"];
                Reload = (long)row["reload"];
                Picture = (string)row["picture"];
                Width = int.Parse(res[0]);
                Height = int.Parse(res[1]);
                Score = (double)row["score"];
                Round_num = (long)row["round_num"];
                Circles = Circle.Parse((string)row["colors"], circleFilter, Width, Height);
                Changes = (string)row["changes"];
            }

            public class Circle
            {
                public int X { get; set; }
                public int Y { get; set; }
                public ColorRgb Color { get; set; }
                public int CreationTime { get; set; }
                public int LastEditTime { get; set; }
                public int EntryOrder { get; set; }

                public static int debugCirclesPassed = 0;
                public static int debugCirclesDenied = 0;

                public delegate bool CircleFilter(Circle circle, int index);

                public static Circle[] Parse(string v, CircleFilter filter = null, int PictureWidth = int.MaxValue, int PictureHeight = int.MaxValue)
                {
                    var colorConverter = new ColorConverter();
                    var circles = v.Split(']');
                    var list = new List<Circle>(5);

                    int i = 0;

                    foreach (string circle in circles)
                    {
                        if (circle == "")
                            break;

                        var circleSplit = circle.Trim('\r', '[', ']', ',').Split(',');

                        var c = new Circle()
                        {
                            X = int.Parse(circleSplit[0]),
                            Y = int.Parse(circleSplit[1]),
                            Color = ColorRgb.FromHex(circleSplit[2]),
                            CreationTime = int.Parse(circleSplit[3]),
                            LastEditTime = int.Parse(circleSplit[4]),
                            EntryOrder = i,
                        };

                        // for some reason, there are bad circles in the dataset
                        if (c.X >= 0 && c.Y >= 0 && c.X < PictureWidth && c.Y < PictureHeight)
                        {
                            if (filter == null || filter(c, i))
                            {
                                list.Add(c);
                                debugCirclesPassed++;
                            }

                            i++; // i can be incremented only if it's a valid circle, but must be incremented if a circle was filtered out
                        } else
                        {
                            debugCirclesDenied++;
                        }                            
                    }

                    if(list.Count == 0)
                    {
                        Console.WriteLine($"no circles in string {v}");
                    }
                    
                    return list.ToArray();
                }

                public Point FindMeantPixel<T>(ColorsBitmap<T> bitmap) where T : Color3d, new()
                {
                    int beginX = Math.Max(0, X - SearchDiameter / 2);
                    int beginY = Math.Max(0, Y - SearchDiameter / 2);

                    Point closestPoint = new Point(X, Y);
                    double closestColorDistance = Color.Get4dDistanceL2(bitmap.GetAveragePixel(X, Y), 0);

                    for (int iX = 0; iX < SearchDiameter; iX++)
                    {
                        for (int iY = 0; iY < SearchDiameter; iY++)
                        {

                            int currentPointX = beginX + iX;
                            int currentPointY = beginY + iY;

                            if (currentPointX < 0 || currentPointY < 0 || currentPointX >= bitmap.Width || currentPointY >= bitmap.Height)
                                continue;

                            int distFromPoint = (int)Math.Sqrt((X - currentPointX) * (X - currentPointX) + (Y - currentPointY) * (Y - currentPointY));

                            if (distFromPoint <= SearchDiameter / 2)
                            {
                                var pixel = bitmap.GetAveragePixel(currentPointX, currentPointY);
                                double colorDist = Color.Get4dDistanceL2(pixel, distFromPoint);

                                if (colorDist > MinimumColorDist && colorDist < closestColorDistance)
                                {
                                    closestColorDistance = colorDist;
                                    closestPoint = new Point(currentPointX, currentPointY);
                                }
                            }
                        }
                    }

                    return closestPoint;
                }

                //public unsafe Point FindTarget<T>(ColorsBitmap<T> bitmap) where T : Color3d, new()
                //{
                //    int searchDiameter = 70;

                //    Point closestPoint = default(Point);
                //    double closestColorDistance = double.MaxValue;

                //    int startX = Math.Max(0, X - searchDiameter / 2);
                //    int startY = Math.Max(0, Y - searchDiameter / 2);
                //    int endX = Math.Min(X + searchDiameter / 2, bitmap.Width);
                //    int endY = Math.Min(Y + searchDiameter / 2, bitmap.Height);

                //    // Lock the bitmap's bits.
                //    //var imageSizeRect = new Rectangle(startX, startY, searchDiameter, searchDiameter);
                //    //var bitmapData = bitmap.LockBits(imageSizeRect, ImageLockMode.ReadOnly, bitmap.PixelFormat);

                //    //int bytesPerColor = bitmapData.PixelFormat == PixelFormat.Format32bppArgb ? 4 : 3;

                //    //byte* p = (byte*)bitmapData.Scan0.ToPointer();

                //    for (int y = startY; y < endY; y++)
                //    {
                //        //byte* row = &p[y * bitmapData.Stride];

                //        for (int x = startX; x < endX; x++)
                //        {
                //            //int posInRow = x * bytesPerColor;

                //            //byte Bcolor = row[posInRow];
                //            //byte Gcolor = row[posInRow + 1];
                //            //byte Rcolor = row[posInRow + 2];

                //            //var pixel = Color.FromArgb(Rcolor, Gcolor, Bcolor);
                //            var pixel = bitmap.GetPixel(x, y);
                //            int distFromPoint = (int)Math.Sqrt((X - x) * (X - x) + (Y - y) * (Y - y));

                //            if (distFromPoint <= searchDiameter)
                //            {
                //                Debug.Assert(bitmap.GetPixel(x, y) == pixel);

                //                double colorDist = Color.GetDistanceColorSqPositionLin(pixel, distFromPoint);

                //                if (colorDist < closestColorDistance)
                //                {
                //                    closestColorDistance = colorDist;
                //                    closestPoint = new Point(x, y);
                //                }
                //            }
                //        }
                //    }

                //    // Unlock the bits.
                //    //bitmap.UnlockBits(bitmapData);

                //    return closestPoint;
                //}
            }

            public void Draw<T>(Bitmap bitmapToDraw, ColorsBitmap<T> bitmapToSearchTarget, bool showRadius, bool showTarget, bool showOrderOfCircles) where T : Color3d, new()
            {
                var g = Graphics.FromImage(bitmapToDraw);

                foreach (Circle c in Circles)
                {
                    if (showTarget)
                    {
                        var target = c.FindMeantPixel(bitmapToSearchTarget);
                                                 
                        var pen2 = new Pen(Color.LightSkyBlue, 3);
                        g.DrawLine(pen2, c.X, c.Y, target.X, target.Y);
                    }

                    var pen = new Pen((Color)c.Color.ConvertToRgb(), 5);
                    g.DrawEllipse(pen, c.X - 5, c.Y - 5, 10, 10);

                    if(showOrderOfCircles)
                    {
                        var brush = Brushes.Coral;
                        var font = new Font(FontFamily.GenericSansSerif, 6, FontStyle.Bold);
                        g.DrawString(c.EntryOrder.ToString(), font, brush, c.X, c.Y);
                    }

                    if (showRadius) {
                        var pen2 = new Pen(Color.GreenYellow, 1);
                        g.DrawEllipse(pen2, c.X - SearchDiameter/2, c.Y - SearchDiameter/2, SearchDiameter, SearchDiameter);
                    }                    
                }
            }
        }
    }
}
