using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using WARTLS.CLASSES;
using WARTLS.NETWORK;
using static System.Net.Mime.MediaTypeNames;

namespace WARTLS
{
    class Program
    {
        
        static void Main(string[] args)
        {
            
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            Console.Title = $"WarTLS Server {Assembly.GetExecutingAssembly().GetName().Version.Major}.{Assembly.GetExecutingAssembly().GetName().Version.Minor}";
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine($@"
     __      __             ___________.____       _________
    /  \    /  \_____ ______\__    ___/|    |     /   _____/
    \   \/\/   /\__  \\_  __ \|    |   |    |     \_____  \ 
     \        /  / __ \|  | \/|    |   |    |___  /        \
      \__/\  /  (____  /__|   |____|   |_______ \/_______  /
           \/        \/                        \/        \/ 
                                                  (Server)
    ");

            
            Console.ResetColor();
            new Core();
            Thread.Sleep(-1);
        }
    }
}
