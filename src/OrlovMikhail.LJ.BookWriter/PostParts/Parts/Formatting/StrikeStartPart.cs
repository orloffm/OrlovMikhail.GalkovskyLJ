using System.Diagnostics;

namespace OrlovMikhail.LJ.BookWriter
{
    [DebuggerDisplay("Strike \\/")]
    public class StrikeStartPart : FormattingStartBasePart
    {
        static StrikeStartPart _instance = new StrikeStartPart();

        public static StrikeStartPart Instance { get { return _instance; } }

        private StrikeStartPart()
        {

        }
    }
}