using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.BookWriter
{
    /// <summary>Merges multiple line breaks into paragraph starts,
    /// removes double paragraph starts.</summary>
    public class LineBreaksMergingProcessor : ProcessorBase
    {
        protected internal override void ProcessInternal(List<PostPartBase> items)
        {
            // First, merge line breaks.
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
                    items[i] = new ParagraphStartPart();
                }
            }

            // Now, consume them by paragraphs.
            for(int i = items.Count - 1; i >= 1; i--)
            {
                // Is it a paragraph start?
                ParagraphStartPart rtpp = items[i] as ParagraphStartPart;
                if(rtpp == null)
                    continue;

                // Is there a line break after?
                if(i < items.Count - 1)
                {
                    LineBreakPart lbp = items[i + 1] as LineBreakPart;
                    if(lbp != null)
                    {
                        // Remove it, it's not needed.
                        items.RemoveAt(i + 1);
                    }
                }

                // Are there line breaks or paragraphs just before?
                do
                {
                    NewBlockStartBasePart nbsp = items[i - 1] as NewBlockStartBasePart;
                    if(nbsp == null)
                        break;

                    items.RemoveAt(i - 1);
                    i--;
                } while(i >= 1);
            }
        }
    }
}
