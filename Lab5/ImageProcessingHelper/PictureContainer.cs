// -----------------------------------------------------------------------
// <copyright file="PictureContainer.cs" company="BSUIR">
// Pavel Torchilov
// Sergey Belko
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Drawing;

namespace ImageProcessingHelper
{
    /// <summary>
    /// Class for store pictures in bitmap
    /// </summary>
    public class PictureContainer
    {
        /// <summary>
        /// Gets the Picture.
        /// </summary>
        /// <value>
        /// The Picture.
        /// </value>
        public Bitmap Picture { get; private set; }

        /// <summary>
        /// The size of Picture
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PictureContainer" /> class.
        /// </summary>
        /// <param name="picture">The path.</param>
        /// <param name="size">The size.</param>
        public PictureContainer(Bitmap picture, int size)
        {
            Picture = picture;
            Size = size;
        }

        public double[] ConvertToVector()
        {
            var vectorLength = Size * Size;

            var vector = new double[vectorLength];

            for (var i = 0; i < Size; i++)
            {
                for (var j = 0; j < this.Size; j++)
                {
                    var pixel = Picture.GetPixel(i, j);

                    if (pixel.R > 250)
                    {
                        vector[i * Size + j] = -1;
                    }
                    else
                    {
                        vector[i * Size + j] = 1;
                    }
                }
            }

            return vector;
        }
    }
}