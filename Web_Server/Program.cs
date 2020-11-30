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
        public enum Code : int { OK = 200, NOT_FOUND = 404 }

        static bool running = true;
        static void Main(string[] args)
        {

            const string DIRECTORY = "pages";

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
                Console.WriteLine("Waiting for connection");
                Socket communication = generalHandler.Accept();

                do
                {
                    int bytesRec = communication.Receive(bytesMsg);
                    data += Encoding.UTF8.GetString(bytesMsg, 0, bytesRec);

                } while (data.IndexOf("\r\n\r\n") == -1);

                Console.WriteLine("Data received:\n" + data);

                string[] fields = data.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                if(fields[0].StartsWith("GET"))
                {
                    string filename = fields[0].Split(' ')[1];
                    string message = "";

                    if(filename == "/ls")
                    {
                        string tree = GetTree(DIRECTORY);
                        message = CreateMessage(fields[0], (int)Code.OK, (Code)200, tree);
                    }
                    else
                    {
                        string path = GetFilePath(DIRECTORY, filename);
                        
                    }

                    communication.Send(Encoding.UTF8.GetBytes(message));
                }

                communication.Shutdown(SocketShutdown.Both);
                communication.Close();
                data = "";
            }

            Console.ReadKey();


        }

        static string CreateMessage(string head, int code, Code info, string content)
        {

            return string.Format($"HTTP/{HTTPGetVersion(head)} {code} {info}\r\n\r\n{content}\r\n");

        }

        static string HTTPGetVersion(string data)
        {
            string[] values = data.Split(' ');
            foreach(string value in values)
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
