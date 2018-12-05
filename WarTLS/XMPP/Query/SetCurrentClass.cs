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
    class SetCurrentClass : Stanza
    {
        string Channel;

        public SetCurrentClass(Client User, XmlDocument Packet) : base(User, Packet)
        {

            User.Player.CurrentClass = byte.Parse(base.Query.Attributes["current_class"].InnerText);
            Process();
        }
        internal override void Process()
        {
            XDocument Packet = new XDocument();

            XElement iqElement = new XElement(Gateway.JabberNS + "iq");
            iqElement.Add(new XAttribute("type", "result"));
            iqElement.Add(new XAttribute("from", base.To));
            iqElement.Add(new XAttribute("to", User.JID));
            iqElement.Add(new XAttribute("id", base.Id));

            XElement queryElement = new XElement(Stanza.NameSpace + "query");

            XElement accountElement = new XElement("setcurrentclass");


            queryElement.Add(accountElement);
            iqElement.Add(queryElement);

            Packet.Add(iqElement);

            User.Send(Packet.ToString(SaveOptions.DisableFormatting));
        }
    }
}
