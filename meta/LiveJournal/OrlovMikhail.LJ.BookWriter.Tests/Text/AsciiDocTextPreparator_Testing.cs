using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace OrlovMikhail.LJ.BookWriter.AsciiDoc.Tests
{
    [TestFixture]
    public class AsciiDocTextPreparator_Testing
    {
        [TestCase("\"Общее\"", "«Общее»")]
        [TestCase("„Jeszcze Polska nie zginęła“", "«Jeszcze Polska nie zginęła»")]
        [TestCase("Что „лучше“ – сыто есть", "Что «лучше»{nbsp}&#8212; сыто есть")]
        [TestCase("\"А в \"Б\"\"", "«А в{nbsp}«Б»»")]
        public void ConvertsQuotes(string source, string expected)
        {
            AsciiDocTextPreparator adtp = new AsciiDocTextPreparator();
            string result = adtp.Prepare(source);

            Assert.AreEqual(expected, result);
        }
    }
}
