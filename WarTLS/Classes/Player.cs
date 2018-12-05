using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;
using System.Xml.Linq;
using WARTLS.CLASSES.GAMEROOM;
using WARTLS.XMPP.QUERY;

namespace WARTLS.CLASSES
{
    class Player
    {
        internal long UserID = -1;
        internal string Nickname = "NoNameYet";
        internal string Gender = "male";
        internal DateTime LastSeen = DateTime.Now;
        internal XmlDocument Settings = new XmlDocument();
        internal XmlDocument Stats = new XmlDocument();
        internal XmlDocument RandomBox = new XmlDocument();
        internal XmlDocument Achievements = new XmlDocument();
        private XmlDocument notifications = new XmlDocument();
        internal XmlDocument friends = new XmlDocument();
        internal Clan Clan;
        internal GameRoom GameRoom;
        internal string GroupId = "";
        internal byte OldRank = 0;
        internal double Height = 1;
        internal double Fatness = 1;
        internal string Head = "default_head_04";
        internal byte CurrentClass = 0;
        internal int Experience = 0;
        internal int BannerMark = 0;
        internal int BannerBadge = 0;
        internal int BannerStripe = 0;
        internal int GameMoney = 777500;
        internal int CryMoney = 777500;
        internal int CrownMoney = 777500;
        internal Random Random = new Random();
        internal bool SoldierPassed = false;
        internal bool EngineerPassed = false;
        internal bool MedicPassed = false;
        internal bool SoldierSuggest = true;
        internal bool EngineerSuggest = false;
        internal bool MedicSuggest = false;
        internal string UnlockedMissions = "none,trainingmission,all";
        internal long ItemSeed => Items.Count + 1;
        internal List<Item> Items { get; set; } = GameResources.NewbieItems;
        internal bool ProfileCreated => this.Nickname != "" && this.Nickname != null;
        internal Timer Token = new Timer(6000);
        internal int MessagesSended = 0;
        internal byte Rank 
        {
            get
            {
                foreach(XmlNode RankUp in GameResources.ExpCurve["exp_curve"].ChildNodes)
                    if (this.Experience < int.Parse(RankUp.Attributes["exp"].InnerText))
                        return byte.Parse((int.Parse(RankUp.Name.Replace("level",""))-1).ToString());

                return 90;
            }
        }
        internal void UpdateAchievement(XmlNode Chunk)
        {
            foreach(XmlNode Achieve in Achievements.FirstChild.ChildNodes)
            {
                if (Achieve.Attributes["achievement_id"].InnerText == Chunk.Attributes["achievement_id"].InnerText)
                {
                    Achievements.FirstChild.RemoveChild(Achieve);
                    Achievements.FirstChild.AppendChild(Achievements.ImportNode(Chunk, true));
                    return;
                }
            }
            Achievements.FirstChild.AppendChild(Achievements.ImportNode(Chunk, true));
        }
        internal XElement Friends
        {
            get
            {
                
                XElement FriendListElement = new XElement("friend_list");
                foreach(XmlElement Friend in friends["friends"].ChildNodes)
                {
                    long ID = long.Parse(Friend.InnerText);
                    Client OnlineUser = ArrayList.OnlineUsers.Find(Attribute => Attribute.Player.UserID == ID);

                    XElement FriendElement = new XElement("friend");
                    if(OnlineUser != null)
                    {
                        FriendElement.Add(new XAttribute("jid", OnlineUser.JID));
                        FriendElement.Add(new XAttribute("profile_id", OnlineUser.Player.UserID));
                        FriendElement.Add(new XAttribute("nickname", OnlineUser.Player.Nickname));
                        FriendElement.Add(new XAttribute("status", OnlineUser.Status));
                        FriendElement.Add(new XAttribute("experience", OnlineUser.Player.Experience));
                        FriendElement.Add(new XAttribute("location", OnlineUser.Location));
                    }
                    else
                    {
                        Player TargetOffline = new Player()
                        {
                            UserID = ID
                        };
                        if (TargetOffline.Load())
                        {
                            FriendElement.Add(new XAttribute("jid", ""));
                            FriendElement.Add(new XAttribute("profile_id", TargetOffline.UserID));
                            FriendElement.Add(new XAttribute("nickname", TargetOffline.Nickname));
                            FriendElement.Add(new XAttribute("status", 0));
                            FriendElement.Add(new XAttribute("experience", TargetOffline.Experience));
                            FriendElement.Add(new XAttribute("location", ""));
                        }
                        else
                        {
                            FriendElement.Add(new XAttribute("jid", ""));
                            FriendElement.Add(new XAttribute("profile_id", ID));
                            FriendElement.Add(new XAttribute("nickname", $"без_имени_{ID}"));
                            FriendElement.Add(new XAttribute("status", 0));
                            FriendElement.Add(new XAttribute("experience", 0));
                            FriendElement.Add(new XAttribute("location", ""));
                        }
                    }
                    FriendListElement.Add(FriendElement);
                }
                return FriendListElement;
            }
        }
        internal void AddFriend(string ID)
        {
            XmlElement FriendElement = friends.CreateElement("friend");
            FriendElement.InnerText = ID;
            friends["friends"].AppendChild(FriendElement);
        }
        internal void RemoveFriend(string ID)
        {
            foreach(XmlNode Friend in friends["friends"].ChildNodes)
            {
                if (Friend.InnerText == ID)
                    friends["friends"].RemoveChild(Friend);
            }
        }
        internal byte RoomStatus = 0;
        internal XmlDocument Notifications
        {
            get
            {
                foreach (XmlNode Notification in notifications["notifications"].ChildNodes)
                {
                    if (Notification.Attributes["type"].InnerText == "128")
                    {
                        Client OnlineUser = ArrayList.OnlineUsers.Find(Attribute => Attribute.Player.Nickname == Notification["invite_result"].Attributes["nickname"].InnerText);
                        if (OnlineUser != null)
                        {
                            Notification["invite_result"].Attributes["profile_id"].InnerText = OnlineUser.Player.UserID.ToString();
                            Notification["invite_result"].Attributes["jid"].InnerText = OnlineUser.JID;
                            Notification["invite_result"].Attributes["nickname"].InnerText = OnlineUser.Player.Nickname;
                            Notification["invite_result"].Attributes["status"].InnerText = OnlineUser.Status.ToString();
                            Notification["invite_result"].Attributes["location"].InnerText = OnlineUser.Location.ToString();
                            Notification["invite_result"].Attributes["experience"].InnerText = OnlineUser.Player.Experience.ToString();

                        }
                        else
                        {
                            Player TargetOffline = new Player()
                            {
                                Nickname = Notification["invite_result"].Attributes["nickname"].InnerText
                            };
                            TargetOffline.Load();

                            Notification["invite_result"].Attributes["profile_id"].InnerText = TargetOffline.UserID.ToString();
                            Notification["invite_result"].Attributes["jid"].InnerText = "";
                            Notification["invite_result"].Attributes["nickname"].InnerText = TargetOffline.Nickname;
                            Notification["invite_result"].Attributes["status"].InnerText = "0";
                            Notification["invite_result"].Attributes["location"].InnerText = "";
                            Notification["invite_result"].Attributes["experience"].InnerText = TargetOffline.Experience.ToString();


                        }

                    }
                }
                return notifications;
            }
        }
        
        internal Player()
        {
            this.friends.LoadXml("<friends/>");
            this.RandomBox.LoadXml("<randomboxes/>");
            this.Achievements.LoadXml("<achievements/>");
            this.Stats.LoadXml("<stats/>");
            this.notifications.LoadXml("<notifications/>");
        }
        internal byte UnlockedClasses
        {
            get
            {
                byte I = 5;
                if (MedicPassed) I += 8;
                if (EngineerPassed) I += 16;
                return I;
            }
        }
        internal byte TutorialPassed
        {
            get
            {
                byte I = 0;
                if (SoldierPassed) I += 1;
                if (MedicPassed) I += 2;
                if (EngineerPassed) I += 4;
                return I;
            }
        }
        internal byte TutorialSuggest
        {
            get
            {
                byte I = 0;
                if (SoldierSuggest) I += 1;
                if (MedicSuggest) I += 2;
                if (EngineerSuggest) I += 4;
                return I;
            }
        }

        internal void AddMoneyNotification(string Currency, int Amount = 0, string Message = "")
        {
            XElement notifElement = new XElement("notif");
            notifElement.Add(new XAttribute("id", this.Random.Next(999999, Int32.MaxValue)));
            notifElement.Add(new XAttribute("type", 2048));
            notifElement.Add(new XAttribute("confirmation", 1));
            notifElement.Add(new XAttribute("from_jid", "wartls@server"));
            notifElement.Add(new XAttribute("message", Message));

            XElement gamemoneyElement = new XElement("give_money");

            gamemoneyElement.Add(new XAttribute("currency", Currency));
            gamemoneyElement.Add(new XAttribute("type", "1"));
            gamemoneyElement.Add(new XAttribute("amount", Amount));
            gamemoneyElement.Add(new XAttribute("notify", "1"));

            notifElement.Add(gamemoneyElement);

            XmlReader Reader = notifElement.CreateReader();
            Reader.Read();
            XmlDocument Node = new XmlDocument();
            Node.Load(Reader);

            this.Notifications.FirstChild.AppendChild(this.Notifications.ImportNode(Node.DocumentElement, true));
        }

        internal void AddFriendNotification(string Initiator, bool isClan = false)
        {
            XElement notifElement = new XElement("notif");
            notifElement.Add(new XAttribute("id", this.Random.Next(999999, Int32.MaxValue)));
            notifElement.Add(new XAttribute("type", isClan ? 32 : 64));
            notifElement.Add(new XAttribute("confirmation", 1));
            notifElement.Add(new XAttribute("from_jid", "wartls@server"));
            notifElement.Add(new XAttribute("message", ""));

            XElement gamemoneyElement = new XElement("invitation");

            if (isClan)
            {
                gamemoneyElement.Add(new XAttribute("clan_name", Initiator));
                gamemoneyElement.Add(new XAttribute("clan_id", 1));
            }
            else
                gamemoneyElement.Add(new XAttribute("initiator", Initiator));
            gamemoneyElement.Add(new XAttribute("timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds()));
            notifElement.Add(gamemoneyElement);

            XmlReader Reader = notifElement.CreateReader();
            Reader.Read();
            XmlDocument Node = new XmlDocument();
            Node.Load(Reader);

            this.Notifications.FirstChild.AppendChild(this.Notifications.ImportNode(Node.DocumentElement, true));
        }

        internal void AddFriendResultNotification(long ProfileId,string Jid,string Nickname,int Status,string Location,int Experience,string Result)
        {
            XElement notifElement = new XElement("notif");
            notifElement.Add(new XAttribute("id", this.Random.Next(999999, Int32.MaxValue)));
            notifElement.Add(new XAttribute("type", 128));
            notifElement.Add(new XAttribute("confirmation", 1));
            notifElement.Add(new XAttribute("from_jid", "wartls@server"));
            notifElement.Add(new XAttribute("message", ""));

            XElement gamemoneyElement = new XElement("invite_result");

            gamemoneyElement.Add(new XAttribute("profile_id", ProfileId));
            gamemoneyElement.Add(new XAttribute("jid", Jid));
            gamemoneyElement.Add(new XAttribute("nickname", Nickname));
            gamemoneyElement.Add(new XAttribute("status", Status));
            gamemoneyElement.Add(new XAttribute("location", Location));
            gamemoneyElement.Add(new XAttribute("experience", Experience));
            gamemoneyElement.Add(new XAttribute("result", Result));
            gamemoneyElement.Add(new XAttribute("invite_date", 0));

            notifElement.Add(gamemoneyElement);

            XmlReader Reader = notifElement.CreateReader();
            Reader.Read();
            XmlDocument Node = new XmlDocument();
            Node.Load(Reader);

            this.Notifications.FirstChild.AppendChild(this.Notifications.ImportNode(Node.DocumentElement, true));
        }
        internal void AddItemNotification(string OfferType, string Name, int Amount = 0, string Message = "")
        {
            XElement notifElement = new XElement("notif");
            notifElement.Add(new XAttribute("id", this.Random.Next(999999, Int32.MaxValue)));
            notifElement.Add(new XAttribute("type", 256));
            notifElement.Add(new XAttribute("confirmation", 1));
            notifElement.Add(new XAttribute("from_jid", "wartls@server"));
            notifElement.Add(new XAttribute("message", Message));

            XElement gamemoneyElement = new XElement("give_item");

            gamemoneyElement.Add(new XAttribute("name", Name));
            gamemoneyElement.Add(new XAttribute("offer_type", OfferType));
            if (OfferType == "Expiration")
                gamemoneyElement.Add(new XAttribute("extended_time", Amount));
            else if (OfferType == "Consumable")
                gamemoneyElement.Add(new XAttribute("consumables_count", Amount));
            gamemoneyElement.Add(new XAttribute("notify", "1"));

            notifElement.Add(gamemoneyElement);

            XmlReader Reader = notifElement.CreateReader();
            Reader.Read();
            XmlDocument Node = new XmlDocument();
            Node.Load(Reader);

            this.Notifications.FirstChild.AppendChild(this.Notifications.ImportNode(Node.DocumentElement, true));
        }
        internal void AddRandomBoxNotification(string Box, string Message = "")
        {
            XElement notifElement = new XElement("notif");
            notifElement.Add(new XAttribute("id", this.Random.Next(999999, Int32.MaxValue)));
            notifElement.Add(new XAttribute("type", 8192));
            notifElement.Add(new XAttribute("confirmation", 1));
            notifElement.Add(new XAttribute("from_jid", "wartls@server"));
            notifElement.Add(new XAttribute("message", Message));

            XElement gamemoneyElement = new XElement("give_random_box");

            gamemoneyElement.Add(new XAttribute("name", Box));
            gamemoneyElement.Add(new XAttribute("notify", 1));
            XElement PurchasedItem = new XElement("purchased_item");
            foreach(XElement Prize in GeneratePrizes(Box))
                PurchasedItem.Add(Prize);
            gamemoneyElement.Add(PurchasedItem);
            notifElement.Add(gamemoneyElement);
            
            XmlReader Reader = notifElement.CreateReader();
            Reader.Read();
            XmlDocument Node = new XmlDocument();
            Node.Load(Reader);

            this.Notifications.FirstChild.AppendChild(this.Notifications.ImportNode(Node.DocumentElement, true));
        }
        internal void AddRankNotifierNotification(byte OldRank,byte NewRank, string Message = "")
        {
            XElement notifElement = new XElement("notif");
            notifElement.Add(new XAttribute("id", this.Random.Next(999999, Int32.MaxValue)));
            notifElement.Add(new XAttribute("type", 131072));
            notifElement.Add(new XAttribute("confirmation", 1));
            notifElement.Add(new XAttribute("from_jid", "wartls@server"));
            notifElement.Add(new XAttribute("message", Message));

            XElement gamemoneyElement = new XElement("new_rank_reached");

            gamemoneyElement.Add(new XAttribute("old_rank", OldRank));
            gamemoneyElement.Add(new XAttribute("new_rank", NewRank));

            notifElement.Add(gamemoneyElement);

            XmlReader Reader = notifElement.CreateReader();
            Reader.Read();
            XmlDocument Node = new XmlDocument();
            Node.Load(Reader);

            this.Notifications.FirstChild.AppendChild(this.Notifications.ImportNode(Node.DocumentElement, true));
        }
        internal Item AddItem(Item Item)
        {
            foreach(Item ItemInStorage in Items)
            {
                if(ItemInStorage.ItemType == Item.ItemType && ItemInStorage.Name == Item.Name)
                {
                    switch (ItemInStorage.ItemType)
                    {
                        case ItemType.CONSUMABLE:
                            ItemInStorage.Quantity += Item.Quantity;             
                            break;
                        case ItemType.PERMANENT:
                            ItemInStorage.DurabilityPoints += Item.DurabilityPoints;
                            break;
                        case ItemType.TIME:
                            if (ItemInStorage.SecondsLeft <= 0)
                                ItemInStorage.ExpirationTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + Item.SecondsLeft;
                            else
                                ItemInStorage.ExpirationTime = ItemInStorage.ExpirationTime += Item.SecondsLeft;
                            break;
                    }
                    return ItemInStorage;
                }
            }
            Item.ID = this.ItemSeed;
            Items.Add(Item);
            return Item;
        }
        internal void Save()
        {
            XElement Items = new XElement("items");

            foreach (Item Item in this.Items.ToArray())
                Items.Add(Item.Serialize());
            this.LastSeen = DateTime.Now;
            new SqlCommand($"UPDATE players SET LastActivity='{LastSeen.ToString("yyyy-MM-ddTHH:mm:ss")}',Friends='{this.friends.InnerXml}',Notifications='{this.Notifications.InnerXml}',Achievements='{this.Achievements.InnerXml}',Stats='{this.Stats.InnerXml}',Settings='{this.Settings.InnerXml}',Token='{this.Token}',Nickname='{this.Nickname}',Experience='{this.Experience}', Avatar='{this.Avatar.ToString(SaveOptions.DisableFormatting)}',Items='{Items.ToString(SaveOptions.DisableFormatting)}',RandomBox='{this.RandomBox.InnerXml}' WHERE  ID={this.UserID};", SQL.Handler).ExecuteNonQuery();
        }
        internal XElement Avatar
        {
            get
            {
                XElement AvatarElement = new XElement("profile");
                AvatarElement.Add(new XAttribute("gender", Gender));
                AvatarElement.Add(new XAttribute("height", Height));
                AvatarElement.Add(new XAttribute("fatness", Fatness));
                AvatarElement.Add(new XAttribute("head", Head));
                AvatarElement.Add(new XAttribute("current_class", CurrentClass));
                AvatarElement.Add(new XAttribute("banner_mark", BannerMark));
                AvatarElement.Add(new XAttribute("banner_stripe", BannerStripe));
                AvatarElement.Add(new XAttribute("banner_badge", BannerBadge));
                AvatarElement.Add(new XAttribute("game_money", GameMoney));
                AvatarElement.Add(new XAttribute("cry_money", CryMoney));
                AvatarElement.Add(new XAttribute("crown_money", CrownMoney));
                AvatarElement.Add(new XAttribute("medic_tutorial_passed", MedicPassed));
                AvatarElement.Add(new XAttribute("engineer_tutorial_passed", EngineerPassed));
                AvatarElement.Add(new XAttribute("soldier_tutorial_passed", SoldierPassed));
                AvatarElement.Add(new XAttribute("soldier_tutorial_suggest", SoldierSuggest));
                AvatarElement.Add(new XAttribute("engineer_tutorial_suggest", EngineerSuggest));
                AvatarElement.Add(new XAttribute("medic_tutorial_suggest", MedicSuggest));
                AvatarElement.Add(new XAttribute("unlocked_missions", UnlockedMissions));

                return AvatarElement;
            }
        }
        internal List<XElement> GeneratePrizes(string BoxName,int OfferId=0)
        {
            int Opened = -1;
            foreach(XmlNode OpenedBox in RandomBox.FirstChild.ChildNodes)
            {
                if (OpenedBox.Attributes["name"].InnerText == BoxName)
                {
                    Opened = int.Parse(OpenedBox.Attributes["opened"].InnerText);
                    break;
                }
            }
            List<XElement> PrizesWinned = new List<XElement>();
            XmlDocument BoxXML = new XmlDocument();
            try
            {
                BoxXML.Load($"Gamefiles/ShopItems/{BoxName}.xml");
            }
            catch
            {
                
                return new List<XElement>();
            }
            byte Index = 0;
            foreach (XmlNode GroupXML in BoxXML["shop_item"]["random_box"].ChildNodes)
            {
                SortedDictionary<double, XmlNode> Chances = new SortedDictionary<double, XmlNode>();
                List<XmlNode> PermanentItems = new List<XmlNode>();
                List<XmlNode> GoldItems = new List<XmlNode>();
                List<XmlNode> OtherItems = new List<XmlNode>();


                foreach (XmlNode Group in GroupXML.ChildNodes)
                {
                    if (Group.Attributes["name"].InnerText.Contains("smugglers_card")) continue;
                    //Карты черного рынка не поддерживаются!
                    if (Group.Attributes["name"].InnerText.Contains("gold"))
                        GoldItems.Add(Group);
                    else if (Group.Attributes["amount"] == null && Group.Attributes["expiration"] == null)
                        PermanentItems.Add(Group);
                    OtherItems.Add(Group);
                    do
                    {
                        try
                        {
                            if (Group.Attributes["weight"].InnerText.Length == 1)
                                Chances.Add(double.Parse($"0.{Group.Attributes["weight"].InnerText}", CultureInfo.InvariantCulture), Group);
                            if (Group.Attributes["weight"].InnerText.Length == 2)
                                Chances.Add(double.Parse($"{Group.Attributes["weight"].InnerText.Substring(0, 1)}.{Group.Attributes["weight"].InnerText.Substring(1)}", CultureInfo.InvariantCulture), Group);
                            if (Group.Attributes["weight"].InnerText.Length == 3)
                                Chances.Add(double.Parse($"{Group.Attributes["weight"].InnerText.Substring(0, 2)}.{Group.Attributes["weight"].InnerText.Substring(2)}", CultureInfo.InvariantCulture), Group);
                            break;
                        }
                        catch (ArgumentException)
                        {
                            Group.Attributes["weight"].InnerText = (Int32.Parse(Group.Attributes["weight"].InnerText) + 4).ToString();
                        }
                    } while (true);
                }
                double MinChance = Chances.FirstOrDefault(x => true).Key;
                double MaxChance = Chances.LastOrDefault(x => true).Key;
                double Randomize = this.Random.NextDouble(0, MaxChance);
                Console.WriteLine(Randomize);
                List<XmlNode> PotentialPrizes = new List<XmlNode>();
                foreach (double _Item in Chances.Keys)
                {
                    if (_Item >= Randomize)
                        //if (Chances[_Item].Attributes["amount"] != null || Chances[_Item].Attributes["expiration"] != null)
                            PotentialPrizes.Add(Chances[_Item]);
                }
                bool NoOthers = false;
                if (PotentialPrizes.Count == 0)
                {
                    NoOthers = true;
                    foreach (double _Item in Chances.Keys)
                    {
                        if (_Item > Randomize)
                            PotentialPrizes.Add(Chances[_Item]);
                    }
                }
                XmlNode Prize = PotentialPrizes[0];
                Item Item = null;

                int Hours = -1;


                if (GoldItems.Count >0 && Opened % 1000 == 0 && Index == 0 )//Условие выпадения - золото
                {
                    Prize = GoldItems[Random.Next(0, GoldItems.Count)];
                    Item = AddItem(new Item(ItemType.PERMANENT, ItemSeed, Prize.Attributes["name"].InnerText, 0, 0, 36000));

                    XElement PermanentItemXML = new XElement("profile_item");
                    PermanentItemXML.Add(new XAttribute("name", Item.Name));
                    PermanentItemXML.Add(new XAttribute("profile_item_id", Item.ID));
                    PermanentItemXML.Add(new XAttribute("offerId", OfferId));
                    PermanentItemXML.Add(new XAttribute("added_expiration", 0));
                    PermanentItemXML.Add(new XAttribute("added_quantity", 0));
                    PermanentItemXML.Add(new XAttribute("error_status", "0"));
                    PermanentItemXML.Add(Item.Serialize());

                    PrizesWinned.Add(PermanentItemXML);

                }

                else if(Prize.Attributes["name"].InnerText == "game_money_item_01")
                {
                    this.GameMoney += int.Parse(Prize.Attributes["amount"].InnerText);

                    XElement profileItemElement = new XElement("game_money");
                    profileItemElement.Add(new XAttribute("name", "game_money_item_01"));
                    profileItemElement.Add(new XAttribute("added", Prize.Attributes["amount"].InnerText));
                    profileItemElement.Add(new XAttribute("total", this.Experience));
                    profileItemElement.Add(new XAttribute("offerId", OfferId));

                    PrizesWinned.Add(profileItemElement);
                }

                else if(Prize.Attributes["name"].InnerText == "exp_item_01")
                {
                    this.Experience += int.Parse(Prize.Attributes["amount"].InnerText);

                    XElement profileItemElement = new XElement("exp");
                    profileItemElement.Add(new XAttribute("name", "exp_item_01"));
                    profileItemElement.Add(new XAttribute("added", Prize.Attributes["amount"].InnerText));
                    profileItemElement.Add(new XAttribute("total", this.Experience));
                    profileItemElement.Add(new XAttribute("offerId", OfferId));

                    PrizesWinned.Add(profileItemElement);
                }
                else
                {

                    if (Prize.Attributes["expiration"] != null)
                    {
                        Hours = int.Parse(new Regex("[0-9]*").Match(Prize.Attributes["expiration"].InnerText).Value);
                        if (Prize.Attributes["expiration"].InnerText.Contains("d"))
                        {
                            Hours = (int)TimeSpan.FromDays(Hours).TotalHours;
                            Item = new Item(ItemType.TIME, ItemSeed, Prize.Attributes["name"].InnerText, Hours);
                        }
                        if (Prize.Attributes["expiration"].InnerText.Contains("h"))
                            Item = new Item(ItemType.TIME, ItemSeed, Prize.Attributes["name"].InnerText, Hours);
                    }
                    else if(Prize.Attributes["amount"] == null && Prize.Attributes["expiration"] == null)
                        Item = new Item(ItemType.PERMANENT, ItemSeed, Prize.Attributes["name"].InnerText, 0, 0);
                    else if (Prize.Attributes["amount"] != null)
                        Item = new Item(ItemType.CONSUMABLE, ItemSeed, Prize.Attributes["name"].InnerText, 0, int.Parse(Prize.Attributes["amount"].InnerText), 0);
                    if (Item != null)
                        Item = this.AddItem(Item);
                    
                    XElement profileItemElement = new XElement("profile_item");
                    profileItemElement.Add(new XAttribute("name", Item.Name));
                    profileItemElement.Add(new XAttribute("profile_item_id", Item.ID));
                    profileItemElement.Add(new XAttribute("offerId", OfferId));
                    profileItemElement.Add(new XAttribute("added_expiration", Prize.Attributes["expiration"] != null ? Prize.Attributes["expiration"].InnerText : ""));
                    profileItemElement.Add(new XAttribute("added_quantity", Prize.Attributes["amount"] != null ? Prize.Attributes["amount"].InnerText : ""));
                    profileItemElement.Add(new XAttribute("error_status", "0"));
                    profileItemElement.Add(Item.Serialize());

                    PrizesWinned.Add(profileItemElement);
                }
                Index++;
            }
            
            XmlAttribute OpenedAttrib = RandomBox.CreateAttribute("opened"); OpenedAttrib.Value = Opened.ToString();
            if (Opened == -1)
            {
                OpenedAttrib.Value = "1";
                XmlAttribute NameAttrib = RandomBox.CreateAttribute("name"); NameAttrib.Value = BoxName;
                XmlElement BoxElement = this.RandomBox.CreateElement("box");
                BoxElement.Attributes.Append(NameAttrib);
                BoxElement.Attributes.Append(OpenedAttrib);
                RandomBox.FirstChild.AppendChild(BoxElement);
            }
            else
            {
                Opened++;
                foreach (XmlNode OpenedBox in RandomBox.FirstChild.ChildNodes)
                {
                    if (OpenedBox.Attributes["name"].InnerText == BoxName)
                    {
                        OpenedBox.Attributes["opened"].InnerText = Opened.ToString();
                        break;
                    }
                }
            }

            return PrizesWinned;
        }

        internal bool Load()
        {
            SqlCommand Command = new SqlCommand();
            Command.Connection = SQL.Handler;
            if(this.UserID != -1)
                Command.CommandText = $"SELECT * FROM players WHERE ID='{this.UserID}';";
            else
                Command.CommandText = $"SELECT * FROM players WHERE Nickname='{this.Nickname}';";

            using (SqlDataReader Reader = Command.ExecuteReader())
            {
                if (!Reader.HasRows) { Reader.Close(); return false; }
                Reader.Read();

                this.UserID = Reader.GetInt64(0);
                this.Nickname = Reader.GetString(1);
                if (this.Nickname == "" || this.Nickname == null)
                    return true;

                this.Experience = Reader.GetInt32(2);
                this.OldRank = Rank;
                using (XmlReader AvatarXML = Reader.GetXmlReader(3))
                {
                    AvatarXML.Read();
                    this.Gender = AvatarXML["gender"].ToString();
                    this.Head = AvatarXML["head"].ToString();
                    this.GameMoney = int.Parse(AvatarXML["game_money"].ToString());
                    this.CrownMoney = int.Parse(AvatarXML["crown_money"].ToString());
                    this.CryMoney = int.Parse(AvatarXML["cry_money"].ToString());
                    this.Fatness = double.Parse(AvatarXML["fatness"].ToString(), CultureInfo.InvariantCulture);
                    this.Height = double.Parse(AvatarXML["height"].ToString(), CultureInfo.InvariantCulture);
                    this.CurrentClass = byte.Parse(AvatarXML["current_class"].ToString());
                    this.BannerBadge = int.Parse(AvatarXML["banner_badge"].ToString());
                    this.BannerMark = int.Parse(AvatarXML["banner_mark"].ToString());
                    this.BannerStripe = int.Parse(AvatarXML["banner_stripe"].ToString());
                    this.UnlockedMissions = AvatarXML["unlocked_missions"].ToString();

                    this.SoldierPassed = true; // AvatarXML["soldier_tutorial_passed"].ToString() == "true" ? true:false ;
                    this.MedicPassed = true;//AvatarXML["medic_tutorial_passed"].ToString() == "true" ? true : false;
                    this.EngineerPassed = true;//AvatarXML["engineer_tutorial_passed"].ToString() == "true" ? true : false;
                    this.SoldierSuggest = true;//AvatarXML["soldier_tutorial_suggest"].ToString() == "true" ? true : false;
                    this.MedicSuggest = true;//AvatarXML["medic_tutorial_suggest"].ToString() == "true" ? true : false;
                    this.EngineerSuggest = true;//AvatarXML["engineer_tutorial_suggest"].ToString() == "true" ? true : false;

                }
                LastSeen = Reader.GetDateTime(12);
                using (XmlReader ItemsXMLReader = Reader.GetXmlReader(4))
                {
                    this.Items = new List<Item>();
                    XmlDocument ItemsXML = new XmlDocument();
        
                        ItemsXMLReader.Read();
                        ItemsXML.Load(ItemsXMLReader);

                        foreach (XmlElement ItemXML in ItemsXML["items"].ChildNodes)
                        {
                            Item Item = new Item();
                            Item.Create(ItemXML);
                            this.Items.Add(Item);
                        }
                }
                using (XmlReader ConfigXMLReader = Reader.GetXmlReader(5))
                {
                    ConfigXMLReader.Read();
                    this.Settings.Load(ConfigXMLReader);
                }
                using (XmlReader ConfigXMLReader = Reader.GetXmlReader(7))
                {
                    ConfigXMLReader.Read();
                    this.Achievements.Load(ConfigXMLReader);
                }
                using (XmlReader ConfigXMLReader = Reader.GetXmlReader(8))
                {
                    ConfigXMLReader.Read();
                    this.notifications.Load(ConfigXMLReader);
                }
                using (XmlReader ConfigXMLReader = Reader.GetXmlReader(9))
                {
                    ConfigXMLReader.Read();
                    this.Stats.Load(ConfigXMLReader);
                }
                using (XmlReader ConfigXMLReader = Reader.GetXmlReader(11))
                {
                    ConfigXMLReader.Read();
                    this.RandomBox.Load(ConfigXMLReader);
                }
                using (XmlReader ConfigXMLReader = Reader.GetXmlReader(10))
                {
                    ConfigXMLReader.Read();
                    this.friends.Load(ConfigXMLReader);
                }
                Reader.Close();
            }
            return true;
        }
    }
    public static class RandomExtensions
    {
        public static double NextDouble(
            this Random random,
            double minValue,
            double maxValue)
        {
            return random.NextDouble() * (maxValue - minValue) + minValue;
        }
    }
}
