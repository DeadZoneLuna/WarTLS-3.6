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
    class GameRoom_Kick : Stanza
    {
        string Channel;
        GameRoom Room;
        Client Target;
        public GameRoom_Kick(Client User, XmlDocument Packet) : base(User, Packet)
        {
            Room = User.Player.GameRoom;
            long TargetId = long.Parse(base.Query.Attributes["target_id"].InnerText);
            Target = Room.Core.Users.Find(Attribute => Attribute.Player.UserID == TargetId);

            new GameRoom_OnKicked(Target);
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

            XElement accountElement = new XElement("gameroom_kick");
            accountElement.Add(Room.Serialize());
            queryElement.Add(accountElement);
            iqElement.Add(queryElement);

            Packet.Add(iqElement);
            base.Compress(ref Packet);
            User.Send(Packet.ToString(SaveOptions.DisableFormatting));
        }
    }
}
