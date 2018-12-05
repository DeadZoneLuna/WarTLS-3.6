using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using WARTLS.CLASSES;
using WARTLS.NETWORK;

namespace WARTLS.XMPP.QUERY
{
    class ShopBuyOffer : Stanza
    {
        Item Buyed;
        List<XElement> profileItemElements = new List<XElement>();
        List<Item> PurchasedItems = new List<Item>();
        int OfferId;
        public ShopBuyOffer(Client User, XmlDocument Packet) : base(User, Packet)
        {
            
            List<int> Offers = new List<int>();
            if (base.Query.Name == "shop_buy_multiple_offer")
                foreach (XmlElement el in base.Query.ChildNodes)
                {
                    OfferId = int.Parse(el.Attributes["id"].InnerText);
                    Offers.Add(int.Parse(el.Attributes["id"].InnerText));
                }
            else
            {
                OfferId = int.Parse(base.Query.Attributes["offer_id"].InnerText);
                Offers.Add(OfferId);
            }
            foreach (int OfferId in Offers)
            {

                foreach (XmlNode Offer in GameResources.ShopOffers["items"].ChildNodes)
                {
                    if (int.Parse(Offer.Attributes["id"].InnerText) == OfferId)
                    {
                        int GameMoneyCost = int.Parse(Offer.Attributes["game_price"].InnerText);
                        int CryMoneyCost = int.Parse(Offer.Attributes["cry_price"].InnerText);
                        int CrownMoneyCost = int.Parse(Offer.Attributes["crown_price"].InnerText);

                        User.Player.CrownMoney -= CrownMoneyCost;
                        User.Player.GameMoney -= GameMoneyCost;
                        User.Player.CryMoney -= CryMoneyCost;

                        if (Offer.Attributes["name"].InnerText.Contains("game_money_item_01"))
                        {
                            User.Player.GameMoney += int.Parse(Offer.Attributes["quantity"].InnerText);
                        }
                        else if (Offer.Attributes["name"].InnerText.Contains("box"))
                        {
                            profileItemElements.AddRange (User.Player.GeneratePrizes(Offer.Attributes["name"].InnerText, OfferId));
                        }
                        else if (Offer.Attributes["name"].InnerText.Contains("bundle_item"))
                        {
                            XmlDocument BundleItemXML = new XmlDocument();
                            BundleItemXML.Load($"Gamefiles/ShopItems/{Offer.Attributes["name"].InnerText}.xml");
                            foreach (XmlNode Bundle in BundleItemXML["shop_item"]["bundle"].ChildNodes)
                            {
                                if (Bundle.Attributes["expiration"] != null)
                                {
                                    if (Bundle.Attributes["expiration"].InnerText.Contains("d"))
                                    {
                                        int Hours = (int)TimeSpan.FromDays(int.Parse(new Regex("[0-9]*").Match(Bundle.Attributes["expiration"].InnerText).Value)).TotalHours;
                                        PurchasedItems.Add(new Item(ItemType.TIME, User.Player.ItemSeed, Bundle.Attributes["name"].InnerText, Hours) { BundleTime = Bundle.Attributes["expiration"].InnerText });
                                    }
                                    else if (Bundle.Attributes["expiration"].InnerText.Contains("h"))
                                    {
                                        int Hours = int.Parse(new Regex("[0-9]*").Match(Bundle.Attributes["expiration"].InnerText).Value);
                                        PurchasedItems.Add(new Item(ItemType.TIME, User.Player.ItemSeed, Bundle.Attributes["name"].InnerText, Hours) { BundleTime = Bundle.Attributes["expiration"].InnerText });
                                    }
                                }
                                else if (Bundle.Attributes["regular"] != null)
                                {
                                    PurchasedItems.Add(new Item(ItemType.NO_REPAIR, User.Player.ItemSeed, Bundle.Attributes["name"].InnerText));
                                }
                                else if (Bundle.Attributes["amount"] != null)
                                {
                                    PurchasedItems.Add(new Item(ItemType.CONSUMABLE, User.Player.ItemSeed, Bundle.Attributes["name"].InnerText, 0, int.Parse(Bundle.Attributes["amount"].InnerText)) { BundleQuantity = Bundle.Attributes["amount"].InnerText });
                                }
                                else
                                {
                                    PurchasedItems.Add(new Item(ItemType.PERMANENT, User.Player.ItemSeed, Bundle.Attributes["name"].InnerText));
                                }
                            }
                        }
                        else if (Offer.Attributes["expirationTime"].InnerText != "0")
                        {
                            int Hours = int.Parse(new Regex("[0-9]*").Match(Offer.Attributes["expirationTime"].InnerText).Value);

                            if (Offer.Attributes["expirationTime"].InnerText.Contains("d"))
                            {
                                Hours = (int)TimeSpan.FromDays(Hours).TotalHours;
                                PurchasedItems.Add(new Item(ItemType.TIME, User.Player.ItemSeed, Offer.Attributes["name"].InnerText, Hours));
                            }
                            if (Offer.Attributes["expirationTime"].InnerText.Contains("h"))
                            {
                                PurchasedItems.Add(new Item(ItemType.TIME, User.Player.ItemSeed, Offer.Attributes["name"].InnerText, Hours));
                            }
                        }
                        else if (Offer.Attributes["durabilityPoints"].InnerText != "0")
                        {
                            PurchasedItems.Add(new Item(ItemType.PERMANENT, User.Player.ItemSeed, Offer.Attributes["name"].InnerText, 0, 0, long.Parse(Offer.Attributes["durabilityPoints"].InnerText)));
                        }
                        else if (Offer.Attributes["quantity"].InnerText != "0")
                        {
                            PurchasedItems.Add(new Item(ItemType.CONSUMABLE, User.Player.ItemSeed, Offer.Attributes["name"].InnerText, 0, int.Parse(Offer.Attributes["quantity"].InnerText), 0));
                        }
                        else
                        {
                            PurchasedItems.Add(new Item(ItemType.NO_REPAIR, User.Player.ItemSeed, Offer.Attributes["name"].InnerText, 0, 0, 0));
                        }

                        if (profileItemElements.Count == 0)
                            foreach (Item PurchasedItem in PurchasedItems)
                            {
                                Item ItemOnUser = PurchasedItem;
                                XElement profileItemElement = new XElement("profile_item");

                                if (PurchasedItem.Name != "game_money_item_01")
                                {
                                    ItemOnUser = User.Player.AddItem(PurchasedItem);

                                    profileItemElement.Add(new XAttribute("name", ItemOnUser.Name));
                                    profileItemElement.Add(new XAttribute("profile_item_id", ItemOnUser.ID));
                                    profileItemElement.Add(new XAttribute("offerId", OfferId));
                                    profileItemElement.Add(new XAttribute("added_expiration", !Offer.Attributes["name"].InnerText.Contains("bundle_item") && !Offer.Attributes["name"].InnerText.Contains("random_box") ? Offer.Attributes["expirationTime"].InnerText : PurchasedItem.BundleTime));
                                    profileItemElement.Add(new XAttribute("added_quantity", !Offer.Attributes["name"].InnerText.Contains("bundle_item") && !Offer.Attributes["name"].InnerText.Contains("random_box") ? Offer.Attributes["quantity"].InnerText : PurchasedItem.BundleQuantity));
                                    profileItemElement.Add(new XAttribute("error_status", "0"));
                                    profileItemElement.Add(ItemOnUser.Serialize());

                                    profileItemElements.Add(profileItemElement);
                                }
                                Buyed = ItemOnUser;
                            }
                        break;
                    }
                }
            }
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

            XElement accountElement = new XElement(base.Query.Name);
            accountElement.Add(new XAttribute("error_status", "0"));
            if (base.Query.Name != "extend_item")
            {
                accountElement.Add(new XAttribute("offer_id", OfferId));

                XElement purchasedItem = new XElement("purchased_item");
                foreach (XElement profileItemElement in profileItemElements)
                    purchasedItem.Add(profileItemElement);

                XElement moneyElement = new XElement("money");
                moneyElement.Add(new XAttribute("cry_money", User.Player.CryMoney));
                moneyElement.Add(new XAttribute("crown_money", User.Player.CrownMoney));
                moneyElement.Add(new XAttribute("game_money", User.Player.GameMoney));
                accountElement.Add(purchasedItem);
                accountElement.Add(moneyElement);
            }
            else
            {
                accountElement.Add(new XAttribute("durability", Buyed.DurabilityPoints));
                accountElement.Add(new XAttribute("total_durability", Buyed.TotalDurabilityPoints));
                accountElement.Add(new XAttribute("expiration_time_utc", Buyed.ExpirationTime));
                accountElement.Add(new XAttribute("seconds_left", Buyed.SecondsLeft));
                accountElement.Add(new XAttribute("cry_money", User.Player.CryMoney));
                accountElement.Add(new XAttribute("game_money", User.Player.GameMoney));
                accountElement.Add(new XAttribute("crown_money", User.Player.CrownMoney));


            }


            queryElement.Add(accountElement);
            iqElement.Add(queryElement);

            Packet.Add(iqElement);

            User.Send(Packet.ToString(SaveOptions.DisableFormatting));
            User.CheckExperience();
        }
    }
}
