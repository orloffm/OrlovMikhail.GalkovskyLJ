using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.Grabber
{
    public interface ISubfolderByEntryGetter
    {
        /// <summary>Given a downloaded entry, returns the suggested subfolder to download it to.</summary>
        void GetSubfolderByFreshEntry(Entry e, out string subFolder, out string filename);
    }
}
