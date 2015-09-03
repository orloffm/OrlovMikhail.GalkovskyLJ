using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.BookWriter
{
    public class ChevronsProcessor : ProcessorBase
    {
        protected internal override void ProcessInternal(List<PostPartBase> items)
        {
            int previousDepth = 0;

            for(int i = -1; i < items.Count; i++)
            {
                if(i >= 0)
                {
                    // On a break;
                    bool isBreak = items[i] is NewBlockStartBasePart;
                    if(!isBreak)
                        continue;
                }

                int nextBreak = FindNextPartIndex<NewBlockStartBasePart>(items, i);

                // All text parts.
                RawTextPostPart[] textPartsBetween = EnumerateTextPartsBetween(items, i, nextBreak).ToArray();

                int chevronsFound = RemoveChevrons(textPartsBetween);

                // Is depth the same as it was?
                if(chevronsFound == previousDepth)
                {
                    // It's the same depth.
                    if(chevronsFound == 0)
                    {
                        // Not a quote at all.
                        // New block can as well be a line break.
                    }
                    else
                    {
                        // Make sure the new block is the proper paragraph start.
                        if(i >= 0 && items[i] is LineBreakPart)
                        {
                            // We remove line breaks between quotations of the same depth.
                            items[i] = new RawTextPostPart(" ");
                        }
                    }
                }
                else
                {
                    // Quotation depth changed.
                    if(i >= 0)
                    {
                        // Thus the break must be a paragraph start.
                        items[i] = new ParagraphStartPart(chevronsFound);
                    }
                    else
                    {
                        // That's the first items. We should start the quotation here.
                        items.Insert(0, new ParagraphStartPart(chevronsFound));
                        i++;
                    }

                    previousDepth = chevronsFound;
                }
            }
        }

        private int RemoveChevrons(RawTextPostPart[] textPartsBetween)
        {
            int chevronCount = 0;
            bool reachedBody = false;

            foreach(RawTextPostPart rtpp in textPartsBetween)
            {
                for(int i = 0; i < rtpp.Text.Length; i++)
                {
                    char c = rtpp.Text[i];

                    if(c == '>')
                        chevronCount++;
                    else if(Char.IsWhiteSpace(c))
                        continue;
                    else
                    {
                        // OK, not a chevron.
                        rtpp.Text = rtpp.Text.Substring(i);
                        reachedBody = true;
                        break;
                    }
                }

                if(reachedBody)
                    break;
            }

            return chevronCount;
        }
    }
}
