using System;
using System.Xml.Serialization;

namespace OrlovMikhail.LJ.Grabber
{
    [Serializable]
    public sealed class Entry : EntryBase
    {
        [XmlElement("prev_url")]
        public string PreviousUrl { get; set; }

        [XmlElement("next_url")]
        public string NextUrl { get; set; }
    }
}