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
    class GameResourcesGet : Stanza
    {
        int From;
        int Left;
        XmlDocument Selected;
        public GameResourcesGet(Client User, XmlDocument Packet) : base(User, Packet)
        {
            From = int.Parse(base.Query.Attributes["from"] == null ? "0" : base.Query.Attributes["from"].InnerText);
            int Index = From / 250;
            if (base.Query.Name == "items")
            {
                Selected = GameResources.ItemsSplited[Index];
                if ((bool)App.Default["UseOldMode"])
                {
                    Selected = GameResources.ItemsSplited[int.Parse(base.Query.Attributes["received"].InnerText) / 250];
                    Left = GameResources.ItemsSplited.Count - (int.Parse(base.Query.Attributes["received"].InnerText) / 250) - 1;
                }
            }
            if (base.Query.Name == "get_configs")
            {
                Selected = GameResources.ConfigsSplited[Index];
                if ((bool)App.Default["UseOldMode"])
                {
                    Selected = GameResources.ConfigsSplited[int.Parse(base.Query.Attributes["received"].InnerText) / 250];
                    Left = GameResources.ConfigsSplited.Count - int.Parse(base.Query.Attributes["received"].InnerText);
                }
            }
            if (base.Query.Name == "shop_get_offers")
            {
                Selected = GameResources.ShopOffersSplited[Index];
                if ((bool)App.Default["UseOldMode"])
                {
                    Selected = GameResources.ShopOffersSplited[int.Parse(base.Query.Attributes["received"].InnerText) / 250];
                    Left = GameResources.ShopOffersSplited.Count - (int.Parse(base.Query.Attributes["received"].InnerText) / 250) - 1;
                }
            }
            if (base.Query.Name == "quickplay_maplist")
            {
                Selected = GameResources.QuickPlayMapListSplited[Index];
                if ((bool)App.Default["UseOldMode"])
                {
                    Selected = GameResources.QuickPlayMapListSplited[int.Parse(base.Query.Attributes["received"].InnerText) / 250];
                    Left = GameResources.QuickPlayMapListSplited.Count - int.Parse(base.Query.Attributes["received"].InnerText);
                }
            }
            if (base.Query.Name == "missions_get_list")
                Selected = GameResources.PvE;
            Process();
        }
        internal override void Process()
        {
            
            XDocument Packet = new XDocument();

            XElement iqElement = new XElement(Gateway.JabberNS + "iq");
            iqElement.Add(new XAttribute("type", "result"));
            iqElement.Add(new XAttribute("from", (bool)App.Default["UseOldMode"] ? base.To : $"masterserver@warface/{User.Channel.Resource}"));
            iqElement.Add(new XAttribute("to", User.JID));
            iqElement.Add(new XAttribute("id", base.Id));

            XElement queryElement = new XElement(Stanza.NameSpace + "query");


            XElement accountElement =new XElement(base.Query.Name);
            if (Selected != null)
                accountElement = XElement.Parse(Selected.InnerXml);
            if ((bool)App.Default["UseOldMode"])
            {
                accountElement.RemoveAttributes();
                accountElement.Add(new XAttribute("token", "-1"));
                accountElement.Add(new XAttribute("left", Left));
            }
            queryElement.Add(accountElement);
           
            iqElement.Add(queryElement);

            Packet.Add(iqElement);
            base.Compress(ref Packet);
            User.Send(Packet.ToString(SaveOptions.DisableFormatting));
        }
    }
}
