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
    class PersistentSettingsSet : Stanza
    {
        string Channel;

        public PersistentSettingsSet(Client User, XmlDocument Packet) : base(User, Packet)
        {
            XmlNode NewSettings = base.Query["settings"];
            foreach(XmlNode Setting in NewSettings.ChildNodes)
            {
                if (User.Player.Settings["settings"] == null)
                    User.Player.Settings.AppendChild(User.Player.Settings.ImportNode(NewSettings,true));
                if(User.Player.Settings["settings"][Setting.Name] == null)
                    User.Player.Settings["settings"].AppendChild(User.Player.Settings.ImportNode(NewSettings.FirstChild, true));
                else
                {
                    foreach(XmlAttribute SettingForSet in base.Query["settings"][Setting.Name].Attributes)
                    {
                        if (User.Player.Settings["settings"][Setting.Name].Attributes[SettingForSet.Name] == null)
                            User.Player.Settings["settings"][Setting.Name].SetAttribute(SettingForSet.Name, SettingForSet.Value);
                        else
                            User.Player.Settings["settings"][Setting.Name].Attributes[SettingForSet.Name].Value = SettingForSet.Value;
                    }
                }
            }

            User.Player.Save();
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

            XElement accountElement = new XElement("persistent_settings_set");


            queryElement.Add(accountElement);
            iqElement.Add(queryElement);

            Packet.Add(iqElement);

            User.Send(Packet.ToString(SaveOptions.DisableFormatting));
        }
    }
}
