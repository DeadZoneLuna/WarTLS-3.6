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
    class Messages : Stanza
    {
        string Channel;

        public Messages(Client User, XmlDocument Packet) : base(User, Packet)
        {
            if (base.To == $"{User.Channel.JID}@conference.warface")
            {
                foreach (Client Receiver in User.Channel.Users.ToArray())
                {
                    XElement MessageElement = new XElement("message",
                        new XAttribute("from", $"global.{User.Channel.Resource}@conference.warface/{User.Player.Nickname}"),
                        new XAttribute("to", Receiver.JID),
                        new XAttribute("type", "groupchat"),
                            new XElement("body", base.Packet["message"].InnerText)
                        );
                    Receiver.Send(MessageElement.ToString(SaveOptions.DisableFormatting));
                }
            }
            else if (User.Player.GameRoom != null && base.To == $"room.{User.Player.GameRoom.Core.RoomId}@conference.warface")
            {
                foreach (Client Receiver in User.Player.GameRoom.Core.Users.ToArray())
                {
                    XElement MessageElement = new XElement("message",
                        new XAttribute("from", $"room.{User.Player.GameRoom.Core.RoomId}@conference.warface/{User.Player.Nickname}"),
                        new XAttribute("to", Receiver.JID),
                        new XAttribute("type", "groupchat"),
                            new XElement("body", base.Packet["message"].InnerText)
                        );
                    Receiver.Send(MessageElement.ToString(SaveOptions.DisableFormatting));
                }
            }
            else if (User.Player.GameRoom != null && base.To == $"team.room.{User.Player.GameRoom.Core.RoomId}@conference.warface")
            {
                foreach (Client Receiver in User.Player.GameRoom.Core.Users.FindAll(Attribute => Attribute.TeamId == User.TeamId).ToArray())
                {
                    XElement MessageElement = new XElement("message",
                        new XAttribute("from", $"team.room.{User.Player.GameRoom.Core.RoomId}@conference.warface/{User.Player.Nickname}"),
                        new XAttribute("to", Receiver.JID),
                        new XAttribute("type", "groupchat"),
                            new XElement("body", base.Packet["message"].InnerText)
                        );
                    Receiver.Send(MessageElement.ToString(SaveOptions.DisableFormatting));
                }
            }
            else if (base.To == "wfc.row_emul.warface" || base.To == "wfc.warface")
            {
                Client Receiver = ArrayList.OnlineUsers.Find(Attribute => Attribute.Player.Nickname == base.Query.Attributes["nick"].InnerText);

                XElement iqElement = new XElement(Gateway.JabberNS + "iq");
                iqElement.Add(new XAttribute("type", "get"));
                iqElement.Add(new XAttribute("from", User.JID));
                iqElement.Add(new XAttribute("to", Receiver.JID));
                iqElement.Add(new XAttribute("id", base.Id));
                XElement queryElement = new XElement(Stanza.NameSpace + "query");
                queryElement.Add(new XElement("message", 
                    new XAttribute("from", User.Player.Nickname), 
                    new XAttribute("nick", Receiver.Player.Nickname), 
                    new XAttribute("message", base.Query.Attributes["message"].InnerText)));
                iqElement.Add(queryElement);

                Receiver.Send(iqElement.ToString(SaveOptions.None));
            }
            else if (base.Type == "result")
                new ToOnlinePlayers(User, Packet);
            else
            {
                try
                {
                    XElement MessageElement = new XElement("message",
        new XAttribute("from", base.To),
        new XAttribute("to", User.JID),
        new XAttribute("type", "error"),
            new XElement("body", base.Packet["message"].InnerText),
            new XElement("error",
                new XAttribute("code", "406"),
                new XAttribute("type", "modify"),
                    new XElement((XNamespace)"urn:ietf:params:xml:ns:xmpp-stanzas" + "not-acceptable"),
                    new XElement((XNamespace)"urn:ietf:params:xml:ns:xmpp-stanzas" + "text",
                        "Only occupants are allowed to send messages to the conference"))
        );
                    User.Send(MessageElement.ToString(SaveOptions.DisableFormatting));
                }
                catch { }
                }
            //User.ShowMessage("vk.com/projectx_games",true);
            //User.Player.AddRandomBoxNotification("random_box_84","А ты че?");
            //new SyncNotification(User).Process();
        }

        //This message not provide Process();
    }
}
