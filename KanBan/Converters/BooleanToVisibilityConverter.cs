using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace KanBan
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool booleanValue = false;
            if (value is bool)
            {
                booleanValue = (bool)value;
            }
            else if (value is bool?)
            {
                if (value != null)
                {
                    bool? nullableValue = (bool?)value;
                    booleanValue = nullableValue.HasValue ? nullableValue.Value : false;
                }
            }
            return (booleanValue) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (value is Visibility ? (Visibility)value == Visibility.Visible : false);
        }
    }
}
