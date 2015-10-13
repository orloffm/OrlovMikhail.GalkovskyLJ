using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.Grabber
{
    /// <summary>Stores files by names. Linear storage,
    /// stores all files in subfolders.</summary>
    public interface IFileStorage : IStorage
    {

    }
}
