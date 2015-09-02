using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.BookWriter
{
    public class LineBreaksMergingProcessor : ProcessorBase
    {
        protected internal override void ProcessInternal(List<PostPartBase> items)
        {
            for(int i = 0; i < items.Count; i++)
            {
                // Is it a line break?
                LineBreakPart rtpp = items[i] as LineBreakPart;
                if(rtpp == null)
                    continue;

                // Are there line breaks later?
                int max = -1;
                for(int p = i + 1; p < items.Count; p++)
                {
                    if(items[p] is LineBreakPart)
                        max = p;
                    else
                        break;
                }

                // Delete those.
                if(max > i)
                {
                    for(int p = max; p > i; p--)
                        items.RemoveAt(p);

                    // Replace the original line break part.
                    items[i] = ParagraphStartPart.Instance;
                }
            }
        }
    }
}
