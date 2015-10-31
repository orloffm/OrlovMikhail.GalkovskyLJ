using System;
using System.Linq;
using System.Text.RegularExpressions;
using log4net;
using OrlovMikhail.LJ.Grabber;

namespace OrlovMikhail.LJ.Galkovsky
{
    public class GalkovskySubfolderGetter : ISubfolderByEntryGetter
    {
        static readonly ILog log = LogManager.GetLogger(typeof(Worker));

        public bool TryExtractNumberFromSubject(string s, out int num)
        {
            if (TryExtractUsualNumber(s, out num))
                return true;

            return TryExtractPSNumber(s, out num);
        }

        public string GetSubfolderByEntrySubject(string s)
        {
            int pe;
            if(!TryExtractNumberFromSubject(s,out pe))
            {
                string error = String.Format("Cannot parse number from subject \"{0}\".", s);
                log.Error(error);
                throw new NotSupportedException(error);
            }

            return pe.ToString().PadLeft(4, '0');
        }

        private bool TryExtractUsualNumber(string subject, out int pe)
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

        private bool TryExtractPSNumber(string subject, out int pe)
        {
            Match m = Regex.Match(subject, @"^PS-(\d+)");
            if (!m.Success)
            {
                pe = 0;
                return false;
            }

            string num = m.Groups[1].Value;
            int value = int.Parse(num);

            // Pad with three zeroes.
            pe = 949 + value;
            return true;
        }
    }
}
