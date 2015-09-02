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

        protected internal int FindNextPart<T, U>(List<PostPartBase> items, int start, bool reverse = false)
            where T : PostPartBase where U : PostPartBase
        {
            int add = reverse ? -1 : +1;

            for (int p = start + add; p < items.Count && p >= 0; p += add)
            {
                PostPartBase z = items[p];
                if (z is T || z is U)
                    return p;
            }

            return -1;
        }

        protected internal int FindNextPart<T>(List<PostPartBase> items, int start, bool reverse = false) where T : PostPartBase
        {
            return FindNextPart<T, T>(items, start, reverse);
        }


        /// <summary>Enumerates characters of text between (not including)
        /// parts of given indeces.</summary>
        /// <param name="a">Index after which.</param>
        /// <param name="b">Index before which.</param>
        public static IEnumerable<char> EnumerateCharsBetween(List<PostPartBase> items, int a, int b)
        {
            int startIndex = (a < 0) ? 0 : a + 1;
            int endIndex = (b < 0) ? items.Count - 1 : b - 1;

            if (startIndex > endIndex)
                throw new ArgumentException();

            for (int i = startIndex; i <= endIndex; i++)
            {
                RawTextPostPart rtpp = items[i] as RawTextPostPart;
                if (rtpp == null)
                    continue;

                foreach (char c in rtpp.Text)
                    yield return c;
            }
        }
    }
}
