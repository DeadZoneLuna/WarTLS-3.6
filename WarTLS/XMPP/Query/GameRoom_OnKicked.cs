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
    class GameRoom_OnKicked : Stanza
    {
        internal enum Reason
        {
            KickedByUser=1,
            NonActivity=2,
            KickedByVoting=3,
            KickedByRank=6,
            KickedClan=7,
            SystemSecurityViolation=8,
            VersionMismatch=9,
            NoAccessPoints=10
        }
        string Channel;
        GameRoom Room;
        Client Target;
        internal Reason KickReason;
        public GameRoom_OnKicked(Client User, XmlDocument Packet) : base(User, Packet)
        {

        }
        public GameRoom_OnKicked(Client User,Reason Reason=Reason.KickedByUser) : base(User, null)
        {
            this.KickReason = Reason;
            User.Player.GameRoom.KickedUsers.Add(User.Player.UserID);
            User.Player.GameRoom.Core.Users.Remove(User);
            User.Player.GameRoom = null;
            Process();
        }
        internal override void Process()
        {
            XDocument Packet = new XDocument();

            XElement iqElement = new XElement(Gateway.JabberNS + "iq");
            iqElement.Add(new XAttribute("type", "get"));
            iqElement.Add(new XAttribute("from", $"k01.warface"));
            iqElement.Add(new XAttribute("to", User.JID));
            iqElement.Add(new XAttribute("id", User.Player.Random.Next(1, Int32.MaxValue)));

            XElement queryElement = new XElement(Stanza.NameSpace + "query");

            XElement accountElement = new XElement("gameroom_on_kicked");
            accountElement.Add(new XAttribute("reason", (byte)KickReason));

            queryElement.Add(accountElement);
            iqElement.Add(queryElement);

            Packet.Add(iqElement);
            
            User.Send(Packet.ToString(SaveOptions.DisableFormatting));
        }
    }
}
