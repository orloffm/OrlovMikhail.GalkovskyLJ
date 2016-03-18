using System;
using System.Linq;
using System.Text.RegularExpressions;
using log4net;
using OrlovMikhail.LJ.Grabber;

namespace OrlovMikhail.LJ.Galkovsky
{
    public class GalkovskyNumberingStrategy : INumberingStrategy
    {
        static readonly ILog log = LogManager.GetLogger(typeof(GalkovskyNumberingStrategy));

        public string GetSubfolderByEntry(string s)
        {
            string subFolder;
            if (TryGetSubfolderByEntry(s, out subFolder))
                return subFolder;

            string error = String.Format("Cannot extract number from subject \"{0}\".", s);
            log.Error(error);
            throw new NotSupportedException(error);
        }

        public bool TryGetSubfolderByEntry(string s, out string sf)
        {
            int num;

            if (TryExtractUsualNumberFromSubject(s, out num))
            {
                // This is a numbered post.
                string ret = num.ToString().PadLeft(4, '0');
                {
                    sf = ret;
                    return true;
                }
            }

            if (TryExtractPSNumberFromSubject(s, out num))
            {
                string ret = "PS-" + num.ToString().PadLeft(3, '0');
                {
                    sf = ret;
                    return true;
                }
            }

            sf = null;
            return false;
        }

        /// <summary>Number (912 or 950) by folder (0912 or PS-001).</summary>
        public int GetSortNumberBySubfolder(string subfolder)
        {
            if (String.IsNullOrWhiteSpace(subfolder))
                throw new ArgumentException();

            int ret;

            // The usual number.
            if (int.TryParse(subfolder, out ret))
                return ret;

            // The PS number.
            if (subfolder.IndexOf("PS-", StringComparison.OrdinalIgnoreCase) == 0)
            {
                int psValue;
                if (int.TryParse(subfolder.Substring(3), out psValue))
                {
                    ret = 949 + psValue;
                    return ret;
                }
            }

            string error = String.Format("Cannot parse number from folder name \"{0}\".", subfolder);
            log.Error(error);
            throw new NotSupportedException(error);
        }

        /// <summary>(912 or PS-1) by sort number (912 or 950).</summary>
        public string GetFriendlyTitleBySortNumber(int? sortNumber)
        {
            if (sortNumber == null)
                return String.Empty;

            int i = sortNumber.Value;
            if (i <= 0)
                throw new ArgumentOutOfRangeException();

            if (i <= 949)
                return i.ToString();

            return "PS-" + (i - 949).ToString();
        }

        private bool TryExtractUsualNumberFromSubject(string subject, out int pe)
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

        private bool TryExtractPSNumberFromSubject(string subject, out int pe)
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
