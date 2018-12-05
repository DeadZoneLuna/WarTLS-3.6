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
    class ChannelOperation : Stanza
    {
        string Version;
        string Resource;
        string RegionId;
        string BuildType;
        enum Error
        {
            NO=-1,
            PROFILE_NOT_EXIST=1,
            INVALID_GAMEVERSION=2,
            BANNED=3,
            FULL_CHANNEL=5
        }
        Error ErrorCode = Error.NO;
        public ChannelOperation(Client User, XmlDocument Packet) : base(User, Packet)
        {
            if (User.Channel != null)
                new ChannelLogout(User);
            //Version = base.Query.Attributes["version"].InnerText;
            if ((bool)App.Default["UseOldMode"])
                Resource = base.To.Split(new[] { "masterserver@warface/" }, StringSplitOptions.RemoveEmptyEntries)[0];
            else
                Resource = base.Query.Attributes["resource"].InnerText;

            //RegionId = base.Query.Attributes["region_id"].InnerText;
            BuildType = base.Query.Attributes["build_type"].InnerText;

            if ((bool)App.Default["UseOldMode"])
                User.Channel = ArrayList.Channels.Find(Attribute => Attribute.Resource == base.To.Split(new[] { "masterserver@warface/" }, StringSplitOptions.RemoveEmptyEntries)[0]);
            else
                User.Channel = ArrayList.Channels.Find(Attribute => Attribute.Resource == Resource);
            if (User.Channel.MinRank >= User.Player.Rank && User.Channel.MaxRank <= User.Player.Rank )
            {
                ErrorCode = Error.FULL_CHANNEL;
                return;
            }
            else
                User.Channel.Users.Add(User);

            if (base.Query.Name == "create_profile" && !User.Player.ProfileCreated)
            {
                User.Player.Nickname = base.Query.Attributes["nickname"].InnerText;
                User.Player.Head = base.Query.Attributes["head"].InnerText.StartsWith("default_head_") ? base.Query.Attributes["head"].InnerText : "default_head_04";
                User.Player.Save();
            }
            
            Process();
            new FriendList(User).Process();
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

            XElement joinChannelElement = new XElement(base.Query.Name);
            
            if (ErrorCode == Error.NO)
            {
                joinChannelElement.Add(new XAttribute("profile_id", User.Player.UserID));
                XElement characterElement = new XElement("character");
                characterElement.Add(new XAttribute("nick", User.Player.Nickname));
                characterElement.Add(new XAttribute("gender", User.Player.Gender));
                characterElement.Add(new XAttribute("height", User.Player.Height));
                characterElement.Add(new XAttribute("fatness", User.Player.Fatness));
                characterElement.Add(new XAttribute("head", User.Player.Head));
                characterElement.Add(new XAttribute("current_class", User.Player.CurrentClass));
                characterElement.Add(new XAttribute("experience", User.Player.Experience));
                characterElement.Add(new XAttribute("pvp_rating", "0"));
                characterElement.Add(new XAttribute("pvp_rating_points", "0"));
                characterElement.Add(new XAttribute("banner_badge", User.Player.BannerBadge));
                characterElement.Add(new XAttribute("banner_mark", User.Player.BannerMark));
                characterElement.Add(new XAttribute("banner_stripe", User.Player.BannerStripe));
                characterElement.Add(new XAttribute("game_money", User.Player.GameMoney));
                characterElement.Add(new XAttribute("cry_money", User.Player.CryMoney));
                characterElement.Add(new XAttribute("crown_money", User.Player.CrownMoney));

                XElement SponsorInfo = new XElement("sponsor_info");
                SponsorInfo.Add(new XElement("sponsor",
                    new XAttribute("sponsor_id", "0"),
                    new XAttribute("sponsor_points", "0"),
                    new XAttribute("next_unlock_item", "mg06_shop")
                    ));
                SponsorInfo.Add(new XElement("sponsor",
                    new XAttribute("sponsor_id", "1"),
                    new XAttribute("sponsor_points", "0"),
                    new XAttribute("next_unlock_item", "sniper_vest_02")

                    ));
                SponsorInfo.Add(new XElement("sponsor",
                    new XAttribute("sponsor_id", "2"),
                    new XAttribute("sponsor_points", "0"),
                    new XAttribute("next_unlock_item", "ss04")

        ));
                if (User.Player.Notifications.FirstChild.ChildNodes.Count > 0)
                {
                    foreach (XmlNode Notification in User.Player.Notifications.FirstChild.ChildNodes)
                        characterElement.Add(XDocument.Parse(Notification.OuterXml).Root);
                }
                characterElement.Add(new XElement("login_bonus",
                    new XAttribute("current_streak", "1"),
                    new XAttribute("current_reward", "0")));
                XElement ProfileProgressionState = new XElement("profile_progression_state");
                ProfileProgressionState.Add(new XAttribute("profile_id", User.Player.UserID));
                ProfileProgressionState.Add(new XAttribute("mission_unlocked", User.Player.UnlockedMissions));
                ProfileProgressionState.Add(new XAttribute("tutorial_unlocked", User.Player.TutorialSuggest));
                ProfileProgressionState.Add(new XAttribute("tutorial_passed", User.Player.TutorialPassed));
                ProfileProgressionState.Add(new XAttribute("class_unlocked", User.Player.UnlockedClasses));

                XElement ChatChannels = new XElement("chat_channels");
                XElement ChatChannel = new XElement("chat");
                ChatChannel.Add(new XAttribute("channel", 0));
                ChatChannel.Add(new XAttribute("channel_id", User.Channel.JID));
                ChatChannel.Add(new XAttribute("service_id", "conference.warface"));

                ChatChannels.Add(ChatChannel);

                foreach (Item Item in User.Player.Items)
                {
                    if (Item.ItemType==ItemType.TIME && Item.SecondsLeft <= 0 && !Item.ExpiredConfirmed)
                    {
                        characterElement.Add(new XElement("expired_item",
                            new XAttribute("id", Item.ID),
                            new XAttribute("name", Item.Name),
                            new XAttribute("slot_ids", Item.Slot)
                        ));
                        Item.ExpiredConfirmed = true;
                    }
                    characterElement.Add(Item.Serialize());
                }
                characterElement.Add(ChatChannels);
                characterElement.Add(SponsorInfo);
                characterElement.Add(ProfileProgressionState);
                characterElement.Add(GameResources.OnlineVariables.ToXDocument().FirstNode);
                joinChannelElement.Add(characterElement);
                queryElement.Add(joinChannelElement);
                iqElement.Add(queryElement);
            }
            else
            {

                XElement errorElement = new XElement((XNamespace)"urn:ietf:params:xml:ns:xmpp-stanzas" + "error");
                errorElement.Add(new XAttribute("type", "cancel"));
                errorElement.Add(new XAttribute("code", 8));
                errorElement.Add(new XAttribute("custom_code", (int)ErrorCode));
                queryElement.Add(joinChannelElement);

                iqElement.Add(queryElement);
                iqElement.Add(errorElement);

            }

            
            Packet.Add(iqElement);
            base.Compress(ref Packet);
            User.Player.Save();
            User.Send(Packet.ToString(SaveOptions.DisableFormatting));
        }
    }
}
