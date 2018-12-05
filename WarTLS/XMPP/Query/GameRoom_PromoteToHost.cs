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
    class GameRoom_PromoteToHost : Stanza
    {
        GameRoom Room;

        public GameRoom_PromoteToHost(Client User, XmlDocument Packet) : base(User, Packet)
        {
            Room = User.Player.GameRoom;
            long NewHost = long.Parse(base.Query.Attributes["new_host_profile_id"].InnerText);


            if ((bool)App.Default["UseOldMode"])
                Room.Core.Revision++;
            Room.Sync(User);
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

            XElement accountElement = new XElement("gameroom_promote_to_host");
            accountElement.Add(User.Player.GameRoom.Serialize());
            queryElement.Add(accountElement);
            iqElement.Add(queryElement);
            /*
            if (Room.Dedicated == null)
            {
                XElement errorElement = new XElement((XNamespace)"urn:ietf:params:xml:ns:xmpp-stanzas" + "error");
                errorElement.Add(new XAttribute("type", "continue"));
                errorElement.Add(new XAttribute("code", 8));
                errorElement.Add(new XAttribute("custom_code", 4));
                iqElement.Add(errorElement);
            }*/

            Packet.Add(iqElement);

            User.Send(Packet.ToString(SaveOptions.DisableFormatting));

        }
    }
}
