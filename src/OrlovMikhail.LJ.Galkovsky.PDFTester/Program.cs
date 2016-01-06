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

            // All available files.
            int? maxFound;
            GalkovskyNumberingStrategy ns = new GalkovskyNumberingStrategy();
            List<Tuple<int, string>> relativePaths = FragmentHelper.GetAllFragmentPaths(fs, ns, root, out maxFound);

            // Splits.
            Split[] splits = Split.LoadSplits(fs, root, maxFound);

            const string sourcePdf = "output\\GalkovskyLJ_{0}.A5.pdf";

            foreach (Split s in splits)
            {
                string fromString = ns.GetFriendlyTitleBySortNumber(s.From);
                string toString = ns.GetFriendlyTitleBySortNumber(s.To);
                string splitInfo = String.Format("{0} ({1}-{2})", s.Name, fromString, toString);

                string relativePath = String.Format(sourcePdf, s.Name);
                string absolutePath = Path.Combine(root, relativePath);

                if (!File.Exists(absolutePath))
                {
                    log.Error(String.Format("File for split {0} doesn't exist.", splitInfo));
                    continue;
                }

                int[] nums;
                string mb = (((double)new FileInfo(absolutePath).Length) / 1024d / 1024d).ToString("#.##");
                int pages;
                try
                {
                    using (PdfReader pdfReader = new PdfReader(absolutePath))
                    {
                        pages = pdfReader.NumberOfPages;
                        IList<Dictionary<string, object>> bookmarks = SimpleBookmark.GetBookmark(pdfReader);
                        nums = GetNumsFromBookmarks(bookmarks);
                    }
                }
                catch
                {
                    log.Error(String.Format("File for split {0} yielded an error.", splitInfo));
                    continue;
                }

                int[] shouldBe = Enumerable.Range(s.From, s.To.Value - s.From + 1).ToArray();
                int[] missing = shouldBe.Except(nums).ToArray();

                if (missing.Length == 0)
                    log.Info(String.Format("File for split {0} is OK. {1} pages, {2} MB.", splitInfo, pages, mb));
                else
                {
                    foreach (int m in missing)
                        log.Error(String.Format("File for split {0} doesn't have record {1}.", splitInfo, m));
                }
            }
        }

        private static int[] GetNumsFromBookmarks(IList<Dictionary<string, object>> bookmarks)
        {
            List<int> ret = new List<int>(100);
            INumberingStrategy gsg = new GalkovskyNumberingStrategy();

            for (int i = 0; i < bookmarks.Count; i++)
            {
                string title = bookmarks[i].Values.First().ToString();

                int num;
                try
                {
                    // Extract number from title.
                    string subFolder;
                    if (!gsg.TryGetSubfolderByEntry(title, out subFolder))
                        continue;
                    num = gsg.GetSortNumberBySubfolder(subFolder);
                    ret.Add(num);
                }
                catch
                {
                }
            }

            // Special case.
            if (ret.Contains(4))
            {
                ret.Add(1);
                ret.Add(2);
                ret.Add(3);
            }

            ret.Sort();

            return ret.ToArray();
        }
    }
}
