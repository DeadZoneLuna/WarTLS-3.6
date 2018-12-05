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
    class InvitationRequest : Stanza
    {
        GameRoom Room;
        InvitationTicket Ticket;
        public InvitationRequest(Client User, XmlDocument Packet = null) : base(User, Packet)
        {
        }
        public InvitationRequest(Client User, InvitationTicket Ticket) : base(User, null)
        {
            if (base.Type == "result") return;
            this.Ticket = Ticket;
            Room = Ticket.Sender.Player.GameRoom;

            Process();
        }
        internal override void Process()
        {
            XDocument Packet = new XDocument();

            XElement iqElement = new XElement(Gateway.JabberNS + "iq");
            iqElement.Add(new XAttribute("type", "get"));
            iqElement.Add(new XAttribute("from", $"masterserver@warface/{User.Channel.Resource}"));
            iqElement.Add(new XAttribute("to", User.JID));
            iqElement.Add(new XAttribute("id", User.Player.Random.Next(1,Int32.MaxValue)));

            XElement queryElement = new XElement(Stanza.NameSpace + "query");

            XElement accountElement = new XElement("invitation_request");
            accountElement.Add(new XAttribute("from", Ticket.Sender.Player.Nickname));
            accountElement.Add(new XAttribute((bool)App.Default["UseOldMode"] ? "token" : "ticket", Ticket.ID));
            accountElement.Add(new XAttribute("room_id", Room.Core.RoomId));
            accountElement.Add(new XAttribute("ms_resource", Ticket.Sender.Channel.Resource));
            accountElement.Add(new XAttribute("is_follow", Ticket.IsFollow ? 1:0));
            accountElement.Add(Room.Serialize());

            queryElement.Add(accountElement);
            iqElement.Add(queryElement);

            Packet.Add(iqElement);
            base.Compress(ref Packet);
            User.Send(Packet.ToString(SaveOptions.DisableFormatting));
        }
    }
}
