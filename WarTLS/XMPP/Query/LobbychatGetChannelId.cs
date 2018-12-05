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
    class LobbychatGetChannelId : Stanza
    {
        string Channel;

        public LobbychatGetChannelId(Client User, XmlDocument Packet) : base(User, Packet)
        {
            Channel = base.Query.Attributes["channel"].InnerText;

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

            string ChannelId = null;
            switch (Channel)
            {
                case "0":
                    ChannelId = $"global.{User.Channel.Resource}";
                    break;
                case "1":
                    ChannelId = $"room.{User.Player.GameRoom.Core.RoomId}";
                    break;
                case "2":
                    ChannelId = $"team.room.{User.Player.GameRoom.Core.RoomId}";
                    break;
            }
            XElement accountElement = new XElement("lobbychat_getchannelid");
            accountElement.Add(new XAttribute("channel", Channel));
            if(ChannelId != null)
                accountElement.Add(new XAttribute("channel_id", ChannelId));
            accountElement.Add(new XAttribute("service_id", "conference.warface"));

            queryElement.Add(accountElement);
            iqElement.Add(queryElement);

            Packet.Add(iqElement);
            base.Compress(ref Packet);
            User.Send(Packet.ToString(SaveOptions.DisableFormatting));
        }
    }
}
