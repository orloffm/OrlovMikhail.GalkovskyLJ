using System.Diagnostics;

namespace OrlovMikhail.LJ.BookWriter
{
    [DebuggerDisplay("{Text}")]
    public class RawTextPostPart : PostPartBase
    {
        public string Text { get; set; }

        public RawTextPostPart(string text)
        {
            Text = text;
        }
    }
}