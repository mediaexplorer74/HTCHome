using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace WeatherClockWidget
{
    public class WallpaperManager
    {
        private readonly List<Wallpaper> _wallpapers;

        public WallpaperManager()
        {
            this._wallpapers = new List<Wallpaper>();
        }

        public enum DateTimeType
        {
            Morning,
            Day,
            Evening,
            Night,
            None
        }

        public enum WeatherType
        {
            Sunny,
            Rainy,
            Cloudy,
            Snowy,
            Stormy,
            Misty,
            Windy
        }

        public void Scan(string dir)
        {
            if (!Directory.Exists(dir)) return;
            foreach (string d in Directory.GetDirectories(dir))
            {
                var files = Directory.GetFiles(d, "*.jpg", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    DateTimeType type1;
                    WeatherType type2 = WeatherType.Sunny;

                    if (!Enum.TryParse(Path.GetFileName(d), true, out type2))
                        return;

                    Wallpaper w;

                    if (Enum.TryParse(Directory.GetParent(file).Name, true, out type1))
                        w = new Wallpaper(file, type1, type2);
                    else
                    {
                        w = new Wallpaper(file, DateTimeType.None, type2);
                    }
                    _wallpapers.Add(w);
                }
            }
        }

        public void ChangeWallpaper(WeatherType weather)
        {
            var wallpapers = from x in _wallpapers where x.Wtype == weather select x;

            if (wallpapers.Count() <= 0) return;
            switch (DateTime.Now.Hour)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    //Night
                    wallpapers = from x in wallpapers where x.Type == DateTimeType.Night select x;
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    //Morning
                    wallpapers = from x in wallpapers where x.Type == DateTimeType.Morning select x;
                    break;
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                    //Day
                    wallpapers = from x in wallpapers where x.Type == DateTimeType.Day select x;
                    break;
                case 18:
                case 19:
                case 20:
                case 21:
                case 22:
                case 23:
                    //Evening
                    wallpapers = from x in wallpapers where x.Type == DateTimeType.Evening select x;
                    break;
            }

            if (wallpapers.Count()<=0)
                wallpapers = from x in _wallpapers where x.Wtype == weather && x.Type == DateTimeType.None select x;

            var r = new Random(Environment.TickCount);
            var index = r.Next(0, wallpapers.Count() -1 );
            HTCHome.Core.WinAPI.SystemParametersInfo(HTCHome.Core.WinAPI.SPI_SETDESKWALLPAPER, 0, wallpapers.ElementAt(index).Path, HTCHome.Core.WinAPI.SPIF_UPDATEINIFILE).ToString();
        }
    }
}
