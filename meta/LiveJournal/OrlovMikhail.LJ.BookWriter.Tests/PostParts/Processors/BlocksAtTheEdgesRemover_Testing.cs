using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NUnit.Framework;

namespace OrlovMikhail.LJ.BookWriter
{
    [TestFixture]
    public class BlocksAtTheEdgesRemover_Testing
    {
        [Test]
        public void KeepsQuotationStartAtTheBeginning()
        {
            PostPartBase[] parts = new PostPartBase[]
            {
                new ParagraphStartPart(1),
                LineBreakPart.Instance,
                new RawTextPostPart("A"),
                new ParagraphStartPart(0),
                new RawTextPostPart("B"),
                new ParagraphStartPart(1),
                LineBreakPart.Instance,
            };

            PostPartBase[] expected = new PostPartBase[]
            {
                new ParagraphStartPart(1),
                new RawTextPostPart("A"),
                new ParagraphStartPart(0),
                new RawTextPostPart("B")
            };

            Check(parts, expected);
        }

        [Test]
        public void RemovesDefaultCase()
        {
            PostPartBase[] parts = new PostPartBase[]
            {
                new ParagraphStartPart(0),
                LineBreakPart.Instance,
                new RawTextPostPart("A"),
                new ParagraphStartPart(0),
                new RawTextPostPart("B"),
                new ParagraphStartPart(1),
                LineBreakPart.Instance,
            };

            PostPartBase[] expected = new PostPartBase[]
            {
                new RawTextPostPart("A"),
                new ParagraphStartPart(0),
                new RawTextPostPart("B")
            };

            Check(parts, expected);
        }

        private void Check(PostPartBase[] parts, PostPartBase[] expected)
        {
            IProcessor cp = new BlocksAtTheEdgesRemover();
            List<PostPartBase> processed = cp.Process(parts);
            CollectionAssert.AreEqual(expected, processed);
        }

    }
}
