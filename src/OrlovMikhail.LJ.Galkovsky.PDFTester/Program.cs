using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using iTextSharp.text.pdf;
using log4net;
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
            List<Tuple<int, string>> relativePaths = FragmentHelper.GetAllFragmentPaths(fs, root, out maxFound);

            // Splits.
            Split[] splits = Split.LoadSplits(fs, root, maxFound);

            const string sourcePdf = "output\\GalkovskyLJ_{0}.A5.pdf";

            foreach (Split s in splits)
            {
                string splitInfo = String.Format("{0} ({1}-{2})", s.Name, s.From, s.To);

                string relativePath = String.Format(sourcePdf, s.Name);
                string absolutePath = Path.Combine(root, relativePath);

                if (!File.Exists(absolutePath))
                {
                    log.Error(String.Format("File for split {0} doesn't exist.", splitInfo));
                    continue;
                }

                int[] nums;
                try
                {
                    using (PdfReader pdfReader = new PdfReader(absolutePath))
                    {

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
                    log.Info(String.Format("File for split {0} is OK.", splitInfo));
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

            for (int i = 0; i < bookmarks.Count; i++)
            {
                string title = bookmarks[i].Values.First().ToString();

                Match m = Regex.Match(title, @"^(\d+)[,.]", RegexOptions.Compiled);
                if (m.Success)
                {
                    string numString = m.Groups[1].Value;
                    int num = Int32.Parse(numString);
                    ret.Add(num);
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
