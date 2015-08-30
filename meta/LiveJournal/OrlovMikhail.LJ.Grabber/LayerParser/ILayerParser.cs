using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.Grabber
{
    /// <summary>Parses rendered page as data.</summary>
    public interface ILayerParser
    {
        /// <summary>Parses content as an entry page.</summary>
        EntryPage ParseAsAnEntryPage(string content);

        string Serialize(EntryPage ep);
    }
}
