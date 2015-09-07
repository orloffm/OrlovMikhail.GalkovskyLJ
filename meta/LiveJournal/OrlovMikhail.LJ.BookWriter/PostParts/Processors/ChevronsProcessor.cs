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
            int previousDepth = 0;

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

                // All text parts.
                RawTextPostPart[] textPartsBetween = EnumerateTextPartsBetween(items, i, nextBreak).ToArray();

                // Replace stuff like "-- text --" with "> text".
                ReplaceSpecialQuotationWithChevrons(textPartsBetween);

                int chevronsFound = RemoveChevrons(textPartsBetween);

                // Is depth the same as it was?
                if (chevronsFound == previousDepth)
                {
                    // It's the same depth.
                    if (chevronsFound == 0)
                    {
                        // Not a quote at all.
                        // New block can as well be a line break.
                    }
                    else
                    {
                        // Make sure the new block is the proper paragraph start.
                        if (i >= 0 && items[i] is LineBreakPart)
                        {
                            // We remove line breaks between quotations of the same depth.
                            items[i] = new RawTextPostPart(" ");
                        }
                    }
                }
                else
                {
                    // Quotation depth changed.
                    if (i >= 0)
                    {
                        // Thus the break must be a paragraph start.
                        items[i] = new ParagraphStartPart(chevronsFound);
                    }
                    else
                    {
                        // That's the first items. We should start the quotation here.
                        items.Insert(0, new ParagraphStartPart(chevronsFound));
                        i++;
                        nextBreak++;
                    }

                    previousDepth = chevronsFound;
                }

                if (chevronsFound != 0)
                {
                    // Remove all formatting from the block.
                    RemoveFormatters(items, i, nextBreak);
                }
            }
        }

        /// <summary>Removes formatters.</summary>
        private void RemoveFormatters(List<PostPartBase> items, int before, int after)
        {
            for (int i = after - 1; i > before; i--)
            {
                if (items[i] is FormattingBasePart)
                    items.RemoveAt(i);
            }
        }

        public static void ReplaceSpecialQuotationWithChevrons(RawTextPostPart[] textPartsBetween)
        {
            if (textPartsBetween.Length == 0)
                return;

            Tuple<string, string>[] specialSelectors = new[]
            {
                Tuple.Create("--", "--"),
                Tuple.Create(@"//", @"//"),
                Tuple.Create(@"\\", @"\\"),
                Tuple.Create(@"\", @"\"),
            };

            int totalTextLength = textPartsBetween.Sum(z => z.Text.Length);

            foreach (var tuple in specialSelectors)
            {
                RawTextPostPart a = textPartsBetween[0];
                RawTextPostPart z = textPartsBetween[textPartsBetween.Length - 1];

                string begin = tuple.Item1;
                string end = tuple.Item2;

                int adornersLength = begin.Length + end.Length;

                // So, is this text adorned by our special formatters?
                bool isAdorned = a.Text.StartsWith(begin) && z.Text.EndsWith(end) && adornersLength < totalTextLength;

                if(isAdorned)
                {
                    // Remove this formatter and prepend a chevron.
                    a.Text = "> " + a.Text.Substring(begin.Length);
                    z.Text = z.Text.Substring(0, z.Text.Length - end.Length);
                }
            }
        }

        public static int RemoveChevrons(RawTextPostPart[] textPartsBetween)
        {
            int chevronCount = 0;
            bool reachedBody = false;

            for (int p = 0; p < textPartsBetween.Length; p++)
            {
                RawTextPostPart rtpp = textPartsBetween[p];
                for (int i = 0; i < rtpp.Text.Length; i++)
                {
                    char c = rtpp.Text[i];

                    if (c == '>')
                        chevronCount++;
                    else if (Char.IsWhiteSpace(c))
                        continue;
                    else
                    {
                        // OK, not a chevron.
                        rtpp.Text = rtpp.Text.Substring(i);
                        reachedBody = true;

                        // Clear text in previous items.
                        for (int q = 0; q < p; q++)
                            textPartsBetween[q].Text = String.Empty;

                        break;
                    }
                }

                if (reachedBody)
                    break;
            }

            return chevronCount;
        }
    }
}
