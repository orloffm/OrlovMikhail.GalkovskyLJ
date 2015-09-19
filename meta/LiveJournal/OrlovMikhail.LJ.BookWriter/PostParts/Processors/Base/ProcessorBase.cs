using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.BookWriter
{
    public abstract class ProcessorBase : IProcessor
    {
        public List<IPostPart> Process(IList<IPostPart> src)
        {
            List<IPostPart> items = src.Select(z => z.FullClone()).ToList();

            ProcessInternal(items);

            return items;
        }

        protected internal abstract void ProcessInternal(List<IPostPart> items);

        [Obsolete]
        protected internal int FindNextPart<T, U>(List<IPostPart> items, int start, bool reverse = false)
            where T : IPostPart
            where U : IPostPart
        {
            int add = reverse ? -1 : +1;

            for(int p = start + add; p < items.Count && p >= 0; p += add)
            {
                IPostPart z = items[p];
                if(z is T || z is U)
                    return p;
            }

            return -1;
        }

        /// <summary>Finds next occurence of item.</summary>
        /// <param name="start">Where to start from. Not included in search.</param>
        /// <param name="reverse">True if we need to look backwards.</param>
        protected internal int FindNextPartIndex<T>(List<IPostPart> items, int start, bool reverse = false) where T : IPostPart
        {
            int add = reverse ? -1 : +1;

            for(int p = start + add; p < items.Count && p >= 0; p += add)
            {
                IPostPart z = items[p];
                if(z is T)
                    return p;
            }

            return -1;
        }

        protected internal T FindNextPart<T>(List<IPostPart> items, int start, bool reverse = false) where T :class, IPostPart
        {
            int index = FindNextPartIndex<T>(items, start, reverse);
            if(index < 0)
                return null;
            else
                return items[index] as T;
        }

        public static IEnumerable<IRendersAsText> EnumerateTextPartsBetween(List<IPostPart> items, int a, int b)
        {
            var indeces = EnumerateIndecesOfBetween<RawTextPostPart>(items, a, b);
            foreach (int i in indeces)
                yield return items[i] as RawTextPostPart;
        }

        public static IEnumerable<int> EnumerateIndecesOfBetween<T>(List<IPostPart> items, int a, int b)where T : IPostPart
        {
            int startIndex = (a < 0) ? 0 : a + 1;
            int endIndex = (b < 0) ? items.Count - 1 : b - 1;

            if (startIndex > endIndex && (a != -1 && b != -1))
                throw new ArgumentException();

            for (int i = startIndex; i <= endIndex; i++)
            {
                if (items[i] is T)
                    yield return i;
            }
        }

        /// <summary>Enumerates characters of text between (not including)
        /// parts of given indeces.</summary>
        /// <param name="a">Index after which.</param>
        /// <param name="b">Index before which.</param>
        public static IEnumerable<char> EnumerateCharsBetween(List<IPostPart> items, int a, int b)
        {
            var rawTextParts = EnumerateTextPartsBetween(items, a, b);

            foreach(RawTextPostPart rtpp in rawTextParts)
            {
                foreach(char c in rtpp.Text)
                    yield return c;
            }
        }
    }
}
