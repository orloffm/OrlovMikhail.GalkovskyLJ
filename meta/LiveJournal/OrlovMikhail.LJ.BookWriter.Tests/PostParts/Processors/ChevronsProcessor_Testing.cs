using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace OrlovMikhail.LJ.BookWriter.Tests
{
    [TestFixture]
    public class ChevronsProcessor_Testing
    {
        [Test]
        public void ReplaceLineBreakIfDifferentNumberOfChevrons()
        {
            PostPartBase[] parts =
            {
                new RawTextPostPart(">A"),
                LineBreakPart.Instance,
                new RawTextPostPart(">B"),
                LineBreakPart.Instance,
                new RawTextPostPart(">>C"),
                LineBreakPart.Instance,
                new RawTextPostPart(">A"),
            };

            ChevronsProcessor cp = new ChevronsProcessor();
            List<PostPartBase> processed = cp.Process(parts);

            PostPartBase[] expected =
            {
                new RawTextPostPart("> A"),
                LineBreakPart.Instance,
                new RawTextPostPart("> B"),
                ParagraphStartPart.Instance,
                new RawTextPostPart("> > C"),
                ParagraphStartPart.Instance,
                new RawTextPostPart("> A"),
            };

            CollectionAssert.AreEqual(expected, processed);
        }
    }
}
