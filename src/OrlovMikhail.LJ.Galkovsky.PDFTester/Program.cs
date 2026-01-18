using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using iText.Kernel.Pdf;
using Serilog;
using OrlovMikhail.LJ.Galkovsky.Tools;
using OrlovMikhail.LJ.Grabber.Extractor.FolderNamingStrategy;

namespace OrlovMikhail.LJ.Galkovsky.PDFTester
{
    class Program
    {
        const string SourcePdf = "output\\GalkovskyLJ_{0}.A5.pdf";

        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();

            Dictionary<string, string> argsDic = ConsoleTools.ArgumentsToDictionary(args);

            if (!argsDic.TryGetValue("root", out string root) || string.IsNullOrWhiteSpace(root))
            {
                Log.Error("Missing required argument: -root");
                PrintUsage();
                return;
            }

            IFileSystem fs = new FileSystem();

            // All available fragment files by entry ids.
            IGalkovskyFolderNamingStrategy ns = new GalkovskyFolderNamingStrategy();
            IFragmentHelper fragHelper = new FragmentHelper(ns);
            Dictionary<long, FragmentInformation> fragsById = fragHelper.GetAllFragmentPaths(fs, root);

            // Splits.
            ISplitLoader splitLoader = new SplitLoader(root);
            Split[] splits = splitLoader.LoadSplits(fs);

            for (int i = 0; i < splits.Length; i++)
            {
                Split s = splits[i];

                string relativePath = String.Format(SourcePdf, s.Name);
                string absolutePath = Path.Combine(root, relativePath);

                if (!File.Exists(absolutePath))
                {
                    Log.Error("File for split {Split} doesn't exist.", s);
                    continue;
                }

                string[] entryKeysInPdf;
                string mb = (new FileInfo(absolutePath).Length / 1024d / 1024d).ToString("#.##");
                int pages;
                try
                {
                    using (PdfDocument pdfDoc = new PdfDocument(new PdfReader(absolutePath)))
                    {
                        pages = pdfDoc.GetNumberOfPages();
                        var outlines = pdfDoc.GetOutlines(true);
                        entryKeysInPdf = GetGalkovskyEntryKeys(outlines, ns);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "File for split {Split} yielded an error.", s);
                    continue;
                }

                FragmentInformation[] allDumps = fragHelper.SelectValuesFor(s, fragsById).ToArray();
                var withFragments = allDumps.Where(z => !String.IsNullOrWhiteSpace(z.RelativeFragmentPath));
                string[] expectedEntryKeys = withFragments.Select(z => z.GalkovskyEntryKey).ToArray();

                string[] missing = expectedEntryKeys.Except(entryKeysInPdf, StringComparer.OrdinalIgnoreCase).ToArray();

                if (missing.Length == 0)
                    Log.Information("File for split {Split} is OK. {Pages} pages, {Size} MB.", s, pages, mb);
                else
                {
                    foreach (string m in missing)
                        Log.Error("File for split {Split} doesn't have record {Record}.", s, m);
                }
            }
        }

        private static string[] GetGalkovskyEntryKeys(PdfOutline outlines, IGalkovskyFolderNamingStrategy gsg)
        {
            List<string> ret = new List<string>(100);

            if (outlines == null)
                return ret.ToArray();

            var children = outlines.GetAllChildren();
            foreach (var outline in children)
            {
                string title = outline.GetTitle();
                if (string.IsNullOrEmpty(title))
                    continue;

                try
                {
                    string key = gsg.GetGalkovskyEntryKey(title);
                    ret.Add(key);
                }
                catch
                {
                }
            }

            // Special case.
            if (ret.Contains("4"))
            {
                ret.Insert(0, "3");
                ret.Insert(0, "2");
                ret.Insert(0, "1");
            }

            return ret.ToArray();
        }

        static void PrintUsage()
        {
            Console.WriteLine("Usage: pdftest -root <folder>");
            Console.WriteLine();
            Console.WriteLine("Arguments:");
            Console.WriteLine("  -root      Root folder containing the book files");
        }
    }
}
