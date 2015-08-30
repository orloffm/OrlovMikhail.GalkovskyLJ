using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OrlovMikhail.LJ.Grabber
{
    [Serializable]
    [DebuggerDisplay("{Id}: {Text}")]
    public abstract class EntryBase : IEntryBase
    {
        public EntryBase()
        {
            Poster = new UserLite();
            PosterUserpic = new Userpic();
        }

        [XmlAttribute("id")]
        public long Id { get; set; }

        [XmlElement("url")]
        public string Url { get; set; }

        [XmlElement("user")]
        public UserLite Poster { get; set; }

        [XmlElement("userpic")]
        [DefaultValue(null)]
        public Userpic PosterUserpic { get; set; }

        [XmlIgnore]
        public bool PosterUserpicSpecified
        {
            get
            {
                if(PosterUserpic == null)
                    return false;

                bool hasAnything = (!String.IsNullOrWhiteSpace(PosterUserpic.Height) ||
                                    !String.IsNullOrWhiteSpace(PosterUserpic.Width) ||
                                    !String.IsNullOrWhiteSpace(PosterUserpic.Url));

                return hasAnything;
            }
        }

        [XmlIgnore]
        public DateTime? Date { get; set; }

        [XmlElement("date")]
        public string DateValue
        {
            get { return Date.HasValue ? Date.Value.ToString("yyyy-MM-dd hh:mm:ss") : null; }
            set { Date = ParseDateTimeFromString(value); }
        }

        public static DateTime? ParseDateTimeFromString(string value)
        {
            if(String.IsNullOrWhiteSpace(value))
                return null;

            string[] s = value.Split(new char[] { '-', ':', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if(s.Length != 6)
                return null;

            DateTime result = new DateTime(int.Parse(s[0]), int.Parse(s[1]), int.Parse(s[2]),
                int.Parse(s[3]), int.Parse(s[4]), int.Parse(s[5]));
            return result;
        }

        [XmlIgnore]
        public bool DateValueSpecified { get { return Date.HasValue; } }

        [XmlElement("text")]
        [DefaultValue(null)]
        public string Text { get; set; }

        [XmlIgnore]
        public bool TextSpecified { get { return !String.IsNullOrWhiteSpace(Text); } }

        [XmlElement("subject")]
        [DefaultValue(null)]
        public string Subject { get; set; }

        [XmlIgnore]
        public bool SubjectSpecified { get { return !String.IsNullOrWhiteSpace(Subject); } }

        public override string ToString()
        {
            return Id.ToString();
        }
    }
}