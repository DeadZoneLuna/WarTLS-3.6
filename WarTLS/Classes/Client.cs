using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WARTLS.CLASSES.GAMEROOM;
using WARTLS.NETWORK;
using WARTLS.XMPP.QUERY;

namespace WARTLS.CLASSES
{
    class Client : IDisposable
    {

        internal Socket Socket;
        internal Player Player;
        internal SslStream SslStream;
        internal Channel Channel;
        internal List<InvitationTicket> InvitationTicket = new List<InvitationTicket>();
        internal byte[] Buffer;
        internal int Status=2;
        internal bool Dedicated = false;
        internal bool Authorized = false;
        internal string JID = null;
        internal string Location = "";
        internal int Received = 0;
        internal byte TeamId = 0;
        internal ushort DedicatedPort = 0;
        internal string IPAddress => Socket.Connected ? ((IPEndPoint)Socket.RemoteEndPoint).Address.ToString():"0.0.0.0";

        internal XElement ToElement()
        {
            XElement Element = new XElement("player");
            Element.Add(new XAttribute("profile_id", Player.UserID));
            Element.Add(new XAttribute("team_id", Channel.ChannelType == "pve" ? 1 : TeamId));
            Element.Add(new XAttribute("status", Player.RoomStatus));
            Element.Add(new XAttribute("observer", "0"));
            Element.Add(new XAttribute("skill", "0.000"));
            Element.Add(new XAttribute("nickname", Player.Nickname));
            Element.Add(new XAttribute("clanName", this.Player.Clan != null ? this.Player.Clan.Name : ""));
            Element.Add(new XAttribute("class_id", Player.CurrentClass));
            Element.Add(new XAttribute("online_id", JID));
            Element.Add(new XAttribute("group_id", Player.GroupId));
            Element.Add(new XAttribute("presence", Status));
            Element.Add(new XAttribute("experience", Player.Experience));
            Element.Add(new XAttribute("banner_badge", Player.BannerBadge));
            Element.Add(new XAttribute("banner_mark", Player.BannerMark));
            Element.Add(new XAttribute("banner_stripe", Player.BannerStripe));
            
            return Element;
        }

        internal void CheckExperience()
        {
            int Upped = this.Player.Rank - this.Player.OldRank;

            for (byte i = 0; i < Upped; i++)
            {
                this.Player.AddRankNotifierNotification(this.Player.OldRank, this.Player.Rank);
                this.Player.OldRank++;
                Player.AddRandomBoxNotification("random_box_rank_00");
            }
            if (Upped > 0)
                new SyncNotification(this).Process();
        }
        internal void ShowMessage(string Text,bool Green = false)
        {

            XElement notifElement = new XElement("notif");
            notifElement.Add(new XAttribute("id", 301));
            notifElement.Add(new XAttribute("type", Green ? 512: 8));
            notifElement.Add(new XAttribute("confirmation", 0));
            notifElement.Add(new XAttribute("from_jid", "wartls@server"));
            notifElement.Add(new XAttribute("message", ""));
            if (Green)
            {
                XElement AnnouncentElement = new XElement("announcement",
                    new XAttribute("id",this.Player.Random.Next(99,9999999)),
                    new XAttribute("is_system", 1),
                    new XAttribute("frequency", 1),
                    new XAttribute("repeat_time", 1),
                    new XAttribute("message", Text),
                    new XAttribute("server", "wartls"),
                    new XAttribute("channel", "wartls"),
                    new XAttribute("place", 1)
                    );
                notifElement.Add(AnnouncentElement);
            }
            else
            {
                XElement MessageElement = new XElement("message",
    new XAttribute("data", Text)
    );
                notifElement.Add(MessageElement);
            }
            XElement gamemoneyElement = new XElement("message",new XAttribute("data",Text));
            new SyncNotification(this, notifElement).Process();
        }
        internal void Send(string Message)
        {
            MemoryStream Packet = new MemoryStream();
            BinaryWriter PacketWrite = new BinaryWriter(Packet);

            if ((bool)App.Default["UseProtect"])
            {
                PacketWrite.Write(Gateway.MagicBytes);
                PacketWrite.Write((long)Encoding.UTF8.GetByteCount(Message));
            }
            PacketWrite.Write(Encoding.UTF8.GetBytes(Message));

            if (SslStream == null)
                try
                {
                    Socket.Send(Packet.ToArray());
                }
                catch
                {
                    return;
                }
            else
                try
                {
                    SslStream.Write(Packet.ToArray());
                }
                catch { return; }
        }

        
        private void SocketWrite(IAsyncResult Result) => Socket.EndSend(Result);

        public void Dispose()
        {
            if (Socket != null)
                Socket.Dispose();
            if (SslStream != null)
                SslStream.Dispose();
        }
    }
}
