using System.Diagnostics;

namespace OrlovMikhail.LJ.BookWriter
{
    /// <summary>Base class for line break and new paragraph start.</summary>
    public abstract class NewBlockStartBasePart : PostPartBase
    {
        public override PostPartBase FullClone() { return this; }
    }
}