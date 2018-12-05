using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;
using System.Xml.Linq;
using WARTLS.CLASSES;
using WARTLS.XMPP;
using WARTLS.XMPP.QUERY;

namespace WARTLS.NETWORK
{
    
    class Gateway
    {
        
        public static readonly byte[] MagicBytes = new byte[] { 0xad,0xde,0xed,0xfe };
        public static readonly XNamespace JabberNS = "jabber:client";
        const ushort BufferSize = 16999;
        internal Dictionary<string, int> Connections = new Dictionary<string, int>();
        private Socket _ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        internal Gateway(int Port = 5222)
        {
            Timer ConnectionsCleaner = new Timer();
            ConnectionsCleaner.Elapsed += delegate (object sender, ElapsedEventArgs e) { Connections.Clear(); };
            ConnectionsCleaner.Interval = TimeSpan.FromSeconds(5).TotalMilliseconds;
            ConnectionsCleaner.Start();

            _ServerSocket.Bind(new IPEndPoint(IPAddress.Any, Port));
            _ServerSocket.Listen(0);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"[{this.GetType().Name}] Server started on {Port} port. Let's play Warface!");
            Console.ResetColor();
            _ServerSocket.BeginAccept(new AsyncCallback(AcceptAsync), null);
        }
        private void AcceptAsync(IAsyncResult Result)
        {
            Socket ReceivedSocket = _ServerSocket.EndAccept(Result);
            Client User = new Client() { Socket = ReceivedSocket, Buffer = new byte[BufferSize] };

            if (Connections.ContainsKey(User.IPAddress))
                Connections[User.IPAddress]++;
            else
                Connections.Add(User.IPAddress, 1);

            if (Connections[User.IPAddress] < 5)
                ReceivedSocket.BeginReceive(User.Buffer, 0, User.Buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveAsync), User);
            else
                User.Dispose();
            _ServerSocket.BeginAccept(new AsyncCallback(AcceptAsync), null);
        }
        internal void WriteLine(string Message,ConsoleColor consoleColor=ConsoleColor.Gray)
        {
            Console.ForegroundColor = consoleColor;
            Console.WriteLine($"[{this}]");
            Console.ResetColor();
        }
        private void ReceiveAsync(IAsyncResult Result)
        {
            Client User = (Client)Result.AsyncState;
            try
            {
                User.Received = User.Socket.EndReceive(Result);
            while (true)
                {

                    
                    if (User.Received == 0 || !User.Socket.Connected)
                        {
                            break;
                        }

                        byte[] Bytes = new byte[User.Received];
                        Array.Copy(User.Buffer, Bytes, User.Received);
                        BinaryReader Stream = new BinaryReader(new MemoryStream(Bytes));
                        string Message;


                        byte[] MagicBytesOnClient = Stream.ReadBytes(4);

                        bool isProtect = MagicBytesOnClient[0] == MagicBytes[0] && MagicBytesOnClient[1] == MagicBytes[1] && MagicBytesOnClient[2] == MagicBytes[2] && MagicBytesOnClient[3] == MagicBytes[3];

                        long PacketLength = 0;
                        if (isProtect)
                            PacketLength = Stream.ReadInt64();
                        else
                        {
                            Stream.BaseStream.Position = 0;
                            PacketLength = Stream.BaseStream.Length;
                        }
                        Message = Encoding.UTF8.GetString(Stream.ReadBytes((int)PacketLength));
                    Console.WriteLine(Message);

                        XmlDocument Packet = new XmlDocument();
                        if (new Regex("<stream:stream([\\s\\S]+?)>").Matches(Message).Count > 0)
                            new StreamStream(User, Message);
                        else
                        {
                        
                            try
                            {

                                Packet.LoadXml(Message);
                                
                                if (Packet["starttls"] != null)
                                    if (Packet["starttls"].NamespaceURI == "urn:ietf:params:xml:ns:xmpp-tls")
                                        new StartTLS(User);
                                if (Packet["auth"] != null)
                                    if (Packet["auth"].NamespaceURI == "urn:ietf:params:xml:ns:xmpp-sasl")
                                        new Auth(User, Packet);
                            if (Packet["iq"] != null)
                            {
                                if (Packet["iq"]["bind"] != null)
                                    if (Packet["iq"]["bind"].NamespaceURI == "urn:ietf:params:xml:ns:xmpp-bind")
                                        new Bind(User, Packet);
                                if (Packet["iq"]["session"] != null)
                                    if (Packet["iq"]["session"].NamespaceURI == "urn:ietf:params:xml:ns:xmpp-session")
                                        new Session(User, Packet);
                                if (Packet["iq"]["query"] != null)
                                    if (Packet["iq"]["query"].NamespaceURI == "urn:cryonline:k01")
                                    {
                                        if (User.Authorized)
                                        {
                                            Type Stanza = Core.MessageFactory.Packets[Packet["iq"]["query"].FirstChild.Name];
                                            Activator.CreateInstance(Stanza, User, Packet);
                                        }
                                    }


                            }
                            else if (Packet["message"] != null)
                                new Messages(User, Packet);
                            }
                            catch (KeyNotFoundException)
                            {
                                Activator.CreateInstance(typeof(UnsupportedStanza), User, Packet);
                            }
                            catch (Exception ex)
                            {
                            if(User.Received != 13 && User.Received != 14 && User.Received != 15 && Message!="protect_init" &&!User.Dedicated)
                                if (ex is XmlException)
                                {
                                    new StreamError(User, "xml-not-well-formed");
                                    break;
                                }
                            }
                        }
                        User.Buffer = new byte[BufferSize];
                        try
                        {
                            if (User.SslStream != null)
                                User.Received = User.SslStream.Read(User.Buffer, 0, User.Buffer.Length);
                            else
                                User.Received = User.Socket.Receive(User.Buffer);
                        }
                        catch (Exception ex)
                        {
                            break;
                        }
                    }
                    
            }
            catch (Exception ex) { }

            //if (User.Dedicated && User.Player.GameRoom != null)
             //   new MissionUnload(User);
            if (User.Socket.Connected || User.SslStream!=null&& User.SslStream.CanWrite)
                User.Send("</stream:stream>");

            User.Socket.Dispose();
            if (User.SslStream != null)
                User.SslStream.Dispose();

            ArrayList.OnlineUsers.Remove(User);
            
            if (User.Channel != null)
            {
                if (User.Player.GameRoom != null)
                    new GameRoom_Leave(User, null);
                User.Channel.Users.Remove(User);
            }
           
            return;
        }
    }
}
