using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace OrlovMikhail.LJ.Grabber
{
    public sealed class LayerParser : ILayerParser
    {
        public EntryPage ParseAsAnEntryPage(string content)
        {
            // Sometimes there are zeroes and control characters.
            StringBuilder sanitizer = new StringBuilder(content.Length);
            foreach(char c in content)
            {
                if(c < 32 && (c != '\n' && c != '\r' && c != '\t'))
                    continue;

                sanitizer.Append(c);
            }
            content = sanitizer.ToString();

            XmlSerializer s = new XmlSerializer(typeof(EntryPage));
            EntryPage ret;
            using(StringReader sr = new StringReader(content))
                ret = (EntryPage)s.Deserialize(sr);

            return ret;
        }

        public string Serialize(EntryPage ep)
        {
            UTF8Encoding enc = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Indent = true;
            settings.ConformanceLevel = ConformanceLevel.Auto;
            settings.Encoding = enc;

            XmlSerializerNamespaces names = new XmlSerializerNamespaces();
            names.Add("", "");

            MemoryStream ms = new MemoryStream();
            XmlWriter writer = XmlWriter.Create(ms, settings);
            XmlSerializer sr = new XmlSerializer(typeof(EntryPage));

            sr.Serialize(writer, ep, names);

            return enc.GetString(ms.ToArray());
        }
    }
}
