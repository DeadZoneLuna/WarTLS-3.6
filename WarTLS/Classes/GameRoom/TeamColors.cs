using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WARTLS.CLASSES.GAMEROOM.CORE
{
    class TeamColors
    {
        internal long UserId = 0;
        internal int Revision = 2;
        internal XElement Serialize()
        {
            XElement gameroomElement = new XElement("team_colors");

            XElement teamcolor1 = new XElement("team_color", new XAttribute("id", "1"), new XAttribute("color", "4294907157"));
            XElement teamcolor2 = new XElement("team_color", new XAttribute("id", "2"), new XAttribute("color", "4279655162"));

            gameroomElement.Add(teamcolor1);
            gameroomElement.Add(teamcolor2);

            return gameroomElement;
        }
    }
}
