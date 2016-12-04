using System;
using System.Linq;
using System.Text.RegularExpressions;
using OrlovMikhail.LJ.Grabber;

namespace OrlovMikhail.LJ.Galkovsky
{
    public class GalkovskyFolderNamingStrategy : IFolderNamingStrategy, IGalkovskyFolderNamingStrategy
    {
        public string GetGalkovskyEntryKey(string subject)
        {
            string partBeforeDot = new string(subject.TakeWhile(c => c != '.' && c != ',').ToArray()).Trim();

            if (partBeforeDot == "ДЕЛИКАТНАЯ ТЕМА")
                return "3";
            else if (partBeforeDot == "УТОЧНЕНИЕ ИНТЕРЕСОВ")
                return "2";
            else if (partBeforeDot == "ПРОБА ПЕРА")
                return "1";

            bool extractedAnIdentifier = !(partBeforeDot.Length == 0 || partBeforeDot.Length == subject.Length);
            if (!extractedAnIdentifier)
                throw new NotSupportedException(string.Format("Subject \"{0}\" does not contain a short part before dot in the beginning.", subject));

            // Fixes A and B typed as Russian letters.
            string latinized = Latinize(partBeforeDot);

            return latinized;
        }

        private string Latinize(string partBeforeDot)
        {
            return partBeforeDot.Replace('А', 'A')
                .Replace('В', 'B');
        }

        public bool TryGetSubfolderByEntry(IEntryBase entry, out string sf)
        {
            string entryKey = GetGalkovskyEntryKey(entry.Subject);

            int num;
            // Usual old entries with consecutive numbers.
            if (TryExtractUsualNumberFromEntryKey(entryKey, out num))
            {
                // This is a numbered post.
                string ret = num.ToString().PadLeft(4, '0');
                {
                    sf = ret;
                    return true;
                }
            }

            // PS entries, still consecutive.
            if (TryExtractPSNumberFromEntryKey(entryKey, out num))
            {
                string ret = "PS-" + num.ToString().PadLeft(3, '0');
                {
                    sf = ret;
                    return true;
                }
            }

            // Fallback.
            sf = GetDefaultFolderNameWithIdentifier(entry.Id, entryKey);
            return true;
        }

        private string GetDefaultFolderNameWithIdentifier(long entryId, string entryKey)
        {
            string defaultFolderName = DefaultFolderNamingStrategy.GetDefaultFolderName(entryId);

            string ret = String.Format("{0}_{1}", defaultFolderName, entryKey);
            return ret;
        }

        private bool TryExtractUsualNumberFromEntryKey(string subject, out int pe)
        {
            string numFromSubject = new string(subject.ToCharArray().TakeWhile(Char.IsNumber).ToArray());

            int num;
            if (!int.TryParse(numFromSubject, out num) || num < 1 || num > 2000)
            {
                pe = 0;
                return false;
            }
            else
            {
                pe = num;
                return true;
            }
        }

        private bool TryExtractPSNumberFromEntryKey(string subject, out int pe)
        {
            Match m = Regex.Match(subject, @"^PS-(\d+)");
            if (!m.Success)
            {
                pe = 0;
                return false;
            }

            string num = m.Groups[1].Value;
            pe = int.Parse(num);

            return true;
        }
    }
}