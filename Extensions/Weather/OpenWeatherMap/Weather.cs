// OpenWeatherMap
// https://api.openweathermap.org/data/2.5/weather?q=Londona&lang=en&appid=e60a227a4667049d23504904815bdd54
// https://api.openweathermap.org/data/2.5/weather?q=Samara&lang=ru&appid=e60a227a4667049d23504904815bdd54

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

using HTCHome.Core;

using WeatherClockWidget;
using WeatherClockWidget.Domain;

namespace OpenWeatherMap
{
    public class Weather : IWeatherProvider
    {
        // Request of location (by City name)
        private const string RequestForLocation = "https://api.openweathermap.org/data/2.5/weather?q={0}&appid=e60a227a4667049d23504904815bdd54";

        // Request of location (by City name and Metric measure unit)
        private const string RequestForCelsius = "https://api.openweathermap.org/data/2.5/weather?q={0}&lang={1}&units=metric&appid=e60a227a4667049d23504904815bdd54";

        // Request of location (by City name and Imperial measure unit)
        private const string RequestForFahrenheit = "https://api.openweathermap.org/data/2.5/weather?q={0}&lang={1}&units=imperial&appid=e60a227a4667049d23504904815bdd54";
        
        
        // GetCoordinates
        public Coordinates GetCoordinates(string locationCode)
        {
            var f = new NumberFormatInfo {CurrencyDecimalSeparator = "."};

            var reader = new XmlTextReader(string.Format(RequestForCelsius, "", locationCode));

            var coord = new Coordinates();
            while (reader.Read())
            {
                if (reader.NodeType != XmlNodeType.Element || !reader.Name.Equals("weather"))
                {
                    continue;
                }

                reader.MoveToAttribute("lat");
                coord.X = Math.Round(Convert.ToDouble(reader.Value, f), 1);
                reader.MoveToAttribute("long");
                coord.Y = Math.Round(Convert.ToDouble(reader.Value, f), 1);
            }
            return coord;
        }


        // GetLocation
        public List<CityLocation> GetLocation(string s)
        {
            CityLocation l = new CityLocation();

            List<CityLocation> result = new List<CityLocation>()
            { new CityLocation { City = "London", Code = "Paris" } };

            //TEMP: RnD only.
            return result;

            //string ns = string.Format(RequestForLocation, s);
            var ns = "https://api.openweathermap.org/data/2.5/weather?q=London&appid=e60a227a4667049d23504904815bdd54";

            XmlTextReader reader = null;

            try
            {
                reader = new XmlTextReader(ns);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("! List<CityLocation> GetLocation - XmlTextReader ex.:" + ex.Message);
            }

            // check...
            if (reader != null)
            {
                try
                {
                    while (reader.Read())
                    {
                        try
                        {
                            if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("weather"))
                            {
                                reader.MoveToAttribute("weatherlocationname");
                                l.City = reader.Value;
                                reader.MoveToAttribute("weatherlocationcode");
                                l.Code = reader.Value;

                                result.Add(l);
                            }
                        }
                        catch (Exception ex1)
                        {
                            Debug.WriteLine("! List<CityLocation> XmlNodeType.Element ex.:" + ex1.Message);
                        }
                    }//while...
                }
                catch (Exception e) 
                {
                    Debug.WriteLine("Weather - reader.Read Exception : " + e.Message);
                }

                reader.Close();
            }
            return result;
        }

        WeatherReport IWeatherProvider.GetWeatherReport(string locale, string locationcode, int degreeType)
        {
            //if (locale == "ru-RU")
            //{
            //    locale = "ru";
            //}

            //if (locale == "en-US")
            //{
            //    locale = "en";
            //}


            string url = string.Format(degreeType == 0 ? RequestForCelsius : RequestForFahrenheit, locale, locationcode);

            string s = GeneralHelper.GetXml(url);

            XDocument doc = null;

            try
            {
                doc = XDocument.Parse(s);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("! Weather - GetWeatherReport - XDocument.Parse Exception: " + ex.Message);
            }

            // check that parsed doc is good...
            if (doc != null)
            { 
                //parse current weather

                WeatherReport result = new WeatherReport();

                var currentWeather =
                    (
                    from x in doc.Descendants("weather")
                    let xElement = x.Element("current")
                    select
                        new
                        {
                            temp = xElement.Attribute("temperature").Value,
                            text = xElement.Attribute("skytext").Value,
                            skycode = xElement.Attribute("skycode").Value
                        }
                    ).FirstOrDefault();


                if (currentWeather != null)
                {
                    result.NowTemp = Convert.ToInt32(currentWeather.temp);
                    result.NowSky = currentWeather.text;
                    result.NowSkyCode = GetWeatherPic(Convert.ToInt32(currentWeather.skycode), 4, 22);
                }


                result.Location = doc.Descendants("weather").FirstOrDefault().Attribute("weatherlocationname").Value;

                //parse forecast
                var days = from x in doc.Descendants("forecast")
                       select
                           new
                           {
                               l = x.Attribute("low").Value,
                               h = x.Attribute("high").Value,
                               skycode = x.Attribute("skycodeday").Value,
                               text = x.Attribute("skytextday").Value
                           };

                List<DayForecast> f = new List<DayForecast>();
                foreach (var d in days)
                {
                    f.Add(new DayForecast());
                    f[f.Count - 1].HighTemperature = Convert.ToInt32(d.h);
                    f[f.Count - 1].LowTemperature = Convert.ToInt32(d.l);
                    f[f.Count - 1].Text = d.text;
                    f[f.Count - 1].SkyCode = GetWeatherPic(Convert.ToInt32(d.skycode), -1, 25);
                }

                result.Forecast = f;

                return result;
            }
            else
            {
                return null;
            }
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