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
    class SendInvitation : Stanza
    {
        string Target;
        enum Error
        {
            SUCCESSFILY_SENDED = 3,
            REJECTED=1,
            IN_PROGRESS=2,
            ALREADY_IN_FRIEND=4,
            USER_OFFLINE=8,
            USER_NOT_FOUND=9,
            USER_NOT_FOUND_IN_LOBBY=10,
            LIMIT_REACHED=11,
            FRIEND_LIMIT_REACHED=12,
            TIMEOUT=14,
            ENABLED_DND=15
        }
        Error ErrorId = Error.SUCCESSFILY_SENDED;

        public SendInvitation(Client User, XmlDocument Packet) : base(User, Packet)
        {
            Target = base.Query.Attributes["target"].InnerText;
            if (Target == User.Player.Nickname) return;

            Client TargetOnline = ArrayList.OnlineUsers.Find(Attribute => Attribute.Player.Nickname == Target);
            if (TargetOnline != null)
            {
                foreach (XmlNode Friend in User.Player.friends["friends"].ChildNodes)
                {
                    if (Friend.InnerText == TargetOnline.Player.UserID.ToString())
                    {
                        ErrorId = Error.ALREADY_IN_FRIEND;
                        Process();
                        return;
                    }
                }
                foreach (XmlNode Notification in TargetOnline.Player.Notifications["notifications"].ChildNodes)
                {
                    if (Notification.Attributes["type"].InnerText == "64")
                    {
                        if (Notification["invitation"].Attributes["initiator"].InnerText == User.Player.Nickname)
                        {
                            ErrorId = Error.IN_PROGRESS;
                            Process();
                            return;
                        }
                    }
                }
                TargetOnline.Player.AddFriendNotification(User.Player.Nickname, false);
                TargetOnline.Player.Save();
                new SyncNotification(TargetOnline).Process();
            }
            else
            {
                Player TargetOffline = new Player()
                {
                    Nickname = Target
                };
                if (!TargetOffline.Load())
                {
                    ErrorId = Error.USER_NOT_FOUND;
                    Process();
                    return;
                }

                foreach (XmlNode Friend in User.Player.friends["friends"].ChildNodes)
                {
                    if(Friend.InnerText == TargetOffline.UserID.ToString())
                    {
                        ErrorId = Error.ALREADY_IN_FRIEND;
                        Process();
                        return;
                    }
                }
                foreach (XmlNode Notification in TargetOffline.Notifications["notifications"].ChildNodes)
                {
                    if (Notification.Attributes["type"].InnerText == "64")
                    {
                        if (Notification["invitation"].Attributes["initiator"].InnerText == User.Player.Nickname)
                        {
                            ErrorId = Error.IN_PROGRESS;
                            Process();
                            return;
                        }
                    }
                }
                TargetOffline.AddFriendNotification(User.Player.Nickname, false);
                TargetOffline.Save();
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

            XElement accountElement = new XElement("send_invitation");
            /*if (User.Player.Settings.ChildNodes.Count != 0)
            {
                string SettingsOnUser = User.Player.Settings.InnerXml;
                accountElement.Add(XDocument.Parse(SettingsOnUser).Root.FirstNode);
            }*/


            queryElement.Add(accountElement);
            
            XElement errorElement = new XElement((XNamespace)"urn:ietf:params:xml:ns:xmpp-stanzas" + "error");
            errorElement.Add(new XAttribute("type", "continue"));
            errorElement.Add(new XAttribute("code", 8));
            errorElement.Add(new XAttribute("custom_code", (int)ErrorId));
            iqElement.Add(queryElement);
            iqElement.Add(errorElement);
            Packet.Add(iqElement);
            
            User.Send(Packet.ToString(SaveOptions.DisableFormatting));
        }
    }
}
