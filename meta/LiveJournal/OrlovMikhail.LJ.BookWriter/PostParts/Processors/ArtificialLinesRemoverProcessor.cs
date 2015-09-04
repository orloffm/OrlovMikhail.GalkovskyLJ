using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.BookWriter
{
    public class ArtificialLinesRemoverProcessor : ProcessorBase
    {
        private const string artificialLineRegexString = @"^\s*(?:-|_|=|\*|\+|\.|\\|\/)+\s*$";
        private readonly Regex _artificialLineRegex;

        public ArtificialLinesRemoverProcessor()
        {
            _artificialLineRegex = new Regex(artificialLineRegexString, RegexOptions.Compiled);
        }

        protected internal override void ProcessInternal(List<PostPartBase> items)
        {
            // At these indeces are the artificial lines.
            List<int> artificialIndeces = GetArtificialIndeces(items);

            for (int p = 0; p < artificialIndeces.Count - 1; p += 2)
            {
                int a = artificialIndeces[p];
                int b = artificialIndeces[p + 1];
                items[a] = ItalicStartPart.Instance;
                items[b] = ItalicEndPart.Instance;
            }

            if (artificialIndeces.Count % 2 != 0)
            {
                // Last unmatched, remove it.
                int i = artificialIndeces.Last();
                if (items[i] is RawTextPostPart)
                {
                    items.RemoveAt(i);
                    i--;
                }
            }
        }

        private List<int> GetArtificialIndeces(List<PostPartBase> items)
        {
            List<int> artificialIndeces = new List<int>();

            for (int i = -1; i < items.Count; i++)
            {
                if (i >= 0)
                {
                    // On a break;
                    bool isBreak = items[i] is NewBlockStartBasePart;
                    if (!isBreak)
                        continue;
                }

                int nextBreak = FindNextPartIndex<NewBlockStartBasePart>(items, i);
                if (nextBreak == 0)
                {
                    // We start from i==-1 to catch line begin.
                    // But if the items start from a new block, skip to it.
                    continue;
                }

                // Is there a single text part on this line?
                int[] textPartsIndecesBetween = EnumerateIndecesOfBetween<RawTextPostPart>(items, i, nextBreak).ToArray();
                if (textPartsIndecesBetween.Length != 1)
                    continue;

                int textIndex = textPartsIndecesBetween[0];
                string text = ((RawTextPostPart)items[textIndex]).Text;

                bool isArtificialLine = _artificialLineRegex.IsMatch(text);
                if (isArtificialLine)
                    artificialIndeces.Add(textIndex);
            }

            return artificialIndeces;
        }
    }
}
