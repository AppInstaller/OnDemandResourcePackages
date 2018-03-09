using System;
using Windows.UI.Xaml.Data;

namespace CoffeeUniversal.Helpers
{
    public class BoolToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool isPinned = (bool)value;
            return isPinned ? "unpin" : "pin";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
