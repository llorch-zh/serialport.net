using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mywork_chuan
{
    class BitmapSerializer
    {
        public static System.Drawing.Bitmap DeSerialize(byte[] data, int offset,int width,int height, BitmapDeSerializeDelegate deserialize)
        {
            byte[] byteData = new byte[data.Length-offset];
            System.Buffer.BlockCopy(data,offset,byteData,0,byteData.Length);
            return deserialize(byteData,width,height);
        }


        public static byte[] Serialize(System.Drawing.Bitmap bmp, BitmapSerializeDelegate serialize)
        {
            return serialize(bmp);
        }
    }
}
