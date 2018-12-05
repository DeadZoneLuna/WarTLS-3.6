using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using WARTLS.CLASSES;
using WARTLS.NETWORK;

namespace WARTLS.XMPP
{
    class Bind
    {
        internal int DedicatedId = 0;
        readonly XNamespace NameSpace = "urn:ietf:params:xml:ns:xmpp-bind";
        internal Bind(Client User,XmlDocument Packet)
        {
            Client OtherUser = ArrayList.OnlineUsers.Find(Attribute => Attribute.JID == $"{User.Player.UserID}@warface/GameClient");
            if (OtherUser != null)
                new StreamError(OtherUser, "conflict");
            if (User.Dedicated)
                User.JID = $"dedicated{DedicatedId}@warface/GameDedicated";
            else
                User.JID = $"{User.Player.UserID}@warface/GameClient";

            XElement IQ = new XElement(Gateway.JabberNS+"iq");
            IQ.Add(new XAttribute("type", "result"));
            IQ.Add(new XAttribute("id", Packet["iq"].Attributes["id"].InnerText));
            IQ.Add(new XAttribute("to", User.JID));

            XElement BIND = new XElement(NameSpace + "bind");
            BIND.Add(new XElement("jid", User.JID));
            IQ.Add(BIND);

            User.Send(IQ.ToString(SaveOptions.DisableFormatting));
        }
    }
}
