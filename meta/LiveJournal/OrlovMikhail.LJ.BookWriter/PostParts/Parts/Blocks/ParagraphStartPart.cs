using System.Diagnostics;

namespace OrlovMikhail.LJ.BookWriter
{
    [DebuggerDisplay("[Paragraph]")]
    public class ParagraphStartPart : NewBlockStartBasePart
    {
        static ParagraphStartPart _instance = new ParagraphStartPart();

        public static ParagraphStartPart Instance { get { return _instance; } }

        private ParagraphStartPart()
        {

        }

        public int QuotationLevel { get; set; }
    }
}