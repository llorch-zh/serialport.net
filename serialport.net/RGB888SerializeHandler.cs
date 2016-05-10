using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace mywork_chuan
{
    class RGB888SerializeHandler : ISerializeHandler
    {
        int _pixelWidth;
        public int PixelWidth
        {
            get { return this._pixelWidth; }
        }
        public RGB888SerializeHandler()
        {
            this._pixelWidth = 3;
        }
        public Bitmap BitmapDeSerialize(byte[] data, int width, int height)
        {
            if (data.Length != width * height * _pixelWidth)
                throw new IndexOutOfRangeException();


            Bitmap retBMP = new Bitmap(width, height);
            PointBitmap pBMP = new PointBitmap(retBMP);
            pBMP.LockBits();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = (y * width + x) * _pixelWidth;

                    if ((index < data.GetLowerBound(0)) || (index >= data.GetUpperBound(0)))
                        throw new IndexOutOfRangeException();

                    int r = data[index];
                    int g = data[index + 1];
                    int b = data[index + 2];
                    Color c_x_y = Color.FromArgb(r, g, b);
                    pBMP.SetPixel(x, y, c_x_y);
                }
            }
            pBMP.UnlockBits();
            return retBMP;
        }
        public byte[] BitmapSerialize(Bitmap bmp)
        {
            throw new NotImplementedException();
        }
    }
}
