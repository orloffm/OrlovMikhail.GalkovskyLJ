using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace OrlovMikhail.LJ.Grabber.Tests
{
    [TestFixture]
    public class SuitableCommentsPicker_Testing
    {
        RepliesHelper _rh;
        SuitableCommentsPicker _picker;

        [SetUp]
        public void Prepare()
        {
            _rh = new RepliesHelper(new EntryBaseHelper(new FileUrlExtractor()));
            _picker = new SuitableCommentsPicker(_rh);
        }

        [Description("Simple tree, one author comment.")]
        [Test]
        public void SelectsCommentsAsExpected()
        {
            EntryPage ep = TestingShared.GenerateEntryPage(makeAllFull: true);

            List<Comment[]> result = _picker.Pick(ep);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(3, result[0].Length);
        }

        [Test]
        public void TopAuthorCommentIsAdded()
        {
            EntryPage ep = new EntryPage();
            ep.Entry.Poster.Username = "A";

            Comment a = new Comment();
            TestingShared.SetIdAndUrls(a, 1, null);
            a.Poster.Username = "A";
            ep.Replies.Comments.Add(a);

            Comment a_b = new Comment();
            TestingShared.SetIdAndUrls(a_b, 2, a);
            a_b.Poster.Username = "B";
            a.Replies.Comments.Add(a_b);

            List<Comment[]> result = _picker.Pick(ep);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].Length);
            Assert.AreSame(a, result[0][0]);
        }

        [Test, Combinatorial]
        public void AddsComplexTreeAsTwoThreads([Values(true, false)]bool firstLeafIsAuthor,
            [Values(true, false)] bool firstReplyIsTheSamePerson)
        {
            EntryPage ep = new EntryPage();
            ep.Entry.Poster.Username = "A";

            // Tree. Should become (B A C A), (X A).
            Comment a = new Comment();
            TestingShared.SetIdAndUrls(a, 1, null);
            a.Poster.Username = "B";
            ep.Replies.Comments.Add(a);

            Comment a_b = new Comment();
            TestingShared.SetIdAndUrls(a_b, 2, a);
            a_b.Poster.Username = "A";
            a.Replies.Comments.Add(a_b);

            Comment a_b_c = new Comment();
            TestingShared.SetIdAndUrls(a_b_c, 3, a_b);
            a_b_c.Poster.Username = firstReplyIsTheSamePerson ? "B" : "C";
            a_b.Replies.Comments.Add(a_b_c);

            Comment a_b_c_d = new Comment();
            TestingShared.SetIdAndUrls(a_b_c_d, 4, a_b_c);
            a_b_c_d.Poster.Username = firstLeafIsAuthor ? "A" : "R";
            a_b_c.Replies.Comments.Add(a_b_c_d);

            Comment a_b_e = new Comment();
            TestingShared.SetIdAndUrls(a_b_e, 5, a_b);
            a_b_e.Poster.Username = "X";
            a_b.Replies.Comments.Add(a_b_e);

            Comment a_b_e_f = new Comment();
            TestingShared.SetIdAndUrls(a_b_e_f, 6, a_b_e);
            a_b_e_f.Poster.Username = "A";
            a_b_e.Replies.Comments.Add(a_b_e_f);

            List<Comment[]> result = _picker.Pick(ep);
            if(firstLeafIsAuthor)
            {
                Assert.AreEqual(2, result.Count);
                CollectionAssert.AreEqual(new[] { a, a_b, a_b_c, a_b_c_d }, result[0]);
                CollectionAssert.AreEqual(new[] { a_b_e, a_b_e_f }, result[1]);
            }
            else
            {
                if(firstReplyIsTheSamePerson)
                {
                    // We take a_b_c.
                    Assert.AreEqual(2, result.Count);
                    CollectionAssert.AreEqual(new[] { a, a_b, a_b_c }, result[0]);
                    CollectionAssert.AreEqual(new[] { a_b_e, a_b_e_f }, result[1]);
                }
                else
                {
                    // We don't take it. We don't care what he wrote.
                    Assert.AreEqual(1, result.Count);
                    CollectionAssert.AreEqual(new[] { a, a_b, a_b_e, a_b_e_f }, result[0]);
                }
            }
        }


    }
}
