using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WARTLS.CLASSES;

namespace WARTLS.XMPP
{
    class StartTLS
    {
        readonly XNamespace NameSpace = "urn:ietf:params:xml:ns:xmpp-tls";
        internal StartTLS(Client User)
        {
            XDocument Packet = new XDocument(new XElement(NameSpace + "proceed"));
            User.Send(Packet.ToString(SaveOptions.DisableFormatting));

            User.SslStream = new SslStream(new NetworkStream(User.Socket, true),true, ServicePointManager.ServerCertificateValidationCallback = delegate { return true; });
            User.SslStream.AuthenticateAsServer(Core.Certificate,true,SslProtocols.Tls,false);
        }
    }
}
