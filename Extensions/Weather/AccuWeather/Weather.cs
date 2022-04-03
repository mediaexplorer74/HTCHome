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
using System.Net;
using System.Collections.Specialized;

namespace AccuWeather
{
    #region IntLangZ
    public class IntLangZ
    {
        public Int32 ID;
        public String Name;
        public CultureInfo ISO;
        public CultureInfo CI;
        public override string ToString()
        {
            String ret = String.Empty;
            ret += ID > 0 ? "ID=" + ID.ToString() : String.Empty;
            ret += (ret != String.Empty ? "&" : String.Empty) + (!String.IsNullOrEmpty(Name) ? "Name=" + Helper.UrlEncode(Name) : String.Empty);
            ret += (ret != String.Empty ? "&" : String.Empty) + (ISO != null ? "ISO=" + ISO.TwoLetterISOLanguageName.ToLower() : String.Empty);
            return ret;
        }
    }
    #endregion

    public class Weather : IWeatherProvider
    {
        #region Language
        IntLangZ Language(CultureInfo ci)
        {
            String nameSmall = ci.TwoLetterISOLanguageName.ToLower();
            String nameFull = ci.IetfLanguageTag.ToLower();
            if (nameSmall == "ca")
                return new IntLangZ()
                {
                    ID = 22,
                    Name = Helper.UrlDecode("Catal%c3%a0"),
                    ISO = new CultureInfo("ca"),
                    CI = new CultureInfo("ca-ES")
                };
            if (nameSmall == "cs")
                return new IntLangZ()
                {
                    ID = 19,
                    Name = Helper.UrlDecode("%c4%8ce%c5%a1tina"),
                    ISO = new CultureInfo("cs"),
                    CI = new CultureInfo("cs-CZ")
                };
            if (nameSmall == "da")
                return new IntLangZ()
                {
                    ID = 4,
                    Name = Helper.UrlDecode("Dansk"),
                    ISO = new CultureInfo("da"),
                    CI = new CultureInfo("da-DK")
                };
            if (nameSmall == "de")
                return new IntLangZ()
                {
                    ID = 9,
                    Name = Helper.UrlDecode("Deutsch"),
                    ISO = new CultureInfo("de"),
                    CI = new CultureInfo("de-DE")
                };
            if (nameFull == "en-gb")
                return new IntLangZ()
                {
                    ID = 28,
                    Name = Helper.UrlDecode("English+(UK)"),
                    ISO = new CultureInfo("en-gb"),
                    CI = new CultureInfo("en-US")
                };
            if (nameFull == "es-ar")
                return new IntLangZ()
                {
                    ID = 15,
                    Name = Helper.UrlDecode("Espa%c3%b1ol"),
                    ISO = new CultureInfo("es-ar"),
                    CI = new CultureInfo("es-AR")
                };
            if (nameFull == "es-mx")
                return new IntLangZ()
                {
                    ID = 16,
                    Name = Helper.UrlDecode("Espa%c3%b1ol+(Latin)"),
                    ISO = new CultureInfo("es-mx"),
                    CI = new CultureInfo("es-MX")
                };
            if (nameSmall == "es")
                return new IntLangZ()
                {
                    ID = 2,
                    Name = Helper.UrlDecode("Castellano"),
                    ISO = new CultureInfo("es"),
                    CI = new CultureInfo("es-ES")
                };
            if (nameFull == "fr-ca")
                return new IntLangZ()
                {
                    ID = 32,
                    Name = Helper.UrlDecode("Fran%c3%a7ais+(Canada)"),
                    ISO = new CultureInfo("fr-ca"),
                    CI = new CultureInfo("fr-FR")
                };
            if (nameSmall == "fr")
                return new IntLangZ()
                {
                    ID = 3,
                    Name = Helper.UrlDecode("Fran%c3%a7ais"),
                    ISO = new CultureInfo("fr"),
                    CI = new CultureInfo("fr-FR")
                };
            if (nameSmall == "it")
                return new IntLangZ()
                {
                    ID = 8,
                    Name = Helper.UrlDecode("Italiano"),
                    ISO = new CultureInfo("it"),
                    CI = new CultureInfo("it-IT")
                };
            if (nameSmall == "hu")
                return new IntLangZ()
                {
                    ID = 20,
                    Name = Helper.UrlDecode("Magyar"),
                    ISO = new CultureInfo("hu"),
                    CI = new CultureInfo("hu-HU")
                };
            if (nameSmall == "nl")
                return new IntLangZ()
                {
                    ID = 6,
                    Name = Helper.UrlDecode("Nederlands"),
                    ISO = new CultureInfo("nl"),
                    CI = new CultureInfo("nl-NL")
                };
            if (nameSmall == "no")
                return new IntLangZ()
                {
                    ID = 7,
                    Name = Helper.UrlDecode("Norsk"),
                    ISO = new CultureInfo("no"),
                    CI = new CultureInfo("nb-NO")
                };
            if (nameSmall == "pl")
                return new IntLangZ()
                {
                    ID = 21,
                    Name = Helper.UrlDecode("Polski"),
                    ISO = new CultureInfo("pl"),
                    CI = new CultureInfo("pl-PL")
                };
            if (nameFull == "pt-br")
                return new IntLangZ()
                {
                    ID = 23,
                    Name = Helper.UrlDecode("Portugu%c3%aas+(Brazil)"),
                    ISO = new CultureInfo("pt-br"),
                    CI = new CultureInfo("pt-BR")
                };
            if (nameSmall == "pt")
                return new IntLangZ()
                {
                    ID = 5,
                    Name = Helper.UrlDecode("Portugu%c3%aas+(Europe)"),
                    ISO = new CultureInfo("pt"),
                    CI = new CultureInfo("pt-PT")
                };
            if (nameSmall == "ro")
                return new IntLangZ()
                {
                    ID = 18,
                    Name = Helper.UrlDecode("Romana"),
                    ISO = new CultureInfo("ro"),
                    CI = new CultureInfo("ro-RO")
                };
            if (nameSmall == "ru")
                return new IntLangZ()
                {
                    ID = 25,
                    Name = Helper.UrlDecode("%d1%80%d1%83%d1%81%d1%81%d0%ba%d0%b8%d0%b9"),
                    ISO = new CultureInfo("ru"),
                    CI = new CultureInfo("ru-RU")
                };
            if (nameSmall == "sv")
                return new IntLangZ()
                {
                    ID = 10,
                    Name = Helper.UrlDecode("Svenska"),
                    ISO = new CultureInfo("sv"),
                    CI = new CultureInfo("sv-SE")
                };
            if (nameSmall == "fi")
                return new IntLangZ()
                {
                    ID = 11,
                    Name = Helper.UrlDecode("Suomi"),
                    ISO = new CultureInfo("fi"),
                    CI = new CultureInfo("fi-FI")
                };
            if (nameSmall == "sk")
                return new IntLangZ()
                {
                    ID = 17,
                    Name = Helper.UrlDecode("Sloven%c4%8dinu"),
                    ISO = new CultureInfo("sk"),
                    CI = new CultureInfo("sk-sk")
                };
            if (nameSmall == "ar")
                return new IntLangZ()
                {
                    ID = 26,
                    Name = Helper.UrlDecode("%d8%b9%d8%b1%d8%a8%d9%8a+(Arabic)"),
                    ISO = new CultureInfo("ar"),
                    CI = new CultureInfo("ar-SA")
                };
            if (nameFull == "zh-cn")
                return new IntLangZ()
                {
                    ID = 13,
                    Name = Helper.UrlDecode("%e4%b8%ad%e6%96%87+(SIM)"),
                    ISO = new CultureInfo("zh-cn"),
                    CI = new CultureInfo("zh-cn")
                };
            if (nameFull == "zh-tw")
                return new IntLangZ()
                {
                    ID = 14,
                    Name = Helper.UrlDecode("%e4%b8%ad%e6%96%87+(Taiwan)"),
                    ISO = new CultureInfo("zh-tw"),
                    CI = new CultureInfo("zh-tw")
                };
            if (nameFull == "zh-hk")
                return new IntLangZ()
                {
                    ID = 12,
                    Name = Helper.UrlDecode("%e4%b8%ad%e6%96%87+(HK)"),
                    ISO = new CultureInfo("zh-hk"),
                    CI = new CultureInfo("en-US")
                };
            //if (nameSmall == "tr")
            //    return new IntLangZ() 
            //    { 
            //        ID = 31, 
            //        Name = Helper.UrlDecode("T%c3%9cRK%c3%87E+(Turkish)"), 
            //        ISO = new CultureInfo("tr"),
            //        CI = new CultureInfo("tr-TR") 
            //    };
            if (nameSmall == "el")
                return new IntLangZ()
                {
                    ID = 27,
                    Name = Helper.UrlDecode("%ce%95%ce%bb%ce%bb%ce%b7%ce%bd%ce%b9%ce%ba%ce%ac+(Greek)"),
                    ISO = new CultureInfo("el"),
                    CI = new CultureInfo("el-GR")
                };
            if (nameSmall == "ja")
                return new IntLangZ()
                {
                    ID = 29,
                    Name = Helper.UrlDecode("%e6%97%a5%e6%9c%ac%e8%aa%9e+(Japanese)"),
                    ISO = new CultureInfo("ja"),
                    CI = new CultureInfo("ja-JP")
                };
            if (nameSmall == "ko")
                return new IntLangZ()
                {
                    ID = 30,
                    Name = Helper.UrlDecode("%ed%95%9c%ea%b5%ad%ec%96%b4+(Korean)"),
                    ISO = new CultureInfo("ko"),
                    CI = new CultureInfo("ko-KR")
                };
            if (nameSmall == "hi")
                return new IntLangZ()
                {
                    ID = 24,
                    Name = Helper.UrlDecode("%e0%a4%b9%e0%a4%bf%e0%a4%a8%e0%a5%8d%e0%a4%a6%e0%a5%80+(Hindi)"),
                    ISO = new CultureInfo("hi"),
                    CI = new CultureInfo("hi-IN")
                };
            if (nameSmall == "he")
                return new IntLangZ()
                {
                    ID = 33,
                    Name = Helper.UrlDecode("%d7%a2%d7%91%d7%a8%d7%99%d7%aa+(Hebrew)"),
                    ISO = new CultureInfo("he"),
                    CI = new CultureInfo("he-IL")
                };
            return new IntLangZ()
            {
                ID = 1,
                Name = Helper.UrlDecode("English+(US)"),
                ISO = new CultureInfo("en-US"),
                CI = new CultureInfo("en-US")
            };
        }
        #endregion

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
                String sData = HTCHome.Core.GeneralHelper.GetXml("http://vwidget.accuweather.com/widget/vista1/weather_data_v2.asp?location="+locationCode);
                XDocument doc = XDocument.Parse(sData, LoadOptions.PreserveWhitespace);
                XElement el = doc.Root.Element(XName.Get("local", doc.Root.Name.Namespace.NamespaceName));
                coord.X = Double.Parse(el.Attribute("lat").Value, nfi);
                coord.Y = Double.Parse(el.Attribute("lon").Value, nfi);
            }
            catch (Exception er)
            {
                HTCHome.Core.Logger.Log("Failed to get coordinates!\r\n" + er.ToString());
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
                String sData = HTCHome.Core.GeneralHelper.GetXml(String.Format("http://vwidget.accuweather.com/widget/vista1/locate_city.asp?location={0}", query));
                XDocument doc = XDocument.Parse(sData, LoadOptions.PreserveWhitespace);
                foreach (XElement el in doc.Root.Element(XName.Get("citylist", doc.Root.Name.Namespace.NamespaceName)).Elements())
                {
                    CityLocation cl = new CityLocation();
                    cl.Code = el.Attribute("location").Value.Trim();
                    cl.Code = cl.Code[cl.Code.Length - 1] == '|' ? cl.Code.Substring(0, cl.Code.Length - 1) : cl.Code;
                    cl.City = String.Format("{0} - {1}", el.Attribute("city").Value, el.Attribute("state").Value);
                    CityLocationLsit.Add(cl);
                }
            }
            catch(Exception er)
            {
                HTCHome.Core.Logger.Log("Failed to get data on location!\r\n" + er.ToString());
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
                //Начинаем
                IntLangZ ilz = Language(new CultureInfo(Widget.LocaleManager.LocaleCode)); //Информация о языке, для передачи в куки

                Uri url = new Uri("http://www.accuweather.com");
                HttpWebRequest request = null;
                HttpWebResponse response = null;
                string result = String.Empty;

                #region Текущие данные
                url = new Uri("http://www.accuweather.com/quick-look.aspx?loc=" + locationcode);
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Proxy = HTCHome.Core.GeneralHelper.Proxy != null ? HTCHome.Core.GeneralHelper.Proxy : WebRequest.GetSystemWebProxy();
                request.Timeout = 30000; //30 секунд
                //SetBrowserHeaders(request);
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(new Cookie("IntLangZ", ilz.ToString(), "/", "www.accuweather.com")); //Язык
                request.CookieContainer.Add(new Cookie("IntPreferencesZ", String.Format("Units={0}", degreeType == 0 ? 1 : 0), "/", "www.accuweather.com")); //тип измерения температуры

                response = (HttpWebResponse)request.GetResponse();
                #region Полученные куки
                NameValueCollection nvc = new NameValueCollection();
                if (response.Cookies["IntLocZ"] != null)
                {
                    String coocVal = Helper.UrlDecode(response.Cookies["IntLocZ"].Value as String); //"IntLocZ"Version=4&Name=New+York&OfficialName=New+York&Lat=40.749&Lon=-73.994&S=CT_&U=MANH&CountryName=United+States&CountryID=US&AdMinID=NY&AdminName=New+York&AdminOfficialName=New+York&CountryCode=US&TzId=21&StandardGmtO=-5&CurrentGmtO=-5&LookupType=postal&PC=10001&Climo=NYC&CityID=&VideoCode=LGA
                    if (!String.IsNullOrEmpty(coocVal) && coocVal.IndexOf("=") > -1)
                        foreach (String c in coocVal.Split('&')) nvc.Add(c.Split('=')[0], c.Split('=')[1]);
                }
                #endregion
                result = new StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                result = Helper.HtmlDecode(result);
                #region Чистка
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
                if (request != null) request = null;
                #endregion

                #region Название населенного пункта и ссылка на прсмотр прогноза
                Regex locRe = new Regex("<a.+?lnkLocation.+?href=\"([^\"]+)\"[^>]*>([^,]+)[^<]*</a>", RegexOptions.IgnoreCase);
                if (locRe.IsMatch(result))
                {
                    report.Url = locRe.Match(result).Groups[1].Value;
                    report.Location = locRe.Match(result).Groups[2].Value;
                }
                #endregion

                #region Изображение
                Regex imgRe = new Regex("<img.+?imgCurConCondition.+?([\\d]+)_int", RegexOptions.IgnoreCase);
                if (imgRe.IsMatch(result))
                    report.NowSkyCode = Int32.Parse(imgRe.Match(result).Groups[1].Value);
                #endregion

                #region Текущая температура
                Regex tempRe = new Regex("<span.+?lblCurrentTemp.+?>([^°]+)°", RegexOptions.IgnoreCase);
                if (tempRe.IsMatch(result))
                    report.NowTemp = Int32.Parse(tempRe.Match(result).Groups[1].Value);
                #endregion

                #region Описание погоды
                Regex descRe = new Regex("<span.+?lblCurrentText.+?>([^<]+)</span>", RegexOptions.IgnoreCase);
                if (descRe.IsMatch(result))
                    report.NowSky = descRe.Match(result).Groups[1].Value.Trim();
                #endregion

                #region Дата время, восход заход
                //Regex dateRe = new Regex("<span.+?lblDate.+?>([^<]*)</span>", RegexOptions.IgnoreCase);
                //if (dateRe.IsMatch(result))
                //    WriteLine(DateTime.Parse(dateRe.Match(result).Groups[1].Value.Trim(), ilz.CI).ToString());

                //Regex curTimeRe = new Regex("<span.+?lblCurrentTime.+?>([^<]*)</span>", RegexOptions.IgnoreCase);
                //if (curTimeRe.IsMatch(result))
                //    WriteLine(DateTime.Parse(curTimeRe.Match(result).Groups[1].Value.Trim(), ilz.CI).ToString());


                //Regex SRTimeRe = new Regex("<span.+?lblSunRiseValue.+?>([^<]*)</span>", RegexOptions.IgnoreCase);
                //if (SRTimeRe.IsMatch(result))
                //    WriteLine(DateTime.Parse(SRTimeRe.Match(result).Groups[1].Value.Trim(), ilz.CI).ToString());

                //Regex SSTimeRe = new Regex("<span.+?lblSunSetValue.+?>([^<]*)</span>", RegexOptions.IgnoreCase);
                //if (SSTimeRe.IsMatch(result))
                //    WriteLine(DateTime.Parse(SSTimeRe.Match(result).Groups[1].Value.Trim(), ilz.CI).ToString());
                #endregion
                #endregion

                #region Прогноз
                url = new Uri("http://www.accuweather.com/forecast.aspx?loc=" + locationcode);

                request = (HttpWebRequest)WebRequest.Create(url);
                request.Proxy = HTCHome.Core.GeneralHelper.Proxy != null ? HTCHome.Core.GeneralHelper.Proxy : WebRequest.GetSystemWebProxy();
                request.Timeout = 30000; //30 секунд
                //SetBrowserHeaders(request);
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(new Cookie("IntLangZ", ilz.ToString(), "/", "www.accuweather.com"));
                request.CookieContainer.Add(new Cookie("IntPreferencesZ", String.Format("Units={0}", degreeType == 0 ? 1 : 0), "/", "www.accuweather.com"));

                result = new StreamReader(((HttpWebResponse)request.GetResponse()).GetResponseStream(), Encoding.UTF8).ReadToEnd();
                if (request != null)
                    request = null;

                result = Helper.HtmlDecode(result);

                DateTime LocToday = DateTime.Now;
                Regex curDateRe = new Regex("<span.+?lblDate.+?>([^<]+)</span>", RegexOptions.IgnoreCase);
                if (curDateRe.IsMatch(result))
                    LocToday = DateTime.Parse(curDateRe.Match(result).Groups[1].Value, ilz.CI);

                String rStr = "<div.+?ForecastIcon.+?>\\s*<img.+?imgIcon.+?([\\d]+)_int.+?>\\s*</div>\\s*" +
                "<div.+?ForecastDescription.+?>\\s*" +
                "<div.+?>\\s*<span.+?lblDate.+?>([^<]+)</span>\\s*</div>\\s*" +
                "<div.+?>\\s*<span.+?lblDesc.+?>([^<]+)</span>\\s*</div>\\s*" +
                "<div.+?>.+?<span.+?lblHigh.+?>([^°]+)[^<]*</span>\\s*</div>\\s*" +
                "<div.+?>.+?</div>\\s*" +
                ".+?<div.+?><a.+?lnkDetails.+?href=\"([^\"]*)\">";
                Regex forecastRe = new Regex(rStr, RegexOptions.IgnoreCase);
                List<ForecastInfo> fiList = new List<ForecastInfo>();
                if (forecastRe.IsMatch(result))
                    foreach (Match m in forecastRe.Matches(result))
                    {
                        Regex dReg = new Regex("[\\d:.\\/\\-]+", RegexOptions.IgnoreCase);
                        ForecastInfo fi = new ForecastInfo();
                        MatchCollection mc = dReg.Matches(m.Groups[2].Value.Trim().Replace(". ", "."));
                        fi.lblDate = DateTime.Parse(mc[mc.Count - 1].Value.Trim(), ilz.CI);
                        fi.lblHigh = Int32.Parse(m.Groups[4].Value);
                        fi.lblDesc = m.Groups[3].Value.Trim();
                        fi.lblDesc = fi.lblDesc[0].ToString().ToUpper() + fi.lblDesc.Substring(1);
                        fi.lnkDetails = m.Groups[5].Value;
                        fi.ForecastIcon = Int32.Parse(m.Groups[1].Value);
                        fiList.Add(fi);
                    }

                int counter = 0;
                for (int i = 0; i < fiList.Count / 2; i++)
                {
                    if (i == 0 && LocToday != fiList[i].lblDate) continue;
                    if (counter < 5)
                    {
                        report.Forecast.Add(new DayForecast() { SkyCode = fiList[i].ForecastIcon, Text = fiList[i].lblDesc + (fiList[i + fiList.Count / 2].lblDesc != fiList[i].lblDesc ? "\r\n" + fiList[i + fiList.Count / 2].lblDesc : String.Empty), Url = fiList[i].lnkDetails, HighTemperature = fiList[i].lblHigh, LowTemperature = fiList[i + fiList.Count / 2].lblHigh });
                        counter++;
                    }
                }
                report.High = report.Forecast[0].HighTemperature;
                report.Low = report.Forecast[0].LowTemperature;
                #endregion
                return report;
            }
            catch (Exception er)
            {
                HTCHome.Core.Logger.Log("Failed to get the forecast!\r\n" + er.ToString());
                return null;
            }
            
        }
        #endregion
    }

    public class ForecastInfo
    {
        public DateTime lblDate;
        public Int32 ForecastIcon;
        public String lblDesc;
        public Int32 lblHigh;
        public String lnkDetails;
    }
}
