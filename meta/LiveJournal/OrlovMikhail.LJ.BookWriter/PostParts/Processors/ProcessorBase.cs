using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.BookWriter
{
    public abstract class ProcessorBase : IProcessor
    {
        public List<PostPartBase> Process(IList<PostPartBase> src)
        {
            List<PostPartBase> items = src.Select(z => z.FullClone()).ToList();

            ProcessInternal(items);

            return items;
        }

        protected internal abstract void ProcessInternal(List<PostPartBase> items);

      protected internal  int FindNextPart<T>(List<PostPartBase> items, int start, bool reverse = false) where T : PostPartBase
        {
            int add = reverse ? -1 : +1;

            for(int p = start + add; p < items.Count && p >= 0; p += add)
                if(items[p] is T)
                    return p;

            return -1;
        }
    }
}
