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

        public override string PrepareUsername(string work)
        {
            return EscapeFormatters(work);
        }

        /// <summary>Escapes _ and * in strings so that
        /// they appear in the final document the way they are
        /// written. This is done by putting \ in front of the starting
        /// element. We do it only if there is an ending element
        /// in the word of the same kind. The advanced part
        /// is that a punctuation immediately after the word
        /// doesn't matter.</summary>
        public static string EscapeFormatters(string work)
        {
            if (String.IsNullOrWhiteSpace(work))
                return work;

            StringBuilder sb = new StringBuilder();

            char[] keys = new char[] { '_', '*' };
            char[] punctuation = new char[] { '!', '?', '.', ',' };

            char[] cs = work.ToCharArray();
            for (int startIndex = 0; startIndex < cs.Length; startIndex++)
            {
                char ci = cs[startIndex];

                // Skip whitespace.
                if (Char.IsWhiteSpace(ci))
                {
                    sb.Append(ci);
                    continue;
                }

                int lastCharacterIndex = FindLastWordCharacter(cs, startIndex);

                // Skip punctuation in the beginning.
                for (int p = startIndex; p <= lastCharacterIndex; p++)
                {
                    char cp = cs[p];
                    if (!punctuation.Contains(cp))
                        break;

                    startIndex++;
                    sb.Append(cp);
                }

                // We put removed punctuation here.
                List<char> punctuationEnd = new List<char>();
                int wordEnd = lastCharacterIndex;
                for (int p = lastCharacterIndex; p > startIndex; p--)
                {
                    char cp = cs[p];
                    if (!punctuation.Contains(cp))
                        break;

                    wordEnd--;
                    punctuationEnd.Insert(0, cp);
                }

                // Now we process the word.
                int a = startIndex, b = wordEnd;
                for (; a < b; a++ )
                {
                    char ca = cs[a];

                    bool isPunctuation = keys.Contains(ca);
                    if (!isPunctuation)
                        break;

                    if (ca != cs[b])
                    {
                        sb.Append(ca);
                        continue;
                    }

                    // OK, a character matches the last one.
                    b--;
                    sb.Append(@"\");
                    sb.Append(ca);
                }

                for (; a <= wordEnd; a++)
                {
                    // Add the rest of the word.
                    sb.Append(cs[a]);
                }

                // Will continue with the next whitespace item.
                sb.Append(punctuationEnd.ToArray());
                startIndex = lastCharacterIndex;
            }

            return sb.ToString();
        }


        private static int FindLastWordCharacter(char[] cs, int startIndex)
        {
            for (int i = startIndex + 1; i < cs.Length; i++)
            {
                if (Char.IsWhiteSpace(cs[i]))
                    return i - 1;
            }

            return cs.Length - 1;
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
