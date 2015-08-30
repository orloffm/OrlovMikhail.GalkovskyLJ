using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.Tools
{
    public static class IOTools
    {
        public static string MakeRelativePath(DirectoryInfoBase fromPath, FileInfoBase toPath)
        {
            if (fromPath == null) throw new ArgumentNullException("fromPath");
            if (toPath == null) throw new ArgumentNullException("toPath");

            Uri fromUri = new Uri(fromPath.FullName + "\\a.txt");
            Uri toUri = new Uri(toPath.FullName);

            if (fromUri.Scheme != toUri.Scheme) { return toPath.FullName; } // path can't be made relative.

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (toUri.Scheme.ToUpperInvariant() == "FILE")
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }
    }
}
