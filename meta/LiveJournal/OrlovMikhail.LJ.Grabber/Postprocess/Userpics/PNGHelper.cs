using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace OrlovMikhail.LJ.Grabber
{
    public static class PNGHelper
    {
        public static byte[] ConvertToPNG(byte[] data)
        {
            using (Image i = PNGHelper.LoadImageFromBytes(data))
            {
                // Is it PNG?
                if (ImageFormat.Png.Equals(i.RawFormat))
                    return data;

                // Otherwise, convert.
                byte[] asPNG = PNGHelper.SaveAsPNG(i);
                return asPNG;
            }
        }

        /// <summary>Returns image as PNG.</summary>
        public static byte[] SaveAsPNG(Image i)
        {
            byte[] ret;
            using (Bitmap bitmapCopy = new Bitmap(i))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    bitmapCopy.Save(ms, ImageFormat.Png);
                    ret = ms.ToArray();
                }
            }
            return ret;
        }

        /// <summary>Load image from bytes.</summary>
        public static Image LoadImageFromBytes(byte[] bmpBytes)
        {
            Image image = null;
            using (MemoryStream stream = new MemoryStream(bmpBytes))
            {
                image = Image.FromStream(stream);
            }

            return image;
        }
    }
}