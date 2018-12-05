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
    class ProfileItemGetStatus : Stanza
    {
        string Nickname;
        Client OnlineUser = null;
        public ProfileItemGetStatus(Client User, XmlDocument Packet) : base(User, Packet)
        {
            
            Nickname = base.Query.Attributes["nickname"].InnerText;
            OnlineUser = ArrayList.OnlineUsers.Find(Attribute => Attribute.Player.Nickname == Nickname);
            if (OnlineUser == null)
            {

            }

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

            XElement accountElement = new XElement("profile_info_get_status");
            accountElement.Add(new XAttribute("nickname", Nickname));
            accountElement.Add(new XElement("profile_info",
                new XElement("info",
                new XAttribute("nickname", Nickname),
                new XAttribute("online_id", "1@warface/GameClient"),
                new XAttribute("status", 1),
                new XAttribute("rank", 1),
                new XAttribute("user_id", 1),
                new XAttribute("profile_id", 1)
                )));
            /*if (User.Player.Settings.ChildNodes.Count != 0)
            {
                string SettingsOnUser = User.Player.Settings.InnerXml;
                accountElement.Add(XDocument.Parse(SettingsOnUser).Root.FirstNode);
            }*/
            queryElement.Add(accountElement);
            iqElement.Add(queryElement);

            Packet.Add(iqElement);

            User.Send(Packet.ToString(SaveOptions.DisableFormatting));
        }
    }
}
