using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using WARTLS.CLASSES;
using WARTLS.NETWORK;

namespace WARTLS.XMPP.QUERY
{
    class ToOnlinePlayers : Stanza
    {
        string Channel;
        Client Receiver;
        public ToOnlinePlayers(Client User, XmlDocument Packet) : base(User, Packet)
        {
            Receiver = ArrayList.OnlineUsers.Find(Attribute => Attribute.JID == base.To);
            Process();
        }
        internal override void Process()
        {

            if (Receiver != null)
                Receiver.Send(base.Packet.InnerXml);
            else
            {
                XDocument Packet = new XDocument();

                XElement iqElement = new XElement("iq");
                iqElement.Add(new XAttribute("type", base.Type));
                if(base.To!=null)
                iqElement.Add(new XAttribute("from", base.To));
                iqElement.Add(new XAttribute("to", User.JID));
                iqElement.Add(new XAttribute("id", base.Id));

                XElement queryElement = new XElement(Stanza.NameSpace + "query");

                XElement errorElement = new XElement("error");
                errorElement.Add(new XAttribute("type", "cancel"));
                errorElement.Add(new XAttribute("code", 503));
                errorElement.Add(new XElement((XNamespace)"urn:ietf:params:xml:ns:xmpp-stanzas" + "service-unavailable"));


                iqElement.Add(queryElement);
                iqElement.Add(errorElement);

                Packet.Add(iqElement);
                User.Send(Packet.ToString(SaveOptions.DisableFormatting));
            }
        }
    }
}
