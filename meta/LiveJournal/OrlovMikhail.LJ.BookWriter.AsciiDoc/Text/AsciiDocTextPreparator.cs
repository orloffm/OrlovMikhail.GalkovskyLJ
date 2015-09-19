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

        /// <summary>Escapes _ and * - one per word.</summary>
        protected override string Preprocess(string work)
        {
            return EscapeFormatters(work);
        }

        public static string EscapeFormatters(string work)
        {
            StringBuilder sb = new StringBuilder();

            char[] keys = new char[] {'_', '*'};
            Dictionary<char, bool> founds = keys.ToDictionary(z => z, q => false);

            foreach (char c in work)
            {
                if (char.IsWhiteSpace(c))
                    founds = keys.ToDictionary(z => z, q => false);
                else if (keys.Contains(c))
                {
                    if (!founds[c])
                    {
                        sb.Append('\\');
                        founds[c] = true;
                    }
                }

                sb.Append(c);
            }

            return sb.ToString();
        }

        protected override void AddPreRegeces(Action<string, string> add)
        {
            add(@"\+$", "\\+$");

            // Prevent headers.
            add(@"^==", "{empty}==");
            add(@"№", "N");
        }
    }
}
