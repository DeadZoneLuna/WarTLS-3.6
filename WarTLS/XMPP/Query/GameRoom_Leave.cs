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
    class GameRoom_Leave : Stanza
    {
        GameRoom Room;

        public GameRoom_Leave(Client User, XmlDocument Packet) : base(User, Packet)
        {
            Room = User.Player.GameRoom;
            //вот тут нужно все обработать)
            if(Room != null) // мы проверяем, игрок привязан ли к какой нибудь комнате. если да, то выполняем выход)
            {
                Room.Core.Users.Remove(User);// Удаляем игрока с комнаты
                User.Player.GameRoom = null; // Отвязываем игрока от комнаты ) и нужно еще сделать проверку, удаляется ли лидер)
                if (false) // удаляется лидер
                {//нужно, как то назначить нового)// вот тут лучше)
                    // как думаете назначать лидера? ну если вышел лидер номер один да, по рангу, или тот, кто первый попадется? лучше по рангу окей
                    Client MaxRank=null; // выполняем перебор игроков
                    foreach(Client UserInRoom in Room.Core.Users.ToArray())
                        if (MaxRank == null || UserInRoom.Player.Experience >= MaxRank.Player.Experience) { MaxRank = UserInRoom; continue; }
                    if (MaxRank == null || Room.Core.Players<=0) User.Channel.GameRoomList.Remove(Room);
                    else
                    {
                      //  Room.RoomMaster.UserId = MaxRank.Player.UserID;
                       // Room.RoomMaster.Revision++;

                        //тут? дад а смотрю как урок по c#)) аа)
                    }
                }
            }
            if(base.Packet!=null)
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

            XElement accountElement = new XElement("gameroom_leave");

            queryElement.Add(accountElement);
            iqElement.Add(queryElement);


            Packet.Add(iqElement);

            User.Send(Packet.ToString(SaveOptions.DisableFormatting));

        }
    }
}
