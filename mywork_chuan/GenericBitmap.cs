using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace mywork_chuan
{
    /// <summary>
    /// bitmap as color array
    /// </summary>
    class GenericBitmap
    {
        Color[] _colorData;
        /// <summary>
        /// Color data
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Color this[int index]
        {
            get { return this._colorData[index]; }
            set { this._colorData[index] = value; }
        }

        int _width;
        /// <summary>
        /// the width.
        /// </summary>
        public int Width
        {
            get { return this._width; }
            set { this._width = value; }
        }

        int _height;
        /// <summary>
        /// the height.
        /// </summary>
        public int Height
        {
            get { return this._height; }
            set { this._height = value; }
        }

        /// <summary>
        /// read from a byte array with a handler assigned
        /// </summary>
        /// <param name="readerHandler"></param>
        /// <param name="rawdata"></param>
        public void ReadFromByteArray(ByteArrayToColorArrayDelegate readerHandler, byte[] rawdata)
        {
            // todo: imp
            if (readerHandler == null || rawdata == null)
                throw new ArgumentNullException();

            this._colorData = readerHandler(rawdata);
        }

        /// <summary>
        /// write to a byte array with a handler assigned
        /// </summary>
        /// <param name="writerHandler"></param>
        /// <param name="rawdata"></param>
        public void WriteToByteArray(ColorArrayToByteArrayDelegate writerHandler, byte[] rawdata)
        {
            // todo: imp
            if (writerHandler == null || rawdata == null)
                throw new ArgumentNullException();

            if (this._colorData == null)
                throw new NullReferenceException();

            rawdata = writerHandler(this._colorData);
        }


        /// <summary>
        /// Get a System.Drawing.Bitmap from GenericBitmap
        /// </summary>
        /// <returns>result bitmap</returns>
        public Bitmap ToBitmap()
        {
            // todo :imp

            // check failure
            if (this._colorData == null || this._width == 0 || this._height == 0)
                throw new ArgumentNullException();

            if (this._colorData.Length != this._width * this._height)
                throw new ArgumentOutOfRangeException();


            // Convert to System.Drawing.Bitmap
            Bitmap ret = new Bitmap(this._width,this._height);
            PointBitmap pBMP = new PointBitmap(ret);
            pBMP.LockBits();
            for (int y = 0; y < this._height; y++)
            {
                for (int x = 0; x < this._width; x++)
                {
                    Color c_x_y = this[this._width * y + x];
                    pBMP.SetPixel(x, y, c_x_y);
                }
            }
            pBMP.UnlockBits();
            return ret;

        }
    }// end class BitmapColors
}//
