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
    class ResyncProfile : Stanza
    {
        string Channel;

        public ResyncProfile(Client User, XmlDocument Packet) : base(User, Packet)
        {


            Process();
        }
        internal override void Process()
        {
            if (base.Type == "result") return;
            XDocument Packet = new XDocument();

            XElement iqElement = new XElement(Gateway.JabberNS + "iq");
            iqElement.Add(new XAttribute("type", base.Type=="get" ? "result" : "get"));
            iqElement.Add(new XAttribute("from", base.To));
            iqElement.Add(new XAttribute("to", User.JID));
            iqElement.Add(new XAttribute("id", base.Type == "get" ? base.Id : $"uid{User.Player.Random.Next(9999,Int32.MaxValue).ToString("x8")}"));

            XElement queryElement = new XElement(Stanza.NameSpace + "query");

            XElement accountElement = new XElement("resync_profile");

            foreach (Item Item in User.Player.Items)
                accountElement.Add(Item.Serialize());

            XElement moneyElement = new XElement("money");
            moneyElement.Add(new XAttribute("cry_money", User.Player.CryMoney));
            moneyElement.Add(new XAttribute("crown_money", User.Player.CrownMoney));
            moneyElement.Add(new XAttribute("game_money", User.Player.GameMoney));

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

            XElement ProfileProgressionState = new XElement("profile_progression_state");
            ProfileProgressionState.Add(new XAttribute("profile_id", User.Player.UserID));
            ProfileProgressionState.Add(new XAttribute("mission_unlocked", User.Player.UnlockedMissions));
            ProfileProgressionState.Add(new XAttribute("tutorial_unlocked", User.Player.TutorialSuggest));
            ProfileProgressionState.Add(new XAttribute("tutorial_passed", User.Player.TutorialPassed));
            ProfileProgressionState.Add(new XAttribute("class_unlocked", User.Player.UnlockedClasses));
            XElement ProgressionState = new XElement("profile_progression_state");
            ProgressionState.Add(ProfileProgressionState);
            queryElement.Add(accountElement);
            queryElement.Add(moneyElement);
            queryElement.Add(characterElement);
            queryElement.Add(ProgressionState);

            iqElement.Add(queryElement);

            Packet.Add(iqElement);
            base.Compress(ref Packet);
            User.Send(Packet.ToString(SaveOptions.DisableFormatting));
        }
    }
}
