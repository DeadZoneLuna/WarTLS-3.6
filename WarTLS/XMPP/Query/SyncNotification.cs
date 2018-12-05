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
    class SyncNotification : Stanza
    {
        string Channel;
        XElement FastNotify = null;
        public SyncNotification(Client User, XmlDocument Packet = null) : base(User, Packet)
        {
        }
        public SyncNotification(Client User,XElement FastNotify) : base(User, null)
        {
            this.FastNotify = FastNotify;
        }
        internal override void Process()
        {
            if (base.Type == "result") return;
            XDocument Packet = new XDocument();
            
            XElement iqElement = new XElement( "iq");
            iqElement.Add(new XAttribute("type", base.Type == "get" ? "result" : "get"));

            iqElement.Add(new XAttribute("from", $"masterserver@warface/wartls"));
            iqElement.Add(new XAttribute("to", User.JID));
            iqElement.Add(new XAttribute("id", base.Type == "get" ? base.Id : $"uid{User.Player.Random.Next(9999, Int32.MaxValue).ToString("x8")}"));

            XElement queryElement = new XElement(Stanza.NameSpace + "query");

            XElement accountElement = new XElement("sync_notifications");
            if (FastNotify == null)
            {
                if (User.Player.Notifications.FirstChild.ChildNodes.Count > 0)
                {
                    foreach (XmlNode Notification in User.Player.Notifications.FirstChild.ChildNodes)
                        accountElement.Add(XDocument.Parse(Notification.OuterXml).Root);
                }
            }
            else
                accountElement.Add(FastNotify);
            
            queryElement.Add(accountElement);
            iqElement.Add(queryElement);
            Packet.Add(iqElement);
            iqElement.Attributes().Where(e => e.IsNamespaceDeclaration).Remove();
            base.Compress(ref Packet);
            User.Send(Packet.ToString(SaveOptions.DisableFormatting));
        }
    }
}
