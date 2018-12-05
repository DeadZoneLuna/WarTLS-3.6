using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using WARTLS.CLASSES;

namespace WARTLS.XMPP
{
    class Auth
    {
        readonly XNamespace NameSpace = "urn:ietf:params:xml:ns:xmpp-sasl";
        internal Auth(Client User, XmlDocument Packet)
        {
            string[] AuthData = Encoding.UTF8.GetString(Convert.FromBase64String(Packet["auth"].InnerText)).Split(new[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
            if (AuthData[0] != "dedicated" && AuthData[1] != "dedicated")
            {
                long ID = long.Parse(AuthData[1]);
                Player Player = new Player()
                {
                    UserID = ID
                };
                if (!Player.Load())
                {
                    XDocument Answer = new XDocument(new XElement(NameSpace + "failure"));
                    User.Authorized = true;
                    ArrayList.OnlineUsers.Add(User);
                    User.Send(Answer.ToString(SaveOptions.DisableFormatting));
                }
                else
                {
                    XDocument Answer = new XDocument(new XElement(NameSpace + "success"));
                    User.Authorized = true;
                    ArrayList.OnlineUsers.Add(User);
                    User.Send(Answer.ToString(SaveOptions.DisableFormatting));
                }
                
                User.Player = Player;
            }
            else
            {
                Player Player = new Player();
                User.Dedicated = true;
                User.Player = Player;
                XDocument Answer = new XDocument(new XElement(NameSpace + "success"));
                User.Authorized = true;
                ArrayList.OnlineUsers.Add(User);
                User.Send(Answer.ToString(SaveOptions.DisableFormatting));
            }
        }
    }
}
