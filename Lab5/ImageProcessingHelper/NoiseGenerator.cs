// -----------------------------------------------------------------------
// <copyright file="NoiseGenerator.cs" company="BSUIR">
// Pavel Torchilov
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Drawing;

namespace ImageProcessingHelper
{
    /// <summary>
    /// Class for generate noise on picture.
    /// </summary>
    public static class NoiseGenerator
    {
        /// <summary>
        /// The random
        /// </summary>
        private static readonly Random Random = new Random();

        /// <summary>
        /// The max value of RGB.
        /// </summary>
        private const int RGBMAX = 255;


        /// <summary>
        /// Generates the specified picture.
        /// </summary>
        /// <param name="picture">The picture.</param>
        /// <param name="noiseLevel">The noise level.</param>
        /// <returns></returns>
        public static PictureContainer Generate(PictureContainer picture, int noiseLevel)
        {
            int randomValue;
            Color pixel;

            for (var i = 0; i < picture.Size; i++)
            {
                for (var j = 0; j < picture.Size; j++)
                {
                    pixel = picture.Picture.GetPixel(i, j);

                    randomValue = Random.Next(0, 100);

                    if (randomValue < noiseLevel)
                    {
                        picture.Picture.SetPixel(i, j, InvertPixel(pixel));
                    }
                }
            }

            return picture;
        }

        /// <summary>
        /// Inverts the pixel.
        /// </summary>
        /// <param name="pixel">The pixel.</param>
        /// <returns></returns>
        private static Color InvertPixel(Color pixel)
        {
            return Color.FromArgb(RGBMAX - pixel.R, RGBMAX - pixel.G, RGBMAX - pixel.B);
        }

    }
    
}