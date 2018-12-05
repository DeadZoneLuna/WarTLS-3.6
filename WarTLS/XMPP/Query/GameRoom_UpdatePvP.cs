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
    class GameRoom_UpdatePvP : Stanza
    {
        string Channel;
        int ErrorId = -1;
        public GameRoom_UpdatePvP(Client User, XmlDocument Packet) : base(User, Packet)
        {
            GameRoom Room = User.Player.GameRoom;
            try
            {
                if (Room.Mission.Map.FirstChild.Attributes["uid"].InnerText != base.Query.Attributes["mission_key"].InnerText)
                {
                    Room.Mission.Map = GameResources.Maps.Find(Attribute => Attribute.FirstChild.Attributes["uid"].InnerText == base.Query.Attributes["mission_key"].InnerText);
                    Room.Mission.Revision++;
                }
            }
            catch { ErrorId = 1; Process();return; }
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
                Room.CustomParams.MaxPlayers = byte.Parse(base.Query.Attributes["max_players"].InnerText);
            if (base.Query.Attributes["inventory_slot"] != null)
                Room.CustomParams.InventorySlot = int.Parse(base.Query.Attributes["inventory_slot"].InnerText);
            if (base.Query["class_rifleman"] != null)
                Room.CustomParams.SoldierEnabled = base.Query["class_rifleman"].Attributes["enabled"].InnerText == "1";
            if (base.Query["class_medic"] != null)
                Room.CustomParams.MedicEnabled = base.Query["class_medic"].Attributes["enabled"].InnerText == "1";
            if (base.Query["class_engineer"] != null)
                Room.CustomParams.EngineerEnabled = base.Query["class_engineer"].Attributes["enabled"].InnerText == "1";
            if (base.Query["class_sniper"] != null)
                Room.CustomParams.SniperEnabled = base.Query["class_sniper"].Attributes["enabled"].InnerText == "1";

            Room.CustomParams.Revision++;
            foreach (Client Member in Room.Core.Users.ToArray())
                Member.Player.RoomStatus = 0; 
            Room.Sync(User); 
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

            XElement accountElement = new XElement("gameroom_update_pvp");
            accountElement.Add(User.Player.GameRoom.Serialize());
            queryElement.Add(accountElement);
            iqElement.Add(queryElement);

            XElement errorElement = new XElement((XNamespace)"urn:ietf:params:xml:ns:xmpp-stanzas" + "error");
            errorElement.Add(new XAttribute("type", "cancel"));
            errorElement.Add(new XAttribute("code", 8));
            errorElement.Add(new XAttribute("custom_code", ErrorId));
            if (ErrorId != -1)
                iqElement.Add(errorElement);
            Packet.Add(iqElement);

            base.Compress(ref Packet);
            User.Send(Packet.ToString(SaveOptions.DisableFormatting));
        }
    }
}
