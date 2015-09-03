using System.Diagnostics;

namespace OrlovMikhail.LJ.BookWriter
{
    [DebuggerDisplay("Italic \\/")]
    public class ItalicStartPart : FormattingBasePart
    {
        static ItalicStartPart _instance = new ItalicStartPart();

        public static ItalicStartPart Instance { get { return _instance; } }

        private ItalicStartPart()
        {

        }
    }
}