using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using WARTLS.CLASSES.GAMEROOM;

namespace WARTLS.CLASSES
{
    class Channel
    {
        internal string Resource;
        internal ushort ServerId;
        internal string ChannelType;
        internal string RankGroup = "all";
        internal double Load => Users.Count / 53;
        internal int Online => Users.Count;
        internal byte MinRank = 1;
        internal byte MaxRank = 90;
        internal string JID => $"global.{Resource}";
        internal string Bootstrap = "";
        internal List<GameRoom> GameRoomList = new List<GameRoom>();
        internal List<Client> Users = new List<Client>();
        internal Channel(string Resource,ushort ServerId,string ChannelType,byte MinRank,byte MaxRank)
        {
            this.Resource = Resource;
            this.ServerId = ServerId;
            this.ChannelType = ChannelType;
            this.MinRank = MinRank;
            this.MaxRank = MaxRank;
        }
        internal XElement Serialize()
        {
            XElement serverElement = new XElement("server");
            serverElement.Add(new XAttribute("resource", Resource));
            serverElement.Add(new XAttribute("server_id", ServerId));
            serverElement.Add(new XAttribute("channel", ChannelType));
            serverElement.Add(new XAttribute("rank_group", RankGroup));
            serverElement.Add(new XAttribute("load", Load));
            serverElement.Add(new XAttribute("online", Online));
            serverElement.Add(new XAttribute("min_rank", MinRank));
            serverElement.Add(new XAttribute("max_rank", MaxRank));
            serverElement.Add(new XAttribute("bootstrap", Bootstrap));

            XElement loadStats = new XElement("load_stats");

            XElement quickplayStat = new XElement("load_stat");
            quickplayStat.Add(new XAttribute("type", "quick_play"));
            quickplayStat.Add(new XAttribute("value", "255"));

            XElement survivalStat = new XElement("load_stat");
            survivalStat.Add(new XAttribute("type", "survival"));
            survivalStat.Add(new XAttribute("value", "255"));

            XElement pveStat = new XElement("load_stat");
            pveStat.Add(new XAttribute("type", "pve"));
            pveStat.Add(new XAttribute("value", "255"));

            loadStats.Add(quickplayStat);
            loadStats.Add(survivalStat);
            loadStats.Add(pveStat);

            serverElement.Add(loadStats);
            return serverElement;
        }
    }
}
