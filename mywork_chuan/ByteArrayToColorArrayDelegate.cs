using System.Drawing;

namespace mywork_chuan
{
    delegate  void ByteArrayRenderDelegate(Bitmap bmp,byte[] data);

    delegate Color[] ByteArrayToColorArrayDelegate(byte[] raw);
    delegate byte[]  ColorArrayToByteArrayDelegate(Color[] colors);
}
