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
    class PersistentSettingsGet : Stanza
    {
        string Channel;

        public PersistentSettingsGet(Client User, XmlDocument Packet) : base(User, Packet)
        {


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

            XElement accountElement = new XElement("persistent_settings_get");
            
            if (User.Player.Settings.ChildNodes.Count != 0)
            {
                if ((bool)App.Default["UseOldMode"])
                {
                    foreach(XmlNode Setting in User.Player.Settings["settings"].ChildNodes)
                    {
                        accountElement.Add(XDocument.Parse(Setting.InnerXml == "" ? Setting.OuterXml : Setting.InnerXml).Root);
                    }
                }
                else
                {
                    foreach (XmlNode Setting in User.Player.Settings["settings"].ChildNodes)
                    {
                        accountElement.Add(XDocument.Parse(Setting.OuterXml).Root);
                    }
                }
            }
            queryElement.Add(accountElement);
            iqElement.Add(queryElement);

            Packet.Add(iqElement);
            base.Compress(ref Packet);
            User.Send(Packet.ToString(SaveOptions.DisableFormatting));
        }
    }
}
