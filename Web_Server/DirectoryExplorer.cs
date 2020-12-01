using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Web_Server
{
    static class DirectoryExplorer
    {
        static public string GetTree(string path, string dir)
        {
            string tree = dir + "\n";

            // Esplora l'albero di file e subdirectories
            tree += BrowseDirectory(path);

            return tree;

        }

        static public string FindFile(string dir, string file)
        {
            // Ricava i files e le directory della directory specificata
            string[] files = Directory.GetFiles(dir);
            string[] subdir = Directory.GetDirectories(dir);

            foreach (string item in files)
            {
                if (Path.GetFileName(item) == file)
                    return item;
            }

            foreach (string sub in subdir)
            {
                // Stampa il nome della directory e poi esplora file e sottodirectories di quella attraverso la ricorsione
                DirectoryInfo directory = new DirectoryInfo(sub);
                return FindFile(sub + "\\", file);
            }

            return "404";
        }

        static public string BrowseDirectory(string dir)
        {
            string list = "";

            // Ricava i files e le directory della directory specificata
            string[] files = Directory.GetFiles(dir);
            string[] subdir = Directory.GetDirectories(dir);

            foreach (string file in files)
            {
                // Inserisce all'inizio i files
                list += "   " + Path.GetFileName(file) + "\n";
            }

            foreach (string sub in subdir)
            {
                // Stampa il nome della directory e poi esplora file e sottodirectories di quella attraverso la ricorsione
                DirectoryInfo directory = new DirectoryInfo(sub);
                list += "   " + directory.Name + "\n";
                list += "   " + BrowseDirectory(sub + "\\");
            }

            return list;
        }

        static public string GetFilePath(string dir, string file)
        {
            return null;
        }

        static List<string> GetDirectoryPages(string dir)
        {
            List<string> pages = new List<string>();

            return null;
        }

        static public string GetDirectoryPage(string dir, int i)
        {
            string[] paths = Directory.GetFiles(dir);

            if (i < paths.Length)
                return paths[i];
            else
                return "404";
        }

        static public string GetDirectoryPath(string current, string dirName)
        {
            string[] directories = Directory.GetDirectories(current);

            // Cerca la directory che continene la directory specificata come home directory
            for (int i = 0; i < 4 && !directories.Contains(current + dirName); i++)
            {
                string[] tmp = current.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                string sTemp = "";

                // Va indietro di uno step ogni ciclo
                for (int k = 0; k < tmp.Length - 1; k++)
                    sTemp = sTemp + tmp[k] + "\\";

                current = sTemp;
                directories = Directory.GetDirectories(current);
            }

            return current + dirName + "\\";
        }
    }
}
