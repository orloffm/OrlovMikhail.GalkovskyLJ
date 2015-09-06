using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace OrlovMikhail.LJ.BookWriter.Tests
{
    [TestFixture]
    public class QuoteNormalizingProcessor_Testing
    {
        [Test]
        public void Basic()
        {
            int[] input = new int[] { 0, 1, 3, 1, 0, 4 };
            int[] output = new int[] { 0, 1, 2, 1, 0, 1 };

            Check(input, output);
        }

        [Test]
        public void EdgeCase1()
        {
            int[] input = new int[] { 0, 1, 3, 0 };
            int[] output = new int[] { 0, 1, 2, 0 };

            Check(input, output);
        }

        [Test]
        public void EdgeCase2()
        {
            int[] input = new int[] { 0, 3, 1, 0 };
            int[] output = new int[] { 0, 2, 1, 0 };

            Check(input, output);
        }

        [Test]
        public void EdgeCase3()
        {
            int[] input = new int[] { 0, 4, 0 };
            int[] output = new int[] { 0, 1, 0 };

            Check(input, output);
        }

        private void Check(int[] input, int[] output)
        {
            PostPartBase[] parts = input.Select(i => new ParagraphStartPart(i) as PostPartBase).ToArray();
            PostPartBase[] expected = output.Select(i => new ParagraphStartPart(i) as PostPartBase).ToArray();

            IProcessor cp = new QuoteNormalizingProcessor();
            List<PostPartBase> processed = cp.Process(parts);
            CollectionAssert.AreEqual(expected, processed);
        }
    }
}
