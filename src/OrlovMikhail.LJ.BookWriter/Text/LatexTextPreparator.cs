using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.BookWriter
{
    public class LatexTextPreparator : TextPreparator, ITextPreparator
    {
        protected override string GetNoBreakString() { return "~"; }

        protected override string GetLaquoString() { return @"\лк "; }

        protected override string GetRaquoString() { return @"\пк "; }

        protected override string GetMDashString() { return @"\мт "; }

        protected override void AddPreRegeces(Action<string, string> add)
        {
            // общее для теха
            add(@"\\", @"\textbackslash");
            add("_", @"\_");
            add("&", @"\&");
            add("%", @"\%");
        }

        protected override void AddPostRegeces(Action<string, string> add)
        {
            add(@"\.\\пк", @"\пк.");

            // многоточие
            add(@"\.\.\.", @"\мт ");
            add("…", @"\мт ");
            add(@" \\мт", @"\мт");
            add(@"\\мт  ", @"\мт ");
        }
    }
}
