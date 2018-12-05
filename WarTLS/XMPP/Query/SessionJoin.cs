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
    class SessionJoin : Stanza
    {
        GameRoom Room;

        public SessionJoin(Client User, XmlDocument Packet=null) : base(User, Packet)
        {
            if (base.Type == "result") return;
            Room = User.Player.GameRoom;
            if (base.Type == "get" || base.Type==null)
                Process();
        }
        internal override void Process()
        {
            if (base.Type == "result") return;
            XDocument Packet = new XDocument();

            XElement iqElement = new XElement(Gateway.JabberNS + "iq");
            iqElement.Add(new XAttribute("type", base.Type == "get" ? "result":"get"));
            iqElement.Add(new XAttribute("from", $"k01.warface"));
            iqElement.Add(new XAttribute("to", User.JID));
            iqElement.Add(new XAttribute("id", base.Type == "get" ? base.Id : User.Player.Random.Next(99999,Int32.MaxValue).ToString()));

            XElement queryElement = new XElement(Stanza.NameSpace + "query");

            XElement accountElement = new XElement("session_join");
            accountElement.Add(new XAttribute("room_id", Room.Core.RoomId));
            accountElement.Add(new XAttribute("server", "WARTLS AT TOMSK CITY #1"));
            accountElement.Add(new XAttribute("hostname", User.IPAddress=="127.0.0.1" ? "127.0.0.1": "90.188.115.76"));
            accountElement.Add(new XAttribute("port", 64090));
            accountElement.Add(new XAttribute("local", "0"));
            accountElement.Add(new XAttribute("session_id", Room.Session.ID));

            queryElement.Add(accountElement);
            iqElement.Add(queryElement);

            Packet.Add(iqElement);
            base.Compress(ref Packet);
            User.Send(Packet.ToString(SaveOptions.DisableFormatting));
        }
    }
}
