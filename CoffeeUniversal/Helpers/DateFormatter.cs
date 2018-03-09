using System;
using Windows.UI.Xaml.Data;

namespace CoffeeUniversal.Helpers
{
    public class DateFormatter : IValueConverter
    {
        public object Convert(object value, Type targetType, object formatString, string language)
        {
            if (formatString == null)
            {
                return value;
            }

            return string.Format((string)formatString, value);
        }

        public object ConvertBack(object value, Type targetType, object formatString, string language)
        {
            return value;
        }
    }
}
