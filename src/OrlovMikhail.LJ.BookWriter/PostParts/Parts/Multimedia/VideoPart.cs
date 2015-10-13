using System;
using System.Diagnostics;
using System.IO.Abstractions;

namespace OrlovMikhail.LJ.BookWriter
{
    [DebuggerDisplay("[Video, {URL}]")]
    public class VideoPart : MultimediaBasePart, IEquatable<VideoPart>
    {
        public string URL { get; set; }

        public VideoPart(string url)
        {
            URL = url;
        }

        #region equality
        public override int GetHashCode()
        {
            return URL.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return AreEqual(this, obj as VideoPart);
        }

        public static bool AreEqual(VideoPart p1, VideoPart p2)
        {
            if(ReferenceEquals(p1, p2))
                return true;
            else if(ReferenceEquals(p1, null) || ReferenceEquals(p2, null))
                return false;

            return String.Equals(p1.URL, p2.URL, StringComparison.OrdinalIgnoreCase);
        }

        public bool Equals(VideoPart other)
        {
            return AreEqual(this, other);
        }
        #endregion
    }
}