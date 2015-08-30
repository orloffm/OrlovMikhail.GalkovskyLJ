using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.BookWriter.AsciiDoc
{
    public class AsciiDocBookWriterFactory : IBookWriterFactory
    {
        public IBookWriter Create(DirectoryInfoBase bookRootLocation, FileInfoBase path)
        {
            return new AsciiDocBookWriter(bookRootLocation, path);
        }
    }
}
