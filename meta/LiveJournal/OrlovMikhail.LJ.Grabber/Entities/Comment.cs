using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OrlovMikhail.LJ.Grabber
{
    [Serializable]
    [DebuggerDisplay("{Id} {Poster.Username}: {Text}")]
    public sealed class Comment : EntryBase, IHasReplies, IEquatable<Comment>
    {
        public Comment()
        {
            this.IsFull = true;
            this.Policy = UsagePolicy.Default;
            this.Replies = new Replies();
        }

        [XmlAttribute("depth")]
        public int Depth { get; set; }

        /// <summary>Allows to override the usage of the 
        /// comment when writing the target file.</summary>
        [XmlAttribute("policy")]
        [DefaultValue(UsagePolicy.Default)]
        public UsagePolicy Policy { get; set; }

          [XmlIgnore]
        public bool PolicySpecified { get{return Policy != UsagePolicy.Default; }}

        [XmlAttribute("full")]
        [DefaultValue(true)]
        public bool IsFull { get; set; }

        [XmlIgnore]
        public bool IsFullSpecified { get { return !IsFull; } }

        [XmlAttribute("deleted")]
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }

        [XmlIgnore]
        public bool IsDeletedSpecified { get { return IsDeleted; } }

        [XmlAttribute("screened")]
        [DefaultValue(false)]
        public bool IsScreened { get; set; }

        [XmlIgnore]
        public bool IsScreenedSpecified { get { return IsScreened; } }

        [XmlAttribute("frozen")]
        [DefaultValue(false)]
        public bool IsFrozen { get; set; }

        [XmlIgnore]
        public bool IsFrozenSpecified { get { return IsFrozen; } }

        [XmlAttribute("suspended")]
        [DefaultValue(false)]
        public bool IsSuspendedUser { get; set; }

        [XmlIgnore]
        public bool IsSuspendedUserSpecified { get { return IsSuspendedUser; } }

        [XmlElement("comments")]
        public Replies Replies { get; set; }

        [XmlElement("parent")]
        public string ParentUrl { get; set; }

        [XmlIgnore]
        public bool ParentUrlSpecified { get { return !String.IsNullOrWhiteSpace(ParentUrl); } }

        public Comment MakeClone()
        {
            return this.MemberwiseClone() as Comment;
        }

        #region equality
        public bool Equals(Comment other)
        {
            return other!=null&& this.Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Comment);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
        #endregion
    }

    public enum UsagePolicy
    {
        Default = 0,
        Ignore,
        Forced,
    }
}