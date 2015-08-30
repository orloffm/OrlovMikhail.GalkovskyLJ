using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using OrlovMikhail.Tools;

namespace OrlovMikhail.LJ.Galkovsky.Preparator
{
    class Program
    {
        static readonly ILog log = LogManager.GetLogger(typeof(Program));

        private const string splitFileName = "split.txt";
        private const string galkovskyFormat = "GalkovskyLJ_{0}.asc";

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            IFileSystem fs = new FileSystem();

            Dictionary<string, string> argsDic = ConsoleTools.ArgumentsToDictionary(args);
            string root = argsDic.GetExistingOrDefault("root");
            if (String.IsNullOrEmpty(root))
            {
                log.Error("No \root specified.");
                return;
            }

            // Splits.
            string[] lines = fs.File.ReadAllLines(fs.Path.Combine(root, splitFileName));
            Split[] splits = lines.Select(z => z.Split('\t'))
                .Select(z => new Split()
                {
                    Name = z[0],
                    From = int.Parse(z[1]),
                    Description = z[2]
                }).ToArray();
            for (int i = 0; i < splits.Length - 1; i++)
                splits[i].To = splits[i + 1].From - 1;

            // All available files.
            DirectoryInfoBase rootInfo = fs.DirectoryInfo.FromDirectoryName(root);
            FileInfoBase[] fragments = rootInfo.EnumerateFiles("fragment.asc", SearchOption.AllDirectories).ToArray();
            List<Tuple<int, string>> relativePaths = new List<Tuple<int, string>>();
            foreach (FileInfoBase fragment in fragments)
            {
                int number = int.Parse(fragment.Directory.Name);
                log.Info(rootInfo.FullName);
                log.Info(fragment.FullName);
                string relativePath = IOTools.MakeRelativePath(rootInfo, fragment);

                relativePaths.Add(Tuple.Create(number, relativePath));
            }

            foreach (Split s in splits)
            {
                log.InfoFormat("Writing {0}...", s.Name);

                StringBuilder sb = new StringBuilder();
                string title = "ЖЖ Галковского. " + s.Description;
                sb.AppendLine(title);
                sb.AppendLine(new string('=', title.Length));
                sb.AppendLine(":doctype: book");
                sb.AppendLine(":docinfo:");
                sb.AppendLine(":toc:");
                sb.AppendLine(":toclevels: 2");

                string[] matchingPaths = relativePaths
                    .Where(z => z.Item1 >= s.From && (!s.To.HasValue || z.Item1 <= s.To.Value))
                    .OrderBy(z => z.Item1)
                    .Select(z => z.Item2).ToArray();

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
    }

    class Split
    {
        public string Name { get; set; }
        public int From { get; set; }
        public int? To { get; set; }
        public string Description { get; set; }
    }
}
