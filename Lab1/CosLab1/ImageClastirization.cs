using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace CosLab1
{
    class ImageClastirization
    {

       
        private List<ConnectedField> connectedFields;
        private Dictionary<int, ConnectedField> clasters;        
        private bool clasterizationReady;
        private int[,] imageMap;
        private int num = 0;

        public Bitmap ProcessedImage { get; private set; }

        public ImageClastirization(Bitmap inputImage)
        {
            ProcessedImage = inputImage;
            imageMap = new int[ProcessedImage.Width, ProcessedImage.Height];
            clasters = new Dictionary<int, ConnectedField>(2) { { 1, new ConnectedField() }, { 2, new ConnectedField() } };
        }

      
        public void Clastiraize()
        {
            var filedsFinder = new ConnectedFieldsFinder();
            connectedFields = filedsFinder.Find(ProcessedImage, ref imageMap);

            ClastirizationHelper.CalculateParameters(connectedFields, ref imageMap, ProcessedImage);
            num = 0;
            CalculateKMeans();
            ClastirizationHelper.PaintFields(connectedFields, ref imageMap, ProcessedImage);
        }       

        private void CalculateKMeans()
        {
            foreach (var i in clasters)
            {
                i.Value.SetRandomValuesToField(i.Key);
            }

            num++;

            foreach (var i in connectedFields)
            {
                DetermineClaster(i);
            }

            while (!clasterizationReady)
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

            MessageBox.Show("Number of iterations = " + num);
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

        
    }
}
