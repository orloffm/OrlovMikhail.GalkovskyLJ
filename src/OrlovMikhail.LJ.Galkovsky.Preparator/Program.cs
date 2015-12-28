using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using OrlovMikhail.Tools;
using OrlovMikhail.LJ.Grabber;

namespace OrlovMikhail.LJ.Galkovsky.Preparator
{
    class Program
    {
        static readonly ILog log = LogManager.GetLogger(typeof(Program));

        private const string galkovskyFormat = "GalkovskyLJ_{0}.asc";

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            IFileSystem fs = new FileSystem();

            Dictionary<string, string> argsDic = ConsoleTools.ArgumentsToDictionary(args);
            string root = argsDic.GetExistingOrDefault("root");
            if (String.IsNullOrEmpty(root))
            {
                log.Error(@"No \root specified.");
                return;
            }

            // Delete existing.
            string[] existingFiles = Directory.GetFiles(root, String.Format(galkovskyFormat, "*"));
            foreach (string existingFile in existingFiles)
                File.Delete(existingFile);

            // All available files.
            int? maxFound;
            GalkovskyNumberingStrategy ns = new GalkovskyNumberingStrategy();
            List<Tuple<int, string>> relativePaths = FragmentHelper.GetAllFragmentPaths(fs, ns, root, out maxFound);

            // Splits.
            Split[] splits = Split.LoadSplits(fs, root, maxFound);

            foreach (Split s in splits)
            {
                string lead = MakeLead(splits, s, ns);

                string[] matchingPaths = relativePaths
                    .Where(z => z.Item1 >= s.From && (!s.To.HasValue || z.Item1 <= s.To.Value))
                    .OrderBy(z => z.Item1)
                    .Select(z => z.Item2).ToArray();

                if (matchingPaths.Length == 0)
                    continue;

                log.InfoFormat("Writing {0}...", s.Name);

                StringBuilder sb = new StringBuilder();
                string title = String.Format("ЖЖ Галковского. Часть {0}. {1}", s.Name, s.Description);
                sb.AppendLine(title);
                sb.AppendLine(new string('=', title.Length));
                sb.AppendLine(":doctype: book");
                sb.AppendLine(":docinfo:");
                sb.AppendLine(":toc:");
                sb.AppendLine(":toclevels: 2");
                sb.AppendLine(":imagesdir: .");

                // Shared lead.
                sb.AppendLine();
                sb.AppendLine(lead);

                foreach (string matchingPath in matchingPaths)
                {
                    sb.AppendLine();
                    sb.AppendFormat("include::{0}[]", matchingPath);
                    sb.AppendLine();
                }

                string content = sb.ToString();
                string fileName = String.Format(galkovskyFormat, s.Name);
                string fileLocation = fs.Path.Combine(root, fileName);
                fs.File.WriteAllText(fileLocation, content, new UTF8Encoding(true));
            }
        }

        private static string MakeLead(Split[] splits, Split current, INumberingStrategy ns)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("****");
            sb.AppendLine("Записи разбиты по книгам следующим образом:");
            sb.AppendLine();

            sb.AppendLine("[grid=\"none\",cols=\"^1,^2,<10\"]");
            sb.AppendLine("|====");

            for (int i = 0; i < splits.Length; i++)
            {
                Split s = splits[i];
                bool isCurrent = s.Name == current.Name;
                string formatter = isCurrent ? "**" : "";

                string fromString = ns.GetFriendlyTitleBySortNumber(s.From);
                string toString = ns.GetFriendlyTitleBySortNumber(s.To);

                sb.AppendLine(String.Format("|{4}{0}{4}|{4}{1}&#8211;{2}{4}|{4}{3}{4}", s.Name, fromString, toString, s.Description, formatter));
            }

            sb.AppendLine("|====");
            sb.Append("****");

            return sb.ToString();
        }
    }
}
