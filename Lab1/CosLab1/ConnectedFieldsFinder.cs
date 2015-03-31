using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;

namespace CosLab1
{
    class ConnectedFieldsFinder
    {
        private int xMapPosition;
        private int yMapPosition;
        private int yMaxBuf;
        private int xMinBuf;
        private int xMaxBuf;
        private Bitmap processedImage;
        private List<ConnectedField> connectedFields;
        private int[,] imageMap;

        public ConnectedFieldsFinder()
        {
            connectedFields = new List<ConnectedField>();
        }

        public List<ConnectedField> Find(Bitmap processedImage, ref int[,] imageMap)
        {
            this.processedImage = processedImage;
            //imageMap = new int[processedImage.Width, processedImage.Height];            
            this.imageMap = imageMap;

            var t = new Thread(FindConnectedFileds, 4000000);
            t.Start();
            t.Join();

            return connectedFields;
        }
        

        public void  FindConnectedFileds()
        {
            for (yMapPosition = 0; yMapPosition < processedImage.Height; yMapPosition++)
            {
                for (xMapPosition = 0; xMapPosition < processedImage.Width; xMapPosition++)
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
                (processedImage.GetPixel(x, y).R < 250)) return false;

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
                RecursiveSearch(x - 1, y);
            }
            if (x < processedImage.Width - 1)
            {
                RecursiveSearch(x + 1, y);
            }
            if (y > 0)
            {
                RecursiveSearch(x, y - 1);
            }
            if (y < processedImage.Height - 1)
            {
                RecursiveSearch(x, y + 1);
            }

            return true;
        }

    }
}
