using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace CosLab1
{
    static class ClastirizationHelper
    {
        public static void PaintFields(List<ConnectedField> connectedFields, ref int[,] imageMap, Bitmap ProcessedImage)
        {
            foreach (var i in connectedFields)
            {
                for (var x = i.XStart; x < i.XEnd + 1; x++)
                {
                    for (var y = i.YStart; y < i.YEnd + 1; y++)
                    {
                        if (imageMap[x, y] == 1)
                        {
                            ProcessedImage.SetPixel(x, y,
                                i.ClasterNumber == 1 ? Color.FromArgb(140, 10, 140) : Color.FromArgb(40, 190, 180));
                        }
                    }
                }
            }
        }

        public static void CalculateParameters(List<ConnectedField> connectedFields, ref int[,] imageMap, Bitmap ProcessedImage)
        {
            foreach (var x in connectedFields)
            {
                x.CalculateArea(ref imageMap);
                x.CalculateWeightCenters(ref imageMap);
                x.CalculatePerimeter(ProcessedImage);
                x.CalculateDensity(ref imageMap);
                x.CalculateElongationAndMainAxisOrientation(ref imageMap);
            }
        }
    }
}
