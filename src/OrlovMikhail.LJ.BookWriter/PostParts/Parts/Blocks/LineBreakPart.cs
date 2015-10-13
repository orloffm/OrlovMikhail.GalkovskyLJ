using System.Diagnostics;

namespace OrlovMikhail.LJ.BookWriter
{
    [DebuggerDisplay("[Line break]")]
    public class LineBreakPart : NewBlockStartBasePart
    {
        static LineBreakPart _instance = new LineBreakPart();

        public static LineBreakPart Instance { get { return _instance; } }

        private LineBreakPart()
        {

        }

        public override IPostPart FullClone() { return this; }
    }
}