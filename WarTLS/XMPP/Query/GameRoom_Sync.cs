using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using WARTLS.CLASSES;
using WARTLS.CLASSES.GAMEROOM;
using WARTLS.NETWORK;

namespace WARTLS.XMPP.QUERY
{
    class GameRoom_Sync : Stanza
    {
        string Channel;

        public GameRoom_Sync(Client User, XmlDocument Packet=null) : base(User, Packet)
        {
        }
        internal override void Process()
        {
            if (User.Player.GameRoom == null) return;
            XDocument Packet = new XDocument();

            XElement iqElement = new XElement(Gateway.JabberNS + "iq");
            iqElement.Add(new XAttribute("type", base.Type == "get" ? "result" : "get"));
            iqElement.Add(new XAttribute("from", $"k01.warface"));
            iqElement.Add(new XAttribute("to", User.JID));
            iqElement.Add(new XAttribute("id", base.Type == "get" ? base.Id : User.Player.Random.Next(99999, Int32.MaxValue).ToString()));

            XElement queryElement = new XElement(Stanza.NameSpace + "query");

            XElement accountElement = new XElement("gameroom_sync");
            accountElement.Add(User.Player.GameRoom.Serialize());
            queryElement.Add(accountElement);
            iqElement.Add(queryElement);

            Packet.Add(iqElement);
            base.Compress(ref Packet);
            User.Send(Packet.ToString(SaveOptions.DisableFormatting));
        }
    }
}
