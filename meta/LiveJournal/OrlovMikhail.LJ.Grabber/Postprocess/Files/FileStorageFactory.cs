using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.Grabber
{
    public class FileStorageFactory : IFileStorageFactory
    {
        private readonly IFileSystem _fs;

        public FileStorageFactory(IFileSystem fs)
        {
            _fs = fs;
        }

        public IFileStorage CreateOn(string dumpLocation)
        {
            return new FileStorage(_fs, dumpLocation);
        }
    }
}
