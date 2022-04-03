using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeatherClockWidget;
using System.Xml.Linq;
using System.IO;
using HTCHome.Core;
using WeatherClockWidget.Domain;
using System.Xml;

namespace WeatherChannel
{
    public class Weather : IWeatherProvider
    {
        private const string RequestForCelsius =
            "http://gadgets.weather.com/weather/local/{0}?dayf=5&cc=*&unit=m";
        private const string RequestForFahrenheit =
            "http://gadgets.weather.com/weather/local/{0}?dayf=5&cc=*";
        private const string RequestForLocation =
            "http://gadgets.weather.com/search/search/?where={0}";

        private string[] cities; // localized cities names
        private string[] weather; // localized weather strings

        public WeatherClockWidget.Domain.Coordinates GetCoordinates(string locationCode)
        {
            throw new NotImplementedException();
        }

        public List<WeatherClockWidget.Domain.CityLocation> GetLocation(string s)
        {
            string city = GetEnglishCityName(s);
            var result = new List<CityLocation>();

            XDocument doc = XDocument.Load(string.Format(RequestForLocation, city));

            var cities = from x in doc.Root.Descendants("loc")
                         select
                             new
                             {
                                 code = x.Attribute("id").Value,
                                 name = x.Value
                                 //url = x.Descendants("url").First().Value
                             };
            foreach (var c in cities)
            {
                CityLocation i = new CityLocation();
                i.City = c.name;
                i.Code = c.code;
                result.Add(i);
            }
            return result;
        }

        public WeatherClockWidget.Domain.WeatherReport GetWeatherReport(string locale, string locationcode, int degreeType)
        {
            if (string.IsNullOrEmpty(locationcode))
                locationcode = "USNY0996";
            string url = string.Format(degreeType == 0 ? RequestForCelsius : RequestForFahrenheit, locationcode);

            if (cities == null && File.Exists(HTCHome.Core.Environment.Path + "\\WeatherClock\\WeatherProviders\\" + locale + "\\Cities.data"))
            {
                cities = File.ReadAllLines(HTCHome.Core.Environment.Path + "\\WeatherClock\\WeatherProviders\\" + locale + "\\Cities.data", Encoding.UTF8);
            }

            if (weather == null && File.Exists(HTCHome.Core.Environment.Path + "\\WeatherClock\\WeatherProviders\\" + locale + "\\Weather.data"))
            {
                weather = File.ReadAllLines(HTCHome.Core.Environment.Path + "\\WeatherClock\\WeatherProviders\\" + locale + "\\Weather.data", Encoding.UTF8);
            }

            string s = GeneralHelper.GetXml(url);

            if (string.IsNullOrEmpty(s))
                return null;
            //parse current weather
            XDocument doc = XDocument.Parse(s);
            string location = doc.Descendants("loc").FirstOrDefault().Element("dnam").Value;
            if (location.Contains(","))
                location = location.Substring(0, location.IndexOf(","));

            var conditions = from x in doc.Descendants("cc")
                             select
                                 new
                                 {
                                     temp = x.Descendants("tmp").First().Value,
                                     text = x.Descendants("t").First().Value,
                                     weathericon = x.Descendants("icon").First().Value,
                                     //url = x.Descendants("url").First().Value
                                 };

            var result = new WeatherReport();

            var currentCondition = conditions.FirstOrDefault();
            if (currentCondition != null)
            {
                result.Location = GetCityName(location);
                if (currentCondition.temp != "N/A")
                    result.NowTemp = Convert.ToInt32(currentCondition.temp);
                else
                    result.NowTemp = 0;
                result.NowSky = GetWeatherString(currentCondition.text);
                result.NowSkyCode = GetWeatherPic(Convert.ToInt32(currentCondition.weathericon), 4, 22);
                //result.Url = currentCondition.url;
            }

            //parse forecast
            var days = from x in doc.Descendants("dayf").First().Descendants("day")
                       select new
                       {
                           h = x.Element("hi").Value,
                           l = x.Element("low").Value,
                           text = x.Descendants("part").First().Element("t").Value,
                           icon = x.Descendants("part").First().Element("icon").Value,
                           //url = x.Element("url").Value
                       };

            List<DayForecast> f = new List<DayForecast>();
            foreach (var d in days)
            {
                f.Add(new DayForecast());
                if (d.h != "N/A")
                    f[f.Count - 1].HighTemperature = Convert.ToInt32(d.h);
                if (d.l != "N/A")
                    f[f.Count - 1].LowTemperature = Convert.ToInt32(d.l);
                f[f.Count - 1].Text = GetWeatherString(d.text);
                f[f.Count - 1].SkyCode = GetWeatherPic(Convert.ToInt32(d.icon), 0, 23);
                //f[f.Count - 1].Url = d.url;
            }

            result.Forecast = f;
            return result;
        }

        // returns localized city name
        private string GetCityName(string city)
        {
            if (cities != null)
            {
                //var c = from x in cities where x.StartsWith(city) select x;
                var c = Array.Find(cities, x => x.StartsWith(city));
                if (c == null)
                    return city;

                string result = c.Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries)[1];
                if (!string.IsNullOrEmpty(result))
                {
                    return result;
                }
            }
            return city;
        }

        // returns english city name from localized string
        private string GetEnglishCityName(string city)
        {
            if (cities != null)
            {
                //var c = from x in cities where x.StartsWith(city) select x;
                var c = Array.Find(cities, x => x.ToLower().EndsWith(city.ToLower()));
                if (c == null)
                    return city;

                string result = c.Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries)[0];
                if (!string.IsNullOrEmpty(result))
                {
                    return result;
                }
            }
            return city;
        }

        // returns localized weather string
        private string GetWeatherString(string w)
        {
            if (weather != null)
            {
                //var c = from x in cities where x.StartsWith(city) select x;
                var c = Array.Find(weather, x => x.StartsWith(w));
                if (c == null)
                    return w;

                string result = c.Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries)[1];
                if (!string.IsNullOrEmpty(result))
                {
                    return result;
                }
            }
            return w;
        }

        public static int GetWeatherPic(int skycode, int sunrise, int sunset)
        {
            switch (skycode)
            {
                case 26:
                    if (DateTime.Now.Hour >= sunset || DateTime.Now.Hour <= sunrise)
                        return 34;
                    else
                        return 2;
                case 27:
                    if (DateTime.Now.Hour >= sunset || DateTime.Now.Hour <= sunrise)
                        return 35;
                    else
                        return 3;
                case 28:
                    if (DateTime.Now.Hour >= sunset || DateTime.Now.Hour <= sunrise)
                        return 38;
                    else
                        return 6;
                case 35:
                case 39:
                    return 12;
                case 45:
                case 46:
                    return 8;
                case 19:
                case 20:
                case 21:
                case 22:
                    if (DateTime.Now.Hour >= sunset || DateTime.Now.Hour <= sunrise)
                        return 37;
                    else
                        return 11;
                case 29:
                case 30:
                    if (DateTime.Now.Hour >= sunset || DateTime.Now.Hour <= sunrise)
                        return 35;
                    else
                        return 3;
                case 33:
                    if (DateTime.Now.Hour >= sunset || DateTime.Now.Hour <= sunrise)
                        return 38;
                    else
                        return 6;
                case 5:
                case 13:
                case 14:
                case 15:
                case 16:
                    return 22;
                case 18:
                case 25:
                case 41:
                case 42:
                case 43:
                    return 25;
                case 1:
                case 2:
                case 3:
                case 4:
                case 37:
                case 38:
                case 47:
                    return 15;
                case 31:
                case 32:
                case 34:
                case 36:
                case 44:
                    if (DateTime.Now.Hour >= sunset || DateTime.Now.Hour <= sunrise)
                    {
                        return 33;
                    }
                    else
                        return 1;
                case 23:
                case 24:
                    return 32;
                case 9:
                case 10:
                case 11:
                case 12:
                case 40:
                    return 18;
                case 6:
                case 7:
                case 8:
                case 17:
                    return 15;
                default:
                    if (DateTime.Now.Hour >= sunset || DateTime.Now.Hour <= sunrise)
                        return 33;
                    else
                        return 1;
            }
        }
    }
}
