using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.BookWriter
{
    public class ListsDisablerProcessor : ProcessorBase
    {
        private const string lineStartRegexString = @"^[*\da-zА-ЯA-Zа-я]*\.";
        private readonly Regex _lineStartRegex;

        public ListsDisablerProcessor()
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
                    bool previousIsBreak = previous == null || previous is NewBlockStartBasePart;

                    // Prepend numbers with {empty} to disable lists.
                    if(previousIsBreak)
                    {
                        bool startsWithNumberAndDot = _lineStartRegex.IsMatch(rtpp.Text);
                        if(startsWithNumberAndDot)
                            rtpp.Text = "{empty}" + rtpp.Text;
                    }
                }
            }
        }
    }
}
