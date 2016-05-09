using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace mywork_chuan
{
    /// <summary>
    /// holder of  implementions of delegate ByteArrayToColorArrayDelegate
    /// </summary>
    class BitmapColorReader
    {
        public Color[] RGB565ReadHandler(byte[] rawdata)
        {
            throw new NotImplementedException();
        }

        public Color[] RGB888ReadHandler(byte[] rawdata)
        {
            throw new NotImplementedException();
        }

        public Color[] TestHandler(byte[] rawdata)
        {
            Color[] ret = new Color[rawdata.Length/2];
            for (int i = 0; i < ret.Length; i++)
            {
                int r = rawdata[2*i];
                int g = rawdata[2*i] * 1 / 3+rawdata[2*i+1]*2/3;
                int b = rawdata[2*i+1];
                ret[i] = Color.FromArgb(r,g,b);
            }
            return ret;
        }
    }
}
