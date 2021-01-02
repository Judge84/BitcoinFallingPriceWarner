using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinFallingPriceWarner
{
    /// <summary>
    /// the WebsiteReader
    /// </summary>
    public class WebsiteReader
    {

        /// <summary>
        /// read Async a website url, 
        /// </summary>
        /// <param name="url">The url like https://www.bitcoin.de/de/btceur/home/reload-trades?type=order</param>
        /// <param name="contentLength">Count of Chars of return value, if -1 the full content is returned</param>
        /// <returns>the content, if the content string.empty an exception is thrown</returns>
        public async Task<string> ReadWebsite(string url, int contentLength)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "the dark side of the moon");
            string content = string.Empty;
            try
            {
                content = await client.GetStringAsync(url);
            }catch (Exception e)
            {
                Logger.Log(Logger.LogLevel.Error, $"ReadWebsite: {url}",
                    e.Message);
            }



            if (contentLength > 0)
            {
                return content.Substring(0, contentLength);
            } else
            {
                return content;
            }
        }

    }
}
