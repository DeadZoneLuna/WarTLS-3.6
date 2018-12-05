using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

using static WARTLS.Tools;

namespace WARTLS.CLASSES
{
    class Stanza
    {
        public static XNamespace NameSpace = "urn:cryonline:k01";
        protected Client User;
        protected XmlDocument Packet;
        protected XmlNode Query;
        internal string To;
        internal string Id;
        internal string Type;
        internal string Name;
        public Stanza(Client User,XmlDocument Packet)
        {
            this.User = User;
            if (Packet == null) return;
            if(Packet["message"]==null)
            Name = Packet["iq"]["query"].FirstChild.Name;
            try
            {
                To = Packet[Packet.FirstChild.Name].Attributes["to"].InnerText;
            }
            catch { }
            if(Packet[Packet.FirstChild.Name].Attributes["id"]!=null)
                Id = Packet[Packet.FirstChild.Name].Attributes["id"].InnerText;
            if (Packet[Packet.FirstChild.Name].Attributes["type"] != null)
                Type = Packet[Packet.FirstChild.Name].Attributes["type"].InnerText;
            if (Packet["iq"] !=null && Packet["iq"]["query"].FirstChild != null)
                this.Query = Packet["iq"]["query"].FirstChild;


            this.Packet = Packet;
        }
        internal virtual void Process()
        {

        }
        internal void Uncompress(ref XmlDocument Packet)
        {
            XmlDocument Document = new XmlDocument();
            string Decompressed = DeflateTool.Decode(Packet.LastChild.LastChild.LastChild.Attributes["compressedData"].InnerText);
            Packet.FirstChild.FirstChild.RemoveAll();
            Document.LoadXml(Decompressed);
            XmlNode Child = Packet.ImportNode(Document.DocumentElement, true);
            Packet.FirstChild.FirstChild.AppendChild(Child);
            Packet.LastChild.LastChild.ReplaceChild(Packet.LastChild.LastChild.LastChild, Child);
        }
        internal void Compress(ref XDocument xDocument)
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }

            XmlDocument Header = new XmlDocument();

            XmlDocument Answ = xmlDocument;
            XmlNode Node = Answ.FirstChild.FirstChild.FirstChild;

            string Name = Node.Name;
            string Encoded = DeflateTool.Encode(Node.OuterXml);
            int OriginalLenght = Encoding.UTF8.GetByteCount(Node.OuterXml);
            Header.LoadXml($"<data query_name='{Name}' compressedData='{Encoded}' originalSize='{OriginalLenght}'/>");

            foreach (XmlAttribute Att in Node.Attributes)
            {
                XmlAttribute CreatedAttrib = Header.CreateAttribute(Att.Name);
                CreatedAttrib.InnerText = Att.InnerText;
                Header.FirstChild.Attributes.Prepend(CreatedAttrib);
            }
            XmlNode Imported = Answ.ImportNode(Header.DocumentElement, true);
            Answ.FirstChild.FirstChild.RemoveAll();
            Answ.FirstChild.FirstChild.PrependChild(Imported);

            using (var nodeReader = new XmlNodeReader(xmlDocument))
            {
                nodeReader.MoveToContent();
                xDocument = XDocument.Load(nodeReader);
            }
        }
    }

    public static class DocumentExtensions
    {
        public static XmlDocument ToXmlDocument(this XDocument xDocument)
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            return xmlDocument;
        }

        public static XDocument ToXDocument(this XmlDocument xmlDocument)
        {
            using (var nodeReader = new XmlNodeReader(xmlDocument))
            {
                nodeReader.MoveToContent();
                return XDocument.Load(nodeReader);
            }
        }
    }

}
