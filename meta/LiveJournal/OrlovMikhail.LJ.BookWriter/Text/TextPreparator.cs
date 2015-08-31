using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.BookWriter
{
    public abstract class TextPreparator : ITextPreparator
    {
        /// <summary>These require no-break after.</summary>
        string[] noBrKeyWords = { "вне", "под", "над", "из", "по", "на", "со", "но", "за", "и", "в", "а", "к", "с", "о", "у" };

        List<Tuple<string, string>> strings;

        private string nobr;
        private string laquo;
        private string raquo;
        private string emdash;

        protected abstract string GetNoBreakString();
        protected abstract string GetLaquoString();
        protected abstract string GetRaquoString();
        protected abstract string GetMDashString();

        public TextPreparator()
        {
            strings = null;
        }

        void Initialize()
        {
            nobr = GetNoBreakString();
            laquo = GetLaquoString();
            raquo = GetRaquoString();
            emdash = GetMDashString();

            strings = new List<Tuple<string, string>>();

            Action<string, string> add = (_f, _t) => { strings.Add(Tuple.Create(_f, _t)); };

            AddPreRegeces(add);
            AddMainRegeces(add);
            AddPostRegeces(add);
        }

        public string Prepare(string text)
        {
            if (strings == null)
                Initialize();

            string work = text;

            work = ReplaceByRegeces(work);
            work = InsertNoBR(work);

            return work;
        }

        #region by regeces
        protected virtual void AddPostRegeces(Action<string, string> add)
        {

        }

        protected virtual void AddPreRegeces(Action<string, string> add)
        {

        }

        private void AddMainRegeces(Action<string, string> add)
        {
            // начало и конец строки
            add("^\"", laquo);
            add("\"$", raquo);
            add("\"\r\n", @"\" + raquo + @"\r\n");
            add("\"\n", @"\" + raquo + @"\n");

            // остальные кавычки
            add(" \"", " " + laquo);
            add("“ ", raquo);
            add(" „", laquo);
            add("«", laquo);
            add("” ", raquo + " ");
            add("\" ", raquo + " ");
            add("»", raquo);
            add("\":", raquo + ":");
            add(@"""\.", raquo + ".");
            add(@"\.""", raquo + ".");
            add(@"""\?", raquo + "?");
            add(@"""\!", raquo + "!");
            add("\",", raquo + ",");

            add(@"""\)", raquo + ")");
            add(@"\(""", "(" + raquo);

            add(@"\)""", ")" + laquo);
            add(@"""\(", raquo + "(");

            add("!\"", @"!" + raquo);
            add(@"\.\.\.""", @"..." + raquo);

            // тире
            add(" - ", nobr + emdash + " ");
            add(" – ", nobr + emdash + " ");
            add(" — ", nobr + emdash + " ");
            add(" -- ", nobr + emdash + " ");
            add(" --- ", nobr + emdash + " ");
            add("^- ", emdash + " ");
            add("^– ", emdash + " ");
            add("^— ", emdash + " ");
            add("^-- ", emdash + " ");
            add("^--- ", emdash + " ");
        }

        string ReplaceByRegeces(string work)
        {
            for (int i = 0; i < strings.Count; i++)
            {
                var tuple = strings[i];
                string a = tuple.Item1;
                string b = tuple.Item2;

                work = Regex.Replace(work, a, b, RegexOptions.Multiline | RegexOptions.Singleline);
            }
            return work;
        }
        #endregion

        #region nobr
        string InsertNoBR(string work)
        {
            StringBuilder sb = new StringBuilder(work.Length);

            for (int i = 0; i < work.Length; i++)
            {
                int foundTill = PositiveLookAhead(work, i);
                if (foundTill >= 0)
                {
                    // OK, add characters, continue;
                    for (int f = i; f < foundTill; f++)
                        sb.Append(work[f]);
                    sb.Append(nobr);
                    i = foundTill;
                    continue;
                }

                sb.Append(work[i]);
            }


            return sb.ToString();
        }

        /// <summary>Searches for </summary>
        /// <param name="work">Source string.</param>
        /// <param name="start">Where to look for tilde candidate from.</param>
        int PositiveLookAhead(string work, int start)
        {
            bool hasSpaceTokenAtStart = Char.IsWhiteSpace(work, start);

            // It must be space after initial character.
            if (start > 0 && !hasSpaceTokenAtStart)
                return -1;

            int wordMustStartAt = hasSpaceTokenAtStart ? start + 1 : 0;

            if (work.Length < wordMustStartAt + 2)
                return -1;

            for (int i = 0; i < noBrKeyWords.Length; i++)
            {
                if (work.IndexOf(noBrKeyWords[i], wordMustStartAt, StringComparison.OrdinalIgnoreCase) != wordMustStartAt)
                    continue;

                int afterIndex = wordMustStartAt + noBrKeyWords[i].Length;
                if (afterIndex >= work.Length)
                    continue;

                bool isSpace = work[afterIndex] == ' ';
                if (!isSpace)
                    continue;

                return afterIndex;
            }

            return -1;
        }
        #endregion
    }
}
