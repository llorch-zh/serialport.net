using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace mywork_chuan
{
    class RawRender
    {
        public void SimpleRGB(Bitmap bmp,byte[] data)
        {
            PointBitmap pb = new PointBitmap(bmp);
            pb.LockBits();
            for (int i = 0; i < data.Length; i += 2)
            {
                int x = (i % 640) / 2;
                int y = i / 640;

                int red = data[i] * 4 > 255 ? data[i] : 255;
                int green = data[i + 1] * 4 > 255 ? data[i + 1] : 255;
                int blue = red / 3 + green * 2 / 3;
                //int red = (Global.DATA[i] & 0xF8) >> 3;
                //int green = ((Global.DATA[i] & 0x07) << 3) | ((Global.DATA[i + 1] & 0xE0) >> 5);
                //int blue = Global.DATA[i + 1] & 0x1F;
                Color c = Color.FromArgb(red, green, blue);
                pb.SetPixel(x, y, c);
            }
            pb.UnlockBits();
        }

    }
}
