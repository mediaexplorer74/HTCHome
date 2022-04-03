using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
namespace WeatherClockWidget.Domain
{
    public class WeatherReport
    {
        public int High; //high temperature
        public string Location = "New York"; //name of the current location
        public int Low; //low temperature
        public string NowSky; //current weather
        public int NowSkyCode = 1; //skycode used for weather icon
        public int NowTemp; //current temperature
        public string Url; //link to page with forecast
        public List<DayForecast> Forecast = new List<DayForecast>();

        public static WeatherReport Read(string path)
        {
            var result = new WeatherReport();
            if (File.Exists(path))
            {
                var f = new FileInfo(path);
                if (f.Length > 0)
                {
                    using (TextReader textReader = new StreamReader(path))
                    {
                        var deserializer = new XmlSerializer(typeof(WeatherReport));
                        result = (WeatherReport)deserializer.Deserialize(textReader);
                    }
                }
            }
            else
                result.Write(path);
            return result;
        }

        public void Write(string path)
        {
            using (TextWriter textWriter = new StreamWriter(path))
            {
                var serializer = new XmlSerializer(typeof(WeatherReport));
                serializer.Serialize(textWriter, this);
            }
        }
    }
}