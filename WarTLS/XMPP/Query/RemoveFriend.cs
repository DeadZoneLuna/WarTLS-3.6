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
    class RemoveFriend : Stanza
    {
        string Target;

        public RemoveFriend(Client User, XmlDocument Packet) : base(User, Packet)
        {
            Target = base.Query.Attributes["target"].InnerText;
            Client TargetOnline = ArrayList.OnlineUsers.Find(Attribute => Attribute.Player.Nickname == Target);
            if(TargetOnline != null)
            {
                User.Player.RemoveFriend(TargetOnline.Player.UserID.ToString());
                TargetOnline.Player.RemoveFriend(User.Player.UserID.ToString());
                TargetOnline.Player.Save();
                new FriendList(TargetOnline).Process();
            }
            else
            {
                Player TargetOffline = new Player()
                {
                    Nickname = Target
                };
                if (!TargetOffline.Load())
                {
                    Process();
                    return;
                }
                User.Player.RemoveFriend(TargetOffline.UserID.ToString());
                TargetOffline.RemoveFriend(User.Player.UserID.ToString());
                TargetOffline.Save();
            }
            new FriendList(User).Process();
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

            XElement accountElement = new XElement("remove_friend");

            queryElement.Add(accountElement);
            iqElement.Add(queryElement);

            Packet.Add(iqElement);
            base.Compress(ref Packet);
            User.Send(Packet.ToString(SaveOptions.DisableFormatting));
        }
    }
}
