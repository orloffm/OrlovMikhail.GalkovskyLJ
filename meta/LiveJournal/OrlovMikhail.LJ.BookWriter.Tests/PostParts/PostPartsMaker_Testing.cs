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
        public void ExtractsUserNamePartSimple()
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
            Assert.AreEqual("orloffm", ((UserLinkPart)result[0]).Username);
        }


        [Test]
        public void DoesntAddFinishingFormattingIfNotNeeded()
        {
            List<HTMLTokenBase> tokens = new List<HTMLTokenBase>();

            tokens.Add(new TextHTMLToken("A"));
            tokens.Add(TagHTMLToken.FromTag("<br/>"));
            tokens.Add(TagHTMLToken.FromTag("<br/>"));
            tokens.Add(TagHTMLToken.FromTag("<i>"));
            tokens.Add(new TextHTMLToken("B"));
            tokens.Add(TagHTMLToken.FromTag("</i>"));
            tokens.Add(new TextHTMLToken(" "));
            tokens.Add(TagHTMLToken.FromTag("<br/>"));
            tokens.Add(TagHTMLToken.FromTag("<br/>"));
            tokens.Add(new TextHTMLToken("C"));

            PostPartBase[] result = _m.CreateTextParts(tokens.ToArray(), null).ToArray();
            Assert.AreEqual(7, result.Length);
            Assert.IsInstanceOf<RawTextPostPart>(result[0]);
            Assert.IsInstanceOf<ParagraphStartPart>(result[1]);
            Assert.IsInstanceOf<ItalicStartPart>(result[2]);
            Assert.IsInstanceOf<RawTextPostPart>(result[3]);
            Assert.IsInstanceOf<ItalicEndPart>(result[4]);
            Assert.IsInstanceOf<ParagraphStartPart>(result[5]);
            Assert.IsInstanceOf<RawTextPostPart>(result[6]);
        }

        [Test]
        public void ParsesQuotedQuestion()
        {
            string source = @"<span  class=""ljuser  i-ljuser  i-ljuser-deleted  i-ljuser-type-P     ""  lj:user=""pitirim_sas"" >" +
                            @"<a href=""http://pitirim-sas.livejournal.com/profile""  target=""_self""  class=""i-ljuser-profile"" >" +
                            @"<img  class=""i-ljuser-userhead""  src=""http://l-stat.livejournal.net/img/userinfo.gif?v=17080?v=129.5"" /></a>" +
                            @"<a href=""http://pitirim-sas.livejournal.com/"" class=""i-ljuser-username""   target=""_self""   ><b>pitirim_sas</b></a></span>: <br />" +
                            @"<br /><i>&gt;&gt;A<br />&gt;&gt;B<br /><br />&gt;C<br />&gt;Z</i><br /><br />Text";

            HTMLParser p = new HTMLParser();
            HTMLTokenBase[] tokens = p.Parse(source).ToArray();

            PostPartBase[] result = _m.CreateTextParts(tokens, null).ToArray();

            PostPartBase[] expected = new PostPartBase[]{
                new UserLinkPart("pitirim_sas"),
                new RawTextPostPart(":"),
                new ParagraphStartPart(2),
                new RawTextPostPart("A B"),
                new ParagraphStartPart(1),
                new RawTextPostPart("C Z"),
                new ParagraphStartPart(),
                new RawTextPostPart("Text"),
            };

            CollectionAssert.AreEqual(expected, result);
        }
    }
}
