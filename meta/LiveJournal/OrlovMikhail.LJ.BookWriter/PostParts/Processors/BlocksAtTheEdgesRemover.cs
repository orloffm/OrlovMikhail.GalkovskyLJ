using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.BookWriter
{
    public class BlocksAtTheEdgesRemover : ProcessorBase
    {
        protected internal override void ProcessInternal(List<PostPartBase> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] is NewBlockStartBasePart)
                {
                    bool shouldStay = (i == 0) && (items[i] is ParagraphStartPart) &&
                                      (((ParagraphStartPart) items[i]).QuotationLevel > 0);

                    if(!shouldStay)
                    {
                        // This is the first. Stay on it.
                        items.RemoveAt(i);
                        i--;
                    }
                }
                else
                    break;
            }

            // Remove all block starters at the end.
            for (int i = items.Count - 1; i >= 0; i--)
            {
                if (items[i] is NewBlockStartBasePart)
                    items.RemoveAt(i);
                else
                    break;
            }
        }
    }
}
