using System;
using System.Diagnostics;

namespace OrlovMikhail.LJ.BookWriter
{
    [DebuggerDisplay("[Paragraph, {QuotationLevel} quote level]")]
    public class ParagraphStartPart : NewBlockStartBasePart, IEquatable<ParagraphStartPart>
    {
        public ParagraphStartPart(int quotationLevel = 0)
        {
            this.QuotationLevel = quotationLevel;
        }

        public int QuotationLevel { get; set; }

        #region equality
        public override int GetHashCode()
        {
            return QuotationLevel.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return AreEqual(this, obj as ParagraphStartPart);
        }

        public static bool AreEqual(ParagraphStartPart p1, ParagraphStartPart p2)
        {
            if(ReferenceEquals(p1, p2))
                return true;
            else if(ReferenceEquals(p1, null) || ReferenceEquals(p2, null))
                return false;

            return p1.QuotationLevel == p2.QuotationLevel;
        }

        public bool Equals(ParagraphStartPart other)
        {
            return AreEqual(this, other);
        }
        #endregion
    }
}