using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mywork_chuan
{
    interface ISerializeHandler
    {
        int PixelWidth { get; }
        System.Drawing.Bitmap BitmapDeSerialize(byte[] data, int width, int height);
        byte[] BitmapSerialize(System.Drawing.Bitmap bmp);
    }
}
