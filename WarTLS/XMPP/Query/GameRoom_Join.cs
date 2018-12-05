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
    class GameRoom_Join : Stanza
    {
        string Channel;
        GameRoom Room;
        int Code = 0;
        public GameRoom_Join(Client User, XmlDocument Packet) : base(User, Packet)
        {
            if (User.Player.GameRoom != null)
                new GameRoom_Leave(User,null);
            Room = User.Channel.GameRoomList.Find(Attribute => Attribute.Core.RoomId == long.Parse(base.Query.Attributes["room_id"].InnerText));
            if (Room == null)
            {
                Code = 2;
                Process();
                return;
            }
            User.TeamId =0;

            int Bw = 0;
            int Wf = 0;
            foreach (Client UserInRoom in Room.Core.Users.ToArray())
            {
                if (UserInRoom.TeamId == 1) Wf++;
                else if(UserInRoom.TeamId == 2) Bw++;
            }
            if (Bw > Wf)
                User.TeamId = 1;
            else if (Wf > Bw)
                User.TeamId = 2;
            else
                User.TeamId = 1;
            User.Player.CurrentClass = byte.Parse(base.Query.Attributes["class_id"].InnerText);
            User.Player.RoomStatus = byte.Parse(base.Query.Attributes["status"].InnerText);
            //User.Player.GroupId = base.Query.Attributes["group_id"].InnerText;

            if (!Room.KickedUsers.Contains(User.Player.UserID))
            {
                User.Player.GameRoom = Room;
                Room.Core.Users.Add(User);
            }
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

            XElement accountElement = new XElement("gameroom_join");
            if(Code==0)
                accountElement.Add(User.Player.GameRoom.Serialize());
            if (Code == 0)
                accountElement.Add(new XAttribute("room_id", Room.Core.RoomId));
            accountElement.Add(new XAttribute("code", Code));

            queryElement.Add(accountElement);
            iqElement.Add(queryElement);

            Packet.Add(iqElement);
            if(Code==0)
            base.Compress(ref Packet);
            User.Send(Packet.ToString(SaveOptions.DisableFormatting));
           
        }
    }
}
