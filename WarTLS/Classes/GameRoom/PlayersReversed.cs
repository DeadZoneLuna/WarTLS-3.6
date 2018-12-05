using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WARTLS.CLASSES.GAMEROOM.CORE
{
    class PlayersReserved
    {
        internal long UserId = 0;
        internal int Revision = 2;
        internal XElement Serialize()
        {
            XElement gameroomElement = new XElement("playersReserved");



            return gameroomElement;
        }
    }
}
