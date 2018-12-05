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
    class BroadcastSessionResults : Stanza
    {
        //Note! Packet should be containts the player_result's element
        internal List<XElement> SessionResults;
        public BroadcastSessionResults(Client User, XmlDocument Packet) : base(User, Packet)
        {
        }
        public BroadcastSessionResults(Client User, List<XElement> Results) : base(User, null) => SessionResults = Results;
        internal override void Process()
        {
            
            XDocument Packet = new XDocument();

            XElement iqElement = new XElement(Gateway.JabberNS + "iq");
            iqElement.Add(new XAttribute("type", "get"));
            iqElement.Add(new XAttribute("from", "k01.warface"));
            iqElement.Add(new XAttribute("to", User.JID));
            iqElement.Add(new XAttribute("id",User.Player.Random.Next(999999,Int32.MaxValue)));

            XElement queryElement = new XElement(Stanza.NameSpace + "query");

            XElement accountElement = new XElement((bool)App.Default["UseOldMode"] ? "brodcast_session_result" : "broadcast_session_result");
            foreach (XElement SessionResult in SessionResults)
                accountElement.Add(SessionResults);
            queryElement.Add(accountElement);
            iqElement.Add(queryElement);

            Packet.Add(iqElement);
            base.Compress(ref Packet);
            User.Send(Packet.ToString(SaveOptions.DisableFormatting));
        }
    }
}
