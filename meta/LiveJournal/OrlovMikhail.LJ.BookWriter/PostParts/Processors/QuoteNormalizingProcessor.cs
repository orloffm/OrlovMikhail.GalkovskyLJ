using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.BookWriter
{
    public class QuoteNormalizingProcessor : ProcessorBase
    {
        protected internal override void ProcessInternal(List<PostPartBase> items)
        {
            List<ParagraphStartPart> psps = new List<ParagraphStartPart>();
            List<int> zeroBlocksIndeces = new List<int>();

            // Find paragraphs and zero indeces.
            for (int i = 0; i < items.Count; i++)
            {
                ParagraphStartPart item = items[i] as ParagraphStartPart;
                if (item == null)
                    continue;

                if (item.QuotationLevel == 0)
                    zeroBlocksIndeces.Add(psps.Count);
                psps.Add(item);
            }

            // For easy processing.
            zeroBlocksIndeces.Insert(0, -1);
            zeroBlocksIndeces.Add(items.Count);

            // Now, for all parts.
            for (int i = 0; i < zeroBlocksIndeces.Count - 1; i++)
            {
                int pre = zeroBlocksIndeces[i];
                int post = zeroBlocksIndeces[i + 1];

                int length = post - pre -1;
                if (length < 1)
                    continue;
                
                ParagraphStartPart[] series = psps.Skip(pre + 1).Take(length).ToArray();

                int[] numbersInTheSeries = series.Select(z => z.QuotationLevel).ToArray();
                int[] processedNumbers;
                bool changed = FixNumbers(numbersInTheSeries, out processedNumbers);

                if (changed)
                {
                    // Update numbers.
                    for (int q = 0; q < numbersInTheSeries.Length; q++)
                        series[q].QuotationLevel = processedNumbers[q];
                }
            }
        }

        public static bool FixNumbers(int[] numbersInTheSeries, out int[] processedNumbers)
        {
            bool changed = false;

            if (numbersInTheSeries.Any(z => z <= 0))
                throw new ArgumentException();

            processedNumbers = numbersInTheSeries.ToArray();
            if (numbersInTheSeries.Length == 0)
                return false;

            int maxLevel = numbersInTheSeries.Max();
            for (int level = 1; level <= maxLevel;)
            {
                bool any = processedNumbers.Any(z => z == level);
                if (!any)
                {
                    // Reduce all by 1.
                    for (int i = 0; i < processedNumbers.Length; i++)
                    {
                        int value = processedNumbers[i];
                        if (value > level)
                            processedNumbers[i] = value - 1;
                    }
                    maxLevel--;
                    changed = true;

                    // Restart.
                    continue;
                }

                // OK, some have this level.
                level++;
            }

            return changed;
        }
    }
}
