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

        [Obsolete]
        protected internal int FindNextPart<T, U>(List<PostPartBase> items, int start, bool reverse = false)
            where T : PostPartBase
            where U : PostPartBase
        {
            int add = reverse ? -1 : +1;

            for(int p = start + add; p < items.Count && p >= 0; p += add)
            {
                PostPartBase z = items[p];
                if(z is T || z is U)
                    return p;
            }

            return -1;
        }

        /// <summary>Finds next occurence of item.</summary>
        /// <param name="start">Where to start from. Not included in search.</param>
        /// <param name="reverse">True if we need to look backwards.</param>
        protected internal int FindNextPartIndex<T>(List<PostPartBase> items, int start, bool reverse = false) where T : PostPartBase
        {
            int add = reverse ? -1 : +1;

            for(int p = start + add; p < items.Count && p >= 0; p += add)
            {
                PostPartBase z = items[p];
                if(z is T)
                    return p;
            }

            return -1;
        }

        protected internal T FindNextPart<T>(List<PostPartBase> items, int start, bool reverse = false) where T : PostPartBase
        {
            int index = FindNextPartIndex<T>(items, start, reverse);
            if(index < 0)
                return null;
            else
                return items[index] as T;
        }

        public static IEnumerable<RawTextPostPart> EnumerateTextPartsBetween(List<PostPartBase> items, int a, int b)
        {
            var indeces = EnumerateIndecesOfBetween<RawTextPostPart>(items, a, b);
            foreach (int i in indeces)
                yield return items[i] as RawTextPostPart;
        }

        public static IEnumerable<int> EnumerateIndecesOfBetween<T>(List<PostPartBase> items, int a, int b)where T : PostPartBase
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
        public static IEnumerable<char> EnumerateCharsBetween(List<PostPartBase> items, int a, int b)
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
