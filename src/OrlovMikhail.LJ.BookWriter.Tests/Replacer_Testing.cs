using System;
using NUnit.Framework;
using Rhino.Mocks;

namespace OrlovMikhail.LJ.BookWriter.Tests
{
    [TestFixture]
    public sealed class Replacer_Testing
    {
        //private TextPreparator _tp;

        // TODO
        //[TestCase("a — b", ExpectedResult = @"a\тире b")]
        //[TestCase("a – b", ExpectedResult = @"a\тире b")]
        //[TestCase("a - b", ExpectedResult = @"a\тире b")]
        //[TestCase("a -- b", ExpectedResult = @"a\тире b")]
        //[TestCase("a --- b", ExpectedResult = @"a\тире b")]
        //[TestCase("a\"", ExpectedResult = @"a\пк")]
        //[TestCase("ю\"\r\n\r\nН", ExpectedResult = "ю\\пк\r\n\r\nН")]
        //[TestCase("a\" b", ExpectedResult = @"a\пк b")]
        //[TestCase("— Г", ExpectedResult = @"\тире Г")]
        //[TestCase("...на", ExpectedResult = @"\мт на")]
        //[TestCase("я не лошадь", ExpectedResult = "я не лошадь")]
        //[TestCase("100$", ExpectedResult = @"100\$")]
        //[TestCase("я не лошадь", ExpectedResult = "я не лошадь")]
        //public string RunReplacement(string source)
        //{
        //    return Replacer.ReplaceIn(source);
        //}

        //[TestCase("Сборная по футболу проиграла в матче", ExpectedResult = "Сборная по~футболу проиграла в~матче")]
        //[TestCase("Сборная России по футболу проиграла команде Австрии в отборочном матче ", ExpectedResult = "Сборная России по~футболу проиграла команде Австрии в~отборочном матче ")]
        //public string RunLargeChunk(string source)
        //{
        //    return Replacer.ReplaceIn(source);
        //}
        
        //[Test, Combinatorial]
        //public void AutoTildeIsInserted(
        //   [Values(' ', '\n', '>', null)] char? pre,
        //   [Values("и", "в", "а", "к", "за", "под", "по", "с", "со","но", "на", "из", "над", "о", "вне", "у", "+")] string content)
        //{
        //    // Usual
        //    string source =  content + " ";
        //    if(pre != null)
        //        source = pre.Value.ToString() + source;

        //    string result = Replacer.ReplaceIn(source);
        //    Assert.AreEqual(source.TrimEnd(' ') + "~", result);

        //    // Upper
        //    source = pre.ToString() + content.ToUpper() + " ";

        //    result = Replacer.ReplaceIn(source);
        //    Assert.AreEqual(source.TrimEnd(' ') + "~", result);
        //}
    }
}
