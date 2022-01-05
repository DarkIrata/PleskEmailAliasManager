using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace PleskEmailAliasManager.Data.PleskXmlApi
{
    [XmlRoot("packet")]
    public class Packet
    {
        private static readonly XmlSerializer serializer = new XmlSerializer(typeof(Packet));
        private readonly XmlSerializerNamespaces emptyNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
        private readonly XmlWriterSettings settings = new XmlWriterSettings()
        {
            Indent = true,
            OmitXmlDeclaration = true
        };

        [XmlElement("customer")]
        public Customer? Customer { get; set; }

        [XmlElement("site")]
        public Site? Site { get; set; }

        [XmlElement("webspace")]
        public Webspace? Webspace { get; set; }

        [XmlElement("mail")]
        public Mail? Mail { get; set; }

        [XmlElement("system")]
        public System? System { get; set; }

        [XmlIgnore]
        public string? RawPacket { get; private set; }

        public string Serialize()
        {
            using (var stream = new StringWriter())
            using (var wr = XmlWriter.Create(stream, this.settings))
            {
                serializer.Serialize(wr, this, this.emptyNamespaces);

                return stream.ToString();
            }
        }

        public static Packet Deserialize(string raw)
        {
            using (var reader = new StringReader(raw))
            {
                var packet = (Packet)serializer.Deserialize(reader)!;
                packet.RawPacket = raw;

                return packet;
            }
        }
    }
}
