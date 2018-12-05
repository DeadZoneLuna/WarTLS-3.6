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
    class GameRoom_SetPlayer : Stanza
    {
        string Channel;

        public GameRoom_SetPlayer(Client User, XmlDocument Packet) : base(User, Packet)
        {
            GameRoom Room = User.Player.GameRoom;

            int Bw = 0;
            int Wf = 0;

            byte TmId = byte.Parse(base.Query.Attributes["team_id"].InnerText);
            if(User.Channel.ChannelType == "pve")
            {
                User.TeamId = 1;
            }
            else if (TmId != 1 && TmId != 2)
            {
                foreach (Client UserInRoom in Room.Core.Users.ToArray())
                {
                    if (UserInRoom.TeamId == 1) Wf++;
                    else if (UserInRoom.TeamId == 2) Bw++;
                }
                if (Bw > Wf)
                    User.TeamId = 1;
                else if (Wf > Bw)
                    User.TeamId = 2;
                else
                    User.TeamId = 1;
            }
            else
                User.TeamId = byte.Parse(base.Query.Attributes["team_id"].InnerText);
            User.Player.RoomStatus = byte.Parse(base.Query.Attributes["status"].InnerText);
            User.Player.CurrentClass = byte.Parse(base.Query.Attributes["class_id"].InnerText);
            
            Process();
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

            XElement accountElement = new XElement("gameroom_setplayer");
            accountElement.Add(User.Player.GameRoom.Serialize());
            queryElement.Add(accountElement);
            iqElement.Add(queryElement);

            Packet.Add(iqElement);
            base.Compress(ref Packet);
            User.Send(Packet.ToString(SaveOptions.DisableFormatting));
        }
    }
}
