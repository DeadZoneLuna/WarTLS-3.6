using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using WARTLS.CLASSES;
using WARTLS.NETWORK;

namespace WARTLS.XMPP
{
    class Session
    {
        readonly XNamespace NameSpace = "urn:ietf:params:xml:ns:xmpp-session";
        internal Session(Client User, XmlDocument Packet)
        {
            XElement IQ = new XElement(Gateway.JabberNS + "iq");
            IQ.Add(new XAttribute("type", "result"));
            IQ.Add(new XAttribute("id", Packet["iq"].Attributes["id"].InnerText));
            if (User.JID != null)
                IQ.Add(new XAttribute("to", User.JID));

            XElement BIND = new XElement(NameSpace + "session");
            IQ.Add(BIND);

            User.Send(IQ.ToString(SaveOptions.DisableFormatting));
        }
    }
}
