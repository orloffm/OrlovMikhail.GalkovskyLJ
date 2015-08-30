using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace OrlovMikhail.LJ.Grabber
{
    public sealed class RepliesHelper : IRepliesHelper
    {
        private readonly IEntryBaseHelper _ebh;
        static readonly ILog log = LogManager.GetLogger(typeof(RepliesHelper));

        public RepliesHelper(IEntryBaseHelper ebh)
        {
            _ebh = ebh;
        }

        #region merge comments from various sources
        public bool MergeFrom<T>(T target, T fullVersion) where T : IHasReplies
        {
            log.DebugFormat("Will merge comment tree from '{0}' with '{1}'.", target, fullVersion);

            // Argument check.
            if(fullVersion == null || target == null)
                throw new ArgumentNullException();

            if(typeof(T) == typeof(Comment))
            {
                // Items are comments.
                Comment a = target as Comment;
                Comment b = fullVersion as Comment;

                if(a.Id != b.Id)
                {
                    string message = "Comments have different ids.";
                    log.Error(message);
                    throw new ArgumentNullException(message);
                }

                return MergeCommentDataInternal(a, b);
            }
            else if(typeof(T) == typeof(EntryPage))
            {
                EntryPage ea = target as EntryPage;
                EntryPage eb = fullVersion as EntryPage;

                return MergeRepliesInternal(ea.Replies, eb.Replies);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        bool MergeCommentDataInternal(Comment target, Comment fullVersion)
        {
            bool updated = false;

            // We do.
            updated |= UpdateDirectDataWith(target, fullVersion);

            // Merge replies.
            updated |= MergeRepliesInternal(target.Replies, fullVersion.Replies);

            return updated;
        }

        /// <summary>This function does the merge. It assumes
        /// that both r are to the same parent.</summary>
        bool MergeRepliesInternal(Replies target, Replies otherSet)
        {
            bool updated = false;

            Dictionary<long, Comment> initial = target.Comments.ToDictionary(z => z.Id, z => z);

            for(int i = 0; i < otherSet.Comments.Count; i++)
            {
                Comment c = otherSet.Comments[i];

                // Do we have a comment with the same id?
                Comment existed;
                if(initial.TryGetValue(c.Id, out existed))
                {
                    // We do.
                    updated |= MergeCommentDataInternal(existed, c);
                }
                else
                {
                    // We don't have a comment with the same id.
                    // Insert it.
                    int bestIndex = target.Comments.FindIndex(z => z.Id > c.Id);
                    if(bestIndex == -1)
                        bestIndex = target.Comments.Count;
                    Comment cloned = c.MakeClone();
                    target.Comments.Insert(bestIndex, cloned);
                    updated = true;
                }
            }

            return updated;
        }

        /// <summary>Some comments are shown collapsed, non-full.
        /// This function replaces them in the tree with full
        /// versions that were downloaded specifically.
        /// We also update text if it was edited to larger text.</summary>
        /// <param name="needUpdate">Initial comment.</param>
        /// <param name="source">Full version of the same comment.</param>
        /// <returns>Updated or not.</returns>
        public bool UpdateDirectDataWith(Comment target, Comment source)
        {
            bool updated = false;

            if(source == null || target == null)
                throw new ArgumentNullException();

            if(!source.IsFull || source.IsDeleted)
                return false;

            if(target.IsFull == false)
            {
                target.IsFull = true;
                updated = true;
            }

            // General data.
            updated |= _ebh.UpdateWith(target, source);

            updated |= _ebh.UpdateStringProperty(source.ParentUrl, target.ParentUrl, s => target.ParentUrl = s);

            if(target.Poster == null && source.Poster != null)
            {
                target.Poster = source.Poster;
                updated = true;
            }

            return updated;
        }
        #endregion

        #region unfold
        static IEnumerable<Comment> Unfold(Replies commentsTree, Func<Comment, bool> matching)
        {
            foreach(Comment c in commentsTree.Comments)
            {
                // The comment itself.
                if(matching(c))
                    yield return c;

                // Next its replies.
                foreach(Comment cs in Unfold(c.Replies, matching))
                    yield return cs;
            }
        }

        public IEnumerable<Comment> EnumerateRequiringFullUp(Replies target)
        {
            return Unfold(target, c => !c.IsFull && !(c.IsDeleted || c.IsScreened || c.IsSuspendedUser));
        }

        public IEnumerable<Comment> EnumerateFull(Replies target)
        {
            return Unfold(target, c => c.IsFull);
        }

        public IEnumerable<Comment> EnumerateAll(Replies target)
        {
            return Unfold(target, c => true);
        }
        #endregion
    }
}
