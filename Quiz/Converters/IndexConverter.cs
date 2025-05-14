using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace Quiz.Converters
{
    public class IndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ListViewItem item = value as ListViewItem;
            if (item == null)
                return 0;

            ListView listView = ItemsControl.ItemsControlFromItemContainer(item) as ListView;
            if (listView == null)
                return 0;

            return listView.ItemContainerGenerator.IndexFromContainer(item);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 