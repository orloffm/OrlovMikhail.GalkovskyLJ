using System;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.Galkovsky.BookMaker
{
    public interface IBookMaker
    {
        Task Make(DirectoryInfoBase bookRootLocation, FileInfoBase[] dumps, bool overwrite);
    }
}
