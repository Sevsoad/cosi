using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CosLab1
{
    class ImageFiltrator
    {
        public Bitmap ProcessedImage { get; private set; }

        public ImageFiltrator(Bitmap inputImage)
        {
            ProcessedImage = inputImage;
        }

        public void Binarize()
        {
            try
            {
                const double threshold = 0.6;

                for (var x = 0; x < ProcessedImage.Width; x++)
                {
                    for (var y = 0; y < ProcessedImage.Height; y++)
                    {
                        ProcessedImage.SetPixel(x, y, ProcessedImage.GetPixel(x, y).GetBrightness() < threshold ? Color.Black : Color.White);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ToGrayTone()
        {
            try
            {
                for (var x = 0; x < ProcessedImage.Width; x++)
                {
                    for (var y = 0; y < ProcessedImage.Height; y++)
                    {
                        var currentPixel = ProcessedImage.GetPixel(x, y);

                        var buffer = (int)(0.3 * currentPixel.R + 0.59 * currentPixel.G
                                           + 0.11 * currentPixel.B);

                        ProcessedImage.SetPixel(x, y, Color.FromArgb(buffer, buffer, buffer));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
