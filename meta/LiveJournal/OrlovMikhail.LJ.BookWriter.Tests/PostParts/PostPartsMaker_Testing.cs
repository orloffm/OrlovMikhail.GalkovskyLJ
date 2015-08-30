using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace OrlovMikhail.LJ.BookWriter.Tests
{
    [TestFixture]
    public class PostPartsMaker_Testing
    {
        private PostPartsMaker _m;

        [SetUp]
        public void Setup()
        {
            _m = new PostPartsMaker();
        }

        [Test]
        public void ExtractsUserNamePart()
        {
            List<HTMLTokenBase> tokens = new List<HTMLTokenBase>();

            tokens.Add(TagHTMLToken.FromTag("<span lj:user=\"orloffm\">"));
            tokens.Add(TagHTMLToken.FromTag("<span class=\"abc\">"));
            tokens.Add(TagHTMLToken.FromTag("<img src=\"user\">"));
            tokens.Add(new TextHTMLToken("abc"));
            tokens.Add(TagHTMLToken.FromTag("</span>"));
            tokens.Add(TagHTMLToken.FromTag("</span>"));

            PostPartBase[] result = _m.CreateTextParts(tokens.ToArray(), null).ToArray();
            Assert.AreEqual(1, result.Length);
            Assert.IsInstanceOf<UserLinkPart>(result[0]);
            Assert.AreEqual("orloffm", ((UserLinkPart) result[0]).Username);
        }
    }
}
