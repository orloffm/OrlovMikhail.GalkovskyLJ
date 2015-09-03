using System.Diagnostics;

namespace OrlovMikhail.LJ.BookWriter
{
    /// <summary>Base class for formatters.</summary>
    public abstract class FormattingBasePart : PostPartBase
    {
        public override PostPartBase FullClone() { return this; }
    }

    /// <summary>Base class for formatters.</summary>
    public abstract class FormattingStartBasePart : FormattingBasePart
    {
        
    }

    /// <summary>Base class for formatters.</summary>
    public abstract class FormattingEndBasePart : FormattingBasePart
    {
        
    }
}