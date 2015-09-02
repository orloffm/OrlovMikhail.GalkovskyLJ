using System;
using System.Diagnostics;

namespace OrlovMikhail.LJ.BookWriter
{
    [DebuggerDisplay("{Text}")]
    public class RawTextPostPart : PostPartBase, IEquatable<RawTextPostPart>
    {
        public string Text { get; set; }

        public RawTextPostPart(string text)
        {
            Text = text;
        }

        public override int GetHashCode()
        {
            return Text.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return AreEqual(this, obj as RawTextPostPart);
        }

        public static bool AreEqual(RawTextPostPart p1, RawTextPostPart p2)
        {
            if (ReferenceEquals(p1, p2))
                            return true;
            else if (ReferenceEquals(p1, null) || ReferenceEquals(p2, null))
                return false;

            return String.Equals(p1.Text, p2.Text, StringComparison.OrdinalIgnoreCase);
        }

        public bool Equals(RawTextPostPart other)
        {
            return AreEqual(this, other);
        }
    }
}