using System;
using NUnit.Framework;

namespace OrlovMikhail.LJ.Grabber.Tests
{
    [TestFixture]
    public class LiveJournalTarget_Testing
    {

        [TestCase(@"http://galkovsky.livejournal.com/15915.html", "galkovsky", 15915L, null, null, false)]
        [TestCase(@"http://galkovsky.livejournal.com/247911.html?thread=91572583&style=mine#t91572583", "galkovsky", 247911L, 91572583L, null, true)]
        [TestCase(@"http://galkovsky.livejournal.com/247911.html?page=2", "galkovsky", 247911L, null, 2, false)]
        public void ParsesRawUrlStringsCorrectly(string source, string username, long postId, long? commentId, int? page, bool styleMine)
        {
            LiveJournalTarget ret = LiveJournalTarget.FromString(source);
            Assert.AreEqual(username, ret.Username);
            Assert.AreEqual(postId, ret.PostId);
            Assert.AreEqual(commentId, ret.CommentId);
            Assert.AreEqual(styleMine, ret.UseStyleMine);
            Assert.AreEqual(page, ret.Page);
        }

        [TestCase("galkovsky", 247911L, 91572583L, null, true, ExpectedResult = @"http://galkovsky.livejournal.com/247911.html?thread=91572583&style=mine")]
        [TestCase("galkovsky", 247911L, 91572583L, null, false, ExpectedResult = @"http://galkovsky.livejournal.com/247911.html?thread=91572583")]
        [TestCase("galkovsky", 247911L, null, null, true, ExpectedResult = @"http://galkovsky.livejournal.com/247911.html?style=mine")]
        [TestCase("galkovsky", 247911L, null, 2, false, ExpectedResult = @"http://galkovsky.livejournal.com/247911.html?page=2")]
        [TestCase("galkovsky", 247911L, null, 2, true, ExpectedResult = @"http://galkovsky.livejournal.com/247911.html?page=2&style=mine")]
        public string ConvertedToStringOK(string username, long postId, long? commentId, int? page, bool styleMine)
        {
            LiveJournalTarget ret = new LiveJournalTarget(username, postId, commentId, page, styleMine);
            string result = ret.ToString();
            return result;
        }

        [TestCase("galkovsky", 247911L, 91572583L, null, true, ExpectedResult = "galkovsky/247911")]
        [TestCase("galkovsky", 247911L, 91572583L, null, false, ExpectedResult = "galkovsky/247911")]
        [TestCase("galkovsky", 247911L, null, null, true, ExpectedResult = "galkovsky/247911")]
        [TestCase("galkovsky", 247911L, null, 1, true, ExpectedResult = "galkovsky/247911")]
        [TestCase("galkovsky", 247911L, null, 2, true, ExpectedResult = "galkovsky/247911#2")]
        public string ToShortStringIsFine(string username, long postId, long? commentId, int? page, bool styleMine)
        {
            LiveJournalTarget ret = new LiveJournalTarget(username, postId, commentId, page, styleMine);
            string result = ret.ToShortString();
            return result;
        }
    }
}
