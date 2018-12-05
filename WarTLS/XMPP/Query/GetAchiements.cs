using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using WARTLS.CLASSES;
using WARTLS.NETWORK;

namespace WARTLS.XMPP.QUERY
{
    class GetAchievements : Stanza
    {
        string Channel;

        public GetAchievements(Client User, XmlDocument Packet) : base(User, Packet)
        {


            Process();
        }
        internal override void Process()
        {
            Client AchieveUser = ArrayList.OnlineUsers.Find(Attribute => Attribute.Player.UserID == long.Parse(base.Query.FirstChild.Attributes["profile_id"].InnerText));
            if (AchieveUser == null)
            {
                
                Player TargetOffline = new Player()
                {
                   UserID = long.Parse(base.Query.FirstChild.Attributes["profile_id"].InnerText)
                };
                if (TargetOffline.Load())
                    AchieveUser = new Client() { Player = TargetOffline };
                else return;
            }
            XDocument Packet = new XDocument();

            XElement iqElement = new XElement(Gateway.JabberNS + "iq");
            iqElement.Add(new XAttribute("type", "result"));
            iqElement.Add(new XAttribute("from", base.To));
            iqElement.Add(new XAttribute("to", User.JID));
            iqElement.Add(new XAttribute("id", base.Id));

            XElement queryElement = new XElement(Stanza.NameSpace + "query");

            XElement accountElement = new XElement("get_achievements");
            XElement achievementElement = new XElement("achievement");
            achievementElement.Add(new XAttribute("profile_id", AchieveUser.Player.UserID));
            if (AchieveUser.Player.Achievements.FirstChild.ChildNodes.Count != 0)
            {
                foreach (XmlNode Notification in AchieveUser.Player.Achievements.FirstChild.ChildNodes)
                    achievementElement.Add(XDocument.Parse(Notification.OuterXml).Root);
            }
            accountElement.Add(achievementElement);
            queryElement.Add(accountElement);
            iqElement.Add(queryElement);

            Packet.Add(iqElement);
            
            base.Compress(ref Packet);
            User.Send(Packet.ToString(SaveOptions.DisableFormatting));
        }
    }
}
