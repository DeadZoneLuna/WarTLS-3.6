using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WARTLS.CLASSES
{
    class GameResources
    {
        public static XmlDocument Items { get; private set; } = new XmlDocument();
        public static XmlDocument PvE { get; private set; } = new XmlDocument();
        public static XmlDocument QuickPlayMapList { get; private set; } = new XmlDocument();
        public static XmlDocument ShopOffers { get; private set; } = new XmlDocument();
        public static XmlDocument Configs { get; private set; } = new XmlDocument();
        public static XmlDocument OnlineVariables { get; private set; } = new XmlDocument();
        public static XmlDocument NewbieItemsXML { get; private set; } = new XmlDocument();
        public static XmlDocument NewbieItemsOldXML { get; private set; } = new XmlDocument();
        public static XmlDocument ExpCurve { get; private set; } = new XmlDocument();
        public static List<Item> NewbieItems { get; private set; } = new List<Item>();
        public static List<XmlDocument> ItemsSplited = new List<XmlDocument>();
        public static List<XmlDocument> Maps = new List<XmlDocument>();
        public static List<XmlDocument> ShopOffersSplited  = new List<XmlDocument>();
        public static List<XmlDocument> ConfigsSplited = new List<XmlDocument>();
        public static List<XmlDocument> QuickPlayMapListSplited = new List<XmlDocument>();

        internal GameResources()
        {
            Items.Load("Gamefiles/Items.xml");
            Configs.Load("Gamefiles/Configs.xml");
            ShopOffers.Load("Gamefiles/ShopOffers.xml");

            PvE.Load("Gamefiles/PvE.xml");
            OnlineVariables.Load("Gamefiles/OnlineVariables.xml");
            NewbieItemsXML.Load("Gamefiles/NewbieItems.xml");
            NewbieItemsOldXML.Load("Gamefiles/NewbieItemsOld.xml");
            QuickPlayMapList.Load("Gamefiles/QuickPlayMapList.xml");
            ExpCurve.Load("Gamefiles/ExpCurve.xml");

            if ((bool)App.Default["UseOldMode"])
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine($"[{this.GetType().Name}] Note! Server started on OLD mode (WF 2013 version's and lower)");
                Console.ResetColor();
                foreach (string Map in Directory.GetFiles("Gamefiles/Maps/Old", "*.xml", SearchOption.TopDirectoryOnly))
                {
                    XmlDocument Document = new XmlDocument();
                    Document.Load(Map);
                    Maps.Add(Document);
                }
                foreach (XmlNode Item in NewbieItemsOldXML["items"].ChildNodes)
                {
                    Item i = new Item();
                    i.Create((XmlElement)Item);
                    NewbieItems.Add(i);
                }
            }
            else
            {
                foreach (string Map in Directory.GetFiles("Gamefiles/Maps", "*.xml", SearchOption.TopDirectoryOnly))
                {
                    XmlDocument Document = new XmlDocument();
                    Document.Load(Map);
                    Maps.Add(Document);
                }
                foreach (XmlNode Item in NewbieItemsXML["items"].ChildNodes)
                {
                    Item i = new Item();
                    i.Create((XmlElement)Item);
                    NewbieItems.Add(i);
                }
            }
            NewbieItems = new List<Item>();
            foreach (XmlNode Item in NewbieItemsOldXML["items"].ChildNodes)
            {
                Item i = new Item();
                i.Create((XmlElement)Item);
                NewbieItems.Add(i);
            }
            SplitGamefiles(Items, ref ItemsSplited);
            SplitGamefiles(ShopOffers, ref ShopOffersSplited);
            SplitGamefiles(Configs, ref ConfigsSplited);
            SplitGamefiles(QuickPlayMapList, ref QuickPlayMapListSplited);
        }
        internal void SplitGamefiles(XmlDocument _From,ref List<XmlDocument> _To,int BlockSize = 250)
        {
            int CurrentPart = 0;
            int Parts = _From["items"].ChildNodes.Count / BlockSize;
            int TotalReaded = 0;
            for(int i = 0; i <= Parts;i++)
            {
                XmlDocument Part = new XmlDocument();
                if (i != Parts)
                {
                    XmlElement ItemElement = Part.CreateElement("items");

                    XmlAttribute Code = Part.CreateAttribute("code");
                    XmlAttribute From = Part.CreateAttribute("from");
                    XmlAttribute To = Part.CreateAttribute("to");
                    XmlAttribute Hash = Part.CreateAttribute("hash");
                    Code.Value = "2";
                    From.Value = TotalReaded.ToString();
                    To.Value = $"{TotalReaded + BlockSize}";
                    Hash.Value = "0";

                    for (int _i=0; _i < BlockSize; _i++)
                    {
                        ItemElement.AppendChild(Part.ImportNode(_From["items"].ChildNodes[CurrentPart+_i], true));
                        TotalReaded += 1;
                    }

                    ItemElement.Attributes.Append(Code);
                    ItemElement.Attributes.Append(From);
                    ItemElement.Attributes.Append(To);
                    ItemElement.Attributes.Append(Hash);
                    Part.AppendChild(ItemElement);
                    CurrentPart += BlockSize;
                }
                else
                {
                    BlockSize = _From["items"].ChildNodes.Count - TotalReaded;
                        XmlElement ItemElement = Part.CreateElement("items");

                        XmlAttribute Code = Part.CreateAttribute("code");
                        XmlAttribute From = Part.CreateAttribute("from");
                        XmlAttribute To = Part.CreateAttribute("to");
                        XmlAttribute Hash = Part.CreateAttribute("hash");
                        Code.Value = "3";
                        From.Value = TotalReaded.ToString();
                        To.Value = $"{TotalReaded + BlockSize}";
                        Hash.Value = "0";

                        for (int __i = 0; __i < BlockSize; __i++)
                        
                            ItemElement.AppendChild(Part.ImportNode(_From["items"].ChildNodes[CurrentPart + __i], true));
                        

                        ItemElement.Attributes.Append(Code);
                        ItemElement.Attributes.Append(From);
                        ItemElement.Attributes.Append(To);
                        ItemElement.Attributes.Append(Hash);
                        Part.AppendChild(ItemElement);
                    }
                _To.Add(Part);
            }
        }
    }
}
