using System.Diagnostics;

namespace OrlovMikhail.LJ.BookWriter
{
    [DebuggerDisplay("Bold /\\")]
    public class BoldEndPart : FormattingEndBasePart
    {
        static BoldEndPart _instance = new BoldEndPart();

        public static BoldEndPart Instance { get { return _instance; } }

        private BoldEndPart()
        {

        }
    }
}