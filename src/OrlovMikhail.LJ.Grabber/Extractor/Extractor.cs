using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace OrlovMikhail.LJ.Grabber
{
    public sealed class Extractor : IExtractor
    {
        readonly ILayerParser _parser;
        public ILJClient Client { get; private set; }
        readonly IEntryPageHelper _entryPageHelper;
        readonly IRepliesHelper _repliesHelper;
        readonly IOtherPagesLoader _otherPagesLoader;

        static readonly ILog log = LogManager.GetLogger(typeof(Extractor));

        public Extractor(ILayerParser parser, ILJClient client,
            IEntryPageHelper entryPageHelper, IRepliesHelper repliesHelper,
            IOtherPagesLoader otherPagesLoader)
        {
            _parser = parser;
            Client = client;
            _entryPageHelper = entryPageHelper;
            _repliesHelper = repliesHelper;
            _otherPagesLoader = otherPagesLoader;
        }

        public bool AbsorbAllData(EntryPage freshSource, ILJClientData clientData, ref EntryPage dumpData)
        {
            bool appliedAnything = false;

            if(dumpData == null)
            {
                dumpData = new EntryPage();
                appliedAnything = true;
            }

            appliedAnything |= _entryPageHelper.AddData(dumpData, freshSource);

            // TryGet all comments.
            EntryPage[] otherPages = _otherPagesLoader.LoadOtherCommentPages(freshSource.CommentPages, clientData);

            foreach(EntryPage pageX in otherPages)
                appliedAnything |= _entryPageHelper.AddData(dumpData, pageX);

            while(true)
            {
                IEnumerable<Comment> allFoldedComments = _repliesHelper.EnumerateRequiringFullUp(dumpData.Replies);
                IEnumerator<Comment> enumerator = allFoldedComments.GetEnumerator();

                int foldedCommentsLeft = 0;
                Comment c = null;

                while(enumerator.MoveNext())
                {
                    foldedCommentsLeft++;
                    if(c == null)
                        c = enumerator.Current;
                }

                // How many comments left?
                log.Info(String.Format("Folded comments left: {0}.", foldedCommentsLeft));
                if(foldedCommentsLeft == 0)
                    break;
                
                LiveJournalTarget commentTarget = LiveJournalTarget.FromString(c.Url);
                EntryPage commentPage = GetFrom(commentTarget, clientData);
                Comment fullVersion = commentPage.Replies.Comments[0];
                if(fullVersion.IsFull == false)
                {
                    // This should be a suspended user.
                    log.Info(String.Format("Comment {0} seems to be from a suspended user.", c));
                    c.IsSuspendedUser = true;
                    continue;
                }

                log.Info(String.Format("Merging comment data for comment {0}.", c));
                appliedAnything |= _repliesHelper.MergeFrom(c, fullVersion);
            }

            return appliedAnything;
        }

        /// <summary>Gets a page from a Url.</summary>
        public EntryPage GetFrom(LiveJournalTarget url, ILJClientData clientData)
        {
            // Gets the string.
            string content = Client.GetContent(url, clientData);

            // Parses the string as an entry page.
            EntryPage p = _parser.ParseAsAnEntryPage(content);

            return p;
        }
    }
}
