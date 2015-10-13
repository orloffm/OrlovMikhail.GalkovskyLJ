using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace OrlovMikhail.LJ.BookWriter.Tests
{
    [TestFixture]
    public class ImagesExtralineProcessor_Testing
    {
        [Test]
        public void SurroundsImages()
        {
            Surrounds(new ImagePart(null));
        }

        [Test]
        public void SurroundsVideos()
        {
            Surrounds(new VideoPart(null));
        }

        void Surrounds(PostPartBase ppb)
        {
            PostPartBase[] parts =
            {
                new RawTextPostPart("A"),
                ppb,
                new RawTextPostPart("B")
            };

            PostPartBase[] expected =
            {
                new RawTextPostPart("A"),
                new ParagraphStartPart(0),
                ppb,
                new ParagraphStartPart(0),
                new RawTextPostPart("B"),
            };

            Check(parts, expected);
        }

        private void Check(PostPartBase[] parts, PostPartBase[] expected)
        {
            IProcessor cp = new ImagesExtralineProcessor();
            List<IPostPart> processed = cp.Process(parts);
            CollectionAssert.AreEqual(expected, processed);
        }
    }
}
