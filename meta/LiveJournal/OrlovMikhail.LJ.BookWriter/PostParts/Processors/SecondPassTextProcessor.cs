using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.BookWriter
{
    public class SecondPassTextProcessor : ProcessorBase
    {
        private const string lineStartRegexString = @"^[*\da-zА-ЯA-Zа-я]*\.";
        private readonly Regex _lineStartRegex;

        public SecondPassTextProcessor()
        {
            _lineStartRegex = new Regex(lineStartRegexString, RegexOptions.Compiled);
        }

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
                    bool previousIsBreak = previous == null || (previous is LineBreakPart || previous is ParagraphStartPart);
                    bool nextIsBreak = next == null || (next is LineBreakPart || next is ParagraphStartPart);

                    // Trim accordingly.
                    if(previousIsBreak && nextIsBreak)
                        rtpp.Text = rtpp.Text.Trim();
                    else if(previousIsBreak)
                        rtpp.Text = rtpp.Text.TrimStart();
                    else if(nextIsBreak)
                        rtpp.Text = rtpp.Text.TrimEnd();

                    // Prepend numbers with {empty} to disable lists.
                    if(previousIsBreak)
                    {
                        bool startsWithNumberAndDot = _lineStartRegex.IsMatch(rtpp.Text);
                        if(startsWithNumberAndDot)
                            rtpp.Text = "{empty}" + rtpp.Text;
                    }

                    // Remove empty text.
                    if(rtpp.Text == String.Empty)
                    {
                        items.RemoveAt(i);
                        i--;
                    }
                }
                else if(items[i] is ParagraphStartPart || items[i] is LineBreakPart)
                {
                    if(previous == null)
                    {
                        // This is the first. Stay on it.
                        items.RemoveAt(i);
                        i--;
                    }
                    else if(next == null)
                    {
                        // This is the last. Go to previous item.
                        items.RemoveAt(i);
                        i -= 2;
                    }
                }
            }
        }
    }
}
