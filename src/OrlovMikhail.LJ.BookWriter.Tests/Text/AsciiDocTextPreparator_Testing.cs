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
        [TestCase("A - B", "A{nbsp}&#8212; B")]
        [TestCase("- B", "&#8212;{nbsp}B")]
        [TestCase(@"http://ya.ru - B", @"http://ya.ru &#8212; B")]
        [TestCase("Вася-\"Пупкин\"", "Вася-«Пупкин»")]
        [TestCase("\"Вася\"-Пупкин", "«Вася»-Пупкин")]
        [TestCase("(\"Общее\")", "(«Общее»)")]
        [TestCase("(\"(Общее)\")", "(«(Общее)»)")]
        [TestCase("„Jeszcze Polska nie zginęła“", "«Jeszcze Polska nie zginęła»")]
        [TestCase("Что „лучше“ – сыто есть", "Что «лучше»{nbsp}&#8212; сыто есть")]
        [TestCase("\"А в \"Б\"\"", "«А в{nbsp}«Б»»")]
        [TestCase(@"(а точнее ""тройки"")", @"(а{nbsp}точнее «тройки»)")]
        public void ConvertsQuotes(string source, string expected)
        {
            AsciiDocTextPreparator adtp = new AsciiDocTextPreparator();
            string result = adtp.Prepare(source);

            Assert.AreEqual(expected, result);
        }

        [TestCase("? ? ?", "? ? ?")]
        [TestCase("ABC", "ABC")]
        [TestCase("?ABC", "?ABC")]
        [TestCase("ABC!?", "ABC!?")]
        [TestCase("_ABC_", @"\_ABC_")]
        [TestCase("_ABC_?", @"\_ABC_?")]
        [TestCase("_A*BC_", @"\_A*BC_")]
        [TestCase("*_A*_", @"*\_A*_")]
        [TestCase("A*B_C_", "A*B_C_")]
        [TestCase("*A*B_C_", @"*A*B_C_")]
        [TestCase("*_ABC_* *_DEF_*", @"\*\_ABC_* \*\_DEF_*")]
        [TestCase("*_ABC_*? *_DEF_*", @"\*\_ABC_*? \*\_DEF_*")]
        public void EscapesFormatters(string source, string expected)
        {
            string result = AsciiDocTextPreparator.EscapeFormatters(source);

            Assert.AreEqual(expected, result);
        }
    }
}
