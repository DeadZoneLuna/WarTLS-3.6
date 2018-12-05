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
    class ClanCreate : Stanza
    {
        string Channel;
        Clan Clan;
        public ClanCreate(Client User, XmlDocument Packet) : base(User, Packet)
        {
            Clan = new Clan(base.Query.Attributes["clan_name"].InnerText, Encoding.UTF8.GetString(Convert.FromBase64String(base.Query.Attributes["description"].InnerText)),User);
            User.Player.Clan = Clan;
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

            XElement accountElement = new XElement("clan_create");
            accountElement.Add(Clan.Serialize());


            queryElement.Add(accountElement);
            iqElement.Add(queryElement);

            Packet.Add(iqElement);
            base.Compress(ref Packet);
            User.Send(Packet.ToString(SaveOptions.DisableFormatting));
        }
    }
}
