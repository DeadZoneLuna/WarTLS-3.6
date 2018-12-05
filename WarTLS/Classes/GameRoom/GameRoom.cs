using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using WARTLS.CLASSES.GAMEROOM.CORE;
using WARTLS.XMPP.QUERY;

namespace WARTLS.CLASSES.GAMEROOM
{
    class GameRoom
    {
        internal static long Seed = 1;
        internal WARTLS.CLASSES.GAMEROOM.CORE.Core Core = new WARTLS.CLASSES.GAMEROOM.CORE.Core();
        internal Client Dedicated;
        internal Session Session = new Session();
        internal PlayersReserved PlayersReserved = new PlayersReserved();
        internal Mission Mission = new Mission();
        internal CustomParams CustomParams = new CustomParams();
        internal TeamColors TeamColors = new TeamColors();
        internal Players Players = new Players();
        internal string Name = "NoNameYet";
        internal List<long> KickedUsers = new List<long>();
        internal void SessionStarter()
        {
            Client Dedicated = ArrayList.OnlineUsers.Find(Attribute => Attribute.Dedicated && Attribute.Player.GameRoom == null);
            if (Dedicated == null) return;

            Dedicated.Player.GameRoom = this;
            this.Dedicated = Dedicated;
            
            //new MissionLoad(Dedicated).Process();
        }
        internal void Sync(Client NonInclude=null)
        {
            if (CustomParams.AutoTeamBalance)
            {
                bool isBw = false;
                foreach (Client User in Core.Users.ToArray())
                {
                    //if (this.Session.Status > 0 && User.Player.RoomStatus == 1) continue;
                    if (isBw) { User.TeamId = 2; isBw = false; }
                    else { User.TeamId = 1; isBw = true; }
                }
            }
            if (this.Dedicated != null)
                //new MissionUpdate(this.Dedicated).Process();
            foreach (Client User in Core.Users.ToArray())
                if(NonInclude != User)
                    new GameRoom_Sync(User).Process();
        }

        internal XElement Serialize(bool IncludeData=false)
        {
            XElement gameroomElement = new XElement("game_room");
            gameroomElement.Add(new XAttribute("room_id", Core.RoomId));
            gameroomElement.Add(new XAttribute("room_type", Core.RoomType));

            //CoreElement.Add(this.Players.Serialize(Core.Users));
           // CoreElement.Add(this.PlayersReserved.Serialize());
            //CoreElement.Add(this.TeamColors.Serialize());


            //gameroomElement.Add(CoreElement);

            gameroomElement.Add(this.Session.Serialize());
            gameroomElement.Add(this.Mission.Serialize(IncludeData));
            gameroomElement.Add(this.CustomParams.Serialize());
            //if(!(bool)App.Default["UseOldMode"])
               // gameroomElement.Add(this.RoomMaster.Serialize());



            return gameroomElement;
        }
    }
}
