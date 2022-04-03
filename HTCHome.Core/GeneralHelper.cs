using System;
using System.Net;
using System.Text;
using System.IO;

namespace HTCHome.Core
{
    public static class GeneralHelper
    {
        public static WebProxy Proxy;
        /// <exception cref = "ArgumentNullException">Argument is null.</exception>
        public static string GetXml(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return String.Empty;
            }

            var webClient = new WebClient();
            if (Proxy != null)
            {
                webClient.Proxy = Proxy;
            }
            webClient.Headers["User-Agent"] =
                "Mozilla/4.0 (Compatible; Windows NT 5.1; MSIE 8.0) (compatible; MSIE 8.0; Windows NT 5.1;)";

            //this doesn't work with encoding
            //return webClient.DownloadString(url);

            try
            {
                //тупо, но пока ничего лучше в голову не пришло
                StreamReader reader = new StreamReader(webClient.OpenRead(url));
                string line = reader.ReadLine(); //read the first line with encoding name
                if (line.Contains("encoding"))
                {
                    string encoding = line.Substring(line.IndexOf("encoding") + 10); //parse encoding name from the first line of xml
                    encoding = encoding.Substring(0, encoding.IndexOf('"'));
                    reader.Close();

                    reader = new StreamReader(webClient.OpenRead(url), Encoding.GetEncoding(encoding)); //reopen stream with right encoding
                    line = "";
                }
                return line + reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                HTCHome.Core.Logger.Log(ex.ToString());
                return String.Empty;
            }
        }
    }
}