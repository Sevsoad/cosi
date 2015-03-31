using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace CosLab1
{
    class ConnectedField
    {
        public int YStart { get; private set; }
        public int YEnd { get; private set; }
        public int XStart { get; private set; }
        public int XEnd { get; private set; }

        public int ClasterNumber { get; set; }
        public int Area { get; set; }
        public float WeightCenterX { get; set; }
        public float WeightCenterY { get; set; }
        public int Perimeter { get; set; }
        public double Density { get; set; }
        public double Elongation { get; set; }
        public double MainAxisOrientation { get; set; }

        public ConnectedField(int yS, int yE, int xS, int xE)
        {
            YStart = yS;
            YEnd = yE;
            XStart = xS;
            XEnd = xE;
        }

        public ConnectedField() { }

        public void CalculateArea(ref int[,] imageMap)
        {
            var area = 0;

            for (var i = XStart; i < XEnd; i++)
            {
                for (var j = YStart; j < YEnd; j++)
                {
                    if (imageMap[i, j] == 1)
                    {
                        area++;
                    }
                }
            }

            Area = area;
        }

        public void CalculateWeightCenters(ref int[,] imageMap)
        {
            if (Area == 0)
            {
                throw new ArgumentException("First calculate area of object");
            }

            var tempX = 1;
            var tempY = 1;

            for (var i = XStart; i < XEnd; i++)
            {
                for (var j = YStart; j < YEnd; j++)
                {
                    if (imageMap[i, j] != 1) continue;

                    tempX += i - XStart;
                    tempY += j - YStart;
                }
            }

            // ReSharper disable once PossibleLossOfFraction
            WeightCenterX = tempX / Area;

            // ReSharper disable once PossibleLossOfFraction
            WeightCenterY = tempY / Area;
        }

        public void CalculatePerimeter(Bitmap processedImage)
        {
            var perimeterBuf = 0;
            var xs = XStart;
            var ys = YStart;

            for (var i = xs; i < XEnd - 1; i++)
            {
                for (var j = ys; j < YEnd - 1; j++)
                {
                    if ((processedImage.GetPixel(i - 1, j).R > 250 ||
                        processedImage.GetPixel(i + 1, j).R > 250
                        || processedImage.GetPixel(i, j - 1).R > 250 ||
                        processedImage.GetPixel(i, j + 1).R > 250) &&
                        processedImage.GetPixel(i, j).R < 250)
                    {
                        perimeterBuf++;
                    }
                }
            }

            Perimeter = perimeterBuf;
        }

        public void CalculateDensity(ref int[,] imageMap)
        {
            Density = Math.Pow(Perimeter, 2) / Area;
        }

        public void CalculateElongationAndMainAxisOrientation(ref int[,] imageMap)
        {
            double m11 = 0;
            double m20 = 0;
            double m02 = 0;

            for (var i = XStart; i < XEnd; i++)
            {
                for (var j = YStart; j < YEnd; j++)
                {
                    if (imageMap[i, j] != 1) continue;

                    m11 += (i - WeightCenterX - XStart) *
                        (j - WeightCenterY - YStart);
                    m20 += Math.Pow((i - WeightCenterX - XStart), 2);
                    m02 += Math.Pow((j - WeightCenterY - YStart), 2);
                }
            }

            Elongation = (m20 + m02 + Math.Pow(Math.Pow(m20 - m02, 2) +
                                                     4 * Math.Pow(m11, 2), 0.5)) /
                               (m20 + m02 - Math.Pow(Math.Pow(m20 - m02, 2) +
                                                     4 * Math.Pow(m11, 2), 0.5));

            MainAxisOrientation = 0.5 * Math.Atan(2 * m11 / (m20 - m02));
        }

        public void SetRandomValuesToField(int key)
        {
            if (key == 1)
            {
                Area = 1;
                Perimeter = 1;
                Density = 1;
                Elongation = 1;
            }
            if (key == 2)
            {
                Area = 2;
                Perimeter = 2;
                Density = 2;
                Elongation = 2;
            }
        }
    }
}
