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
            for (int i = 0; i < items.Count; i++)
            {
                RawTextPostPart rtpp = items[i] as RawTextPostPart;
                if (rtpp == null)
                    continue;

                PostPartBase previous = (i > 0 ? items[i - 1] : null);
                bool previousIsBreak = previous == null || (previous is LineBreakPart || previous is ParagraphStartPart);
                if (previousIsBreak)
                {
                    int currentChevronCount = CalculateChevronCount(rtpp.Text);
                    if (currentChevronCount == 0)
                        continue;

                    // Make sure spaces are alright.
                    rtpp.Text = InsertSpacesAfterChevrons(rtpp.Text);

                    // OK, so this line is a blockquote...
                    if (previous is LineBreakPart)
                    {
                        // ...and it is after a linebreak. If the number of chevrons
                        // here is different from the line before, it must be separated with
                        // a paragraph start.

                        int previousLineChevronCount = FindPreviousLineChevronCount(items, i - 1);

                        // Should we replace it with paragraph start?
                        if (previousLineChevronCount != currentChevronCount)
                            items[i - 1] = ParagraphStartPart.Instance;
                    }

                    int nextBreakIndex = FindNextPart<LineBreakPart, ParagraphStartPart>(items, i);
                    if (nextBreakIndex == -1 || items[nextBreakIndex] is ParagraphStartPart)
                    {
                        // OK, done with this one.
                        continue;
                    }

                    // We have next break index, it is a line break, not paragraph start.
                    int nextLineChevronCount = FindNextLineChevronCount(items, nextBreakIndex);
                    if (nextLineChevronCount != currentChevronCount)
                        items[nextBreakIndex] = ParagraphStartPart.Instance;

                    // Will go to next line begin.
                    i = nextBreakIndex;
                }
            }
        }

        /// <summary>How many chevrons are there in the beginning of the next line.</summary>
        /// <param name="nextBreakIndex">Break at the start of the wanted line.</param>
        public int FindNextLineChevronCount(List<PostPartBase> items, int nextBreakIndex)
        {
            int followingBreak = FindNextPart<LineBreakPart, ParagraphStartPart>(items, nextBreakIndex);
            IEnumerable<char> text = EnumerateCharsBetween(items, nextBreakIndex, followingBreak);

            int count = CalculateChevronCount(text);
            return count;
        }

        /// <summary>How many chevrons are there in the beginning of the previous line.</summary>
        /// <param name="currentLineBreakIndex">Break at the end of the wanted line.</param>
        public int FindPreviousLineChevronCount(List<PostPartBase> items, int currentLineBreakIndex)
        {
            int precedingBreak = FindNextPart<LineBreakPart, ParagraphStartPart>(items, currentLineBreakIndex, true);
            IEnumerable<char> text = EnumerateCharsBetween(items, precedingBreak, currentLineBreakIndex);

            int count = CalculateChevronCount(text);
            return count;
        }

        private int CalculateChevronCount(IEnumerable<char> text)
        {
            int found = 0;
            foreach (char c in text)
            {
                if (c == '>')
                    found++;
                else if (Char.IsWhiteSpace(c))
                    continue;
                else
                    break;
            }

            return found;
        }

        /// <summary>Makes sure chevrons are separated with spaces.</summary>
        /// <param name="chevronCount">How many chevrons are there.</param>
        public static string InsertSpacesAfterChevrons(string text)
        {
            for (int i = 0; i < text.Length - 1; i += 2)
            {
                if (text[i] == '>')
                {
                    if (!Char.IsWhiteSpace(text[i + 1]))
                    {
                        // Insert space.
                        text = text.Substring(0, i + 1) + " " + text.Substring(i + 1);
                    }
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
