using System;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace OrlovMikhail.LJ.Grabber
{
    public static class ImageHelper
    {
        public static byte[] ConvertToPNG(byte[] data)
        {
            using (Image i = ImageHelper.LoadImageFromBytes(data))
            {
                // Is it PNG?
                if (ImageFormat.Png.Equals(i.RawFormat))
                    return data;

                // Otherwise, convert.
                byte[] asPNG = ImageHelper.SaveAsPNG(i);
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

        /// <summary>Returns image as PNG.</summary>
        public static byte[] SaveAsJPEG(Image i, long quality = 60)
        {
            if (quality < 5 || quality > 100)
                throw new ArgumentException();

            byte[] ret;
            using (Bitmap bitmapCopy = new Bitmap(i))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    ImageCodecInfo jpgEncoder = ImageCodecInfo.GetImageDecoders().FirstOrDefault(z => z.FormatID == ImageFormat.Jpeg.Guid);

                    EncoderParameters encoderParams = new EncoderParameters(1);
                    EncoderParameter qualityParam = new EncoderParameter(Encoder.Quality, quality);
                    encoderParams.Param[0] = qualityParam;

                    bitmapCopy.Save(ms, jpgEncoder, encoderParams);
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

        public static byte[] EnsureNotHugePNGOrJPEG(Image image, byte[] data, out string newExtension)
        {
            if (data == null)
                throw new ArgumentNullException();

            bool createdImage = false;
            newExtension = null;

            try
            {
                if (image == null)
                {
                    createdImage = true;
                    image = LoadImageFromBytes(data);
                }

                bool isGoodType = ImageFormat.Png.Equals(image.RawFormat) || ImageFormat.Jpeg.Equals(image.RawFormat);
                Size boundingBox = new Size(2000, 2000);
                bool isGoodSize = image.Width < boundingBox.Width && image.Height < boundingBox.Height;

                if (!isGoodSize)
                {
                    double wFactor = ((double)boundingBox.Width) / image.Width;
                    double hFactor = ((double)boundingBox.Height) / image.Height;

                    double factorToUse = Math.Min(wFactor, hFactor);
                    int newWidth = (int)Math.Round(factorToUse * image.Width);
                    int newHeight = (int)Math.Round(factorToUse * image.Height);

                    Bitmap resized = ResizeImage(image, newWidth, newHeight);
                    createdImage = true;
                    image = resized;
                }

                if (!isGoodType || !isGoodSize)
                {
                    // Re-save to JPEG.
                    data = SaveAsJPEG(image, 70);
                    newExtension = "jpg";
                }
            }
            finally
            {
                if (createdImage)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    image.Dispose();
                }
            }

            return data;
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}