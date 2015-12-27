using System.Collections.Generic;

namespace OrlovMikhail.LJ.Grabber
{
    public interface IRepliesHelper
    {
        /// <summary>Merges comments tree. Objects should point to
        /// equal items. They both should be entry pages or the same comment.
        /// If they are comments, its content is also merged.</summary>
        /// <param name="target">Tree point from which we want to update.</param>
        /// <param name="fullVersion">Tree point from other source with full or extra data.</param>
        bool MergeFrom<T>(T target, T fullVersion) where T : IHasReplies;

        /// <summary>All non-deleted comments that don't have content.</summary>
        IEnumerable<Comment> EnumerateRequiringFullUp(Replies target);
        IEnumerable<Comment> EnumerateFull(Replies target);
        IEnumerable<Comment> EnumerateAll(Replies target);
    }
}