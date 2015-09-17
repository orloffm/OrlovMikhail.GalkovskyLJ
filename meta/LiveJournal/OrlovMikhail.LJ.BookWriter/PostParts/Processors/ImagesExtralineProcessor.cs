using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.BookWriter
{
    public class ImagesExtralineProcessor : ProcessorBase
    {
        protected internal override void ProcessInternal(List<IPostPart> items)
        {
            for(int i = 0; i < items.Count; i++)
            {
                if(!(items[i] is MultimediaBasePart))
                    continue;

                IPostPart previous = (i > 0 ? items[i - 1] : null);
                IPostPart next = (i < items.Count - 1 ? items[i + 1] : null);

                if(previous is LineBreakPart)
                    items[i - 1] = new ParagraphStartPart();
                else if(previous != null && !(previous is ParagraphStartPart))
                {
                    items.Insert(i, new ParagraphStartPart());
                    i++;
                }

                if(next is LineBreakPart)
                    items[i + 1] = new ParagraphStartPart();
                else if(next != null && !(next is ParagraphStartPart))
                {
                    items.Insert(i + 1, new ParagraphStartPart());

                    // Can skip, whatever.
                    i++;
                }
            }
        }
    }
}
