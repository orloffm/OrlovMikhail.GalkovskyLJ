using System;
using System.IO;
using System.IO.Abstractions;

namespace OrlovMikhail.LJ.Galkovsky.Tools
{
    public static class IOTools
    {
        public static string MakeRelativePath(IDirectoryInfo relativeTo, IFileInfo path)
        {
            return Path.GetRelativePath(relativeTo.FullName, path.FullName);
        }
    }
}
