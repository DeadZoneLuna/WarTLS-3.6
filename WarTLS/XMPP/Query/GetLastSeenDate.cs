using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using WARTLS.CLASSES;
using WARTLS.NETWORK;

namespace WARTLS.XMPP.QUERY
{
    class GetLastSeenDate : Stanza
    {
        string LastSeen="1";
        string ProfileId;

        public GetLastSeenDate(Client User, XmlDocument Packet) : base(User, Packet)
        {
            Player TargetOffline = null;
            ProfileId = (bool)App.Default["UseOldMode"] ? base.Query.Attributes["nickname"].InnerText : base.Query.Attributes["profile_id"].InnerText;
            long ID = (bool)App.Default["UseOldMode"] ? -1 : long.Parse(ProfileId);
            if(ID > 0)
                TargetOffline = new Player()
                {

                    UserID = ID
                };
            else
                TargetOffline = new Player()
                {
                    Nickname= ProfileId
                };

            if (!TargetOffline.Load())
            {
                Process();
                return;
            }
            else
            {
                LastSeen = ((DateTimeOffset)TargetOffline.LastSeen).ToUnixTimeSeconds().ToString();
            }
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

            XElement accountElement = new XElement("get_last_seen_date");
            accountElement.Add(new XAttribute("profile_id", ProfileId));
            accountElement.Add(new XAttribute("last_seen", LastSeen));

            queryElement.Add(accountElement);
            iqElement.Add(queryElement);

            Packet.Add(iqElement);
    
            User.Send(Packet.ToString(SaveOptions.DisableFormatting));
        }
    }
}
