using System.Diagnostics;

namespace OrlovMikhail.LJ.BookWriter
{
    /// <summary>Base class for formatters.</summary>
    public abstract class FormattingBasePart : PostPartBase
    {
        public override PostPartBase FullClone() { return this; }
    }
}