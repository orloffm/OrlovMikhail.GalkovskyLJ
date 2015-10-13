using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.BookWriter
{
    public interface IHTMLParser
    {
        /// <summary>Returns tokens as is.</summary>
        IEnumerable<HTMLTokenBase> Parse(string html);
    }
}
