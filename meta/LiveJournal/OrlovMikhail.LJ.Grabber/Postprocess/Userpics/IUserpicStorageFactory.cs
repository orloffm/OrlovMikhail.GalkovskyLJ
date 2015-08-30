using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.Grabber
{
    public interface IUserpicStorageFactory
    {
        IUserpicStorage CreateOn(string basePath);
    }
}
