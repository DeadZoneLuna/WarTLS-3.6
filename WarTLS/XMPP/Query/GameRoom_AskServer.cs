using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using WARTLS.CLASSES;
using WARTLS.CLASSES.GAMEROOM;
using WARTLS.NETWORK;

namespace WARTLS.XMPP.QUERY
{
    class GameRoom_AskServer : Stanza
    {
        GameRoom Room;

        public GameRoom_AskServer(Client User, XmlDocument Packet) : base(User, Packet)
        {

            Room = User.Player.GameRoom;
            Thread Thread = new Thread(Room.SessionStarter);
            Thread.Start();
            Thread.Sleep(2400);
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

            XElement accountElement = new XElement("gameroom_askserver");

            queryElement.Add(accountElement);
            iqElement.Add(queryElement);
            
            if (Room.Dedicated == null)
            {
                XElement errorElement = new XElement((XNamespace)"urn:ietf:params:xml:ns:xmpp-stanzas" + "error");
                errorElement.Add(new XAttribute("type", "continue"));
                errorElement.Add(new XAttribute("code", 8));
                errorElement.Add(new XAttribute("custom_code", 4));
                iqElement.Add(errorElement);
            }

            Packet.Add(iqElement);

            User.Send(Packet.ToString(SaveOptions.DisableFormatting));

        }
    }
}
