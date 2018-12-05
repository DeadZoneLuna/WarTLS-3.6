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
    class InvitationAccept : Stanza
    {

        InvitationTicket Ticket;
        public InvitationAccept(Client User, XmlDocument Packet = null) : base(User, Packet)
        {
            if (base.Type == "result") return;
            if ((bool)App.Default["UseOldMode"])
                Ticket = User.InvitationTicket.Find(Attribute => Attribute.ID == base.Query.Attributes["token"].InnerText);
            else 
                Ticket = User.InvitationTicket.Find(Attribute => Attribute.ID == base.Query.Attributes["ticket"].InnerText);
            Ticket.Result = byte.Parse(base.Query.Attributes["result"].InnerText);
            new InvitationResult(Ticket.Sender,Ticket);
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

            XElement accountElement = new XElement("invitation_accept");

            queryElement.Add(accountElement);
            iqElement.Add(queryElement);

            Packet.Add(iqElement);
            base.Compress(ref Packet);
            User.Send(Packet.ToString(SaveOptions.DisableFormatting));
        }
    }
}
