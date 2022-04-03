using System.Collections.Generic;

using WeatherClockWidget.Domain;

namespace WeatherClockWidget
{
    public interface IWeatherProvider
    {
        Coordinates GetCoordinates(string locationCode);

        List<CityLocation> GetLocation(string s);

        WeatherReport GetWeatherReport(string locale, string locationcode, int degreeType);
    }
}