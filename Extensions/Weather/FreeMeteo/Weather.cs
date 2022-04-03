using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeatherClockWidget;
using WeatherClockWidget.Domain;
using System.Xml.Linq;
using System.Xml;
using System.Text.RegularExpressions;
using System.IO;
using HTCHome.Core;
using System.Globalization;
using System.Collections;

namespace FreeMeteo
{
    #region Citydata
    public class Citydata
    {
        public Int32 ID;
        public String Name;
        public String FullName;
        public Double Lat;
        public Double Lon;
        public Int32 Height;
        public override string ToString()
        {
            return String.Format("{0}\t{1}\t{2}\t{3}\t{4}", ID, FullName, Lat, Lon, Height);
        }
    }
    #endregion

    public class Weather : IWeatherProvider
    {
        #region Languages
        enum Languages
        {
            Arabic = 19,
            Bulgarian = 22,
            Catalan = 24,
            Chinese = 7,
            Croatian = 21,
            Czech = 12,
            Danish = 8,
            Dutch = 11,
            English = 1,
            Finnish = 10,
            French = 6,
            German = 3,
            Greek = 2,
            Hungarian = 16,
            Italian = 13,
            Norwegian = 9,
            Polish = 20,
            Portuguese = 18,
            Romanian = 15,
            Russian = 14,
            Serbian = 23,
            Spanish = 4,
            Swedish = 5,
            Turkish = 17
        }
        #endregion

        #region GetLaguageId
        Int32 GetLaguageId()
        {
            CultureInfo ci = new CultureInfo(Widget.LocaleManager.LocaleCode);
            foreach (string l in Enum.GetNames(typeof(Languages)))
                if (ci.EnglishName.ToLower().IndexOf(l.ToLower()) == 0)
                    return Convert.ToInt32((Languages)Enum.Parse(typeof(Languages), l, true));
            return 1;
        }
        #endregion

        private const string RequestForCelsiusCurent = "http://freemeteo.com/default.asp?pid=15&gid={0}&la={1}&sub_units=1";
        private const string RequestForFahrenheitCurent = "http://freemeteo.com/default.asp?pid=15&gid={0}&la={1}&sub_units=2";
        private const string RequestForCelsiusForecast = "http://freemeteo.com/default.asp?pid=23&gid={0}&la={1}&sub_units=1";
        private const string RequestForFahrenheitForecast = "http://freemeteo.com/default.asp?pid=23&gid={0}&la={1}&sub_units=2";
        private const string RequestForLocation = "http://freemeteo.com/ajax/ajaxProxy.asp?c=PointFinder&m=getContent&page={2}&Country=0&City={0}&isP=0&sortBy=7&sortOrder=desc&la={1}&ajax=1";
        private const string UrlDetail = "http://freemeteo.com/default.asp?pid=22&la={1}&gid={0}&nDate={2}";
        NumberFormatInfo nfi = new NumberFormatInfo() { CurrencyDecimalSeparator = "." };

        #region GetCoordinates
        /// <summary>
        /// Получение координат
        /// </summary>
        /// <param name="locationCode">Код населенного пункта</param>
        /// <returns>Координаты</returns>
        public Coordinates GetCoordinates(string locationCode)
        {
            var coord = new Coordinates();
            try
            {
                String SearchUrl = String.Format(RequestForCelsiusCurent, locationCode, 1);
                String sData = HTCHome.Core.GeneralHelper.GetXml(SearchUrl);
                Regex coordRe = new Regex("<span class=\"location\">[^:]+:\\s*(\\-*[\\d]+\\.[\\d]+)\\s*[^:]+:\\s*(\\-*[\\d]+\\.[\\d]+)", RegexOptions.IgnoreCase);
                if (coordRe.IsMatch(sData))
                {
                    coord.X = Convert.ToDouble(coordRe.Match(sData).Groups[1].Value.Trim(), nfi);
                    coord.Y = Convert.ToDouble(coordRe.Match(sData).Groups[2].Value.Trim(), nfi);
                }
            }
            catch (Exception er)
            {
                HTCHome.Core.Logger.Log("Ошибка получения координат\r\n" + er.ToString());
            }
            return coord;
        }
        #endregion
        #region GetLocation
        /// <summary>
        /// Получение списка населенных пунктов
        /// </summary>
        /// <param name="query">Текст для поиска</param>
        /// <returns>Список населенных пунктов</returns>
        public List<CityLocation> GetLocation(string query)
        {
            List<CityLocation> CityLocationLsit = new List<CityLocation>();
            try
            {
                #region Предварительные установки
                Int32 language = GetLaguageId();
                Int32 page = 1;
                String reListTXT = "<A href=\"default.asp\\?pid=[\\d]+&gid=([\\d]+)&la=[\\d]+[^\"]*\">([^\\<]+)</a>\\s*(<span[^>]+>[^<]+</span>)*\\s*</td>\\s*" +
                    "<td class=\"results_blue\" title=\"([^\"]*)\"[^>]*>([^\\<]+)</td>\\s*" +
                    "<td class=\"results_blue\"[^>]*>([^\\<]+)</td>\\s*" +
                    "<td class=\"results_blue_sm\"[^>]*>([^\\<]+)</td>\\s*" +
                    "<td class=\"results_blue_sm\"[^>]*>([^\\<]+)</td>\\s*" +
                    "<td class=\"results_blue_sm_r\"[^>]*>([^\\<]+)</td>\\s*";
                Regex reList = new Regex(reListTXT, RegexOptions.IgnoreCase);
                Regex rePages = new Regex("<font class=\"nav_num\">\\s*[\\d]+/([\\d]+)\\s*</font>", RegexOptions.IgnoreCase);
                List<Citydata> cList = new List<Citydata>();
                #endregion
                #region Данные с первой страницы
                String SearchUrl = String.Format(RequestForLocation, Helper.UrlEncode(query), language.ToString(), page.ToString());
                String sData = HTCHome.Core.GeneralHelper.GetXml(SearchUrl);
                if (reList.IsMatch(sData))
                    foreach (Match m in reList.Matches(sData))
                        cList.Add(
                            new Citydata()
                            {
                                ID = Convert.ToInt32(m.Groups[1].Value.Trim()),
                                Name = m.Groups[2].Value.Trim(),
                                FullName = String.Format("{0} - {1}/{3}", m.Groups[2].Value.Trim(), m.Groups[4].Value.Trim(), m.Groups[5].Value.Trim(), m.Groups[6].Value.Trim()),
                                Lat = Convert.ToDouble(m.Groups[7].Value.Trim(), nfi),
                                Lon = Convert.ToDouble(m.Groups[8].Value.Trim(), nfi),
                                Height = Convert.ToInt32(m.Groups[9].Value.Trim())
                            });
                #endregion
                #region Если есть еще страницы, то их собираем
                if (rePages.IsMatch(sData))
                {
                    Int32 pages = Convert.ToInt32(rePages.Match(sData).Groups[1].Value.Trim());
                    for (Int32 p = page; p < pages; p++)
                    {
                        SearchUrl = String.Format(RequestForLocation, Helper.UrlEncode(query), language.ToString(), page.ToString());
                        sData = HTCHome.Core.GeneralHelper.GetXml(SearchUrl);
                        if (reList.IsMatch(sData))
                            foreach (Match m in reList.Matches(sData))
                                cList.Add(
                                    new Citydata()
                                    {
                                        ID = Convert.ToInt32(m.Groups[1].Value.Trim()),
                                        Name = m.Groups[2].Value.Trim(),
                                        //FullName = String.Format("{0} - {3}/{1}({2})", m.Groups[2].Value.Trim(), m.Groups[4].Value.Trim(), m.Groups[5].Value.Trim(), m.Groups[6].Value.Trim()),
                                        FullName = String.Format("{0} - {1}/{3}", m.Groups[2].Value.Trim(), m.Groups[4].Value.Trim(), m.Groups[5].Value.Trim(), m.Groups[6].Value.Trim()),
                                        Lat = Convert.ToDouble(m.Groups[7].Value.Trim(), nfi),
                                        Lon = Convert.ToDouble(m.Groups[8].Value.Trim(), nfi),
                                        Height = Convert.ToInt32(m.Groups[9].Value.Trim())
                                    });
                    }
                }
                #endregion
                foreach (Citydata cd in cList)
                    CityLocationLsit.Add(new CityLocation() { Code = cd.ID.ToString(), City = cd.FullName });
            }
            catch(Exception er)
            {
                HTCHome.Core.Logger.Log("Ошибка получения данных о населенном пункте\r\n" + er.ToString());
            }
            return CityLocationLsit;
        }
        #endregion
        #region GetWeatherReport
        public WeatherReport GetWeatherReport(string locale, string locationcode, int degreeType)
        {
            WeatherReport report = new WeatherReport();
            try
            {
                Int32 language = GetLaguageId();
                #region Текущая погода
                String SearchUrl = String.Format(degreeType == 0 ? RequestForCelsiusCurent : RequestForFahrenheitCurent, locationcode, language);
                String sData = HTCHome.Core.GeneralHelper.GetXml(SearchUrl);
                if (string.IsNullOrEmpty(sData))
                    return null;

                Regex name = new Regex("<DIV class=report_img_new_city>(.+?)</div>", RegexOptions.IgnoreCase);
                Regex re_tags = new Regex("</?[^>]+>", RegexOptions.IgnoreCase);
                Regex coord = new Regex("<span class=\"location\">[^:]+:\\s*(\\-*[\\d]+\\.[\\d]+)\\s*[^:]+:\\s*(\\-*[\\d]+\\.[\\d]+)", RegexOptions.IgnoreCase);
                Regex icon = new Regex("\\.\\./templates/default/icons/([^\\.]+).swf", RegexOptions.IgnoreCase);
                Regex temp = new Regex(degreeType == 0 ? "<FONT class=temperature>([^<]+)<" : "<FONT class=temperature>(\\d+)\\s*&deg;F<", RegexOptions.IgnoreCase);
                Regex wdesc = new Regex("<DIV class=report_img_new_bottom>([^<]+)<", RegexOptions.IgnoreCase);
                if (name.IsMatch(sData))
                    report.Location = re_tags.Replace(name.Match(sData).Groups[1].Value.Trim(), String.Empty).Split(',')[0].Trim();

                Double Lat = Double.MinValue;
                Double Lon = Double.MinValue;
                if (coord.IsMatch(sData))
                {
                    Lat = Convert.ToDouble(coord.Match(sData).Groups[1].Value.Trim(), nfi);
                    Lon = Convert.ToDouble(coord.Match(sData).Groups[2].Value.Trim(), nfi);
                }
                if (temp.IsMatch(sData))
                    report.NowTemp = Convert.ToInt32(temp.Match(sData).Groups[1].Value.Trim());
                if (wdesc.IsMatch(sData))
                    report.NowSky = wdesc.Match(sData).Groups[1].Value.Trim();
                if (icon.IsMatch(sData))
                {
                    DateTime sunrise = DateTime.Today.AddHours(4);
                    DateTime sunset = DateTime.Today.AddHours(22);
                    if (Lat != Double.MinValue && Lon != Double.MinValue)
                    {
                        SunCalculator sc = new SunCalculator(DateTime.Today, Lat, Lon);
                        sunrise = sc.DSunRise;
                        sunset = sc.DSunSet;
                    }
                    report.NowSkyCode = GetWeatherPic(icon.Match(sData).Groups[1].Value.Trim(), sunrise, sunset);
                }
                report.Url = SearchUrl;
                #endregion
                #region Пргноз
                SearchUrl = String.Format(degreeType == 0 ? RequestForCelsiusForecast : RequestForFahrenheitForecast, locationcode, language);
                sData = Helper.HtmlDecode(HTCHome.Core.GeneralHelper.GetXml(SearchUrl));

                String reTxt = "<img src=\"../templates/default/iconsgif/([\\d]+).gif\" width=\"50\" height=\"41\"></TD>\\s*<TD class=\"tbl_stations_content\">" +
                    "([^<]+)<br>\\s*<span\\s*style=\"text-align:right;width:100\\%;\">\\s*<a[^>]+>[^<]+</a></span></TD>\\s*" +
                    "<TD class=\"tbl_stations_content\"\\s*NOWRAP><b>[^:]+:([^<]+)<sup>o</sup>C\\s*</b><BR>[^:]+:([^<]+)<sup>o</sup>C\\s*</TD>";
                String reTxtF = "<img src=\"../templates/default/iconsgif/([\\d]+).gif\" width=\"50\" height=\"41\"></TD>\\s*<TD class=\"tbl_stations_content\">" +
                    "([^<]+)<br>\\s*<span\\s*style=\"text-align:right;width:100\\%;\">\\s*<a[^>]+>[^<]+</a></span></TD>\\s*" +
                    "<TD class=\"tbl_stations_content\"\\s*NOWRAP><b>[^:]+:\\s*([\\d]+)\\s*°F\\s*</b><BR>[^:]+:\\s*([\\d]+)\\s*°F\\s*</TD>";

                Regex reForecast = new Regex(degreeType == 0 ? reTxt : reTxtF, RegexOptions.IgnoreCase);
                if (reForecast.IsMatch(sData))
                {
                    Int32 num = 0;
                    foreach (Match m in reForecast.Matches(sData))
                    {
                        if (num == 0)
                        {
                            report.High = Convert.ToInt32(m.Groups[3].Value.Trim());
                            report.Low = Convert.ToInt32(m.Groups[4].Value.Trim());
                        }
                        report.Forecast.Add(
                            new DayForecast() 
                            {
                                Url = String.Format(UrlDetail, locationcode, language, num), 
                                Text = m.Groups[2].Value.Trim().Replace("  ", " "), 
                                SkyCode = GetWeatherPic(m.Groups[1].Value.Trim(), DateTime.MinValue, DateTime.MaxValue), 
                                HighTemperature = Convert.ToInt32(m.Groups[3].Value.Trim()), 
                                LowTemperature = Convert.ToInt32(m.Groups[4].Value.Trim()) 
                            });
                        num++;
                        if (num == 5) break;
                    }
                }
                #endregion
                return report;
            }
            catch (Exception er)
            {
                HTCHome.Core.Logger.Log("Ошибка получения прогноза\r\n" + er.ToString());
                return null;
            }
        }
        #endregion
        #region GetWeatherPic
        private int GetWeatherPic(string icon, DateTime sunrise, DateTime sunset)
        {
            if (String.IsNullOrEmpty(icon)) return 1;
            icon = icon.ToLower().Replace("n", String.Empty);
            bool isDay = DateTime.Now > sunrise && DateTime.Now < sunset;
            #region Fog
            switch (icon)
            {
                case "1f": { return isDay ? 5 : 37; }
                case "3f": 
                case "4f": { return isDay ? 11 : 11; }
                default: break;
            }
            icon = icon.ToLower().Replace("f", String.Empty);
            #endregion
            switch (Convert.ToInt32(icon))
            {
                case 1: { return isDay ? 1 : 33; }
                case 2: { return isDay ? 2 : 34; }
                case 3: { return isDay ? 6 : 38; }
                case 4: { return isDay ? 7 : 8; }
                case 5: { return isDay ? 14 : 39; }
                case 6:
                case 7:
                case 8: { return isDay ? 12 : 18; }
                case 9:
                case 10:
                case 11:
                case 16:
                case 17:
                case 22:
                case 23:
                case 28:
                case 29:
                case 39:
                case 44:
                case 49: { return isDay ? 15 : 15; }
                case 12:
                case 13:
                case 18:
                case 19:
                case 35:
                case 36:
                case 40:
                case 41: { return isDay ? 26 : 26; }
                case 14:
                case 15:
                case 20:
                case 21:
                case 37:
                case 38:
                case 42:
                case 43: { return isDay ? 29 : 29; }
                case 24:
                case 25: { return isDay ? 24 : 24; }
                case 26:
                case 27: { return isDay ? 22 : 22; }
                case 30:
                case 31:
                case 32:
                case 33: { return isDay ? 13 : 40; }
                case 34: { return isDay ? 16 : 42; }
                case 45:
                case 46: { return isDay ? 21 : 43; }
                case 47:
                case 48: { return isDay ? 23 : 44; }
                default: break;
            }
            return 1;
        }
        #endregion
    }
}
