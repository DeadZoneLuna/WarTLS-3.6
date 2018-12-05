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
    class InvitationResult : Stanza
    {
        InvitationTicket Ticket;

        public InvitationResult(Client User, XmlDocument Packet = null) : base(User, Packet)
        {
        }
        public InvitationResult(Client User, InvitationTicket Ticket) : base(User, null)
        {
            this.Ticket = Ticket;
            Process();
        }
        internal override void Process()
        {
            XDocument Packet = new XDocument();

            XElement iqElement = new XElement(Gateway.JabberNS + "iq");
            iqElement.Add(new XAttribute("type", "get"));
            iqElement.Add(new XAttribute("from", $"masterserver@warface/{User.Channel.Resource}"));
            iqElement.Add(new XAttribute("to", User.JID));
            iqElement.Add(new XAttribute("id", User.Player.Random.Next(1, Int32.MaxValue)));

            XElement queryElement = new XElement(Stanza.NameSpace + "query");

            XElement accountElement = new XElement("invitation_result");
            accountElement.Add(new XAttribute("result", Ticket.Result));
            accountElement.Add(new XAttribute("user", Ticket.Receiver.Player.Nickname));
            accountElement.Add(new XAttribute("is_follow", Ticket.IsFollow ? 1:0));
            accountElement.Add(new XAttribute("user_id", Ticket.Receiver.Player.UserID));


            queryElement.Add(accountElement);
            iqElement.Add(queryElement);

            Packet.Add(iqElement);
            base.Compress(ref Packet);
            User.Send(Packet.ToString(SaveOptions.DisableFormatting));
            Ticket.Receiver.InvitationTicket.Remove(Ticket);
            Ticket.Sender.InvitationTicket.Remove(Ticket);

        }
    }
}
