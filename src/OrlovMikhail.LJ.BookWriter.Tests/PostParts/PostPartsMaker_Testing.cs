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
        private HTMLParser _p;
        private PostPartsMaker _m;

        [SetUp]
        public void Setup()
        {
            _p = new HTMLParser();
            _m = new PostPartsMaker();
        }

        private void Check(string html, IPostPart[] expected)
        {
            HTMLTokenBase[] tokens = _p.Parse(html).ToArray();
            IPostPart[] result = _m.CreateTextParts(tokens, null).ToArray();

            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void ExtractsYouTubeVideo()
        {
            string source = @"<iframe src=""http://l.lj-toys.com/?auth_token=sessionless%3A1439474400%3Aembedcontent%3A1380996%2686%261%260%26youtube%26Y64bw6Xxduw%3A72e47f9e2e972625858c7dc3fd62b06dc4758bea&amp;source=youtube&amp;vid=Y64bw6Xxduw&amp;moduleid=86&amp;preview=0&amp;journalid=1380996&amp;noads=1"" width=""640"" height=""390"" frameborder=""0"" class=""lj_embedcontent"" allowfullscreen name=""embed_1380996_86""></iframe>";

            PostPartBase[] expected = new PostPartBase[]{
                new VideoPart(@"https://www.youtube.com/watch?v=Y64bw6Xxduw"),
            };

            Check(source, expected);
        }

        [Test]
        public void ReplacesSpecialQuotationStyle_756()
        {
            string source = @"-- A --<br /><br />Z ";

            PostPartBase[] expected = new PostPartBase[]{
                new ParagraphStartPart(1),
                new RawTextPostPart("A"),
                new ParagraphStartPart(),
                new RawTextPostPart("Z"),
            };

            Check(source, expected);
        }

        [Test]
        public void KeepsTheSpaceAfterUserName()
        {
            string source = @" <br /><br /><span  class=""ljuser  i-ljuser  i-ljuser-deleted  i-ljuser-type-P     ""  lj:user=""sharkun"" ><a href=""http://sharkun.livejournal.com/profile""  target=""_self""  class=""i-ljuser-profile"" ><img  class=""i-ljuser-userhead""  src=""http://l-stat.livejournal.net/img/userinfo.gif?v=17080?v=129.5"" /></a><a href=""http://sharkun.livejournal.com/"" class=""i-ljuser-username""   target=""_self""   ><b>sharkun</b></a></span> Z";

            PostPartBase[] expected = new PostPartBase[]{
                new UserLinkPart("sharkun"),
                new RawTextPostPart(" Z"),
            };

            Check(source, expected);
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

            IPostPart[] result = _m.CreateTextParts(tokens.ToArray(), null).ToArray();
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

            IPostPart[] result = _m.CreateTextParts(tokens.ToArray(), null).ToArray();
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
        public void ParsesDoubleQuotedQuestion()
        {
            string source = @"Z: <br /><br />" +
                            @"<i>&gt;A<br />" +
                            @"&gt;B<br />" +
                            @"&gt;C<br /><br />" +
                            @"&gt;&gt;D<br /><br />" +
                            @"&gt;E<br />" +
                            @"&gt;F<br />" +
                            @"&gt;G </i><br /><br />Z";

            IPostPart[] expected = new PostPartBase[]{
                new RawTextPostPart("Z:"),
                new ParagraphStartPart(1),
                new RawTextPostPart("A B C"),
                new ParagraphStartPart(2),
                new RawTextPostPart("D"),
                new ParagraphStartPart(1),
                new RawTextPostPart("E F G"),
                new ParagraphStartPart(0),
                new RawTextPostPart("Z"),
            };

            Check(source, expected);
        }

        [Test]
        public void ParsesEmptyAsEmpty()
        {
            HTMLParser p = new HTMLParser();
            HTMLTokenBase[] tokens = p.Parse("").ToArray();

            IPostPart[] result = _m.CreateTextParts(tokens, null).ToArray();
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void NoSuddenPlus_124()
        {
            string source = @"A. <br /><br /> <br />B:<br /><i>&gt;C</i><br />D";

            PostPartBase[] expected = new PostPartBase[]{
                EmptyPostPart.Instance,
                new RawTextPostPart("A."),
                new ParagraphStartPart(),
                new RawTextPostPart("B:"),
                new ParagraphStartPart(1),
                new RawTextPostPart("C"),
                new ParagraphStartPart(),
                new RawTextPostPart("D"),
            };

            Check(source, expected);
        }

        [Test]
        public void PreventsEmDashesAfterURLs()
        {
            string source = @"A<br /><a href='http://www.kreml.org/users/' rel='nofollow'>http://www.kreml.org/users/</a> - B";

            IPostPart[] expected = new PostPartBase[]{
                new RawTextPostPart("A"),
                LineBreakPart.Instance,
                new RawTextPostPart(@"http://www.kreml.org/users/ - B")
            };

            Check(source, expected);
        }

        [Test]
        public void ParsesQuotedQuestion()
        {
            string source = @"<span  class=""ljuser  i-ljuser  i-ljuser-deleted  i-ljuser-type-P     ""  lj:user=""pitirim_sas"" >" +
                            @"<a href=""http://pitirim-sas.livejournal.com/profile""  target=""_self""  class=""i-ljuser-profile"" >" +
                            @"<img  class=""i-ljuser-userhead""  src=""http://l-stat.livejournal.net/img/userinfo.gif?v=17080?v=129.5"" /></a>" +
                            @"<a href=""http://pitirim-sas.livejournal.com/"" class=""i-ljuser-username""   target=""_self""   ><b>pitirim_sas</b></a></span>: <br />" +
                            @"<br /><i>&gt;&gt;A<br />&gt;&gt;B<br /><br />&gt;C<br />&gt;Z</i><br /><br />Text";

            IPostPart[] expected = new PostPartBase[]{
                new UserLinkPart("pitirim_sas"),
                new RawTextPostPart(":"),
                new ParagraphStartPart(2),
                new RawTextPostPart("A B"),
                new ParagraphStartPart(1),
                new RawTextPostPart("C Z"),
                new ParagraphStartPart(),
                new RawTextPostPart("Text"),
            };

            Check(source, expected);
        }
    }
}
