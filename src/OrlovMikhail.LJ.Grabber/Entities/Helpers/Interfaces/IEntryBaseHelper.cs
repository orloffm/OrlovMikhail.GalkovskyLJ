using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.Grabber
{
    public interface IEntryBaseHelper
    {
        bool UpdateWith(EntryBase target, EntryBase source);

        bool UpdateStringProperty(string sourceValue, string targetValue, Action<string> targetSetter);

        /// <summary>Enumerates through all distinct userpics
        /// present in the entry.</summary>
        /// <param name="source">Entry.</param>
        /// <returns>Pairs of usernames and userpics.</returns>
        Tuple<string, Userpic>[] GetUserpics(IEnumerable<EntryBase> source);

        /// <summary>Returns all files linked to, including images.</summary>
        /// <param name="source">Entry and/or comments.</param>
        /// <returns>Array of files.</returns>
        Uri[] GetFiles(IEnumerable<EntryBase> source);
    }
}
