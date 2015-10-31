using log4net;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.Grabber
{
    public class Worker : IWorker
    {
        private readonly IFileSystem _fs;
        private readonly IExtractor _ext;
        private readonly ILayerParser _lp;
        private readonly IRelatedDataSaver _rds;
        private readonly ISuitableCommentsPicker _scp;

        static readonly ILog log = LogManager.GetLogger(typeof(Worker));

        public Worker(IFileSystem fs, IExtractor ext,
            ILayerParser lp, IRelatedDataSaver rds, ISuitableCommentsPicker scp)
        {
            _fs = fs;
            _ext = ext;
            _lp = lp;
            _rds = rds;
            _scp = scp;
        }

        class SubfolderPassthrough : ISubfolderByEntryGetter
        {
            private readonly string _innerFolder;

            public SubfolderPassthrough(string innerFolder)
            {
                _innerFolder = innerFolder;
            }

            public string GetSubfolderByEntrySubject(string s)
            {
                return _innerFolder;
            }
        }

        public EntryPage WorkInGivenTarget(string URI, string rootLocation, string innerFolder, string cookie)
        {
            SubfolderPassthrough p = new SubfolderPassthrough(innerFolder);
            EntryPage ret = Work(URI, rootLocation, p, cookie);
            return ret;
        }

        public EntryPage Work(string URI, string rootLocation, ISubfolderByEntryGetter subFolderGetter, string cookie)
        {
            LiveJournalTarget t = LiveJournalTarget.FromString(URI);
            ILJClientData cookieData = _ext.Client.CreateDataObject(cookie);

            // Get fresh version.
            log.InfoFormat("Extracting {0}...", t);
            EntryPage freshSource = _ext.GetFrom(t, cookieData);

            string innerFolder = subFolderGetter.GetSubfolderByEntrySubject(freshSource.Entry.Subject);
            string subFolder = String.Format("{0}\\{1}", freshSource.Entry.Date.Value.Year, innerFolder);
            string filename = "dump.xml";

            string workLocation = _fs.Path.Combine(rootLocation, subFolder);
            log.Info("Will work from " + workLocation);

            EntryPage ep = null;
            string dumpFile = _fs.Path.Combine(workLocation, filename);
            if (_fs.File.Exists(dumpFile))
            {
                log.Info("File " + filename + " exists, will load it...");
                ep = _lp.ParseAsAnEntryPage(_fs.File.ReadAllText(dumpFile));
            }
            else
                log.Info("File " + filename + " does not exist.");


            bool needsSaving = _ext.AbsorbAllData(freshSource, cookieData, ref ep);

            log.Info("Will save changes: " + needsSaving + ".");
            if (needsSaving)
            {
                // Save the content as is.
                string content = _lp.Serialize(ep);
                _fs.Directory.CreateDirectory(workLocation);

                UTF8Encoding enc = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);
                _fs.File.WriteAllText(dumpFile, content, enc);

                // Pick usable comments.
                List<Comment[]> comments = _scp.Pick(ep);
                log.Info("Picked threads: " + comments.Count + ".");

                // Everything we want to store.
                List<EntryBase> allData = new List<EntryBase>();
                allData.Add(ep.Entry);
                allData.AddRange(comments.SelectMany(a => a));

                log.Info("Making sure everything is saved.");
                _rds.EnsureAllIsSaved(allData, rootLocation, workLocation);
            }

            log.Info("Finished.");
            return ep;
        }
    }
}
