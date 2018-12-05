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
    class InvitationSend : Stanza
    {
        string Channel;
        InvitationTicket Ticket;
        enum Results
        {
            Rejected=1,
            AutoRejected=2,
            MissionLocked=12,
            RankRestricted=13,
            FullRoom=14,
            Banned=15,
            BuildType=16,
            NotInClan=18,
            Participate=19,
            AllClassLocked=20,
            VersionMismatch=21,
            NoAccessTokens=22
        }
        public InvitationSend(Client User, XmlDocument Packet) : base(User, Packet)
        {
            if(!(bool)App.Default["UseOldMode"])
            User.Player.GroupId = base.Query.Attributes["group_id"].InnerText;

            Client Receiver = ArrayList.OnlineUsers.Find(Attribute => Attribute.Player.Nickname == base.Query.Attributes["nickname"].InnerText);
            InvitationTicket InvitationTicket = new InvitationTicket(User, Receiver)
            {
                GroupId = (bool)App.Default["UseOldMode"] ? "" : base.Query.Attributes["group_id"].InnerText
            };

            Ticket = InvitationTicket;
            InvitationTicket.IsFollow = base.Query.Attributes["is_follow"].InnerText == "1";

            if (!(User.Channel.MinRank <= Receiver.Player.Rank && User.Channel.MaxRank >= Receiver.Player.Rank))
                InvitationTicket.Result = (byte)Results.RankRestricted;

                if (InvitationTicket.Result == 255)
            {
                Receiver.InvitationTicket.Add(InvitationTicket);
                User.InvitationTicket.Add(InvitationTicket);

                new InvitationRequest(Receiver,InvitationTicket);
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

            XElement accountElement = new XElement("invitation_send");


            queryElement.Add(accountElement);
            iqElement.Add(queryElement);

            if (Ticket.Result != 255)
            {
                XElement errorElement = new XElement((XNamespace)"urn:ietf:params:xml:ns:xmpp-stanzas" + "error");
                errorElement.Add(new XAttribute("type", "continue"));
                errorElement.Add(new XAttribute("code", 8));
                errorElement.Add(new XAttribute("custom_code", Ticket.Result));
                iqElement.Add(errorElement);
            }
            Packet.Add(iqElement);

            User.Send(Packet.ToString(SaveOptions.DisableFormatting));
        }
    }
}
