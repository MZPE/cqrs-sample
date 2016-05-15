using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensioenSysteem.Database
{
    class Program
    {
        static void Main(string[] args)
        {
            string serverExe = FindServerExe(@"..\..\..\Packages\");
            if (serverExe != null)
            {
                Process.Start(serverExe);
            }
            else
            {
                throw new FileNotFoundException("Raven.Server.exe not found.");
            }
        }

        /// <summary>
        /// Zoek server.exe onafhankelijk van versienummer. 
        /// </summary>
        /// <param name="packagesFolder">De packages folder om in te zoeken.</param>
        /// <returns>Pad naar de exe of null als hij niet gevonden kan worden.</returns>
        private static string FindServerExe(string packagesFolder)
        {
            string[] folders = Directory.GetDirectories(packagesFolder, "RavenDB.Server.*");
            if (folders.Length == 1)
            {
                string folder = Path.Combine(folders[0], "tools");
                string[] files = Directory.GetFiles(folder, "Raven.Server.exe");
                if (files.Length == 1)
                {
                    return files[0];
                }
            }
            return null;
        }
    }
}
