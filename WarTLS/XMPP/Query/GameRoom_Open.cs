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
    class GameRoom_Open : Stanza
    {
        string Channel;
        int Code=0;
        public GameRoom_Open(Client User, XmlDocument Packet) : base(User, Packet)
        {
            if (User.Channel.ChannelType == "pve")
            {
                base.Query.Attributes["max_players"].InnerText = "5";
            }
            else
            {
                if (byte.Parse(base.Query.Attributes["max_players"].InnerText) > 16)
                    Code = 3;
                if (byte.Parse(base.Query.Attributes["max_players"].InnerText) < 4)
                    Code = 3;
                if (byte.Parse(base.Query.Attributes["max_players"].InnerText) % 2 != 0)
                    Code = 3;
            }
            if (Code != 0) {Process(); return; }
            string Uid = base.Query.Attributes["mission"].InnerText;
            if (User.Player.GameRoom != null)
                new GameRoom_Leave(User, null);

            GameRoom Room = new GameRoom();
            Room.Core.RoomId = GameRoom.Seed;
            GameRoom.Seed++;
           
           
            if (User.Channel.ChannelType != "pve")
                
                Room.Mission.Map = GameResources.Maps.Find(Attribute => Attribute.FirstChild.Attributes["uid"].InnerText == Uid);
            else
            {
                Room.Mission.Map = GameResources.Maps.Find(Attribute => Attribute.FirstChild.Attributes["uid"].InnerText == Uid);
            }
            Room.Core.Users.Add(User);
            Room.Core.RoomType = User.Channel.ChannelType == "pve" ? (byte)1 : (byte)2;
            if (base.Query.Attributes["group_id"] != null)
                User.Player.GroupId = base.Query.Attributes["group_id"].InnerText;
            if (base.Query.Attributes["private"] != null)
                Room.Core.Private = base.Query.Attributes["private"].InnerText == "1";
            if (base.Query.Attributes["friendly_fire"] != null)
                Room.CustomParams.FriendlyFire = base.Query.Attributes["friendly_fire"].InnerText == "1";
            if (base.Query.Attributes["enemy_outlines"] != null)
                Room.CustomParams.EmenyOutlines = base.Query.Attributes["enemy_outlines"].InnerText == "1";
            if (base.Query.Attributes["auto_team_balance"] != null)
                Room.CustomParams.AutoTeamBalance = base.Query.Attributes["auto_team_balance"].InnerText == "1";
            if (base.Query.Attributes["dead_can_chat"] != null)
                Room.CustomParams.DeadCanChat = base.Query.Attributes["dead_can_chat"].InnerText == "1";
            if (base.Query.Attributes["join_in_the_process"] != null)
                Room.CustomParams.JoinInProcess = base.Query.Attributes["join_in_the_process"].InnerText == "1";
            if (base.Query.Attributes["max_players"] != null)
                Room.CustomParams.MaxPlayers = User.Channel.ChannelType == "pve" ? (byte)5 : byte.Parse(base.Query.Attributes["max_players"].InnerText);
            if (base.Query.Attributes["inventory_slot"] != null)
                Room.CustomParams.InventorySlot = int.Parse(base.Query.Attributes["inventory_slot"].InnerText);

            if (base.Query.Attributes["class_rifleman"] != null)
                Room.CustomParams.SoldierEnabled = base.Query["class_rifleman"].Attributes["enabled"].InnerText == "1";
            if (base.Query.Attributes["class_medic"] != null)
                Room.CustomParams.MedicEnabled = base.Query["class_medic"].Attributes["enabled"].InnerText == "1";
            if (base.Query.Attributes["class_engineer"] != null)
                Room.CustomParams.EngineerEnabled = base.Query["class_engineer"].Attributes["enabled"].InnerText == "1";
            if (base.Query.Attributes["class_sniper"] != null)
                Room.CustomParams.SniperEnabled = base.Query["class_sniper"].Attributes["enabled"].InnerText == "1";

            User.Player.GameRoom = Room;
            User.Channel.GameRoomList.Add(Room);
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

            XElement accountElement = new XElement("gameroom_open");
            if (Code != 0)
            {
                XElement errorElement = new XElement((XNamespace)"urn:ietf:params:xml:ns:xmpp-stanzas" + "error");
                errorElement.Add(new XAttribute("type", "continue"));
                errorElement.Add(new XAttribute("code", 8));
                errorElement.Add(new XAttribute("custom_code", Code));
                iqElement.Add(errorElement);
            }else
                accountElement.Add(User.Player.GameRoom.Serialize());
            queryElement.Add(accountElement);
            iqElement.Add(queryElement);

            Packet.Add(iqElement);
            if(Code==0)
            base.Compress(ref Packet);
            User.Send(Packet.ToString(SaveOptions.DisableFormatting));
        }
    }
}
