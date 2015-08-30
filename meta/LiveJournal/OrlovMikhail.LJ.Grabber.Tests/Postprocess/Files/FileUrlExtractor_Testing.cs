using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace OrlovMikhail.LJ.Grabber.Tests
{
    [TestFixture]
    public class FileUrlExtractor_Testing
    {
        private string _html;
        private IFileUrlExtractor _ex;

        private const string URL1 = @"http://www.samisdat.com/picture/LJ3/3409.jpg";
        private const string URL2 = @"http://www.samisdat.com/picture/LJ3/3423.jpg";

        [SetUp]
        public void LoadHTML()
        {
            string content = TestingShared.GetFileContent("testpage_247911.xml");

            LayerParser p = new LayerParser();
            _html = p.ParseAsAnEntryPage(content).Entry.Text;
            _ex = new FileUrlExtractor();
        }

        [Test]
        public void ParsesFromContent()
        {
            string[] urls = _ex.GetImagesURLs(_html);

            Assert.IsTrue(urls.Length > 5);
            CollectionAssert.Contains(urls, URL1);
            CollectionAssert.Contains(urls, URL2);
        }

        [Test]
        public void ProvidesUrlsForReplacing()
        {
            int called = 0;
            Func<string, string> matcher = _s =>
            {
                called++;
                if(_s == URL1)
                    return "ABC";
                else if(_s == URL2)
                    return null;
                else
                    return _s;
            };

            string result = _ex.ReplaceFileUrls(_html, matcher);
            Assert.IsFalse(result.Contains(URL1));
            Assert.IsTrue(result.Contains(URL2));
            Assert.IsTrue(result.Contains("<img src=\"ABC\""));
        }
    }
}
