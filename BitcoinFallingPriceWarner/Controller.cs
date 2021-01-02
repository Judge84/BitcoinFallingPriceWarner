using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BitcoinFallingPriceWarner
{

    /// <summary>
    /// The Controller of BitcoinFallingPriceWarner. It is the entry-Point
    /// </summary>
    public class Controller
    {
        private string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private string filename = "lastCourses.txt";
        public static Settings Settings; 
        Logger logger = null;
        /// <summary>
        /// constructor, to set the data dir and file for saving last prices, default is
        ///         folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        ///         filename = "WriteLines.txt";
        /// </summary>
        public Controller(Settings settings, string folderPath = null, string filename = null)
        {
            Settings = settings;
            if (!(String.IsNullOrEmpty(folderPath) || String.IsNullOrEmpty(filename)))
            {
                this.folderPath = folderPath;
                this.filename = filename;
            
            }
            //init logger
            logger = new Logger(this.folderPath);
            Logger.Log(Logger.LogLevel.Trace, "Controller", $"Starting with {this.folderPath} and {this.filename} ");
        
        }

        public void closeAndDisposeAll()
        {
            logger.closeAndDispose();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        public async Task ReadWebsite(string url)
        {
            Logger.Log(Logger.LogLevel.Trace, "Controller", $"ReadWebsite: {url}");

            WebsiteReader reader = new WebsiteReader();
            string content = await reader.ReadWebsite(url, -1);
           
            List<double> lastPrices=  readData(content);

            writeToLog(lastPrices);

            savePrice(lastPrices, folderPath, filename);

            readLastPriceAndDecide(folderPath, filename, Settings.AmountDifferenzForSendingWarning);



        }

        private async void readLastPriceAndDecide(string docPath, string filename, double difference)
        {
            
            SortedList<DateTime, Double> data = new SortedList<DateTime, double>();

            string path = $"{docPath}/{filename}";
            string[] allLines;
            using (var reader = File.OpenText(path))
            {
                var fileText = await reader.ReadToEndAsync();
                allLines = fileText.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            }

            foreach(string oneLine in allLines)
            {
                if (oneLine.Length > 0)
                {
                    string[] separatedLine = oneLine.Split(";");
                    try
                    {
                        data.Add(DateTime.Parse(separatedLine[0]), Double.Parse(separatedLine[1]));
                    }
                    catch (Exception e)
                    {
                        Logger.Log(Logger.LogLevel.Warn, "readLastPriceAndDecide",
                            $"Can not Parse: {separatedLine}" + e.Message);
                    }
                }

            }


            var allDatetimes = data.Keys;
            var lastDatetime = allDatetimes[allDatetimes.Count - 1];
            var tenthLastDatetime = lastDatetime; 
;           if (allDatetimes.Count > 9)
            {
                tenthLastDatetime = allDatetimes[allDatetimes.Count - 10];
            }

            Logger.Log(Logger.LogLevel.Trace, "readLastPriceAndDecide",
                $"10. letzter Wert:\t {tenthLastDatetime} \t\t {data[tenthLastDatetime]}");
            Logger.Log(Logger.LogLevel.Warn, "readLastPriceAndDecide",
                $"    letzter Wert:\t {lastDatetime} \t\t {data[lastDatetime]}");

            if(data[tenthLastDatetime]+ difference <  data[lastDatetime])
            {
                Mailer.sendEmail(
                $"VERKAUFEN!!!!!!\n" +
                $"10.letzter Wert:\t { tenthLastDatetime} \t\t { data[tenthLastDatetime]}\n" +
                $"    letzter Wert:\t {lastDatetime} \t\t {data[lastDatetime]}"
                );

            }
            else
            {
                Logger.Log(Logger.LogLevel.Trace, "readLastPriceAndDecide", $"OK");
            }


        }
    

        /// <summary>
        /// here is the magic to get the prices from the website
        /// </summary>
        /// <param name="xmlContent"> The content</param>
        /// <returns></returns>
        private List<double> readData(string xmlContent)
        {
            //cutting out new line and backslash
            string preparedXmlContent = xmlContent.Replace("\n", "");
            preparedXmlContent = preparedXmlContent.Replace("\\", "");
  
            //to read easier, make xml document ;-)    
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<oneroot>" + preparedXmlContent + "</oneroot>");
            XmlNode root = doc.DocumentElement;
            root.SelectSingleNode("/oneroot");

            List<double> lastPrices = new List<double>();
           
            //go for every <tr> entry
            foreach(XmlNode tr in root.ChildNodes)
            {
                //every <tr> entry has x attributes
                foreach (XmlAttribute attribute in tr.Attributes)
                {
                    //data-critical-price containts the bitcoin course
                    if (attribute.Name == "data-critical-price")
                    {
                        //Website delivers Englisch format, e.x. 23000.57
                        System.Globalization.CultureInfo EnglishCulture = new System.Globalization.CultureInfo("en-EN");
                        lastPrices.Add(Double.Parse(attribute.Value, EnglishCulture
                        ));
                    }
                }
            }
            
            return lastPrices;
       }

        private void writeToLog(List<double> lastPrices)
        {
            Logger.Log(Logger.LogLevel.Trace, "lastPricesFromWebsite", $" at {DateTime.Now.ToString()}");
            foreach (var price in lastPrices)
            {
                Logger.Log(Logger.LogLevel.Trace, "lastPricesFromWebsite", $" {price}");
            }
        }


        private void savePrice(List<double> lastPrices, string docPath, string filename)
        {
            double midPrice = calcMidPrice(lastPrices);
            
            string oneLine = $"{DateTime.Now.ToString()};{midPrice}";
            Logger.Log(Logger.LogLevel.Trace, "calculatedmidPrice", $" {oneLine}");

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, filename), true))
            {
                outputFile.WriteLine(oneLine);
            }
        }

        private double calcMidPrice(List<double> lastPrices)
        {
            if (lastPrices.Count < 1)
            {
                return 0.0;
            }
            double sum = 0;
            foreach(double price in lastPrices)
            {
                sum = sum + price;
            }
            return Math.Round(sum / lastPrices.Count,2);
        }
    }
}
