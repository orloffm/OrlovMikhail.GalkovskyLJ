using System.Diagnostics;

namespace OrlovMikhail.LJ.BookWriter
{
    [DebuggerDisplay("Strike /\\")]
    public class StrikeEndPart : FormattingEndBasePart
    {
        static StrikeEndPart _instance = new StrikeEndPart();

        public static StrikeEndPart Instance { get { return _instance; } }

        private StrikeEndPart()
        {

        }
    }
}