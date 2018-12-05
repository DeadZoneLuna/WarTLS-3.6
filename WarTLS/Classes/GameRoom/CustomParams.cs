using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WARTLS.CLASSES.GAMEROOM.CORE
{
    class CustomParams
    {
        
        internal bool FriendlyFire = false;
        internal bool EmenyOutlines = false;
        internal bool AutoTeamBalance = true;
        internal bool DeadCanChat = true;
        internal bool JoinInProcess = true;
        internal byte MaxPlayers = 16;
        internal int InventorySlot;
        internal bool SoldierEnabled = true;
        internal bool MedicEnabled = true;
        internal bool EngineerEnabled = true;
        internal bool SniperEnabled = true;
        internal byte ClassRestriction
        {
            get
            {
                byte Start = 224;
                if (SoldierEnabled) Start += 1;
                if (MedicEnabled) Start += 8;
                if (EngineerEnabled) Start += 16;
                if (SniperEnabled) Start += 4;

                return Start;
            }
        }
        internal int Revision = 2;
        internal XElement Serialize()
        {
            XElement gameroomElement = new XElement("custom_params");
            gameroomElement.Add(new XAttribute("friendly_fire", FriendlyFire ? 1:0));
            gameroomElement.Add(new XAttribute("enemy_outlines", EmenyOutlines ? 1 : 0));
            gameroomElement.Add(new XAttribute("auto_team_balance", AutoTeamBalance ? 1 : 0));
            gameroomElement.Add(new XAttribute("dead_can_chat", DeadCanChat ? 1 : 0));
            gameroomElement.Add(new XAttribute("join_in_the_process", JoinInProcess ? 1 : 0));
            gameroomElement.Add(new XAttribute("max_players", MaxPlayers));
            gameroomElement.Add(new XAttribute("inventory_slot", InventorySlot));
            gameroomElement.Add(new XAttribute("class_restriction", ClassRestriction));
            gameroomElement.Add(new XAttribute("revision", Revision));


            return gameroomElement;
        }
    }
}
