using System.Drawing;

namespace mywork_chuan
{
    delegate Bitmap BitmapDeSerializeDelegate(byte[] data,int width,int height);
    delegate byte[] BitmapSerializeDelegate(Bitmap bmp);
}
