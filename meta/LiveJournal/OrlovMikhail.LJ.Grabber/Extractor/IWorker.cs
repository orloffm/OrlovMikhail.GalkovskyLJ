using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.Grabber
{
    /// <summary>Orchestrates everything, downloads stuff to folders.</summary>
    public interface IWorker
    {
        EntryPage Work(string URI, string rootLocation, ISubfolderByEntryGetter subFolderGetter, string cookie);

        EntryPage WorkInGivenTarget(string URI, string rootLocation, string subfolder,string filename, string cookie);
    }
}
