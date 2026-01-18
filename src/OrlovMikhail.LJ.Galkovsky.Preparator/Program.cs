using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using Serilog;
using OrlovMikhail.LJ.Galkovsky.Tools;
using OrlovMikhail.LJ.Grabber.Extractor.FolderNamingStrategy;

namespace OrlovMikhail.LJ.Galkovsky.Preparator
{
    class Program
    {
        private const string galkovskyFormat = "GalkovskyLJ_{0}.asc";

        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();

            IFileSystem fs = new FileSystem();

            Dictionary<string, string> argsDic = ConsoleTools.ArgumentsToDictionary(args);
            string root = argsDic.GetExistingOrDefault("root");
            if (String.IsNullOrEmpty(root))
            {
                Log.Error("No -root specified.");
                PrintUsage();
                return;
            }

            // Delete existing.
            string[] existingFiles = Directory.GetFiles(root, String.Format(galkovskyFormat, "*"));
            foreach (string existingFile in existingFiles)
                File.Delete(existingFile);

            // All available fragment files by entry ids.
            IGalkovskyFolderNamingStrategy ns = new GalkovskyFolderNamingStrategy();
            IFragmentHelper fragHelper = new FragmentHelper(ns);
            Dictionary<long, FragmentInformation> fragsById = fragHelper.GetAllFragmentPaths(fs, root);

            // Splits from split.txt
            ISplitLoader splitLoader = new SplitLoader(root);
            Split[] splits = splitLoader.LoadSplits(fs);

            for (int i = 0; i < splits.Length; i++)
            {
                Split current = splits[i];

                FragmentInformation[] allDumps = fragHelper.SelectValuesFor(current, fragsById).ToArray();

                // All relative paths for this split record.
                string[] matchingPaths = allDumps.Select(z => z.RelativeFragmentPath)
                          .Where(p => !String.IsNullOrWhiteSpace(p))
                          .ToArray();

                if (matchingPaths.Length == 0)
                    continue;

                Log.Information("Writing {Name}...", current.Name);

                StringBuilder sb = new StringBuilder();
                string title = String.Format("ЖЖ Галковского. Часть {0}. {1}", current.Name, current.Description);
                sb.AppendLine(title);
                sb.AppendLine(new string('=', title.Length));
                sb.AppendLine(":doctype: book");
                sb.AppendLine(":docinfo:");
                sb.AppendLine(":toc:");
                sb.AppendLine(":toclevels: 2");
                sb.AppendLine(":imagesdir: .");

                // Lead.
                sb.AppendLine();
                FragmentInformation firstDump = allDumps.First();
                FragmentInformation lastDump = allDumps.Last();
                string lead = MakeLead(splits, current, firstDump, lastDump);
                sb.AppendLine(lead);

                foreach (string matchingPath in matchingPaths)
                {
                    sb.AppendLine();
                    sb.AppendFormat("include::{0}[]", matchingPath);
                    sb.AppendLine();
                }

                string content = sb.ToString();
                string fileName = String.Format(galkovskyFormat, current.Name);
                string fileLocation = fs.Path.Combine(root, fileName);
                fs.File.WriteAllText(fileLocation, content, new UTF8Encoding(true));
            }
        }

        private static string MakeLead(Split[] splits, Split current, FragmentInformation firstDump, FragmentInformation lastDump)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("****");
            sb.AppendLine("Записи разбиты по книгам следующим образом:");
            sb.AppendLine();

            sb.AppendLine("[grid=\"none\",cols=\"^3,^5,<10\"]");
            sb.AppendLine("|====");

            for (int i = 0; i < splits.Length; i++)
            {
                Split s = splits[i];
                bool isCurrent = s.Name == current.Name;
                string formatter = isCurrent ? "**" : String.Empty;

                sb.AppendLine(String.Format("|{4}{0}{4}|{4}{1}&#8211;{2}{4}|{4}{3}{4}", s.Name,
                    firstDump.GalkovskyEntryKey,
                    lastDump.GalkovskyEntryKey,
                    s.Description,
                    formatter));
            }

            sb.AppendLine("|====");
            sb.Append("****");

            return sb.ToString();
        }

        static void PrintUsage()
        {
            Console.WriteLine("Usage: prepare -root <folder>");
            Console.WriteLine();
            Console.WriteLine("Arguments:");
            Console.WriteLine("  -root      Root folder containing the book files");
        }
    }
}
