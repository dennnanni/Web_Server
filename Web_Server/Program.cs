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

            const string DIRECTORY = "pages"; // Punto 5, configurabilità della home directory
            string path = DirectoryExplorer.GetDirectoryPath(Directory.GetCurrentDirectory(), DIRECTORY);

            int port = 17754;
            byte[] bytesMsg = new byte[1024];
            string data = "";

            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[2]; // 1 - IPv4
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

            Socket generalHandler = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            generalHandler.Bind(localEndPoint);
            generalHandler.Listen(10);

            while (running)
            {
                Console.WriteLine("Waiting for connection");
                Socket communication = generalHandler.Accept(); // Punto 1, listening su porta 17754
                int c = -1;

                do
                {
                    c++;
                    int bytesRec = communication.Receive(bytesMsg);
                    data += Encoding.UTF8.GetString(bytesMsg, 0, bytesRec);

                } while (data.IndexOf("\r\n\r\n") == -1 || data.Length != 0 && c == 4);

                

                string[] fields = data.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                if(fields[0].StartsWith("GET")) // Punto 3, acquisisce solo GET
                {
                    Console.WriteLine("Data received:\n" + data); // Punto 4, visualizza header in console
                    string filename = fields[0].Split(' ')[1];
                    
                    string message = "";

                    if(filename == "/")
                    {
                        string homePagePath = DirectoryExplorer.GetHomePage(path);
                        Console.WriteLine(homePagePath);
                        if (homePagePath == "404")
                        {
                            string tree = DirectoryExplorer.GetTree(path, DIRECTORY, false);
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
                            // La richiesta arriva con i /, per tale motivo devo ricostruire il path compatibilmente
                            // con i path di windows quindi \, splitto la stringa e poi aggiungo tutti i valori della 
                            // stringa ricevuta con separatori \ fino a n-1 perché l'ultimo è il file richiesto
                            string[] part = filename.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                            for (int i = 0; i < part.Length && !part[i].Contains('.'); i++)
                                tempPath += part[i] + "\\";

                            if (part[part.Length - 1].Contains('.'))
                            {
                                filename = part[part.Length - 1];
                            }
                            else
                                filename = "";
                        }

                        // Nel caso l'utente specifichi solo il filename nel browser, la richiesta sarà formata da
                        // /filename, per questo devo togliere il / se voglio renderlo parte del path
                        if (filename.StartsWith("/"))
                        {
                            filename = filename.Substring(1);
                        }

                        try
                        {
                            Directory.GetFiles(tempPath);
                        }
                        catch(Exception ex)
                        {
                            tempPath = "404";
                        }

                        if(tempPath == "404")
                        {
                            message = CreateMessage(fields[0], (int)Code.NOT_FOUND, (Code)400, "ERROR 404: Not Found.");
                        }
                        else if(filename == "")
                        {
                            string homepage = DirectoryExplorer.GetHomePage(tempPath);
                            if (homepage == null)
                            {
                                string filesList = DirectoryExplorer.GetTree(tempPath, new DirectoryInfo(tempPath).Name, false);
                                message = CreateMessage(fields[0], (int)Code.OK, (Code)200, filesList);
                            }
                            else
                            {
                                using (StreamReader fin = new StreamReader(homepage))
                                {
                                    string content = fin.ReadToEnd();
                                    message = CreateMessage(fields[0], (int)Code.OK, (Code)200, content);
                                }
                            }
                        }
                        else
                        {
                            string filePath = DirectoryExplorer.FindFileDirectory(tempPath, filename);

                            if (filePath == "404")
                                message = CreateMessage(fields[0], (int)Code.NOT_FOUND, (Code)400, "ERROR 404: Not Found."); // dice solo header bo
                            else
                                using (StreamReader fin = new StreamReader(filePath))
                                {
                                    string content = fin.ReadToEnd();
                                    message = CreateMessage(fields[0], (int)Code.OK, (Code)200, content);
                                }


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

            return string.Format($"HTTP/1.0 {code} {info}\r\nConnection: close\r\nContent-Type: text/html\r\n\r\n{content}\r\n\r\n");

        }

        

        

        

        

    }
}
