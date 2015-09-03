using System.Diagnostics;

namespace OrlovMikhail.LJ.BookWriter
{
    [DebuggerDisplay("Italic /\\")]
    public class ItalicEndPart : FormattingEndBasePart
    {
        static ItalicEndPart _instance = new ItalicEndPart();

        public static ItalicEndPart Instance { get { return _instance; } }

        private ItalicEndPart()
        {

        }
    }
}