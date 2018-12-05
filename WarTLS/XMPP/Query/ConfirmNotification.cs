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
    class ConfirmNotification : Stanza
    {
        string Channel;

        public ConfirmNotification(Client User, XmlDocument Packet) : base(User, Packet)
        {
            foreach(XmlNode NotificationForComplete in base.Query.ChildNodes)
            {
                foreach (XmlNode NotificationOnUser in User.Player.Notifications["notifications"].ChildNodes)
                {
                    if (NotificationOnUser.Attributes["id"].InnerText == NotificationForComplete.Attributes["id"].InnerText)
                    {
                        if (NotificationOnUser.Attributes["type"].InnerText == "64")
                        {
                            string ReceivedUser = NotificationOnUser["invitation"].Attributes["initiator"].InnerText;
                            Client ReceivedOnline = ArrayList.OnlineUsers.Find(Attribute => Attribute.Player.Nickname == ReceivedUser);


                            if (ReceivedOnline != null)
                            {
                                if (NotificationForComplete["confirmation"].Attributes["result"].InnerText == "0")
                                {
                                    User.Player.AddFriend(ReceivedOnline.Player.UserID.ToString());
                                    ReceivedOnline.Player.AddFriend(User.Player.UserID.ToString());
                                    User.Player.AddFriendResultNotification(ReceivedOnline.Player.UserID, ReceivedOnline.JID, ReceivedOnline.Player.Nickname, ReceivedOnline.Status, ReceivedOnline.Location, User.Player.Experience, NotificationForComplete["confirmation"].Attributes["result"].InnerText);
                                    new SyncNotification(User).Process();
                                }
                                ReceivedOnline.Player.AddFriendResultNotification(User.Player.UserID, User.JID, User.Player.Nickname, User.Status, NotificationForComplete["confirmation"].Attributes["location"].InnerText, User.Player.Experience, NotificationForComplete["confirmation"].Attributes["result"].InnerText);
                                new SyncNotification(ReceivedOnline).Process();
                            }
                            else
                            {
                                Player TargetOffline = new Player()
                                {
                                    Nickname = ReceivedUser
                                };
                                TargetOffline.Load();
                                if (NotificationForComplete["confirmation"].Attributes["result"].InnerText == "0")
                                {
                                    User.Player.AddFriend(TargetOffline.UserID.ToString());
                                    TargetOffline.AddFriend(User.Player.UserID.ToString());
                                    User.Player.AddFriendResultNotification(TargetOffline.UserID,"", TargetOffline.Nickname, 0, "", TargetOffline.Experience, NotificationForComplete["confirmation"].Attributes["result"].InnerText);
                                    new SyncNotification(User).Process();
                                }

                                TargetOffline.AddFriendResultNotification(User.Player.UserID, User.JID, User.Player.Nickname, User.Status, NotificationForComplete["confirmation"].Attributes["location"].InnerText, User.Player.Experience, NotificationForComplete["confirmation"].Attributes["result"].InnerText);
                                TargetOffline.Save();
                            }

                        }
                        User.Player.Notifications["notifications"].RemoveChild(NotificationOnUser);
                        User.Player.Save();
                    }
                }
            }
            User.Player.Save();
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

            XElement accountElement = new XElement("confirm_notification");
            /*if (User.Player.Settings.ChildNodes.Count != 0)
            {
                string SettingsOnUser = User.Player.Settings.InnerXml;
                accountElement.Add(XDocument.Parse(SettingsOnUser).Root.FirstNode);
            }*/
            queryElement.Add(accountElement);
            iqElement.Add(queryElement);

            Packet.Add(iqElement);
            base.Compress(ref Packet);
            User.Send(Packet.ToString(SaveOptions.DisableFormatting));
        }
    }
}
