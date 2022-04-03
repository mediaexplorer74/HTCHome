using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeatherClockWidget
{
    public class Wallpaper
    {
        public Wallpaper(string path, WallpaperManager.DateTimeType type, WallpaperManager.WeatherType wtype)
        {
            this.Type = type;
            Wtype = wtype;
            this.Path = path;
        }

        public WallpaperManager.DateTimeType Type { get; set; }
        public WallpaperManager.WeatherType Wtype { get; set; }
        public string Path { get; private set; }
    }
}
