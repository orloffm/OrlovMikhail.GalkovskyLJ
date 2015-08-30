using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.Grabber
{
    /// <summary>Actually downloads stuff from LiveJournal.</summary>
    public interface ILJClient
    {
        string GetContent(LiveJournalTarget target, ILJClientData data);

        byte[] DownloadFile(Uri target);

        /// <summary>Creates data object based on a string.
        /// This can have various meanings based on the implementers.</summary>
        ILJClientData CreateDataObject(string input);
    }
}
