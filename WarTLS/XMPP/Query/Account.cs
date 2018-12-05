using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using WARTLS.CLASSES;
using WARTLS.EXCEPTION;
using WARTLS.NETWORK;

namespace WARTLS.XMPP.QUERY
{
    class Account : Stanza
    {
        string Login;
        string Password;
        public Account(Client User, XmlDocument Packet):base(User,Packet)
        {
            //throw new StanzaError(User, this, null, 5);
            if (base.Query.Name == "account")
            {
                Login = base.Query.Attributes["login"].InnerText;
                Password = base.Query.Attributes["password"].InnerText;
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

            XElement accountElement = new XElement(base.Query.Name);
            if (base.Query.Name == "account")
            {
                accountElement.Add(new XAttribute("user", User.Player.UserID));
                accountElement.Add(new XAttribute("survival_lb_enabled", "0"));
                accountElement.Add(new XAttribute("active_token", " "));
                accountElement.Add(new XAttribute("nickname", ""));
            }
            XElement masterServersElement = new XElement("masterservers");
            foreach (Channel Channel in ArrayList.Channels)
                masterServersElement.Add(Channel.Serialize());
            accountElement.Add(masterServersElement);
            queryElement.Add(accountElement);
            iqElement.Add(queryElement);

            Packet.Add(iqElement);

            User.Send(Packet.ToString(SaveOptions.DisableFormatting));
        }
    }
}
