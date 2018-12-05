using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WARTLS.CLASSES.GAMEROOM.CORE
{
    class Players
    {
        internal XElement Serialize(List<Client> Users)
        {
            XElement gameroomElement = new XElement("players");
            foreach (Client playerElement in Users.ToArray())
                gameroomElement.Add(playerElement.ToElement());


            return gameroomElement;
        }
    }
}
