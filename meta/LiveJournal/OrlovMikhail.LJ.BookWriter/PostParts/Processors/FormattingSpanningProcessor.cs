using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.BookWriter
{
    /// <summary>Makes sure formatting starts and ends inside
    /// one paragraph.</summary>
    public class FormattingSpanningProcessor : ProcessorBase
    {
        protected internal override void ProcessInternal(List<PostPartBase> items)
        {
            for(int i = 0; i < items.Count; i++)
            {
                ParagraphStartPart p1 = items[i] as ParagraphStartPart;
                if(p1 == null)
                    continue;

                PostPartBase formattingStarter = (i < items.Count - 1 ? items[i + 1] : null);
                if(formattingStarter == null)
                    return;

                PostPartBase needsToEndWith = null;
                if(formattingStarter is ItalicStartPart)
                    needsToEndWith = ItalicEndPart.Instance;
                else if(formattingStarter is BoldStartPart)
                    needsToEndWith = BoldEndPart.Instance;
                else
                    continue;

                int p2Index = FindNextPartIndex<ParagraphStartPart>(items, i);
                if(p2Index <= i)
                    return;

                int precedingPartIndex = p2Index - 1;
                PostPartBase precedingPart = items[precedingPartIndex];
                if(precedingPart.GetType() == needsToEndWith.GetType())
                {
                    i = p2Index - 1;
                    continue;
                }

                // Insert formatting ending part.
                items.Insert(precedingPartIndex + 1, needsToEndWith);
                p2Index++;

                do
                {
                    // Now let's make sure next is the same starting.
                    if(items[p2Index + 1] is ImagePart)
                    {
                        // Don't work with images. Go to next paragraph.
                        p2Index = FindNextPartIndex<ParagraphStartPart>(items, p2Index);
                        if(p2Index < 0)
                            return;

                        continue;
                    }

                    break;
                } while(true);

                // Item after the next paragraph.
                PostPartBase following = items[p2Index + 1];
                if(following.GetType() != formattingStarter.GetType())
                {
                    // Formatting starter there.
                    items.Insert(p2Index + 1, formattingStarter);
                }

                // Go to that paragraph next time.
                i = p2Index - 1;
            }
        }
    }
}
