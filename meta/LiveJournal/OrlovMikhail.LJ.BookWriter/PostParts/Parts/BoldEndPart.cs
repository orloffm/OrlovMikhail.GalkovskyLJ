using System.Diagnostics;

namespace OrlovMikhail.LJ.BookWriter
{
    [DebuggerDisplay("Bold /\\")]
    public class BoldEndPart : PostPartBase
    {
        static BoldEndPart _instance = new BoldEndPart();

        public static BoldEndPart Instance { get { return _instance; } }

        private BoldEndPart()
        {

        }

        public override PostPartBase FullClone() { return this; }
    }
}