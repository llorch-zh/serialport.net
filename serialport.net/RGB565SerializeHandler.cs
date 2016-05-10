using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace mywork_chuan
{
    class RGB565SerializeHandler:ISerializeHandler
    {       
        int _pixelWidth;
        public int PixelWidth
        {
            get { return this._pixelWidth; }
        }
        public RGB565SerializeHandler()
        {
            this._pixelWidth = 2;
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

                    int r = data[index] & 0xF8;
                    int g1 = data[index] & 0x07;
                    int g2 = data[index] & 0xE0;
                    int g = (g1 << 3) | (g2 >> 5);
                    g = g * 2;
                    int b = data[index + 1] & 0x1F;
                    b = b * 3;
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
