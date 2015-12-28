using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.Grabber
{
    /// <summary>Gets numbering by entry.</summary>
    public interface INumberingStrategy
    {
        /// <summary>Returns the name of the subfolder
        /// inside the year folder that the entry should be put to.</summary>
        string GetSubfolderByEntry(string subject);

        /// <summary>Given a subfolder, created by another function
        /// in the interface,
        /// returns the ordered number of the entry.</summary>
        int GetSortNumberBySubfolder(string subfolder);

        /// <summary>Presents the sort number in the user-friendly way.
        /// Most likely, it should be equivalent to the string in the folder name
        /// and in the source subject.</summary>
        string GetFriendlyTitleBySortNumber(int? sortNumber);
    }
}
