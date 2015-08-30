using System.Diagnostics;
using System.IO.Abstractions;

namespace OrlovMikhail.LJ.BookWriter
{
    [DebuggerDisplay("[Image, {Src.FullName}]")]
    public class ImagePart : PostPartBase
    {
        public FileInfoBase Src { get; set; }

        public ImagePart(FileInfoBase src)
        {
            Src = src;
        }
    }
}