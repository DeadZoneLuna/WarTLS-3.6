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
    class GetMasterServer : Stanza
    {
        string Channel;
        string[] UsedResources = new string[0];
        public GetMasterServer(Client User, XmlDocument Packet) : base(User, Packet)
        {
            Channel = base.Query.Attributes["channel"].InnerText;

            if (base.Query.Attributes["used_resources"] != null)
                UsedResources = base.Query.Attributes["used_resources"].InnerText.Split(new[] { ';' },StringSplitOptions.RemoveEmptyEntries);
            Process();
        }
        internal override void Process()
        {
            Channel SelectedChannel=null;
            bool isSelected = false;

            foreach (Channel Channel in ArrayList.Channels) { 
                if (Channel.ChannelType == this.Channel&& !UsedResources.Contains(Channel.Resource))
                
                    if (Channel.MinRank <= User.Player.Rank && Channel.MaxRank >= User.Player.Rank && Channel.Load < 1)
                    {
                        isSelected = true;
                        SelectedChannel = Channel;
                        break;
                    }
                }
            if (!isSelected)
                foreach (Channel Channel in ArrayList.Channels)
                {
                    if (Channel.ChannelType == "pvp_pro" && !UsedResources.Contains(Channel.Resource))
                        if (Channel.MinRank <= User.Player.Rank && Channel.MaxRank >= User.Player.Rank && Channel.Load < 1)
                        {
                            isSelected = true;
                            SelectedChannel = Channel;
                            break;
                        }
                }
            if (!isSelected)
                foreach (Channel Channel in ArrayList.Channels)
                {
                    if (Channel.ChannelType == "pvp_skilled" && !UsedResources.Contains(Channel.Resource))
                        if (Channel.MinRank <= User.Player.Rank && Channel.MaxRank >= User.Player.Rank && Channel.Load < 1)
                        {
                            isSelected = true;
                            SelectedChannel = Channel;
                            break;
                        }
                }
            if (!isSelected)
                foreach (Channel Channel in ArrayList.Channels)
                {
                    if (Channel.ChannelType == "pvp_newbie" && !UsedResources.Contains(Channel.Resource))
                        if (Channel.MinRank <= User.Player.Rank && Channel.MaxRank >= User.Player.Rank && Channel.Load < 1)
                        {
                            isSelected = true;
                            SelectedChannel = Channel;
                            break;
                        }
                }
            if (!isSelected)
                foreach (Channel Channel in ArrayList.Channels)
                {
                    if (Channel.ChannelType == "pve" && !UsedResources.Contains(Channel.Resource))
                        if (Channel.MinRank <= User.Player.Rank && Channel.MaxRank >= User.Player.Rank && Channel.Load < 1)
                        {
                            isSelected = true;
                            SelectedChannel = Channel;
                            break;
                        }
                }
            
            User.Channel = SelectedChannel;
            XDocument Packet = new XDocument();

            XElement iqElement = new XElement(Gateway.JabberNS + "iq");
            iqElement.Add(new XAttribute("type", "result"));
            iqElement.Add(new XAttribute("from", base.To));
            iqElement.Add(new XAttribute("to", User.JID));
            iqElement.Add(new XAttribute("id", base.Id));

            XElement queryElement = new XElement(Stanza.NameSpace + "query");

            XElement accountElement = new XElement("get_master_server");
            if(SelectedChannel != null)
                accountElement.Add(new XAttribute("resource", SelectedChannel.Resource));
            accountElement.Add(new XAttribute("load_index", "255"));

            queryElement.Add(accountElement);
            iqElement.Add(queryElement);

            if (SelectedChannel == null)
            {
                XElement errorElement = new XElement((XNamespace)"urn:ietf:params:xml:ns:xmpp-stanzas" + "error", "UNKNOWN ERROR");
                errorElement.Add(new XAttribute("type", "cancel"));
                errorElement.Add(new XAttribute("code", 503));
                iqElement.Add(errorElement);
            }

            Packet.Add(iqElement);

            User.Send(Packet.ToString(SaveOptions.DisableFormatting));
        }
    }
}
