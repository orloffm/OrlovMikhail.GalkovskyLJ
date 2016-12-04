using System.Collections.Generic;
using System.IO.Abstractions;

namespace OrlovMikhail.LJ.Galkovsky
{
    public interface IFragmentHelper
    {
        /// <summary>Returns a dictionary for all fragment files, of their entry's ids and the relative paths.</summary>
        Dictionary<long, FragmentInformation> GetAllFragmentPaths(IFileSystem fs, string root);

        IEnumerable<FragmentInformation> SelectValuesFor(Split current, Dictionary<long, FragmentInformation> fragsById);
    }
}