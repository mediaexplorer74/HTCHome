using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeatherClockWidget;
using System.Globalization;
using WeatherClockWidget.Domain;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using HTCHome.Core;

namespace Gismeteo
{
    public class tmpForecast : IComparable
    {
        public DateTime dt;
        public String img = String.Empty;
        public String desc = String.Empty;
        public Int32 temp;

        public override string ToString()
        {
            return dt.ToString("dd.MM HH") + " temp:" + temp + ", " + desc + " (" + img + ")";
        }

        public int CompareTo(object obj)
        {
            tmpForecast othert_tmpForecast = obj as tmpForecast;
            if (othert_tmpForecast != null)
                return this.dt.CompareTo(othert_tmpForecast.dt);
            else
                throw new ArgumentException("Object is not a tmpForecast");
        }
    }

    public class Weather : IWeatherProvider
    {
        public WeatherClockWidget.Domain.Coordinates GetCoordinates(string locationCode)
        {
            var coord = new Coordinates() { X = 0, Y = 0 };
            //try { } catch (Exception er) { HTCHome.Core.Logger.Log("Failed to get coordinates!\r\n" + er.ToString()); }
            return coord;
        }

        #region GetLocation
        public List<WeatherClockWidget.Domain.CityLocation> GetLocation(string Query)
        {
            List<CityLocation> CityLocationLsit = new List<CityLocation>();
            if (String.IsNullOrEmpty(Query)) return CityLocationLsit;
            Query = Query.Trim();
            if (String.IsNullOrEmpty(Query)) return CityLocationLsit;
            try
            {
                String JQData = HTCHome.Core.GeneralHelper.GetXml("http://www.gismeteo.ru/ajax/city_search/?searchQuery=х" + Query);
                Regex jqList = new Regex("\"(\\d+)\":\"([^\"]+)\"", RegexOptions.IgnoreCase);
                foreach (Match m in jqList.Matches(new Regex("</?[^>]+>").Replace(JQData, String.Empty)))
                    CityLocationLsit.Add(new CityLocation() { Code = "gis|" + m.Groups[1].Value.Trim(), City = Regex.Unescape(m.Groups[2].Value).Trim() });
            }
            catch (Exception er)
            {
                HTCHome.Core.Logger.Log("Failed to get data on location!\r\n" + er.ToString());
            }
            return CityLocationLsit;
        }
        #endregion

        #region GetWeatherReport
        public WeatherClockWidget.Domain.WeatherReport GetWeatherReport(string locale, string locationcode, int degreeType)
        {
            WeatherReport report = new WeatherReport();
            try
            {
                locationcode = locationcode.IndexOf("gis|") == 0 ? locationcode.Split('|')[1] : String.Empty;
                bool isMetric = degreeType == 0;
                Uri baseUrl = new Uri("http://www.gismeteo.ru");
                if (String.IsNullOrEmpty(locationcode))
                {
                    String RData = Helper.HtmlDecode(GeneralHelper.GetXml(baseUrl.ToString()));
                    Regex reCode = new Regex("<div class=\"wrap f_link\">\\s*<span.+?>.+?</span>\\s*<a.+?href=\"/city/daily/(\\d+)/\">.+?</a>\\s*</div>", RegexOptions.IgnoreCase);
                    Regex reCode1 = new Regex("<div class=\"wrap f_link\">\\s*<a.+?href=\"/city/daily/(\\d+)/\">.+?</a>\\s*</div>", RegexOptions.IgnoreCase);
                    if (reCode.IsMatch(RData)) locationcode = reCode.Match(RData).Groups[1].Value;
                    else if (reCode1.IsMatch(RData)) locationcode = reCode1.Match(RData).Groups[1].Value;
                }
                if (String.IsNullOrEmpty(locationcode))
                {
                    //return NY
                    return report;
                }

                String frm = baseUrl.ToString() + "/city/hourly/" + locationcode + "/";
                String rDataFill = Helper.HtmlDecode(GeneralHelper.GetXml(frm));
                String rData1Fill = Helper.HtmlDecode(GeneralHelper.GetXml(frm + "2/"));

                Regex locName = new Regex("<h3 class=\"type\\w{1}\">([^>]+)</h3>", RegexOptions.IgnoreCase);
                if (locName.IsMatch(rDataFill))
                    report.Location = locName.Match(rDataFill).Groups[1].Value.Trim();

                Regex curent = new Regex("<dl class=\"cloudness\">\\s*<dt.+?new/(.+?).png\\)\"><br></dt>\\s*<dd>(.+?)</dd>\\s*</dl>\\s*<div class=\"temp\">([^°]+)°C</div>", RegexOptions.IgnoreCase);
                if (curent.IsMatch(rDataFill))
                {
                    report.NowSky = GetWeatherDesc(curent.Match(rDataFill).Groups[1].Value, new CultureInfo(Widget.LocaleManager.LocaleCode)) != String.Empty ?
                        GetWeatherDesc(curent.Match(rDataFill).Groups[1].Value, new CultureInfo(Widget.LocaleManager.LocaleCode)) :
                        curent.Match(rDataFill).Groups[2].Value;
                    report.NowSkyCode = GetWeatherPic(curent.Match(rDataFill).Groups[1].Value);
                    report.NowTemp = Convert.ToInt32(curent.Match(rDataFill).Groups[3].Value.Trim());
                    report.Url = baseUrl + "/city/daily/" + locationcode + "/";
                }

                List<tmpForecast> fList = new List<tmpForecast>();

                //Regex list = new Regex("Local: ([^\"]+)\">[^<]*</th>\\s*<td class=\"clicon\">\\s*<img.+?new/(.+?).png\"[^>]*>\\s*</td>\\s*<td class=\"cltext\">([^<]+)</td>\\s*<td class=\"temp\">([^°]+)°</td>", RegexOptions.IgnoreCase);
                Regex list = new Regex("<tr class=\"wrow[^\"]*\" id=\"wrow-([^\"]+)\">\\s*<th[^>]*>[^<]*</th>\\s*<td class=\"clicon\">\\s*<img.+?new/(.+?).png\"[^>]*>\\s*</td>\\s*<td class=\"cltext\">([^<]+)</td>\\s*<td class=\"temp\">([^°]+)°</td>", RegexOptions.IgnoreCase);
                foreach (Match m in list.Matches(rDataFill))
                    fList.Add(new tmpForecast() { dt = Convert.ToDateTime(m.Groups[1].Value.Substring(0, m.Groups[1].Value.LastIndexOf('-')) + " " + m.Groups[1].Value.Substring(m.Groups[1].Value.LastIndexOf('-') + 1) + ":00", new CultureInfo("ru-RU")), img = m.Groups[2].Value.Trim(), desc = m.Groups[3].Value.Trim(), temp = Convert.ToInt32(m.Groups[4].Value.Trim()) });
                foreach (Match m in list.Matches(rData1Fill))
                    fList.Add(new tmpForecast() { dt = Convert.ToDateTime(m.Groups[1].Value.Substring(0, m.Groups[1].Value.LastIndexOf('-')) + " " + m.Groups[1].Value.Substring(m.Groups[1].Value.LastIndexOf('-') + 1) + ":00", new CultureInfo("ru-RU")), img = m.Groups[2].Value.Trim(), desc = m.Groups[3].Value.Trim(), temp = Convert.ToInt32(m.Groups[4].Value.Trim()) });
                fList.Sort();

                for (DateTime dc = DateTime.Today; dc < DateTime.Today.AddDays(5); dc = dc.AddDays(1))
                {
                    DayForecast df = new DayForecast() { HighTemperature = int.MinValue, LowTemperature = int.MaxValue };
                    report.Forecast.Add(df);
                    foreach (tmpForecast tf in fList)
                        if (tf.dt >= dc && tf.dt < dc.AddDays(1))
                        {
                            if (df.HighTemperature < tf.temp) df.HighTemperature = tf.temp;
                            if (df.LowTemperature > tf.temp) df.LowTemperature = tf.temp;
                            if (dc.AddHours(13) <= tf.dt && dc.AddHours(16) > tf.dt)
                            {
                                df.SkyCode = GetWeatherPic(tf.img);
                                df.Text = GetWeatherDesc(tf.img, new CultureInfo(Widget.LocaleManager.LocaleCode)) != String.Empty ?
                                    GetWeatherDesc(tf.img, new CultureInfo(Widget.LocaleManager.LocaleCode)) : tf.desc;
                            }
                        }
                }

            }
            catch (Exception er)
            {
                HTCHome.Core.Logger.Log("Failed to get forecat!\r\n" + er.ToString());
                return null;
            }
            return report;
        }
        #endregion

        #region GetWeatherPic
        private int GetWeatherPic(string icon)
        {
            try
            {
                icon += ".";
                Regex iconRe = new Regex("(([d|n])\\.)((sun|moon)\\.)(c(\\d{1})\\.)*((s|r)(\\d{1})\\.)*((st)\\.)*", RegexOptions.IgnoreCase);
                Boolean isDay = false;
                Int32 cloud = 0;
                Int32 precipType = 0; //0 -дождь, 2 - снег
                Int32 precipitation = 0;
                Boolean ts = false;
                if (iconRe.IsMatch(icon))
                {
                    isDay = iconRe.Match(icon).Groups[2].Value.ToLower() == "d";
                    if (!String.IsNullOrEmpty(iconRe.Match(icon).Groups[6].Value))
                        cloud = Convert.ToInt32(iconRe.Match(icon).Groups[6].Value.ToLower());
                    if (!String.IsNullOrEmpty(iconRe.Match(icon).Groups[9].Value))
                        precipitation = Convert.ToInt32(iconRe.Match(icon).Groups[9].Value.ToLower());
                    precipitation = iconRe.Match(icon).Groups[11].Value.ToLower() != String.Empty ? 4 : precipitation;
                    precipType = iconRe.Match(icon).Groups[8].Value.ToLower() == "s" ? 2 : precipType;
                    Int32 code = cloud * 100 + precipitation * 10 + precipType;
                    //Облачность 0-Ясно, 1-Небольшая облачность, 2-Переменная облачность, 3-Облачно с прояснениями, 4-Облачно
                    //Сила осадков 0-нет, 1-слабый, 2-тип, 3-сильный, 4-гроза
                    //Тип осадков 0-дождь,1-осадки 2-снег
                    isDay = icon.ToLower()[0] == 'd';
                    switch (code)
                    {
                        case 0: return isDay ? 1 : 33; //Ясно
                        case 100: return isDay ? 2 : 34; //Небольшая облачность
                        case 200: return isDay ? 3 : 35; //Переменная облачность
                        case 210: return isDay ? 14 : 39; //Переменная облачность, слабый дождь
                        case 211: return isDay ? 26 : 26; //Переменная облачность, слабые осадки
                        case 212: return isDay ? 21 : 44; //Переменная облачность, слабый снег
                        case 220: return isDay ? 14 : 39; //Переменная облачность, дождь
                        case 221: return isDay ? 26 : 26; //Переменная облачность, осадки
                        case 222: return isDay ? 20 : 43; //Переменная облачность, снег
                        case 240: return isDay ? 17 : 41; //Переменная облачность, гроза
                        case 300: return isDay ? 6 : 38; //Облачно с прояснениями
                        case 310: return isDay ? 13 : 40; //Облачно с прояснениями, слабый дождь
                        case 311: return isDay ? 26 : 26; //Облачно с прояснениями, слабый осадки
                        case 312: return isDay ? 21 : 44; //Облачно с прояснениями, слабый снег
                        case 320: return isDay ? 13 : 40; //Облачно с прояснениями, дождь
                        case 321: return isDay ? 26 : 26; //Облачно с прояснениями, осадки
                        case 322: return isDay ? 20 : 43; //Облачно с прояснениями, снег
                        case 340: return isDay ? 16 : 42; //Облачно с прояснениями, гроза
                        case 400: return isDay ? 7 : 8; //Облачно
                        case 410: return isDay ? 12 : 12; //Облачно, слабый дождь
                        case 411: return isDay ? 26 : 26; //Облачно, слабый осадки
                        case 412: return isDay ? 24 : 24; //Облачно, слабый снег
                        case 420: return isDay ? 12 : 12; //Облачно, дождь
                        case 421: return isDay ? 26 : 26; //Облачно, осадки
                        case 422: return isDay ? 19 : 19; //Облачно, снег
                        case 430: return isDay ? 18 : 18; //Облачно, сильный дождь
                        case 431: return isDay ? 26 : 26; //Облачно, сильные осадки
                        case 432: return isDay ? 22 : 22; //Облачно, сильный снег
                        case 440: return isDay ? 15 : 15; //Облачно, гроза
                        default: return 1;
                    }
                }
                return 1;
            }
            catch { return 1; }
        }
        #endregion

        private String GetWeatherDesc(string icon, CultureInfo ci)
        {
            return String.Empty;
        }
    }
}
