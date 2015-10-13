using log4net;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrlovMikhail.LJ.Grabber
{
    public sealed class OtherPagesLoader : IOtherPagesLoader
    {
        private readonly ILayerParser _parser;
        private readonly ILJClient _client;

        static readonly ILog log = LogManager.GetLogger(typeof(OtherPagesLoader));

        public OtherPagesLoader(ILayerParser parser, ILJClient client)
        {
            _parser = parser;
            _client = client;
        }

        public EntryPage[] LoadOtherCommentPages(CommentPages commentPages, ILJClientData clientData)
        {
            int initialIndex = commentPages.Current;
            int total = commentPages.Total;

            log.Info(String.Format("Loading other comment pages given page №{0} out of {1}.", commentPages.Current, commentPages.Total));

            // We need to download these.
            int[] need = Enumerable.Range(1, total).Where(i => i != initialIndex).ToArray();
            IDictionary<int, LiveJournalTarget> targets = new SortedDictionary<int, LiveJournalTarget>();
            IDictionary<int, EntryPage> pages = new SortedDictionary<int, EntryPage>();
            EntryPage p;

            CommentPages latest = commentPages;
            while(pages.Count < need.Length)
            {
                int cur = latest.Current;

                if(cur != 1 && !String.IsNullOrWhiteSpace(latest.FirstUrl))
                    targets[1] = LiveJournalTarget.FromString(latest.FirstUrl);
                if(cur != total && !String.IsNullOrWhiteSpace(latest.LastUrl))
                    targets[total] = LiveJournalTarget.FromString(latest.LastUrl);
                if(!String.IsNullOrWhiteSpace(latest.PrevUrl))
                    targets[cur - 1] = LiveJournalTarget.FromString(latest.PrevUrl);
                if(!String.IsNullOrWhiteSpace(latest.NextUrl))
                    targets[cur + 1] = LiveJournalTarget.FromString(latest.NextUrl);

                // First target without a page.
                int keyToDownload = targets.Keys.First(z => z != initialIndex && !pages.ContainsKey(z));
                log.Info(String.Format("Will download page №{0}.", keyToDownload));
                LiveJournalTarget targetToDownload = targets[keyToDownload];

                // Download the page.
                string content = _client.GetContent(targetToDownload, clientData);
                p = _parser.ParseAsAnEntryPage(content);
                latest = p.CommentPages;
                pages[keyToDownload] = p;
                log.Info(String.Format("Parsed page №{0}.", keyToDownload));
            }

            EntryPage[] ret = pages.Values.ToArray();
            return ret;
        }
    }
}