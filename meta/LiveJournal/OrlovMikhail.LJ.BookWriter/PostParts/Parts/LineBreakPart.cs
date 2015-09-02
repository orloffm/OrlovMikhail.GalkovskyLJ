using System.Diagnostics;

namespace OrlovMikhail.LJ.BookWriter
{
    [DebuggerDisplay("[Line break]")]
    public class LineBreakPart : PostPartBase
    {
         static LineBreakPart _instance = new LineBreakPart();

        public static LineBreakPart Instance { get { return _instance; } }

        private LineBreakPart()
        {

        }

        public override PostPartBase FullClone() { return this; }
    }
}