using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.BookWriter
{
    /// <summary>Makes sure formatting starts and ends inside
    /// one paragraph.</summary>
    public class FormattingSpanningProcessor : ProcessorBase
    {
        protected internal override void ProcessInternal(List<PostPartBase> items)
        {
            for(int i = -1; i < items.Count; i++)
            {
                if(i >= 0)
                {
                    ParagraphStartPart p1 = items[i] as ParagraphStartPart;
                    if(p1 == null)
                        continue;
                }

                // This is what the block starts with.
                FormattingStartBasePart formattingStarter = (i < items.Count - 1 ? items[i + 1] : null) as FormattingStartBasePart;
                if(formattingStarter == null)
                    continue;

                // Block needs to end with this.
                FormattingEndBasePart needsToEndWith = null;
                if(formattingStarter is ItalicStartPart)
                    needsToEndWith = ItalicEndPart.Instance;
                else if(formattingStarter is BoldStartPart)
                    needsToEndWith = BoldEndPart.Instance;
                else
                    continue;

                int p2Index = FindNextPartIndex<ParagraphStartPart>(items, i);
                if(p2Index == -1)
                {
                    // This is the end.
                    return;
                }

                bool blockEndsWithIt = CheckIfBlockHasClosing(items, i + 1, p2Index, needsToEndWith.GetType());

                if(blockEndsWithIt)
                {
                    // OK, no need for this paragraph.
                    i = p2Index - 1;
                    continue;
                }

                // Insert formatting ending part.
                items.Insert(p2Index, needsToEndWith);
                p2Index++;

                // Next paragraph that is not an image. Images shouldn't be bothered.
                // Most likely p3Index = p2Index.
                int p3Index = FindNextNonImageParagraphStartPartIndex(items, p2Index);

                // Item after the next paragraph.
                PostPartBase following = items[p3Index + 1];
                if(following.GetType() != formattingStarter.GetType())
                {
                    // Formatting starter there!
                    items.Insert(p3Index + 1, formattingStarter);
                }

                // Go to that paragraph next time.
                i = p3Index - 1;
            }
        }

        private int FindNextNonImageParagraphStartPartIndex(List<PostPartBase> items, int p2Index)
        {
            do
            {
                // Now let's make sure next is the same starting.
                if(items[p2Index + 1] is MultimediaBasePart)
                {
                    // Don't work with images. Go to next paragraph.
                    p2Index = FindNextPartIndex<ParagraphStartPart>(items, p2Index);
                    if(p2Index < 0)
                        return -1;

                    continue;
                }

                break;
            } while(true);

            return p2Index;
        }

        /// <summary>Checks that formatting closes.</summary>
        /// <param name="startP">Beginning.</param>
        /// <param name="nextP">Item after the last one.</param>
        /// <param name="ender">What type closes the formatting.</param>
        private bool CheckIfBlockHasClosing(List<PostPartBase> items, int startP, int nextP, Type ender)
        {
            Type starter = items[startP].GetType();

            int count = 0;

            for(int p = startP; p < nextP; p++)
            {
                PostPartBase current = items[p];
                if(current.GetType() == starter)
                    count++;
                else if(current.GetType() == ender)
                    count--;
            }

            return count <= 0;
        }
    }
}
