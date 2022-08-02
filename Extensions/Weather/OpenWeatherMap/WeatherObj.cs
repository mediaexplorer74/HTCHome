using System;
using System.Collections.Generic;
using System.Text;

namespace OpenWeatherMap.Models
{
    public class ForecastInfo
    {
        public string cod { get; set; }
        public int message { get; set; }
        public int cnt { get; set; }
        public List[] list { get; set; }
        public City city { get; set; }

    }

   

    public class OneCallInfo
    {
        public float lon { get; set; }
        public float lat { get; set; }
               
        public string timezone { get; set; }

        public float timezone_offset { get; set; }

        public List[] current { get; set; }
        public List[] minutely { get; set; }
        public ForecastList[] daily { get; set; }
        
        public List[] alerts { get; set; }

    }

    public class City
    {
        public int id { get; set; }
        public string name { get; set; }
        public Coord coord { get; set; }
        public string country { get; set; }
        public int population { get; set; }
        public int timezone { get; set; }
        public int sunrise { get; set; }
        public int sunset { get; set; }
    }

    public class List
    {
        public int dt { get; set; }

        public Main main { get; set; }
        public Weather[] weather { get; set; }
        public Clouds clouds { get; set; }
        public Wind wind { get; set; }
        public Sys sys { get; set; }
        public string dt_txt { get; set; }
        public Rain rain { get; set; }
    }


    public class ForecastList
    {
        public int dt { get; set; }

        public int sunrise { get; set; }
        public int sunset { get; set; }

        public Temp temp { get; set; }
        public Weather[] weather { get; set; }
        public int clouds { get; set; }
        
        //public Wind wind { get; set; }
        //public Sys sys { get; set; }
        //public string dt_txt { get; set; }
        //public Rain rain { get; set; }

        public float snow { get; set; }
        public float uvi { get; set; }
    }

    public class Main
    {
        public float temp { get; set; }
        public float temp_min { get; set; }
        public float temp_max { get; set; }
        public int pressure { get; set; }
        public int sea_level { get; set; }
        public int grnd_level { get; set; }
        public int humidity { get; set; }
        public float temp_kf { get; set; }
    }


    public class Temp
    {
        public float day { get; set; }
        public float min { get; set; }
        public float max { get; set; }
        public float night { get; set; }

        public float eve { get; set; }
        public float morn { get; set; }
    }


    public class Rain
    {
        public float _3h { get; set; }
    }

    public class WeatherInfo
    {
        public int dt { get; set; }
        public Coord coord { get; set; }
        public Weather[] weather { get; set; }
        public string _base { get; set; }
        public Main main { get; set; }
        public int visibility { get; set; }
        public Wind wind { get; set; }
        
        public Clouds clouds { get; set; }
        public Sys sys { get; set; }
        public int timezone { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int cod { get; set; }
    }


    public class ForecastWeatherInfo
    {
        public int dt { get; set; }
        public Coord coord { get; set; }
        public Weather[] weather { get; set; }
        public string _base { get; set; }
        public Main main { get; set; }
        public int visibility { get; set; }
        public Wind wind { get; set; }

        public int clouds { get; set; }
        public Sys sys { get; set; }
        public int timezone { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int cod { get; set; }
    }
    public class Coord
    {
        public float lon { get; set; }
        public float lat { get; set; }
    }


    public class Wind
    {
        public float speed { get; set; }
        public int deg { get; set; }
    }

    public class Clouds
    {
        public int all { get; set; }
    }

    public class Sys
    {
        public int type { get; set; }
        public int id { get; set; }
        public string country { get; set; }
        public int sunrise { get; set; }
        public int sunset { get; set; }
    }

    public class Weather
    {
        public int id { get; set; }
        public string main { get; set; }
        public string description { get; set; }
        public string icon { get; set; }

    }

}
