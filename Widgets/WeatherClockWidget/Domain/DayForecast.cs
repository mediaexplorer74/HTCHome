namespace WeatherClockWidget.Domain
{
    public class DayForecast
    {
        public DayForecast()
        {
            Text = "Clear";
            SkyCode = 1;
        }

        public int HighTemperature { get; set; }

        public int LowTemperature { get; set; }

        public string Text { get; set; }

        public int SkyCode { get; set; }

        public string Url { get; set; }
    }
}
