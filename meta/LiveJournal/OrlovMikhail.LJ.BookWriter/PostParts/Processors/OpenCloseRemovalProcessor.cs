using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.BookWriter
{
    public class OpenCloseRemovalProcessor : ProcessorBase
    {
        protected internal override void ProcessInternal(List<PostPartBase> items)
        {
            for (int i = 0; i < items.Count - 1; i++)
            {
                if ((items[i] is ItalicStartPart && items[i + 1] is ItalicEndPart)
                    || (items[i] is BoldStartPart && items[i + 1] is BoldEndPart))
                {
                    items.RemoveAt(i);
                    items.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}
