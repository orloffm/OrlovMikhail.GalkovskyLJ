using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.BookWriter
{
    /// <summary>Swaps opening formatting after line breaks
    /// and closing formatting before line breaks.</summary>
    public class LineBreakAdjacentFormattingSwapProcessor : ProcessorBase
    {
        protected internal override void ProcessInternal(List<PostPartBase> items)
        {
            for(int i = 0; i < items.Count - 1; i++)
            {
                // Try remove line break after formatting start.
                bool isBegin = items[i] is FormattingStartBasePart;
                if(isBegin)
                {
                    bool nextIsBreak = items[i + 1] is LineBreakPart;
                    if(nextIsBreak)
                    {
                        FormattingStartBasePart current = (FormattingStartBasePart)items[i];
                        items.RemoveAt(i);
                        items.Insert(i + 1, current);
                    }
                }
            }

            for(int i = items.Count - 1; i > 0; i--)
            {
                // Try remove line break before formatting end.
                bool isEnd = items[i] is FormattingEndBasePart;
                if(isEnd)
                {
                    bool previousIsBreak = items[i - 1] is LineBreakPart;
                    if(previousIsBreak)
                    {
                        FormattingEndBasePart current = (FormattingEndBasePart)items[i];
                        items.RemoveAt(i);
                        items.Insert(i - 1, current);
                    }
                }
            }
        }
    }
}
