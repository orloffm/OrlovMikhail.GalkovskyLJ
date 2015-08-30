using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;

namespace OrlovMikhail.LJ.Grabber
{
    /// <summary>Creates a file storage.</summary>
    public interface IFileStorageFactory
    {
        /// <summary>Directory that holds original data.</summary>
        IFileStorage CreateOn(string dumpLocation);
    }
}
