using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using WeatherClockWidget.Domain;

namespace WeatherClockWidget
{
    public class WeatherProvider
    {
        private readonly string _path;

        private Assembly _assembly;

        private IWeatherProvider _provider;

        public WeatherProvider(string path)
        {
            _path = path;
            Name = path.Substring(path.LastIndexOf(@"\") + 1, path.Length - path.LastIndexOf(@"\") - 5);
        }

        public string Name { get; private set; }
        public bool IsLoaded { get; set; }

        public static int ToCelsius(int degrees)
        {
            return (int) Math.Round((decimal) ((degrees - 32) * 5 / 9), 1);
        }

        public static int ToFahrenheit(int degrees)
        {
            return (int) Math.Round((decimal) (degrees * 9 / 5 + 32), 1);
        }

        public Coordinates GetCoordinates(string locationCode)
        {
            return _provider.GetCoordinates(locationCode);
        }

        public List<CityLocation> GetLocation(string s)
        {
            try
            {
                return _provider.GetLocation(s);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Can't find location.\n" + ex.Message);
                HTCHome.Core.Logger.Log("Can't find location.\n" + ex.ToString());
                return null;
            }
        }

        public WeatherReport GetWeatherReport
        (
            string locale, 
            string locationcode, 
            int degreeType
        )
        {
            try
            {
                var weatherReport = _provider.GetWeatherReport
                    (locale, 
                    locationcode, 
                    degreeType);
                return weatherReport;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("WeatherWidgetClock - GetWeatherReport Exception: " 
                    + ex.Message);

                HTCHome.Core.Logger.Log("Can't get weather report.\n"
                    + ex.ToString());
            }
            return null;
        }

        /// <exception cref = "System.TypeLoadException"><c>TypeLoadException</c>.</exception>
        public void Load()
        {
            _assembly = Assembly.LoadFrom(_path);
            Type providerType =
                _assembly.GetTypes().FirstOrDefault(type => typeof (IWeatherProvider).IsAssignableFrom(type));
            if (providerType == null)
            {
                IsLoaded = false;
                throw new TypeLoadException(String.Format("Failed to find IWeatherProvider in {0}", _path));
            }

            _provider = Activator.CreateInstance(providerType) as IWeatherProvider;
            IsLoaded = true;
        }
    }
}