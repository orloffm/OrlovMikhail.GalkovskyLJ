using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.BookWriter
{
    public class DoubleSpacesRemovalProcessor : ProcessorBase
    {
        protected internal override void ProcessInternal(List<IPostPart> items)
        {
            for(int i = 0; i < items.Count; i++)
            {
                RawTextPostPart rtpp = items[i] as RawTextPostPart;
                if(rtpp == null)
                    continue;

                // Double spaces.
                rtpp.Text = rtpp.Text.Replace("  ", " ");
            }
        }
    }
}
