using System.Diagnostics;

namespace OrlovMikhail.LJ.BookWriter
{
    [DebuggerDisplay("Bold \\/")]
    public class BoldStartPart : FormattingBasePart
    {
        static BoldStartPart _instance = new BoldStartPart();

        public static BoldStartPart Instance { get { return _instance; } }

        private BoldStartPart()
        {

        }
    }
}