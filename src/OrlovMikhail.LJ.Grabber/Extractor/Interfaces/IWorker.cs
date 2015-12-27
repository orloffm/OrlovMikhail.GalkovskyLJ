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
        EntryPage Work(string URI, string rootLocation, INumberingStrategy subFolderGetter, string cookie);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="URI"></param>
        /// <param name="rootLocation"></param>
        /// <param name="innerFolder">Folder inside year folder.</param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        EntryPage WorkInGivenTarget(string URI, string rootLocation, string innerFolder, string cookie);
    }
}
