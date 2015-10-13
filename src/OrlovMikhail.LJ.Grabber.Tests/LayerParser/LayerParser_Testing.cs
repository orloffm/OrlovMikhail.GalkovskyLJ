using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace OrlovMikhail.LJ.Grabber.Tests
{
    [TestFixture]
    public class LayerParser_Testing
    {
        [TestCase("2015-6-26 14:48:5")]
        [TestCase("2015-12-26 8:51:34")]
        public void ParsesDateTimeFromString(string source)
        {
            DateTime? value = EntryBase.ParseDateTimeFromString(source);
            Assert.IsTrue(value.HasValue);
        }

        [Test]
        public void ParsesTestPage()
        {
            string content = TestingShared.GetFileContent("testpage_247911.xml");

            LayerParser p = new LayerParser();
            EntryPage page = p.ParseAsAnEntryPage(content);

            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Replies);

            // Has userpic.
            Userpic userpic = page.Entry.PosterUserpic;
            Assert.IsNotNull(userpic);

            // Comment deserialization
            Comment firstComment = page.Replies.Comments[0];
            Assert.AreEqual(91506535, firstComment.Id);
            Assert.IsTrue(firstComment.IsFull, "By default a comment is treated as full.");

            Assert.AreEqual(new DateTime(2015, 6, 25, 15, 16, 50), firstComment.Date.Value);
            Assert.AreEqual("1", firstComment.Text);

            Comment innerComment = firstComment.Replies.Comments[0];
            Assert.AreEqual("http://galkovsky.livejournal.com/247911.html?thread=91589479#t91589479", innerComment.Url);

            // Entry text deserialization
            string entryText = page.Entry.Text;
            Assert.IsTrue(entryText.StartsWith("<p><center><img"));
        }

        [Test]
        public void StoresUserpic()
        {
            string content = TestingShared.GetFileContent("testpage_247911.xml");

            LayerParser p = new LayerParser();
            EntryPage page = p.ParseAsAnEntryPage(content);

            page.Replies.Comments.Clear();

            string serialized = p.Serialize(page);
            Assert.IsTrue(serialized.Contains("<userpic "));
        }

        [Test]
        public void StoresUsername()
        {
            string content = TestingShared.GetFileContent("testpage_247911.xml");

            LayerParser p = new LayerParser();
            EntryPage page = p.ParseAsAnEntryPage(content);

            page.Replies.Comments.Clear();

            string serialized = p.Serialize(page);
            page = p.ParseAsAnEntryPage(serialized);

            Assert.IsNotNullOrEmpty(page.Entry.Poster.Username);
        }
    }
}
