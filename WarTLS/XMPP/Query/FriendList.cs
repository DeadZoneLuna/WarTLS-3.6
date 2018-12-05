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
    class FriendList : Stanza
    {
        string Channel;

        public FriendList(Client User, XmlDocument Packet=null) : base(User, Packet)
        {
        }
        internal override void Process()
        {
            if (base.Type == "result") return;
            XDocument Packet = new XDocument();

            XElement iqElement = new XElement("iq");
            iqElement.Add(new XAttribute("type", base.Type == "get" ? "result" : "get"));
            iqElement.Add(new XAttribute("from", $"masterserver@warface/{User.Channel.Resource}"));
            iqElement.Add(new XAttribute("to", User.JID));
            iqElement.Add(new XAttribute("id", base.Type == "get" ? base.Id : $"uid{User.Player.Random.Next(9999, Int32.MaxValue).ToString("x8")}"));

            XElement queryElement = new XElement(Stanza.NameSpace + "query");

            queryElement.Add(User.Player.Friends);
            iqElement.Add(queryElement);

            Packet.Add(iqElement);
            base.Compress(ref Packet);
            User.Send(Packet.ToString(SaveOptions.DisableFormatting));
        }
    }
}
