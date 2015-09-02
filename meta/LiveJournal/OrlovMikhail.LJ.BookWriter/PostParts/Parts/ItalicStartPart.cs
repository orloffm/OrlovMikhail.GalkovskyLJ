using System.Diagnostics;

namespace OrlovMikhail.LJ.BookWriter
{
    [DebuggerDisplay("Italic \\/")]
    public class ItalicStartPart : PostPartBase
    {
        static ItalicStartPart _instance = new ItalicStartPart();

        public static ItalicStartPart Instance { get { return _instance; } }

        private ItalicStartPart()
        {

        }

        public override PostPartBase FullClone() { return this; }
    }
}