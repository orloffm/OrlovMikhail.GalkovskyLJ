using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.BookWriter
{
    /// <summary>Prepends possible formatting issues in Asciidoc with an empty
    /// item. Other possible writers can just ignore this.</summary>
    public class ListsDisablerProcessor : ProcessorBase
    {
        private const string lineStartRegexPattern = @"^(?:[*\da-zА-ЯA-Zа-я]*\.|\[)";
        private readonly Regex _lineStartRegex;

        public ListsDisablerProcessor()
        {
            _lineStartRegex = new Regex(lineStartRegexPattern, RegexOptions.Compiled);
        }

        protected internal override void ProcessInternal(List<PostPartBase> items)
        {
            for(int i = 0; i < items.Count; i++)
            {
                RawTextPostPart rtpp = items[i] as RawTextPostPart;

                if(rtpp == null)
                    continue;

                // What should be trimmed?
                PostPartBase previous = (i > 0 ? items[i - 1] : null);
                bool previousIsBreak = previous == null || previous is NewBlockStartBasePart;

                // Prepend numbers with {empty} to disable lists.
                if(!previousIsBreak)
                    continue;

                bool shouldPrependWithEmpty = _lineStartRegex.IsMatch(rtpp.Text);
                if(!shouldPrependWithEmpty)
                    continue;

                items.Insert(i, EmptyPostPart.Instance);
                i++;
            }
        }
    }
}
