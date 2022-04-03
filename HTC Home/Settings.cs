using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Drawing;

namespace HTCHome
{
    public class Settings
    {
        public List<string> LoadedWidgets;
        public string Locale;
        public bool EnableGlass = true;
        public bool Autostart = false;
        public bool EnableUpdates = true;
        public bool useProxy = false;
        public string proxyAddress = "http://";
        public int proxyPort;
        public string proxyUsername = "Username";
        public string proxyPassword = "Password";

        public static Settings Read(string path)
        {
            var result = new Settings();
            if (File.Exists(path))
            {
                var f = new FileInfo(path);
                if (f.Length > 0)
                {
                    using (TextReader textReader = new StreamReader(path))
                    {
                        var deserializer = new XmlSerializer(typeof(Settings));
                        result = (Settings)deserializer.Deserialize(textReader);
                    }
                }
                else
                {
                    App.Log("Settings file is corrupted.");
                }
            }

            if (result.LoadedWidgets == null || result.LoadedWidgets.Count == 0)
            {
                result.LoadedWidgets = new List<string>();
                result.LoadedWidgets.Add("Weather/Clock widget");
            }
            return result;
        }

        public void Write(string path)
        {
            using (TextWriter textWriter = new StreamWriter(path))
            {
                var serializer = new XmlSerializer(typeof(Settings));
                serializer.Serialize(textWriter, this);
            }
        }
    }
}
