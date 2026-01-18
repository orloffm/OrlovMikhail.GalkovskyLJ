using System;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.Galkovsky.BookMaker
{
    public interface IBookMaker
    {
        Task Make(IDirectoryInfo bookRootLocation, IFileInfo[] dumps, bool overwrite);
    }
}
