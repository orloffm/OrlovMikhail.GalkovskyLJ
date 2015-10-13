using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace OrlovMikhail.LJ.Grabber
{
    public class EntryPageHelper : IEntryPageHelper
    {
        private readonly IEntryHelper _entryHelper;
        private readonly IRepliesHelper _repliesHelper;

        public EntryPageHelper(IEntryHelper entryHelper, IRepliesHelper repliesHelper)
        {
            _entryHelper = entryHelper;
            _repliesHelper = repliesHelper;
        }

        static readonly ILog log = LogManager.GetLogger(typeof(EntryPageHelper));

        /// <summary>Adds everything possible to target from another source.
        /// This may include new comments, full comments, larger text etc.</summary>
        /// <param name="target">Object to add data to.</param>
        /// <param name="otherSource">Object to use data from.</param>
        public bool AddData(EntryPage target, EntryPage otherSource)
        {
            bool updated = false;

            // Check arguments.
            if(target == null || otherSource == null)
                throw new ArgumentNullException();


            // Entry.
            log.DebugFormat("Updating content for entry page '{0}' from other source.", target);
            updated |= _entryHelper.UpdateWith(target.Entry, otherSource.Entry);

            // Comments.
            log.DebugFormat("Updating comments for entry page '{0}' from other source.", target);
            updated |= _repliesHelper.MergeFrom(target, otherSource);

            // CommentPages.
            if(target.CommentPages != null && !CommentPages.IsEmpty(target.CommentPages))
            {
                log.DebugFormat("Cleaning comment pages information for entry page '{0}'.", target);
                target.CommentPages = CommentPages.Empty;
                updated = true;
            }

            return updated;
        }
    }
}
