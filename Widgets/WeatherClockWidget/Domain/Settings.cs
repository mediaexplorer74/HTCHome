using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace WeatherClockWidget.Domain
{
    //note i'm sure this class is useless
    //maybe, but i will better spend time to add something new than to remove this class ;)
    public class Settings
    {
        public bool Debug;

        public int DegreeType;

        public bool EnableSounds = true;

        public bool EnableWallpaperChanging = false;

        public bool EnableWeather = true;

        public bool EnableWeatherAnimation = true;

        public double Interval = 30;

        public List<CityLocation> LastCities;

        public string Locationcode = "";

        public double Opacity = 1;

        public bool Pinned = false;

        public double RefreshWeatherAnimInterval;

        public double ScaleFactor = 1;

        public bool ShowIconOnTaskbar = true;

        public bool ShowForecast = true;

        public string Skin = "Modern Sense";

        public int Sunrise = 4;

        public int Sunset = 22;

        public int TimeMode;

        public double Top = -1;

        public double Left = -1;

        public bool Topmost;

        public string WeatherProvider = "MSN";

        public string WallpapersFolder = HTCHome.Core.Environment.Path + "\\WeatherClock\\Wallpapers";

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
                    HTCHome.Core.Logger.Log("Settings file is corrupted.");
                }
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
