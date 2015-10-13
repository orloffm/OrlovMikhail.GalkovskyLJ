using System;
using System.Linq;
using log4net;
using OrlovMikhail.LJ.Grabber;

namespace OrlovMikhail.LJ.Galkovsky
{
    public class GalkovskySubfolderGetter : ISubfolderByEntryGetter
    {
        static readonly ILog log = LogManager.GetLogger(typeof(Worker));

        public void GetSubfolderByFreshEntry(Entry e, out string subFolder, out string filename)
        {
            string numFromSubject = new string(e.Subject.ToCharArray().TakeWhile(Char.IsNumber).ToArray());
            int num;
            if (!int.TryParse(numFromSubject, out num) || num < 1 || num > 2000)
            {
                string error = String.Format("Cannot parse number from subject \"{0}\".", e.Subject);
                log.Error(error);
                throw new NotSupportedException(error);
            }
            subFolder = String.Format("{0}\\{1}", e.Date.Value.Year, num.ToString().PadLeft(4, '0'));

            filename = "dump.xml";
        }
    }
}
