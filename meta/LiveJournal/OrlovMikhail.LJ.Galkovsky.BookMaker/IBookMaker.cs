using System;
using System.IO.Abstractions;

namespace OrlovMikhail.LJ.Galkovsky.BookMaker
{
    public interface IBookMaker
    {
        void Make(DirectoryInfoBase bookRootLocation, FileInfoBase sourceFile, FileInfoBase targetFile);
    }
}
