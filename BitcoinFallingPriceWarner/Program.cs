using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;


namespace BitcoinFallingPriceWarner
{
    public class Program
    {
        private static string folderFromExe = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        private static string filename = "lastCourses.txt";
        private static string url = "https://www.bitcoin.de/de/btceur/home/reload-trades?type=order";
        
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine($"\n Url: {url} \n Zum Starten Taste drücken");
            Console.WriteLine("Press the Escape (Esc) key to quit, or any key for one request: \n");

            Settings settings = loadSettings();
            Controller c = new Controller(settings, folderFromExe, filename);
            ConsoleKeyInfo cki;
            do
            {
                cki = Console.ReadKey();
                c.ReadWebsite(settings.UrlToGetInformation);

            } while (cki.Key != ConsoleKey.Escape);



        }

        private static Settings loadSettings()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json").Build();
            var section = config.GetSection(nameof(Settings));
            var settings = section.Get<Settings>();

            return settings;
        }

        /// <summary>
        /// get the newest prices, save and send email if neccessary
        /// </summary>
        /// <returns></returns>
        public static async void BitcoinFallingPriceWarner(Settings settings)
        {
            Controller c = new Controller(settings, folderFromExe, filename);
            await c.ReadWebsite(settings.UrlToGetInformation);
            //if read then dispose
            c.closeAndDisposeAll(); 
        }



    }
}
