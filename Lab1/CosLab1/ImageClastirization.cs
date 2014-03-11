using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace CosLab1
{
    class ImageClastirization
    {
        private readonly int[,] imageMap;
        private int xMapPosition;
        private int yMapPosition;
        private List<ConnectedField> connectedFields;
        private Dictionary<int, ConnectedField> clasters;
        private int yMaxBuf;
        private int xMinBuf;
        private int xMaxBuf;
        private bool clasterizationReady;

        private int num = 0;

        private class ConnectedField
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

            public ConnectedField()
            {}
        }

        public Bitmap Image { get; private set; }

        public ImageClastirization(string fileName)
        {
            Image = new Bitmap(fileName);
            imageMap = new int[Image.Width, Image.Height];
            connectedFields = new List<ConnectedField>();
            clasters = new Dictionary<int, ConnectedField>(2) 
            {{1, new ConnectedField()}, {2, new ConnectedField()}};
        }

        private void FindConnectedFields()
        {
            for (yMapPosition = 0; yMapPosition < Image.Height; yMapPosition++)
            {
                for (xMapPosition = 0; xMapPosition < Image.Width; xMapPosition++)
                {
                    xMinBuf = xMapPosition;
                    xMaxBuf = xMapPosition;
                    yMaxBuf = yMapPosition;

                    if (!RecursiveSearch(xMapPosition, yMapPosition) ||
                        (xMaxBuf - xMinBuf < 5)) continue;

                    connectedFields.Add(new ConnectedField(yMapPosition, yMaxBuf, xMinBuf,
                        xMaxBuf));
                }
            }
        }

        private Boolean RecursiveSearch(int x, int y)
        {
            if ((imageMap[x, y] == 1) ||
                (Image.GetPixel(x, y).R < 250)) return false;

            imageMap[x, y] = 1;

            if (y > yMaxBuf)
            {
                yMaxBuf = y;
            }
            if (x > xMaxBuf)
            {
                xMaxBuf = x;
            }
            if (x < xMinBuf)
            {
                xMinBuf = x;
            }

            if (x > 0)
            {
                RecursiveSearch(x-1, y);
            }
            if (x < Image.Width - 1)
            {
                RecursiveSearch(x+1, y);
            }
            if (y > 0)
            {
                RecursiveSearch(x, y-1);
            }
            if (y < Image.Height - 1)
            {
                RecursiveSearch(x, y+1);
            }

            return true;
        }

        public void ToGrayTone()
        {
            try
            {
                for (var x = 0; x < Image.Width; x++)
                {
                    for (var y = 0; y < Image.Height; y++)
                    {
                        var currentPixel = Image.GetPixel(x, y);

                        var buffer = (int)(0.3 * currentPixel.R + 0.59 * currentPixel.G
                                           + 0.11 * currentPixel.B);

                        Image.SetPixel(x, y, Color.FromArgb(buffer, buffer, buffer));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void Binarize()
        {
            try
            {
                const double threshold = 0.6;

                for (var x = 0; x < Image.Width; x++)
                {
                    for (var y = 0; y < Image.Height; y++)
                    {
                         Image.SetPixel(x, y, Image.GetPixel(x, y).GetBrightness() < threshold ? Color.Black : Color.White);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void Clastiraize()
        {
            var t = new Thread(FindConnectedFields, 4000000);
            t.Start();
            t.Join();

            CalculateParameters();
            num = 0;
            CalculateKMeans();
            PaintFields();
        }

        private void PaintFields()
        {
            foreach (var i in connectedFields)
            {
                for (var x = i.XStart; x < i.XEnd+1; x++)
                {
                    for (var y = i.YStart; y < i.YEnd+1; y++)
                    {
                        if (imageMap[x, y] == 1)
                        {
                            Image.SetPixel(x, y,
                                i.ClasterNumber == 1 ? Color.FromArgb(140, 10, 140) : Color.FromArgb(40, 190, 180));
                        }
                    }
                }
            }
        }

        private void CalculateKMeans()
        {
            foreach (var i in clasters)
            {
                SetRandomValuesToField(i.Value, i.Key);
            }

            num++;

            foreach (var i in connectedFields)
            {
                DetermineClaster(i);
            }

            while (! clasterizationReady)
            {
                clasterizationReady = true;

                num++;

                foreach (var i in clasters)
                {
                    UpdateClastersCenter(i.Value, i.Key);
                }
                
                foreach (var i in connectedFields)
                {
                    DetermineClaster(i);
                }
            }

            MessageBox.Show("Number of iterations = " +  num);
        }

        private void UpdateClastersCenter(ConnectedField claster, int clasterNumber)
        {
            var clasterFields = 0;
            var tempArea = 0;
            var tempPerimeter = 0;
            var tempDensity = 0d;
            var tempElongation = 0d;

            foreach (var i in connectedFields)
            {
                if (i.ClasterNumber != clasterNumber) continue;

                clasterFields++;
                tempArea += i.Area;
                tempPerimeter += i.Perimeter;
                tempDensity += i.Density;
                tempElongation += i.Elongation;
            }

            if (clasterFields == 0) return;

            claster.Area = tempArea / clasterFields;
            claster.Perimeter = tempPerimeter / clasterFields;
            claster.Density = tempDensity / clasterFields;
            claster.Elongation = tempElongation / clasterFields;

        }

        private void DetermineClaster(ConnectedField field)
        {
            var cl1 = clasters[1];
            var cl2 = clasters[2];

            var euclidMetric1 = Math.Pow(Math.Pow(field.Area - cl1.Area, 2) +
                       Math.Pow(field.Perimeter - cl1.Perimeter, 2) +
                       Math.Pow(field.Density - cl1.Density, 2) +
                       Math.Pow(field.Elongation - cl1.Elongation, 2), 0.5);
            var euclidMetric2 = Math.Pow(Math.Pow(field.Area - cl2.Area, 2) +
                       Math.Pow(field.Perimeter - cl2.Perimeter, 2) +
                       Math.Pow(field.Density - cl2.Density, 2) +
                       Math.Pow(field.Elongation - cl2.Elongation, 2), 0.5);

            var clasterNumber = euclidMetric1 < euclidMetric2 ? 1 : 2;

            if (field.ClasterNumber != clasterNumber)
            {
                clasterizationReady = false;
            }

            field.ClasterNumber = clasterNumber;
        }

        private static void SetRandomValuesToField(ConnectedField field, int key)
        {
            if (key == 1)
            {
                field.Area = 1;
                field.Perimeter = 1;
                field.Density = 1;
                field.Elongation = 1;
            }
            if (key == 2)
            {
                field.Area = 2;
                field.Perimeter = 2;
                field.Density = 2;
                field.Elongation = 2;
            }
        }

        private void CalculateParameters()
        {
            foreach (var x in connectedFields)
            {
                CalculateArea(x);
                CalculateWeightCenters(x);
                CalculatePerimeter(x);
                CalculateDensity(x);
                CalculateElongationAndMainAxisOrientation(x);
            }
        }

        private void CalculateArea(ConnectedField field)
        {
            var area = 0;

            for (var i = field.XStart; i < field.XEnd; i++)
            {
                for (var j = field.YStart; j < field.YEnd; j++)
                {
                    if (imageMap[i, j] == 1)
                    {
                        area++;
                    }
                }
            }

            field.Area = area;
        }

        private void CalculateWeightCenters(ConnectedField field)
        {
            if (field.Area == 0)
            {
                throw new ArgumentException("First calculate area of object");
            }

            var tempX = 1;
            var tempY = 1;

            for (var i = field.XStart; i < field.XEnd; i++)
            {
                for (var j = field.YStart; j < field.YEnd; j++)
                {
                    if (imageMap[i, j] != 1) continue;

                    tempX += i - field.XStart;
                    tempY += j - field.YStart;
                }
            }

            // ReSharper disable once PossibleLossOfFraction
            field.WeightCenterX = tempX / field.Area;

            // ReSharper disable once PossibleLossOfFraction
            field.WeightCenterY = tempY / field.Area;
        }

        private void CalculatePerimeter(ConnectedField field)
        {
            var perimeterBuf = 0;
            var xs = field.XStart;
            var ys = field.YStart;

            for (var i = xs; i < field.XEnd - 1; i++)
            {
                for (var j = ys; j < field.YEnd - 1; j++)
                {
                    if ((Image.GetPixel(i - 1, j).R > 250 ||
                        Image.GetPixel(i + 1, j).R > 250
                        || Image.GetPixel(i, j - 1).R > 250 ||
                        Image.GetPixel(i, j + 1).R > 250) &&
                        Image.GetPixel(i, j).R < 250)
                    {
                        perimeterBuf++;
                    }
                }
            }

            field.Perimeter = perimeterBuf;
        }

        private static void CalculateDensity(ConnectedField field)
        {

            field.Density = Math.Pow(field.Perimeter, 2)/field.Area;

        }

        private void CalculateElongationAndMainAxisOrientation(ConnectedField field)
        {
            double m11 = 0;
            double m20 = 0;
            double m02 = 0;

            for (var i = field.XStart; i < field.XEnd; i++)
            {
                for (var j = field.YStart; j < field.YEnd; j++)
                {
                    if (imageMap[i, j] != 1) continue;

                    m11 += (i - field.WeightCenterX - field.XStart) *
                        (j - field.WeightCenterY - field.YStart);
                    m20 += Math.Pow((i - field.WeightCenterX - field.XStart), 2);
                    m02 += Math.Pow((j - field.WeightCenterY - field.YStart), 2);
                }
            }

            field.Elongation = (m20 + m02 + Math.Pow(Math.Pow(m20 - m02, 2) +
                                                     4*Math.Pow(m11, 2), 0.5))/
                               (m20 + m02 - Math.Pow(Math.Pow(m20 - m02, 2) +
                                                     4*Math.Pow(m11, 2), 0.5));

            field.MainAxisOrientation = 0.5*Math.Atan(2*m11/(m20 - m02));
        }

    }


}
