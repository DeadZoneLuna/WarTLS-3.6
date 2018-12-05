using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using WARTLS.CLASSES;
using WARTLS.NETWORK;

namespace WARTLS
{
    class Core
    {
        internal static X509Certificate2 Certificate;
        internal static MessageFactory MessageFactory;
        internal Core()
        {
            Console.WriteLine();

            Certificate = new X509Certificate2("Cert/Server.pfx", "x");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine($"[CertificateManager] Certificate X509 is loaded");
            
            MessageFactory = new MessageFactory();
            new ArrayList();
            new GameResources();
            new SQL();
            ArrayList.Channels.Add(new Channel("pve_001", 1, "pve", 1, 90));
            //ArrayList.Channels.Add(new Channel("pvp_newbie_1", 101, "pvp_newbie", 1, 10));
            //ArrayList.Channels.Add(new Channel("pvp_skilled_1", 201, "pvp_skilled", 11, 20));
            ArrayList.Channels.Add(new Channel("pvp_pro_3", 301, "pvp_pro", 1, 90));
            new Gateway(5222);
        }
    }
}
