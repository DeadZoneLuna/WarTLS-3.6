using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WARTLS.CLASSES.GAMEROOM.CORE
{
    class Session
    {
        internal byte Status = 0;
        internal byte GameProcess = 0;
        internal long StartTime = 0;
        internal int Revision = 1;
        internal string ID = "";
        internal XElement Serialize()
        {
            XElement gameroomElement = new XElement("session");
            gameroomElement.Add(new XAttribute("id", ID));
            gameroomElement.Add(new XAttribute("status", Status));
            gameroomElement.Add(new XAttribute("game_progress", GameProcess));
            gameroomElement.Add(new XAttribute("start_time", StartTime));
            gameroomElement.Add(new XAttribute("revision", Revision));


            return gameroomElement;
        }
    }
}
