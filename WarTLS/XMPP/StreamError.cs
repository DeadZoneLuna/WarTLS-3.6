using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WARTLS.CLASSES;

namespace WARTLS.XMPP
{
    class StreamError
    {
        internal string To;
        internal string Xmlns;
        internal string XmlnsUrl;
        internal StreamError(Client User, string Error)
        {
            User.Send($"<stream:error><{Error} xmlns='urn:ietf:params:xml:ns:xmpp-streams'/></stream:error></stream:stream>");
            User.Socket.Dispose();
            if (User.SslStream != null)
                User.SslStream.Dispose();
        }
    }
}
