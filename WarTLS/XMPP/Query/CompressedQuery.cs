using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using WARTLS.CLASSES;

namespace WARTLS.XMPP.QUERY
{
    internal class CompressedQuery : Stanza
    {
        public CompressedQuery(Client User, XmlDocument Packet) : base(User, Packet)
        {
            base.Uncompress(ref Packet);


            string Name = Packet.FirstChild.FirstChild.FirstChild.Name.Replace(":", "_");

            Type Type = Core.MessageFactory.Packets[Name];

            if (Type != null)
            {
                Activator.CreateInstance(Type, User, Packet);
            }
        }
    }
}