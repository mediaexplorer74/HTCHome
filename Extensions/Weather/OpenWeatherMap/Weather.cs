// OpenWeatherMap
// https://api.openweathermap.org/data/2.5/weather?q=London&lang=en&appid=e60a227a4667049d23504904815bdd54
// https://api.openweathermap.org/data/2.5/weather?q=Samara&lang=ru&appid=e60a227a4667049d23504904815bdd54
// https://api.openweathermap.org/data/2.5/forecast?q=Moscow&exclude=current,minutely,hourly,alerts&appid=e60a227a4667049d23504904815bdd54&units=metric&lang=ru&mode=xml&cnt=5


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
using OpenWeatherMap.Models;
using Newtonsoft.Json;

namespace OpenWeatherMap
{
    public class Weather : IWeatherProvider
    {
        // Request Current location (by City name)
        private const string CurrentForLocation = "https://api.openweathermap.org/data/2.5/weather?q={0}&mode=xml&appid=e60a227a4667049d23504904815bdd54";

        // Request Current Weather of selected location (by City name and Metric/Imperial measure unit)
        private const string CurrentForCelsius = "https://api.openweathermap.org/data/2.5/weather?q={1}&mode=xml&lang={0}&units=metric&appid=e60a227a4667049d23504904815bdd54";
        private const string RequestForFahrenheit = "https://api.openweathermap.org/data/2.5/weather?q={1}&mode=xml&lang={0}&units=imperial&appid=e60a227a4667049d23504904815bdd54";


        // Request Weather Forecast of selected  location  (by City name and Metric/Imperial measure unit)
        private const string ForecastForCelsius = "https://api.openweathermap.org/data/2.5/forecast?q={1}&exclude=current,minutely,hourly,alerts&appid=e60a227a4667049d23504904815bdd54&units=metric&lang={0}&mode=xml";
        private const string ForecastForFahrenheit = "https://api.openweathermap.org/data/2.5/forecast?q={1}&exclude=current,minutely,hourly,alerts&appid=e60a227a4667049d23504904815bdd54&units=imperial&lang={0}&mode=xml";


        // GetCoordinates
        public Coordinates GetCoordinates(string locationCode)
        {
            var f = new NumberFormatInfo {CurrencyDecimalSeparator = "."};

            var reader = new XmlTextReader(string.Format(ForecastForCelsius, "", locationCode));

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

            List<CityLocation> result = new List<CityLocation>();
            //{ new CityLocation { City = "London", Code = "Paris" } };

            //TEMP: RnD only.
            //return result;

            string ns = string.Format(CurrentForLocation, s);
            //string ns = "https://api.openweathermap.org/data/2.5/weather?q={s}&appid=e60a227a4667049d23504904815bdd54&mode=xml";

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
                            if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("city"))
                            {
                                reader.MoveToAttribute("name");
                                l.City = reader.Value;
                                reader.MoveToAttribute("id");
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


        // Converts String to Int (trims leading . and ,)
        public int SpecStrToInt(string s)
        {

            string NTemp;
            char Ch1 = '.';
            char Ch2 = ',';
            int indexOfCh = 0;

            indexOfCh = s.IndexOf(Ch1);

            if (indexOfCh > 0)
            {
                NTemp = s.Substring(0, indexOfCh);
            }
            else
            {
                NTemp = s;
            }


            indexOfCh = NTemp.IndexOf(Ch2);

            if (indexOfCh > 0)
            {
                NTemp = NTemp.Substring(0, indexOfCh);
            }

            int NT = Convert.ToInt32(NTemp);
            return NT;
        }
        WeatherReport IWeatherProvider.GetWeatherReport(string locale, string locationcode, int degreeType)
        {
            string localecode = "en";
            if (locale == "ru-RU")
            {
                localecode = "ru";
            }


            //string url = string.Format(degreeType == 0 ? CurrentForCelsius : CurrentForFahrenheit, localecode, locationcode);
            string url = string.Format(degreeType == 0 ? ForecastForCelsius : ForecastForFahrenheit, localecode, locationcode);

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

                var currentWeather1 =
                    (
                    from x in doc.Descendants("time")
                    let xElement = x.Element("temperature")
                    select
                        new
                        {
                            temp = xElement.Attribute("value").Value,
                            unit = xElement.Attribute("unit").Value,
                            maxtemp = xElement.Attribute("max").Value
                        }
                    ).FirstOrDefault();

                // get current temperature
                string currentWeatherTemp = currentWeather1.temp;

                var currentWeather2 =
                   (
                   from x in doc.Descendants("time")
                   let xElement = x.Element("symbol")
                   select
                       new
                       {
                           skycode = xElement.Attribute("number").Value,
                           skyname = xElement.Attribute("name").Value,
                           icon = xElement.Attribute("var").Value
                       }
                   ).FirstOrDefault();

                // get current skycode
                int currentWeatherSkyCode = Convert.ToInt32(currentWeather2.skycode);
                
                // get current skyname
                string currentWeatherSkyName = currentWeather2.skyname;


                if (currentWeatherTemp != null)
                {
                    int NTemp = SpecStrToInt(currentWeatherTemp);                  
                                       

                    result.NowTemp = NTemp;//(Math.Round(Convert.ToDouble(NTemp), 0));
                    result.NowSky = currentWeatherSkyName;//.text;
                    result.NowSkyCode = currentWeatherSkyCode;//GetWeatherPic(Convert.ToInt32(Math.Round(Convert.ToDouble(currentWeather.skycode.Replace(".",",")))), 4, 22);
                }


                result.Location = locationcode;//doc.Descendants("city").FirstOrDefault().Attribute("name").Value;

                //parse forecast
               
                var Days1 = (
                   from x in doc.Descendants("time")
                   let xElement = x.Element("temperature")
                   select
                       new
                       {
                           temp = xElement.Attribute("value").Value,
                           text = xElement.Attribute("unit").Value,
                           maxtemp = xElement.Attribute("max").Value,
                           lowtemp = xElement.Attribute("min").Value
                       }
                   );

                var Days2 = (
                   from x in doc.Descendants("time")
                   let xElement = x.Element("symbol")
                   select
                       new
                       {
                           skycode = xElement.Attribute("number").Value,
                           skyname = xElement.Attribute("name").Value,
                           icon = xElement.Attribute("var").Value
                       }
                   );


                List<DayForecast> f = new List<DayForecast>();

                int counter1 = 0;
                foreach (var d1 in Days1)
                {
                    // Пропускаем "зачетверенные" данные по правилу "каж. день лишь 1 данные"  
                    if (counter1 % 4 == 0)
                    {

                        f.Add(new DayForecast());


                        f[f.Count - 1].HighTemperature = SpecStrToInt(d1.maxtemp);//Convert.ToInt32(d.maxtemp);

                        f[f.Count - 1].LowTemperature = SpecStrToInt(d1.lowtemp);// Convert.ToInt32(d.lowtemp);

                        //f[f.Count - 1].Text = d1.text;

                        // f[f.Count - 1].SkyCode = GetWeatherPic(SpecStrToInt(Days2[counter1].skycode), -1, 25);

                        int counter2 = 0;

                        foreach (var d2 in Days2)
                        {
                            if (counter1 == counter2)
                            {
                                f[f.Count - 1].Text = d2.skyname;
                                
                                f[f.Count - 1].SkyCode = GetWeatherPic(SpecStrToInt(d2.skycode), -1, 25);

                                break;
                            }
                            counter2++;
                        }
                    }

                    counter1++;
                }

                result.Forecast = f;

                return result;
            }
            else
            {
                return null;
            }

        }//GetWeatherReport


        // GetWeatherPic
        public static int GetWeatherPic(int skycode, int sunrise, int sunset)
        {
            switch (skycode)
            {
                case 804:
                case 26:
                    if (DateTime.Now.Hour >= sunset || DateTime.Now.Hour <= sunrise)
                        return 34;
                    else
                        return 2;
                case 803:
                case 27:
                    if (DateTime.Now.Hour >= sunset || DateTime.Now.Hour <= sunrise)
                        return 35;
                    else
                        return 3;
                case 802:
                case 28:
                    if (DateTime.Now.Hour >= sunset || DateTime.Now.Hour <= sunrise)
                        return 38;
                    else
                        return 6;
                case 801:
                case 35:
                case 39:
                    return 12;
               
                case 45:
                case 46:
                    return 8;
                case 799:
                case 19:
                case 20:
                case 21:
                case 22:
                    if (DateTime.Now.Hour >= sunset || DateTime.Now.Hour <= sunrise)
                        return 37;
                    else
                        return 11;
                case 798:
                case 29:
                case 30:
                    if (DateTime.Now.Hour >= sunset || DateTime.Now.Hour <= sunrise)
                        return 35;
                    else
                        return 3;
                case 797:
                case 33:
                    if (DateTime.Now.Hour >= sunset || DateTime.Now.Hour <= sunrise)
                        return 38;
                    else
                        return 6;
                case 796:
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

                case 800: // ясно
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
            }//switch

        }//GetWeatherPic

    }//class end

}//namespace end