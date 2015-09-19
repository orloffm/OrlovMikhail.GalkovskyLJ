using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace OrlovMikhail.LJ.BookWriter.Tests
{
    [TestFixture]
    public class LineBreaksMergingProcessor_Testing
    {
        private void Check(PostPartBase[] parts, PostPartBase[] expected)
        {
            IProcessor cp = new LineBreaksMergingProcessor();
            List<IPostPart> processed = cp.Process(parts);
            CollectionAssert.AreEqual(expected, processed);
        }

        [Test]
        public void MergesLineBreaks()
        {
            PostPartBase[] parts =
            {
                LineBreakPart.Instance,
                LineBreakPart.Instance,
                LineBreakPart.Instance
            };

            PostPartBase[] expected =
            {
                new ParagraphStartPart(),
            };

            Check(parts, expected);
        }

        [Test]
        public void LineBreaksAreConsumedByParagraphs()
        {
            PostPartBase[] parts =
            {
                LineBreakPart.Instance,
                new ParagraphStartPart(0),
                LineBreakPart.Instance,
                LineBreakPart.Instance,
                new ParagraphStartPart(1),
                LineBreakPart.Instance
            };

            PostPartBase[] expected =
            {
                new ParagraphStartPart(1),
            };

            Check(parts, expected);
        }
    }
}
