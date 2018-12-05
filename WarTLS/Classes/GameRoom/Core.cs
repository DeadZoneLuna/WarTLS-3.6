using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WARTLS.CLASSES.GAMEROOM.CORE
{
    class Core
    {
        internal long RoomId = 1;
        internal byte RoomType = 2;
        internal string Name = "NoNameYet";
        internal int TeamSwitched = 0;
        internal bool Private = false;
        internal List<Client> Users = new List<Client>();
        internal int Players => Users.Count;
        internal bool CanStart => true;
        internal bool TeamBalanced => true;
        internal byte MinReadyPlayers = 0;
        internal int Revision = 1;
        internal XElement Serialize(long Master=0)
        {
            XElement gameroomElement = new XElement("core");
            gameroomElement.Add(new XAttribute("teams_switched", TeamSwitched));
            gameroomElement.Add(new XAttribute("room_name", Name));
            gameroomElement.Add(new XAttribute("private", Private ? 1:0));
            gameroomElement.Add(new XAttribute("players", Players));
            gameroomElement.Add(new XAttribute("can_start", CanStart?1:0));
            gameroomElement.Add(new XAttribute("team_balanced", TeamBalanced?1:0));
            gameroomElement.Add(new XAttribute("min_ready_players", MinReadyPlayers));
            if((bool)App.Default["UseOldMode"])
                gameroomElement.Add(new XAttribute("master", Master));
            gameroomElement.Add(new XAttribute("revision", Revision));


            return gameroomElement;
        }
    }
}
