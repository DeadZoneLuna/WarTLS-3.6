using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WARTLS.CLASSES
{
    class Clan
    {
        internal long ID = 1;
        internal string Name;
        internal string Description;
        internal long CreationTime;
        internal Client Master;
        internal int Points=0;
        internal XElement Serialize()
        {
            XElement accountElement = new XElement("clan");
            accountElement.Add(new XAttribute("name", Name));
            accountElement.Add(new XAttribute("clan_id", this.ID));
            accountElement.Add(new XAttribute("master", Master.Player.UserID));
            accountElement.Add(new XAttribute("clan_points", Points));
            accountElement.Add(new XAttribute("creation_date", CreationTime));
            accountElement.Add(new XAttribute("master_badge", Master.Player.BannerBadge));
            accountElement.Add(new XAttribute("master_stripe", Master.Player.BannerStripe));
            accountElement.Add(new XAttribute("master_mark", Master.Player.BannerMark));
            accountElement.Add(new XAttribute("leaderboard_position", "1"));

            XElement memberElement = new XElement("clan_member_info");
            memberElement.Add(new XAttribute("nickname", Master.Player.Nickname));
            memberElement.Add(new XAttribute("profile_id", this.ID));
            memberElement.Add(new XAttribute("experience", Master.Player.Experience));
            memberElement.Add(new XAttribute("clan_points", 0));
            memberElement.Add(new XAttribute("invite_date", 1));
            memberElement.Add(new XAttribute("clan_role", 1));
            memberElement.Add(new XAttribute("jid", Master.JID));
            memberElement.Add(new XAttribute("status", Master.Status));
            accountElement.Add(memberElement);
            return accountElement;
        }
        internal Clan(string Name,string Description,Client Master)
        {
            this.CreationTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            this.Name = Name;
            this.Description = Description;
            this.Master = Master;
        }
    }
}
