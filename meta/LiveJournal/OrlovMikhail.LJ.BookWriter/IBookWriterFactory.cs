using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.BookWriter
{
    public interface IBookWriterFactory
    {
        IBookWriter Create(DirectoryInfoBase bookRootLocation, FileInfoBase path);
    }
}
