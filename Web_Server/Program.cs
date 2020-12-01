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
            string path = DirectoryExplorer.GetDirectoryPath(Directory.GetCurrentDirectory(), DIRECTORY);

            int port = 17754;
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
                int c = -1;

                do
                {
                    c++;
                    int bytesRec = communication.Receive(bytesMsg);
                    data += Encoding.UTF8.GetString(bytesMsg, 0, bytesRec);

                } while (data.IndexOf("\r\n\r\n") == -1 || data.Length != 0 && c == 4);

                Console.WriteLine("Data received:\n" + data);

                string[] fields = data.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                if(fields[0].StartsWith("GET"))
                {
                    string filename = fields[0].Split(' ')[1];
                    
                    string message = "";

                    if(filename == "/")
                    {
                        string homePagePath = DirectoryExplorer.GetDirectoryPage(path, 0);
                        Console.WriteLine(homePagePath);
                        if (homePagePath == "404")
                        {
                            string tree = DirectoryExplorer.GetTree(path, DIRECTORY);
                            message = CreateMessage(fields[0], (int)Code.OK, (Code)200, tree);
                        }
                        else
                        {
                            using (StreamReader fin = new StreamReader(homePagePath))
                            {
                                string content = fin.ReadToEnd();
                                message = CreateMessage(fields[0], (int)Code.OK, (Code)200, content);
                            }
                        }
                        
                    }
                    else
                    {
                        string tempPath = path;
                        if (filename.Split('/').Length > 2)
                        {
                            string[] part = filename.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                            for (int i = 0; i < part.Length - 1; i++)
                                tempPath += part[i] + "\\";

                            filename = part[part.Length - 1];

                        }

                        if (filename.StartsWith("/"))
                        {
                            filename = filename.Substring(1);
                        }

                        string filePath = DirectoryExplorer.FindFile(tempPath, filename);

                        if (filePath == "404")
                            message = CreateMessage(fields[0], (int)Code.NOT_FOUND, (Code)400, "ERROR 404: Not Found.");
                        else
                            using (StreamReader fin = new StreamReader(filePath))
                            {
                                string content = fin.ReadToEnd();
                                message = CreateMessage(fields[0], (int)Code.OK, (Code)200, content);
                            }
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

            return string.Format($"HTTP/1.0 {code} {info}\r\nConnection: close\r\nContent-Type: text/html\r\n\r\n{content}\r\n");

        }

        //static string HTTPGetVersion(string data)
        //{
        //    string[] values = data.Split(' ');
        //    foreach(string value in values)
        //    {
        //        if (value.Contains("HTTP"))
        //        {
        //            return value.Split('/')[1];
        //        }
        //    }

        //    return "1.0";
        //}

        

        

        

        

    }
}
