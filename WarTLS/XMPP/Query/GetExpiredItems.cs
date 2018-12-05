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
    class GetExpiredItems : Stanza
    {


        public GetExpiredItems(Client User, XmlDocument Packet) : base(User, Packet)
        {

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
            XElement getexpiredItemsElement = new XElement(Stanza.NameSpace + "get_expired_items");

            if (User.Player.Notifications.FirstChild.ChildNodes.Count > 0)
            {
                foreach (XmlNode Notification in User.Player.Notifications.FirstChild.ChildNodes)
                    getexpiredItemsElement.Add(XDocument.Parse(Notification.OuterXml).Root);
            }

            queryElement.Add(getexpiredItemsElement);
            iqElement.Add(queryElement);

            Packet.Add(iqElement);
            base.Compress(ref Packet);
            User.Send(Packet.ToString(SaveOptions.DisableFormatting));
        }
    }
}
