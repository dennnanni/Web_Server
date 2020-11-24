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
            byte[] bytesMsg = new byte[1024];
            string data = "";

            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[1]; // 1 - IPv4
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

            Socket generalHandler = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            generalHandler.Bind(localEndPoint);
            generalHandler.Listen(10);

            while (running)
            {
                Socket communication = generalHandler.Accept();

                do
                {
                    int bytesRec = communication.Receive(bytesMsg);
                    data += Encoding.UTF8.GetString(bytesMsg, 0, bytesRec);

                } while (data.IndexOf("\r\n\r\n") == -1);

                string[] fields = data.Split(' ');
                if(fields[0].ToUpper() == "GET")
                {

                    data = "HTTP/" + HTTPGetVersion(fields) + " 200 segue documento\r\n\r\nciao\r\n\r\n";
                    communication.Send(Encoding.UTF8.GetBytes(data));
                }

                communication.Shutdown(SocketShutdown.Both);
                communication.Close();
            }

            Console.ReadKey();


        }

        static string HTTPGetVersion(string[] data)
        {
            foreach(string value in data)
            {
                if (value.Contains("HTTP"))
                {
                    return value.Split('/')[1];
                }
            }

            return "1.0";
        }
    }
}
