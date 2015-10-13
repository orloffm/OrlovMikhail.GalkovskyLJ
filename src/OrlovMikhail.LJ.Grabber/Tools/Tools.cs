using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.Grabber
{
    public static class Tools
    {
        static readonly string[] imageExtensions = new string[] { "jpg", "jpeg", "gif", "png", "bmp" };
        static readonly string[] extensions = new string[] { "mp3", "pdf" };

        static string GetExtension(string url)
        {
            int dotIndex = url.LastIndexOf('.');
            if(dotIndex < 0 || url.Length == dotIndex + 1)
                return null;
            string ext = url.Substring(dotIndex + 1);
            return ext;
        }

        public static bool IsAnImage(string url)
        {
            string ext = GetExtension(url);
            if(String.IsNullOrWhiteSpace(ext))
                return false;

            return imageExtensions.Any(e => String.Equals(e, ext, StringComparison.OrdinalIgnoreCase));
        }
    }
}
