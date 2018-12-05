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
    class PlayerStatus : Stanza
    {


        public PlayerStatus(Client User, XmlDocument Packet) : base(User, Packet)
        {
            User.Status = int.Parse(base.Query.Attributes["new_status"].InnerText);
            User.Location = base.Query.Attributes["to"].InnerText;
            Process();
            if(User.Player.GameRoom != null)
            {
                User.Player.GameRoom.Sync();
            }
        }
        internal override void Process()
        {
            XDocument Packet = new XDocument();

            XElement iqElement = new XElement(Gateway.JabberNS + "iq");
            iqElement.Add(new XAttribute("type", "result"));
            iqElement.Add(new XAttribute("from", $"k01.warface"));
            iqElement.Add(new XAttribute("to", User.JID));
            iqElement.Add(new XAttribute("id", base.Id));

            XElement queryElement = new XElement(Stanza.NameSpace + "query");
;
            /*if (User.Player.Settings.ChildNodes.Count != 0)
            {
                string SettingsOnUser = User.Player.Settings.InnerXml;
                accountElement.Add(XDocument.Parse(SettingsOnUser).Root.FirstNode);
            }*/

            iqElement.Add(queryElement);

            Packet.Add(iqElement);

            User.Send(Packet.ToString(SaveOptions.DisableFormatting));
        }
    }
}
