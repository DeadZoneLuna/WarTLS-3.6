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
    class GetProfilePerformance : Stanza
    {
        string Channel;

        public GetProfilePerformance(Client User, XmlDocument Packet) : base(User, Packet)
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

            XElement accountElement = new XElement("get_profile_performance");
            XElement pvpModesElement = new XElement("pvp_modes_to_complete");
            XElement pveMissionsPerfomance = new XElement("pve_missions_performance");

            pvpModesElement.Add(new XElement("mode", "ctf"));
            pvpModesElement.Add(new XElement("mode", "dst"));
            pvpModesElement.Add(new XElement("mode", "ptb"));
            pvpModesElement.Add(new XElement("mode", "lms"));
            pvpModesElement.Add(new XElement("mode", "ffa"));
            pvpModesElement.Add(new XElement("mode", "stm"));
            pvpModesElement.Add(new XElement("mode", "tbs"));
            pvpModesElement.Add(new XElement("mode", "dmn"));
            pvpModesElement.Add(new XElement("mode", "hnt"));
            pvpModesElement.Add(new XElement("mode", "tdm"));


            /*if (User.Player.Settings.ChildNodes.Count != 0)
            {
                string SettingsOnUser = User.Player.Settings.InnerXml;
                accountElement.Add(XDocument.Parse(SettingsOnUser).Root.FirstNode);
            }*/
            accountElement.Add(pveMissionsPerfomance);
            accountElement.Add(pvpModesElement);
            queryElement.Add(accountElement);
            iqElement.Add(queryElement);

            Packet.Add(iqElement);
            
            User.Send(Packet.ToString(SaveOptions.DisableFormatting));
        }
    }
}
