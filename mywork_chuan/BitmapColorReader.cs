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
            if (rawdata.Length % 2 == 0)
            {
                Color[] ret = new Color[rawdata.Length / 2];
                for (int i = 0; i < ret.Length; i++)
                {
                    int r = (rawdata[2 * i] & 0xF8) >> 3;
                    r = r * 8;
                    int g1 = (rawdata[2 * i] & 0x07) << 3;
                    int g2 = (rawdata[2 * i + 1] & 0xE0) >> 5;
                    int g = g1 | g2;
                    g = g * 4;
                    int b = rawdata[2 * i + 1] & 0x1F;
                    b = b * 8;
                    ret[i] = Color.FromArgb(r, g, b);
                }
                return ret;
            }
            else { throw new ArgumentOutOfRangeException(); }
        }

        public Color[] RGB888ReadHandler(byte[] rawdata)
        {
            if (rawdata.Length % 3 == 0)
            {
                Color[] ret = new Color[rawdata.Length / 3];
                for (int i = 0; i < ret.Length; i++)
                {
                    int r = rawdata[3 * i];
                    int g = rawdata[3 * i + 1];
                    int b = rawdata[3 * i + 2];
                    ret[i] = Color.FromArgb(r, g, b);
                }
                return ret;
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        public Color[] RGBWin24Handler(byte[] rawdata)
        {
            if (rawdata.Length % 4 == 0)
            {
                Color[] ret = new Color[rawdata.Length / 4];
                for (int i = 0; i < ret.Length; i++)
                {
                    int a = rawdata[4 * i];
                    int r = rawdata[4 * i + 1];
                    int g = rawdata[4 * i + 2];
                    int b = rawdata[4 * i + 3];
                    ret[i] = Color.FromArgb(r, g, b);
                }
                return ret;
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        public Color[] TestHandler(byte[] rawdata)
        {
            if (rawdata.Length % 2 == 0)
            {
                Color[] ret = new Color[rawdata.Length / 2];
                for (int i = 0; i < ret.Length; i++)
                {
                    int r = rawdata[2 * i];
                    int g = rawdata[2 * i] * 1 / 3 + rawdata[2 * i + 1] * 2 / 3;
                    int b = rawdata[2 * i + 1];
                    ret[i] = Color.FromArgb(r, g, b);
                }
                return ret;
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }
}
