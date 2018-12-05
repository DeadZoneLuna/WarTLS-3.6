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
    class TutorialStatus : Stanza
    {
        byte Event;
        string ID;
        public TutorialStatus(Client User, XmlDocument Packet) : base(User, Packet)
        {
            Event = byte.Parse(base.Query.Attributes["event"].InnerText);
            ID = base.Query.Attributes["id"].InnerText;
            if (Event != 3) return;

            foreach(XmlDocument Map in GameResources.Maps)
            {
                if (Map.FirstChild.Attributes["uid"].InnerText == ID)
                {
                    switch (Map.FirstChild.Attributes["name"].InnerText)
                    {
                        case "@name_tutorial_soldier":
                        case "@name_tutorial_medic":
                        case "@name_tutorial_engineer":

                            string EventName = Map.FirstChild.Attributes["name"].InnerText == "@name_tutorial_soldier" ? "tutorial_1_completed" : Map.FirstChild.Attributes["name"].InnerText == "@name_tutorial_medic" ? "tutorial_2_completed" : "tutorial_3_completed";
                            if (Map.FirstChild.Attributes["name"].InnerText == "@name_tutorial_soldier" && !User.Player.SoldierPassed)
                            {
                                User.Player.SoldierPassed = true;
                            }
                            else if (Map.FirstChild.Attributes["name"].InnerText == "@name_tutorial_medic" && !User.Player.MedicPassed)
                            {
                                User.Player.MedicPassed = true;
                            }
                            else if (Map.FirstChild.Attributes["name"].InnerText == "@name_tutorial_engineer" && !User.Player.EngineerPassed)
                            {
                                User.Player.EngineerPassed = true;
                            }
                            else
                                break;
                            foreach (XmlNode RewardInfo in GameResources.Configs["items"]["special_reward_configuration"].ChildNodes)
                            {
                                if(RewardInfo.Attributes["name"].InnerText== EventName)
                                {
                                    foreach(XmlNode Reward in RewardInfo.ChildNodes)
                                    {
                                        if (Reward.Name == "money")
                                            User.Player.AddMoneyNotification(Reward.Attributes["currency"].InnerText, int.Parse(Reward.Attributes["amount"].InnerText));
                                        if (Reward.Name == "item")
                                            User.Player.AddItemNotification(Reward.Attributes["expiration"] != null ? "Expiration" : Reward.Attributes["amount"] != null ? "Consumable":"Permanent", Reward.Attributes["name"].InnerText, Reward.Attributes["expiration"] != null ? int.Parse(new Regex("[0-9]*").Match(Reward.Attributes["expiration"].InnerText).Value)*(Reward.Attributes["expiration"].InnerText.Contains("d") ? 24:1) : Reward.Attributes["amount"] != null ? int.Parse(Reward.Attributes["amount"].InnerText) : 0);

                                        switch (Reward.Name)
                                        {
                                            case "money":
                                                switch (Reward.Attributes["currency"].InnerText)
                                                {
                                                    case "game_money":
                                                        User.Player.GameMoney += int.Parse(Reward.Attributes["amount"].InnerText);
                                                        break;
                                                    case "cry_money":
                                                        User.Player.CryMoney += int.Parse(Reward.Attributes["amount"].InnerText);
                                                        break;
                                                    case "crown_money":
                                                        User.Player.CrownMoney += int.Parse(Reward.Attributes["amount"].InnerText);
                                                        break;

                                                }
                                                break;
                                            case "item":
                                                User.Player.AddItem(new Item(Reward.Attributes["expiration"] != null ? ItemType.TIME : Reward.Attributes["amount"] != null ? ItemType.CONSUMABLE : ItemType.PERMANENT, User.Player.ItemSeed, Reward.Attributes["name"].InnerText, Reward.Attributes["expiration"] != null ? int.Parse(new Regex("[0-9]*").Match(Reward.Attributes["expiration"].InnerText).Value) * (Reward.Attributes["expiration"].InnerText.Contains("d") ? 24 : 1) : 0, Reward.Attributes["amount"] != null ? int.Parse(Reward.Attributes["amount"].InnerText) : 0));
                                                break;
                                        }
                                    }
                                }
                            }
                            break;
                    }
                    
                }
            }

            Process();
            new SyncNotification(User).Process();
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

            XElement accountElement = new XElement("tutorial_status");

            XElement ProfileProgressionState = new XElement("profile_progression_update");
            ProfileProgressionState.Add(new XAttribute("profile_id", User.Player.UserID));
            ProfileProgressionState.Add(new XAttribute("mission_unlocked", User.Player.UnlockedMissions));
            ProfileProgressionState.Add(new XAttribute("tutorial_unlocked", User.Player.TutorialSuggest));
            ProfileProgressionState.Add(new XAttribute("tutorial_passed", User.Player.TutorialPassed));
            ProfileProgressionState.Add(new XAttribute("class_unlocked", User.Player.UnlockedClasses));

            accountElement.Add(ProfileProgressionState);
            queryElement.Add(accountElement);
            iqElement.Add(queryElement);

            Packet.Add(iqElement);
            
            User.Send(Packet.ToString(SaveOptions.DisableFormatting));
        }
    }
}
