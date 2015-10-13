using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.Grabber
{
    public class SuitableCommentsPicker : ISuitableCommentsPicker
    {
        private readonly IRepliesHelper _repliesHelper;

        static readonly ILog log = LogManager.GetLogger(typeof(SuitableCommentsPicker));

        public SuitableCommentsPicker(IRepliesHelper repliesHelper)
        {
            _repliesHelper = repliesHelper;
        }

        public List<Comment[]> Pick(EntryPage ep)
        {
            string authorUsername = ep.Entry.Poster.Username;

            // Get threads from each root comment.
            List<Comment[]> ret = ep.Replies.Comments
                                    .AsParallel().AsOrdered()
                                    .Select(rootComment => ExtractThreads(rootComment, authorUsername))
                                    .SelectMany(a => a)
                                    .ToList();

            // Make sure all author comments were picked.
            AssertAuthorCommentsArePicked(ep, ret);

            return ret;
        }

        private void AssertAuthorCommentsArePicked(EntryPage ep, List<Comment[]> ret)
        {
            string authorUsername = ep.Entry.Poster.Username;
            Comment[] authorCommentsCount = _repliesHelper.EnumerateAll(ep.Replies).Where(z => z.Poster.Username == authorUsername).ToArray();
            Comment[] authorCommentsPicked = ret.SelectMany(z => z).Where(z => z.Poster.Username == authorUsername).ToArray();
            Comment[] authorCommentsLeftBehind = authorCommentsCount.Except(authorCommentsPicked).ToArray();

            if(authorCommentsLeftBehind.Length != 0)
            {
                // Watchdog barks.
                string message = String.Format("Author comments with ids {0} were left behind when picking.", String.Join(", ", authorCommentsLeftBehind.Select(z => z.Id)));
                log.Error(message);
                throw new ApplicationException(message);
            }
        }

        private List<Comment[]> ExtractThreads(Comment rootComment, string authorUsername)
        {
            List<Comment[]> ret = new List<Comment[]>();

            // Create a tree.
            TreeNode<Comment> tree = new TreeNode<Comment>(rootComment, null);
            AddChildren(tree);

            // Links that compose threads.
            LinkStorage<TreeNode<Comment>> ls = new LinkStorage<TreeNode<Comment>>();
            CreateLinksBasedOnAuthorComments(tree, ls, authorUsername);
            AddMissingLinksBetweenAdjacentOnes(tree, ls);
            ReSetParentsForContinuousReplies(tree, ls);

            // This composes final threads.
            IEnumerable<List<Comment>> threads = EnumerateThreads(tree, ls).ToArray();
            foreach(IEnumerable<Comment> iec in threads)
            {
                Comment[] thread = iec.ToArray();
                ret.Add(thread);
            }

            return ret;
        }

        private IEnumerable<List<Comment>> EnumerateThreads(TreeNode<Comment> rootComment, LinkStorage<TreeNode<Comment>> ls)
        {
            HashSet<TreeNode<Comment>> wentThrough = new HashSet<TreeNode<Comment>>();

            // This enumerates all nodes.
            foreach(TreeNode<Comment> node in rootComment)
            {
                if(wentThrough.Contains(node))
                    continue;
                wentThrough.Add(node);

                List<Comment> thread = new List<Comment>();

                // First thread that starts from this node.
                Tuple<TreeNode<Comment>, TreeNode<Comment>> tuple = ls.GetLinksWithTop(node).FirstOrDefault();
                if(tuple != null)
                {
                    // OK, there is a link that starts from this node.

                    // First node.
                    AddCommentToList(thread, tuple.Item1);
                    while(tuple != null)
                    {
                        // Second node.
                        if(tuple.Item1 != tuple.Item2)
                        {
                            AddCommentToList(thread, tuple.Item2);
                            wentThrough.Add(tuple.Item2);
                        }

                        // Go deeper.
                        tuple = ls.GetLinksWithTop(tuple.Item2).Where(z => z.Item1 != z.Item2).FirstOrDefault();
                    }

                }
                else
                {
                    // There weren't any links starting from this one.
                    // Does any end on it?

                    tuple = ls.GetLinkWithBottom(node);

                    if(tuple != null)
                        thread.Add(tuple.Item2.Data);
                }

                // Save thread if not empty.
                if(thread.Count > 0)
                    yield return thread;
            }

            yield break;
        }

        private void AddCommentToList(List<Comment> thread, TreeNode<Comment> treeNode)
        {
            Comment c = treeNode.Data;

            // Full or deleted.
            if(c.IsFull || c.IsDeleted)
                thread.Add(c);
        }

        private void ReSetParentsForContinuousReplies(TreeNode<Comment> tree, LinkStorage<TreeNode<Comment>> ls)
        {
            var allLinksByParent = ls.EnumerateLinks().GroupBy(z => z.Item1);

            foreach(var group in allLinksByParent)
            {
                // (A - B), (A - C).
                Tuple<TreeNode<Comment>, TreeNode<Comment>>[] fromThis = group.ToArray();
                for(int i = 1; i < fromThis.Length; i++)
                {
                    // No (B - ...)?
                    bool previousHasOtherLinks = ls.GetLinksWithTop(fromThis[i - 1].Item2).Any();
                    if(previousHasOtherLinks)
                        continue;

                    // Then remove (A - C), set (B - C).
                    ls.RemoveLink(fromThis[i].Item1, fromThis[i].Item2);
                    ls.AddLink(fromThis[i - 1].Item2, fromThis[i].Item2);
                }
            }
        }

        /// <summary>Goes through comment tree. If two links are adjacent and there are no others,
        /// add a missing link between them.</summary>
        /// <param name="tree">Comment tree.</param>
        /// <param name="ls">Links storage.</param>
        private void AddMissingLinksBetweenAdjacentOnes(TreeNode<Comment> tree, LinkStorage<TreeNode<Comment>> ls)
        {
            Tuple<TreeNode<Comment>, TreeNode<Comment>>[] allLinks = ls.EnumerateLinks().ToArray();

            foreach(var link in allLinks)
            {
                // Child node of (A - B), B.
                TreeNode<Comment> child = link.Item2;

                // Shouldn't have direct children. No (B - C) links.
                bool hasDirectChildren = ls.GetLinksWithTop(child).Where(z => z.Item1 != z.Item2).Any();
                if(hasDirectChildren)
                    continue;

                // First adjacent link, starting from C, (C - D).
                Tuple<TreeNode<Comment>, TreeNode<Comment>> singleAdjacentLink = child.Children.Select(ls.GetLinksWithTop).SelectMany(a => a).FirstOrDefault();
                if(singleAdjacentLink != null)
                {
                    // OK, create a link between (B - C).
                    // Now we have chain (A - B), (B - C), (C - D).
                    ls.AddLink(child, singleAdjacentLink.Item1);

                    // Is (A - B) actually (B - B)? If so, remove it.
                    if(link.Item1 == link.Item2)
                        ls.RemoveLink(link.Item1, link.Item2);
                }
            }
        }

        private void CreateLinksBasedOnAuthorComments(TreeNode<Comment> root, LinkStorage<TreeNode<Comment>> ls, string authorUsername)
        {
            foreach(TreeNode<Comment> node in root)
            {
                Comment c = node.Data;
                bool isAuthor = c.Poster.Username == authorUsername;

                if(isAuthor)
                {
                    // Since it's author, mark comments before and after (if the same poster).
                    Comment cUber = null;
                    if(node.Parent != null)
                        cUber = node.Parent.Data as Comment;

                    if(cUber != null)
                    {
                        // Add link for parent.
                        ls.AddLink(node.Parent, node);
                        string previousUsername = cUber.Poster.Username;

                        TreeNode<Comment>[] childrenOfSamePrevious = node.Children
                                                                .Where(z => SuitableForTaking(z.Data, previousUsername))
                                                                .ToArray();

                        // Add link for all children of the same poster. Most likely, count is <= 1.
                        foreach(TreeNode<Comment> cUnter in childrenOfSamePrevious)
                            ls.AddLink(node, cUnter);
                    }
                    else
                    {
                        // Just the parent comment.
                        // Single element link.
                        ls.AddLink(node, node);
                    }
                }
            }
        }

        private bool SuitableForTaking(Comment z, string previousUsername)
        {
            switch(z.Policy)
            {
                default:
                case UsagePolicy.Default:
                    return z.Poster.Username == previousUsername;

                case UsagePolicy.Ignore:
                    return false;

                case UsagePolicy.Forced:
                    return true;
            }
        }

        private void AddChildren(TreeNode<Comment> parentNode)
        {
            foreach(Comment child in parentNode.Data.Replies.Comments)
            {
                TreeNode<Comment> childNode = parentNode.AddChild(child);
                AddChildren(childNode);
            }
        }
    }
}
