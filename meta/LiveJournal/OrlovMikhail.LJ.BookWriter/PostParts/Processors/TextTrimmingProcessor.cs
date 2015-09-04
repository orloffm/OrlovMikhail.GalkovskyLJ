using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.BookWriter
{
    public class TextTrimmingProcessor : ProcessorBase
    {
        protected internal override void ProcessInternal(List<PostPartBase> items)
        {
            for(int i = 0; i < items.Count; i++)
            {
                PostPartBase previous = (i > 0 ? items[i - 1] : null);
                PostPartBase next = (i < items.Count - 1 ? items[i + 1] : null);

                if(items[i] is RawTextPostPart)
                {
                    RawTextPostPart rtpp = items[i] as RawTextPostPart;

                    // What should be trimmed?
                    bool previousIsBreak = previous == null || (previous is NewBlockStartBasePart);
                    bool nextIsBreak = next == null || (next is NewBlockStartBasePart);

                    // Trim accordingly.
                    if(previousIsBreak && nextIsBreak)
                        rtpp.Text = rtpp.Text.Trim();
                    else if(previousIsBreak)
                        rtpp.Text = rtpp.Text.TrimStart();
                    else if(nextIsBreak)
                        rtpp.Text = rtpp.Text.TrimEnd();

                    // Remove empty text.
                    if(rtpp.Text == String.Empty)
                    {
                        items.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
    }
}
