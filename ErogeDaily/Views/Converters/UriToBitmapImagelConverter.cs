using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ErogeDaily.Views.Converters
{
    public class UriToBitmapImagelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var uri = value as Uri;

            if (uri == null)
            {
                return null;
            }

            var bmp = new BitmapImage();
            bmp.BeginInit();
            if (int.TryParse((string)parameter, out int decodePixelWidth))
            {
                bmp.DecodePixelWidth = decodePixelWidth;
            }
            bmp.UriSource = uri;
            bmp.EndInit();
            return bmp;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
