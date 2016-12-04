using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using iTextSharp.text.pdf;
using log4net;
using OrlovMikhail.LJ.Grabber;
using OrlovMikhail.Tools;

namespace OrlovMikhail.LJ.Galkovsky.PDFTester
{
    class Program
    {
             const string SourcePdf = "output\\GalkovskyLJ_{0}.A5.pdf";

       static readonly ILog log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            Dictionary<string, string> argsDic = ConsoleTools.ArgumentsToDictionary(args);

            if (!SettingsTools.LoadValue("root", argsDic, Settings.Default, s => s.RootFolder))
                return;
            Settings.Default.Save();

            IFileSystem fs = new FileSystem();

            string root = Settings.Default.RootFolder;

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
                    log.Error(String.Format("File for split {0} doesn't exist.", s));
                    continue;
                }


                string[] entryKeysInPdf;
                string mb = (new FileInfo(absolutePath).Length/1024d/1024d).ToString("#.##");
                int pages;
                try
                {
                    using (PdfReader pdfReader = new PdfReader(absolutePath))
                    {
                        pages = pdfReader.NumberOfPages;
                        IList<Dictionary<string, object>> bookmarks = SimpleBookmark.GetBookmark(pdfReader);
                        entryKeysInPdf = GetGalkovskyEntryKeys(bookmarks,ns);
                    }
                }
                catch(Exception ex)
                {
                    log.Error(String.Format("File for split {0} yielded an error.", s), ex);
                    continue;
                }

                FragmentInformation[] allDumps = fragHelper.SelectValuesFor(s, fragsById).ToArray();
                var withFragments = allDumps.Where(z => !String.IsNullOrWhiteSpace(z.RelativeFragmentPath));
                string[] expectedEntryKeys = withFragments.Select(z => z.GalkovskyEntryKey).ToArray();

              
                string[] missing = expectedEntryKeys.Except(entryKeysInPdf, StringComparer.OrdinalIgnoreCase).ToArray();

                if (missing.Length == 0)
                    log.Info(String.Format("File for split {0} is OK. {1} pages, {2} MB.", s, pages, mb));
                else
                {
                    foreach (string m in missing)
                        log.Error(String.Format("File for split {0} doesn't have record {1}.", s, m));
                }
            }
        }

        private static string[] GetGalkovskyEntryKeys(IList<Dictionary<string, object>> bookmarks, IGalkovskyFolderNamingStrategy gsg)
        {
            List<string> ret = new List<string>(100);

            for (int i = 0; i < bookmarks.Count; i++)
            {
                string title = bookmarks[i].Values.First().ToString();

                try
                {
                    // Extract from title.
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
    }
}
