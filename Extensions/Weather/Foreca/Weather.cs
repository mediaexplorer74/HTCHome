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

namespace Foreca
{
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
                bool isEng = new CultureInfo(Widget.LocaleManager.LocaleCode).TwoLetterISOLanguageName.ToLower() == "en";
                Uri url = new Uri(GetLanguageUrl(Widget.LocaleManager.LocaleCode));

                HttpWebRequest request = null;
                HttpWebResponse response = null;
                byte[] bytes = new byte[0];
                Stream os = null;

                #region language cookie
                if (!isEng)
                    try
                    {
                        request = (HttpWebRequest)WebRequest.Create(url);
                        request.Proxy = HTCHome.Core.GeneralHelper.Proxy != null ? HTCHome.Core.GeneralHelper.Proxy : WebRequest.GetSystemWebProxy();
                        request.Timeout = 5000;
                        SetBrowserHeaders(request);
                        //request.UserAgent = "Mozilla/4.0 (Compatible; Windows NT 5.1; MSIE 8.0) (compatible; MSIE 8.0; Windows NT 5.1;)";
                        request.CookieContainer = new CookieContainer();
                        request.AllowAutoRedirect = true;
                        response = (HttpWebResponse)request.GetResponse();
                    }
                    catch { isEng = true; }
                #endregion
                Uri hostUrl = new Uri(url.Scheme + "://" + url.Host + ":" + url.Port.ToString());
                #region Search locations
                string result = String.Empty;
                string resultEn = String.Empty;
                #region languge result
                if (!isEng && response != null)
                {
                    request = (HttpWebRequest)WebRequest.Create(hostUrl.ToString() + "complete.php");
                    request.Proxy = HTCHome.Core.GeneralHelper.Proxy != null ? HTCHome.Core.GeneralHelper.Proxy : WebRequest.GetSystemWebProxy();
                    request.Timeout = 10000;
                    SetBrowserHeaders(request);
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.CookieContainer = new CookieContainer();
                    foreach (Cookie c in response.Cookies)
                        request.CookieContainer.Add(c);
                    bytes = Encoding.ASCII.GetBytes("q=" + Helper.UrlEncode(Query));
                    os = null;
                    try
                    {
                        request.ContentLength = bytes.Length;
                        os = request.GetRequestStream();
                        os.Write(bytes, 0, bytes.Length);
                    }
                    catch { }
                    finally { if (os != null) os.Close(); }
                    result = new StreamReader(((HttpWebResponse)request.GetResponse()).GetResponseStream(), Encoding.UTF8).ReadToEnd();
                }
                #endregion
                #region english result
                request = (HttpWebRequest)WebRequest.Create(hostUrl.ToString() + "complete.php");
                request.Proxy = HTCHome.Core.GeneralHelper.Proxy != null ? HTCHome.Core.GeneralHelper.Proxy : WebRequest.GetSystemWebProxy();
                request.Timeout = 10000;
                SetBrowserHeaders(request);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.CookieContainer = new CookieContainer();
                bytes = Encoding.ASCII.GetBytes("q=" + Helper.UrlEncode(Query));
                os = null;
                try
                {
                    request.ContentLength = bytes.Length;
                    os = request.GetRequestStream();
                    os.Write(bytes, 0, bytes.Length);
                }
                catch { }
                finally { if (os != null) os.Close(); }
                resultEn = new StreamReader(((HttpWebResponse)request.GetResponse()).GetResponseStream(), Encoding.UTF8).ReadToEnd();
                #endregion
                #region Clear streams
                if (request != null) request = null;
                if (os != null) os = null;
                #endregion
                if (isEng) result = resultEn;
                #region Get list data
                if (resultEn.IndexOf("<li style") > 0)
                    resultEn = resultEn.Substring(0, resultEn.IndexOf("<li style"));
                Regex liRe = new Regex("<li id=\"(\\d+)\">(.+?)</li>", RegexOptions.IgnoreCase);
                if (liRe.IsMatch(resultEn))
                    for (int mi = 0; mi < liRe.Matches(resultEn).Count; mi++)
                    {
                        Match mv = liRe.Matches(result)[mi];
                        CityLocationLsit.Add(new CityLocation()
                        {
                            Code = mv.Groups[1].Value,
                            City = new Regex("</?[^>]+>").Replace(mv.Groups[2].Value, String.Empty).Trim()
                        });
                    }
                #endregion
                #endregion
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
                bool isMetric = degreeType == 0;
                Uri url = new Uri(GetLanguageUrl(Widget.LocaleManager.LocaleCode));
                Uri hostUrl = new Uri(url.ToString() + (!String.IsNullOrEmpty(url.Query) ? "&" : "?") + "id=" + locationcode + "&quick_units=" + (isMetric ? "metric" : "us"));
                #region Текущая температура
                String CurentDataUrl = hostUrl.ToString();
                String CurentData = Helper.HtmlDecode(HTCHome.Core.GeneralHelper.GetXml(CurentDataUrl));
                if (string.IsNullOrEmpty(CurentData)) return null;

                #region Название населенного пункта
                Regex CityNameRe = new Regex("<h1 class=\"entry-title\">([^<]+)</h1>", RegexOptions.IgnoreCase);
                if (CityNameRe.IsMatch(CurentData))
                    report.Location = CityNameRe.Match(CurentData).Groups[1].Value.Trim();
                else
                {
                    /*арабский*/
                    CityNameRe = new Regex("<div class=\"to-left\">\\s*<h1[^>]*>([^<]+)</h1>\\s*</div>", RegexOptions.IgnoreCase);
                    if (CityNameRe.IsMatch(CurentData))
                        report.Location = CityNameRe.Match(CurentData).Groups[1].Value.Trim();
                    #region new version 2011-03-03
                    else
                    {
                        CityNameRe = new Regex("<div class=\"to-left\">\\s*<h1[^>]*>([^<]+)</h1>\\s*</div>", RegexOptions.IgnoreCase);
                        if (CityNameRe.IsMatch(CurentData))
                            report.Location = CityNameRe.Match(CurentData).Groups[1].Value.Trim();
                    }
                    #endregion
                }
                #endregion
                Regex CurentRe = new Regex("<div class=\"left\">\\s*<img class=\"symb\" src=\"/img/symb-70x70/([^\\.]+).png\" alt=\"[^\\\"]*\" title=\"([^\\\"]*)\" width=\"70\" />\\s*<span[^>]*>\\s*<strong>([^<]*)</strong>", RegexOptions.IgnoreCase);
                if (CurentRe.IsMatch(CurentData))
                {
                    report.NowSkyCode = GetWeatherPic(CurentRe.Match(CurentData).Groups[1].Value.Trim());
                    report.NowSky = CurentRe.Match(CurentData).Groups[2].Value.Trim();
                    report.NowTemp = Convert.ToInt32(CurentRe.Match(CurentData).Groups[3].Value.Trim());
                    report.Url = CurentDataUrl;
                }
                #region new version 2011-03-03
                else
                {
                    CurentRe = new Regex("<div class=\"left\">\\s*<div class=\"symbol_70x70\\w{1}\\s*symbol_([^_]+)[^>]*></div>\\s*<span[^>]*>\\s*<strong>([^<]*)</strong>", RegexOptions.IgnoreCase);
                    Regex CurentDesc = new Regex("<div class=\"right txt-tight\">([^<]*)", RegexOptions.IgnoreCase);
                    if (CurentRe.IsMatch(CurentData) && CurentDesc.IsMatch(CurentData))
                    {
                        report.NowSkyCode = GetWeatherPic(CurentRe.Match(CurentData).Groups[1].Value.Trim());
                        report.NowTemp = Convert.ToInt32(CurentRe.Match(CurentData).Groups[2].Value.Trim());
                        report.NowSky = CurentDesc.Match(CurentData).Groups[1].Value.Trim();
                        report.Url = CurentDataUrl;
                    }
                    else return null;
                }
                #endregion
                #endregion
                #region Прогноз
                String ForecaDataUrl = hostUrl.ToString() + "&tenday";
                String ForecaData = Helper.HtmlDecode(HTCHome.Core.GeneralHelper.GetXml(ForecaDataUrl));
                Regex ForecastRe = new Regex("<img src=\"/img/symb-50x50/([^\\.]+).png\"\\s*alt=\"([^\\\"]*)\"[^>]*><br />\\s*[^:]+:\\s*<strong>([^°]+)°</strong><br />\\s*[^:]+:\\s*<strong>([^°]+)°</strong><br />\\s*", RegexOptions.IgnoreCase);
                if (ForecastRe.IsMatch(ForecaData))
                {
                    DateTime date = DateTime.Today;
                    Int32 num = 0;
                    foreach (Match m in ForecastRe.Matches(ForecaData))
                    {
                        if (num == 0)
                        {
                            report.High = Convert.ToInt32(m.Groups[3].Value.Trim());
                            report.Low = Convert.ToInt32(m.Groups[4].Value.Trim());
                        }
                        report.Forecast.Add(new DayForecast()
                        {
                            SkyCode = GetWeatherPic(m.Groups[1].Value.Trim()),
                            Text = m.Groups[2].Value.Trim(),
                            HighTemperature = Convert.ToInt32(m.Groups[3].Value.Trim()),
                            LowTemperature = Convert.ToInt32(m.Groups[4].Value.Trim()),
                            Url = hostUrl.ToString() + "&details=" + DateTime.Today.AddDays(num++).ToString("yyyyMMdd")
                        });
                        if (num == 5) break;
                    }
                }
                #region new version 2011-03-03
                else
                {
                    ForecastRe = new Regex("<div class=\"symbol_50x50d symbol_([^_]+)_50x50\"\\s*alt=\"([^\\\"]*)\"[^>]*></div>\\s*<br[^>]*>\\s*[^:]+:\\s*<strong>([^°]+)°</strong><br[^>]*>\\s*[^:]+:\\s*<strong>([^°]+)°</strong><br[^>]*>", RegexOptions.IgnoreCase);
                    if (ForecastRe.IsMatch(ForecaData))
                    {
                        DateTime date = DateTime.Today;
                        Int32 num = 0;
                        foreach (Match m in ForecastRe.Matches(ForecaData))
                        {
                            if (num == 0)
                            {
                                report.High = Convert.ToInt32(m.Groups[3].Value.Trim());
                                report.Low = Convert.ToInt32(m.Groups[4].Value.Trim());
                            }
                            report.Forecast.Add(new DayForecast()
                            {
                                SkyCode = GetWeatherPic(m.Groups[1].Value.Trim()),
                                Text = m.Groups[2].Value.Trim(),
                                HighTemperature = Convert.ToInt32(m.Groups[3].Value.Trim()),
                                LowTemperature = Convert.ToInt32(m.Groups[4].Value.Trim()),
                                Url = hostUrl.ToString() + "?details=" + DateTime.Today.AddDays(num++).ToString("yyyyMMdd") + "&" + (isMetric ? "quick_units=metric" : "quick_units=us") + (url.Query != String.Empty ? "&" + url.Query.Substring(1) : String.Empty)
                            });
                            if (num == 5) break;
                        }
                    }
                }
                #endregion
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
        private int GetWeatherPic(string icon)
        {
            try
            {
                //Облачность 0-Ясно, 1-Небольшая облачность, 2-Переменная облачность, 3-Облачно с прояснениями, 4-Облачно
                //Сила осадков 0-нет, 1-слабый, 2-тип, 3-сильный, 4-гроза
                //Тип осадков 0-дождь,1-осадки 2-снег
                bool isDay = icon.ToLower()[0] == 'd';
                switch (Convert.ToInt32(icon.Substring(1)))
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
            catch { return 1; }
        }
        #endregion

        #region GetLanguageUrl
        String GetLanguageUrl(String LCode)
        {
            try
            {
                switch (new CultureInfo(LCode).TwoLetterISOLanguageName.ToLower())
                {
                    case "ru": return "http://foreca.com/?lang=ru";
                    case "ar": return "http://foreca.com/?lang=ar";
                    case "bn": return "http://foreca.in/?lang=bn";
                    case "bg": return "http://foreca.in/?lang=bg";
                    case "cs": return "http://foreca.cz/?lang=cs";
                    case "da": return "http://foreca.dk/?lang=da";
                    case "de": return "http://foreca.de/?lang=de";
                    case "et": return "http://foreca.com/?lang=et";
                    case "el": return "http://foreca.gr/?lang=el";
                    case "es": return "http://foreca.es/?lang=es";
                    case "fr": return "http://foreca.com/?lang=fr";
                    case "gu": return "http://foreca.in/?lang=gu";
                    case "hi": return "http://foreca.in/?lang=hi";
                    case "hr": return "http://foreca.com/?lang=hr";
                    case "kn": return "http://foreca.in/?lang=kn";
                    case "lt": return "http://foreca.com/?lang=lt";
                    case "hu": return "http://foreca.hu/?lang=hu";
                    case "ml": return "http://foreca.in/?lang=ml";
                    case "is": return "http://foreca.com/?lang=is";
                    case "it": return "http://foreca.it/?lang=it";
                    case "nl": return "http://foreca.nl/?lang=nl";
                    case "ja": return "http://foreca.com/?lang=ja";
                    case "pl": return "http://foreca.pl/?lang=pl";
                    case "pa": return "http://foreca.in/?lang=pa";
                    case "ro": return "http://foreca.ro/?lang=ro";
                    case "sr": return "http://foreca.com/?lang=sr";
                    case "fi": return "http://foreca.fi";
                    case "sv": return "http://foreca.se/?lang=sv";
                    case "ta": return "http://foreca.in/?lang=ta";
                    case "te": return "http://foreca.in/?lang=te";
                    case "tr": return "http://foreca.com/?lang=tr";
                    case "th": return "http://foreca.com/?lang=th";
                    case "uk": return "http://foreca.com/?lang=uk";
                    default: return "http://foreca.com/?lang=en";
                }
            }
            catch { return "http://foreca.com/?lang=en"; }
        }
        #endregion

        #region SetBrowserHeaders
        private void SetBrowserHeaders(HttpWebRequest request)
        {
            #region Mozilla Firefox 3.6.12
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*";
            request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; ru; rv:1.9.2.12) Gecko/20101026 Firefox/3.6.12";
            #endregion
        }
        #endregion
    }
}
