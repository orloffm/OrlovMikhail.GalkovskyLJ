using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.BookWriter
{
    public class LineBreakAroundFormattingRemover : ProcessorBase
    {
        protected internal override void ProcessInternal(List<PostPartBase> items)
        {
            for(int i = 0; i < items.Count; i++)
            {
                // Try remove line break after formatting start.
                bool isBegin = items[i] is ItalicStartPart || items[i] is BoldStartPart;
                if(isBegin)
                {
                    PostPartBase next = (i < items.Count - 1 ? items[i + 1] : null);
                    bool nextIsBreak = next != null && (next is LineBreakPart);

                    if(nextIsBreak)
                        items.RemoveAt(i + 1);
                    continue;
                }

                // Try remove line break before formatting end.
                bool isEnd = items[i] is ItalicEndPart || items[i] is BoldEndPart;
                if(isEnd)
                {
                    PostPartBase previous = (i > 0 ? items[i - 1] : null);
                    bool previousIsBreak = previous != null && (previous is LineBreakPart);

                    if(previousIsBreak)
                    {
                        items.RemoveAt(i - 1);
                        i--;
                    }
                }
            }
        }
    }
}
