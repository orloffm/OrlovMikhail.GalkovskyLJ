using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace OrlovMikhail.LJ.Grabber
{
    [Serializable]
    public sealed class Userpic : IEquatable<Userpic>
    {
        public Userpic()
        {

        }

        [XmlAttribute("url")]
        [DefaultValue(null)]
        public string Url { get; set; }

        [XmlIgnore]
        public bool UrlSpecified { get { return !String.IsNullOrWhiteSpace(Url); } }

        public Uri GetUri()
        {
            if (String.IsNullOrWhiteSpace(Url))
                return null;
            else
                return new Uri(Url);
        }

        [XmlAttribute("height")]
        [DefaultValue(null)]
        public string Height { get; set; }

        [XmlIgnore]
        public bool HeightSpecified { get { return !String.IsNullOrWhiteSpace(Height); } }

        [XmlAttribute("width")]
        [DefaultValue(null)]
        public string Width { get; set; }

        [XmlIgnore]
        public bool WidthSpecified { get { return !String.IsNullOrWhiteSpace(Width); } }

        public static bool AreEqual(Userpic a, Userpic b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return String.Equals(a.Url, b.Url, StringComparison.OrdinalIgnoreCase);
        }

        public bool Equals(Userpic other)
        {
            return AreEqual(this, other);
        }

        public override bool Equals(object obj)
        {
            return AreEqual(this, obj as Userpic);
        }

        public override int GetHashCode()
        {
            if (Url == null)
                return 0;
            else
                return Url.GetHashCode();
        }

        public override string ToString()
        {
            return Url;
        }
    }
}