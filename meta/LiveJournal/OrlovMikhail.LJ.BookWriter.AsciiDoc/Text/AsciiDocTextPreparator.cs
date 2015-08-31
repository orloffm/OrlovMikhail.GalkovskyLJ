using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.BookWriter.AsciiDoc
{
    public class AsciiDocTextPreparator : TextPreparator
    {
        protected override string GetNoBreakString() { return "{nbsp}"; }

        protected override string GetLaquoString() { return "«"; }

        protected override string GetRaquoString() { return "»"; }

        protected override string GetMDashString() { return "&#8212;"; }

        protected override void AddPreRegeces(Action<string, string> add)
        {
            add(@"\+", "_+_");
            add(@"№", "N");
        }
    }
}
