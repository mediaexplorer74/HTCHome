using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using E = HTCHome.Core.Environment;
using System.IO;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace CalendarWidget
{
    public class DayConverter : IValueConverter
    {
        public static Dictionary<DateTime, CalendarEvent> dict = new Dictionary<DateTime, CalendarEvent>();

        static DayConverter()
        {

        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var dates = from x in DayConverter.dict.Keys
                        where ((x.Year == ((DateTime)value).Year) && (x.Month == ((DateTime)value).Month) && (x.Day == ((DateTime)value).Day))
                        select x;
            if (dates.Count() > 0)
                return true;
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
