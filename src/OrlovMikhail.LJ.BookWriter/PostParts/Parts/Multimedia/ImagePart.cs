using System;
using System.Diagnostics;
using System.IO.Abstractions;

namespace OrlovMikhail.LJ.BookWriter
{
    [DebuggerDisplay("[Image, {Src.FullName}]")]
    public class ImagePart : MultimediaBasePart, IEquatable<ImagePart>
    {
        public IFileInfo Src { get; set; }

        public ImagePart(IFileInfo src)
        {
            Src = src;
        }

        #region equality
        public override int GetHashCode()
        {
            return Src.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return AreEqual(this, obj as ImagePart);
        }

        public static bool AreEqual(ImagePart p1, ImagePart p2)
        {
            if(ReferenceEquals(p1, p2))
                return true;
            else if(ReferenceEquals(p1, null) || ReferenceEquals(p2, null))
                return false;

            if(ReferenceEquals(p1.Src, p2.Src))
                return true;
            else if(ReferenceEquals(p1.Src, null) || ReferenceEquals(p2.Src, null))
                return false;

            return String.Equals(p1.Src.FullName, p2.Src.FullName, StringComparison.OrdinalIgnoreCase);
        }

        public bool Equals(ImagePart other)
        {
            return AreEqual(this, other);
        }
        #endregion
    }
}