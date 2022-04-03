using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;

namespace HTCHome.Core
{
    public class LocaleManager
    {
        private ResourceDictionary locale;

        public string LocaleCode
        {
            get;
            set;
        }

        public string LocaleBasePath
        {
            get;
            set;
        }

        public LocaleManager(string localeBasePath)
        {
            locale = new ResourceDictionary();
            LocaleBasePath = localeBasePath;
        }

        public void LoadLocale(string localeCode)
        {
            LocaleCode = localeCode;
            if (File.Exists(LocaleBasePath + "\\" + LocaleCode + ".xaml"))
                try
                {
                    locale.Source = new Uri(LocaleBasePath + "\\" + LocaleCode + ".xaml");
                }
                catch (Exception ex)
                {
                    HTCHome.Core.Logger.Log("Can't load locale " + localeCode + ". Exception: " + ex.ToString());
                    locale.Source = new Uri(LocaleBasePath + "\\en-US.xaml");
                }
            else
                locale.Source = new Uri(LocaleBasePath + "\\en-US.xaml");
        }

        public static string GetLocaleName(string path)
        {
            var l = new ResourceDictionary();
            try
            {
                l.Source = new Uri(path);
                if (l["LocaleName"] != null)
                    return l["LocaleName"].ToString();
            }
            catch (Exception ex)
            {
                Logger.Log("Localization " + path + " is broken.\n" + ex.ToString());
            }
            return String.Empty;
        }

        public static string GetLocaleCode(string path)
        {
            var l = new ResourceDictionary();
            try
            {
                l.Source = new Uri(path);
                if (l["LocaleCode"] != null)
                    return l["LocaleCode"].ToString();
            }
            catch { }
            return String.Empty;
        }

        public string GetString(string s)
        {
            if (locale[s] != null)
                return locale[s].ToString();
            var enLocale = new ResourceDictionary {Source = new Uri(LocaleBasePath + "\\en-US.xaml")};
            return (string)enLocale[s];
        }
    }
}
