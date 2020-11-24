// Denise Nanni 5^F 24-11-2020
// Implementazione di un semplice web server
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Web_Server
{
    /* TO DO:
     * Accept di richieste derivanti da un browser
     * Invio di una stringa random (file)
     */

    class Program
    {
        static bool running = true;
        static void Main(string[] args)
        {
            int port = 57348;

            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[1]; // 1 - IPv4
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

            while (running)
            {

            }

            Console.ReadKey();


        }
    }
}
