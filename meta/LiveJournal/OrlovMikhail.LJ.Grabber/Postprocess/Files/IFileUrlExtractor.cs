using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.Grabber
{
    /// <summary>Extracts all file URLs from HTML.</summary>
    public interface IFileUrlExtractor
    {
        /// <summary>Extracts URLs to files that are listed in the HTML.</summary>
        string[] GetFileUrls(string html);

        /// <summary>Given HTML, replaces all file URLs with the strings provided.</summary>
        /// <param name="html">Source HTML.</param>
        /// <param name="matcher">Function that returns values to replace the source with.</param>
        string ReplaceFileUrls(string html, Func<string, string> matcher);
    }
}
