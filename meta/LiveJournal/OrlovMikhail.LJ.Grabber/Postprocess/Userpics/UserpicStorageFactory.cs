using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.Grabber
{
    public class UserpicStorageFactory : IUserpicStorageFactory
    {
        private readonly IFileSystem _fs;

        public UserpicStorageFactory(IFileSystem fs)
        {
            _fs = fs;
        }

        public IUserpicStorage CreateOn(string basePath)
        {
            return new UserpicStorage(_fs, _fs.Path.Combine(basePath, "userpics"));
        }
    }
}
