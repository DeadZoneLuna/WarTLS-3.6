using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WARTLS.CLASSES;

namespace WARTLS.XMPP
{
    class StreamStream
    {
        internal string To;
        internal string Xmlns;
        internal string XmlnsUrl;
        internal StreamStream(Client User, string Packet)
        {
            MatchCollection StreamParam = new Regex("(?:to=')([\\s\\S]+?)'|xmlns='([\\s\\S]+?)'|xmlns:stream='([\\s\\S]+?)'").Matches(Packet);
            To = StreamParam[0].Value;
            Xmlns = StreamParam[1].Value;
            XmlnsUrl = StreamParam[1].Value;

            //User.Send($"<?xml version='1.0'?><stream:stream xmlns:stream='http://etherx.jabber.org/streams' from='korea_emul.warface' id='{User.Socket.GetHashCode()}' version='1.0'>");
            User.Send($"<stream:features>{(User.SslStream==null && !User.Authorized ? "<starttls xmlns='urn:ietf:params:xml:ns:xmpp-tls'/>" : "")}{(!User.Authorized ? "<mechanisms xmlns='urn:ietf:params:xml:ns:xmpp-sasl'><mechanism>WARFACE</mechanism></mechanisms>" : "<bind xmlns='urn:ietf:params:xml:ns:xmpp-bind' /><session xmlns='urn:ietf:params:xml:ns:xmpp-session' />")}</stream:features>");
        }
    }
}
