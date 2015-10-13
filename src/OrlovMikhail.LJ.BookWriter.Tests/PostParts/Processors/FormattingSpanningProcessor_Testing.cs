using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace OrlovMikhail.LJ.BookWriter.Tests
{
    [TestFixture]
    public class FormattingSpanningProcessor_Testing
    {
        [Test]
        public void InTheMiddle()
        {
            PostPartBase[] parts =
            {
                new ParagraphStartPart(0),
                ItalicStartPart.Instance,
                new RawTextPostPart("A"),
                new ParagraphStartPart(0),
                new RawTextPostPart("B"),
                ItalicEndPart.Instance,
                new ParagraphStartPart(0),
            };

            PostPartBase[] expected =
            {
                new ParagraphStartPart(0),
                ItalicStartPart.Instance,
                new RawTextPostPart("A"),
                ItalicEndPart.Instance,
                new ParagraphStartPart(0),
                ItalicStartPart.Instance,
                new RawTextPostPart("B"),
                ItalicEndPart.Instance,
                new ParagraphStartPart(0),
            };

            Check(parts, expected);
        }

        [Test]
        public void InTheBeginning()
        {
            PostPartBase[] parts =
            {
                ItalicStartPart.Instance,
                new RawTextPostPart("A"),
                new ParagraphStartPart(0),
                new RawTextPostPart("B"),
                ItalicEndPart.Instance,
                new ParagraphStartPart(0),
            };

            PostPartBase[] expected =
            {
                ItalicStartPart.Instance,
                new RawTextPostPart("A"),
                ItalicEndPart.Instance,
                new ParagraphStartPart(0),
                ItalicStartPart.Instance,
                new RawTextPostPart("B"),
                ItalicEndPart.Instance,
                new ParagraphStartPart(0),
            };

            Check(parts, expected);
        }

        private void Check(PostPartBase[] parts, PostPartBase[] expected)
        {
            IProcessor cp = new FormattingSpanningProcessor();
            List<IPostPart> processed = cp.Process(parts);
            CollectionAssert.AreEqual(expected, processed);
        }
    }
}
