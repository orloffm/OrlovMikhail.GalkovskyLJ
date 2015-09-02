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
            int[] artificialIndeces = Enumerable.Range(0, items.Count)
                .Where(z =>
                {
                    RawTextPostPart r = items[z] as RawTextPostPart;
                    if(r == null)
                        return false;

                    // Is it actually a line?
                    PostPartBase previous = (z > 0 ? items[z - 1] : null);
                    PostPartBase next = (z < items.Count - 1 ? items[z + 1] : null);
                    bool previousIsBreak = previous == null || (previous is LineBreakPart || previous is ParagraphStartPart);
                    bool nextIsBreak = next == null || (next is LineBreakPart || next is ParagraphStartPart);

                    if(!(previousIsBreak && nextIsBreak))
                        return false;

                    bool isArtificialLine = _artificialLineRegex.IsMatch(r.Text);
                    return isArtificialLine;
                })
                .ToArray();

            for(int p = 0; p < artificialIndeces.Length - 1; p += 2)
            {
                int a = artificialIndeces[p];
                int b = artificialIndeces[p + 1];
                items[a] = ItalicStartPart.Instance;
                items[b] = ItalicEndPart.Instance;
            }

            if(artificialIndeces.Length % 2 != 0)
            {
                // Last unmatched, remove it.
                int i = artificialIndeces.Last();
                if(items[i] is RawTextPostPart)
                {
                    items.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}
