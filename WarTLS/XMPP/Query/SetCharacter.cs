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
    class SetCharacter : Stanza
    {
        string Channel;

        public SetCharacter(Client User, XmlDocument Packet) : base(User, Packet)
        {
            foreach(XmlNode UpdItem in base.Query.ChildNodes)
            {
                foreach(Item PlItem in User.Player.Items)
                {
                    if (PlItem.ID.ToString() == UpdItem.Attributes["id"].InnerText)
                    {
                        PlItem.Slot = int.Parse(UpdItem.Attributes["slot"].InnerText);

                            PlItem.Equipped = Item.EquipperCalc(PlItem.Slot);
                            //throw new NotImplementedException();
                        PlItem.Config = UpdItem.Attributes["config"].InnerText;
                        PlItem.AttachedTo = UpdItem.Attributes["attached_to"].InnerText;
                    }
                }
            }
            User.Player.Save();
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

            XElement accountElement = new XElement("setcharacter");

            XElement errorElement = new XElement((XNamespace)"urn:ietf:params:xml:ns:xmpp-stanzas" + "error");
            errorElement.Add(new XAttribute("type", "cancel"));
            errorElement.Add(new XAttribute("code", 8));
            errorElement.Add(new XAttribute("custom_code", 66));

            
            queryElement.Add(accountElement);
            iqElement.Add(queryElement);
            //iqElement.Add(errorElement);
            Packet.Add(iqElement);

            User.Send(Packet.ToString(SaveOptions.DisableFormatting));
        }
    }
}
