using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.BookWriter
{
    /// <summary>This interface implementer will generate text in the output file.</summary>
    public interface IRendersAsText : IPostPart
    {
        bool CanBeTrimmed { get; }
    }
}
