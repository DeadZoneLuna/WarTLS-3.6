using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace WARTLS.CLASSES.GAMEROOM.CORE
{
    class Mission
    {
        internal long UserId = 0;
        internal int Revision = 2;
        internal XmlDocument Map;
        internal XmlNode PvEInfo;
        internal XElement Serialize(bool IncludeData = false)
        {
            if (PvEInfo != null)
            {
                XElement gameroomElement1 = XDocument.Parse(PvEInfo.OuterXml).Root;
                if (IncludeData)
                    gameroomElement1.Add(new XAttribute("data", Convert.ToBase64String(Encoding.UTF8.GetBytes(Map.InnerXml))));
                return gameroomElement1;
            }
            XElement gameroomElement = new XElement("mission");
            try
            {
                gameroomElement.Add(new XAttribute("mission_key", Map.FirstChild.Attributes["uid"].InnerText));
            
            gameroomElement.Add(new XAttribute("no_teams", Map.FirstChild.Attributes["game_mode"].InnerText == "ffa" ? 1:0));
            gameroomElement.Add(new XAttribute("mode", Map.FirstChild.Attributes["game_mode"].InnerText));
            gameroomElement.Add(new XAttribute("mode_name", Map.FirstChild["UI"]["GameMode"].Attributes["text"].InnerText));
            //gameroomElement.Add(new XAttribute("mode_icon", Map.FirstChild["UI"]["GameMode"].Attributes["icon"].InnerText));
            gameroomElement.Add(new XAttribute("image", Map.FirstChild["UI"]["Description"].Attributes["icon"].InnerText));
            gameroomElement.Add(new XAttribute("description", Map.FirstChild["UI"]["Description"].Attributes["text"].InnerText));
            gameroomElement.Add(new XAttribute("name", Map.FirstChild.Attributes["name"].InnerText));
            gameroomElement.Add(new XAttribute("difficulty", "normal"));
            gameroomElement.Add(new XAttribute("type", ""));
            gameroomElement.Add(new XAttribute("setting", Map.FirstChild["Basemap"].Attributes["name"].InnerText));
            gameroomElement.Add(new XAttribute("time_of_day", Map.FirstChild.Attributes["time_of_day"].InnerText));
            gameroomElement.Add(new XAttribute("revision", Revision));
            if(IncludeData)
                gameroomElement.Add(new XAttribute("data", Convert.ToBase64String(Encoding.UTF8.GetBytes(Map.InnerXml))));

            }
            catch { }

            return gameroomElement;
        }
    }
}
