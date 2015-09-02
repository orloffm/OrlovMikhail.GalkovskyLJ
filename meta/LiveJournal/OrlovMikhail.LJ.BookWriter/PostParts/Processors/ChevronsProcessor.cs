using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.BookWriter
{
    public class ChevronsProcessor : ProcessorBase
    {
        protected internal override void ProcessInternal(List<PostPartBase> items)
        {
            for(int i = 0; i < items.Count; i++)
            {
                RawTextPostPart rtpp = items[i] as RawTextPostPart;
                if(rtpp == null)
                    continue;

                PostPartBase previous = (i > 0 ? items[i - 1] : null);
                bool previousIsBreak = previous == null || (previous is LineBreakPart || previous is ParagraphStartPart);
                if(previousIsBreak && rtpp.Text.StartsWith(">"))
                {
                    // Make sure spaces are alright.
                    int currentChevronCount;
                    rtpp.Text = InsertSpacesAfterChevrons(rtpp.Text, out currentChevronCount);

                    if(previous is LineBreakPart)
                    {
                        // Should we replace it with paragraph start?

                        int previousLineChevronCount = FindPreviousLineChevronCount(items, i-1);

                        if(previousLineChevronCount < currentChevronCount)
                            items[i - 1] = ParagraphStartPart.Instance;
                    }
                }
            }
        }

        /// <summary>How many chevrons are there in the beginning of the previous line.</summary>
        public static int FindPreviousLineChevronCount(List<PostPartBase> items, int currentLineBreakIndex)
        {
            return 1000;
        }

        // TODO

        /// <summary>Makes sure chevrons are separated with spaces.</summary>
        /// <param name="chevronCount">How many chevrons are there.</param>
        public static string InsertSpacesAfterChevrons(string text, out int chevronCount)
        {
            chevronCount = 0;

            for(int i = 0; i < text.Length - 1; i += 2)
            {
                if(text[i] == '>' && !Char.IsWhiteSpace(text[i + 1]))
                {
                    // Insert space.
                    text = text.Substring(0, i + 1) + " " + text.Substring(i + 1);
                }
                else
                {
                    // Do nothing more.
                    break;
                }
            }

            return text;
        }
    }
}
