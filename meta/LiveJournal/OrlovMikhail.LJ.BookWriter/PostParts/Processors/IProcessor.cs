using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.BookWriter
{
    /// <summary>Converts the list of post parts.</summary>
    public interface IProcessor
    {
        List<PostPartBase> Process(IList<PostPartBase> items);
    }
}
