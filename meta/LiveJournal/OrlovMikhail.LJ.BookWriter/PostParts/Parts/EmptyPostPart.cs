using System;
using System.Diagnostics;

namespace OrlovMikhail.LJ.BookWriter
{
    /// <summary>Zero-width item.</summary>
    [DebuggerDisplay("{empty}")]
    public class EmptyPostPart : PostPartBase, IRendersAsText
    {
        static EmptyPostPart _instance = new EmptyPostPart();

        public static EmptyPostPart Instance { get { return _instance; } }

        private EmptyPostPart()
        {

        }

        public override IPostPart FullClone() { return this; }

        public bool CanBeTrimmed
        {
            get { return false; }
        }
    }
}