using dispensing.CustomConverters.Items;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace dispensing.CustomConverters
{
    public class AnyConverter : IValueConverter
    {
        /// <summary>
        /// 任一项转换器
        /// </summary>
        public List<AnyItem> AnyConverts { get; set; } = new List<AnyItem>();
        /// <summary>
        /// 否则
        /// </summary>
        public AnyOtherItem AnyOther { get; set; } = null;


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (var item in AnyConverts)
            {
                if (value is null || item.From is null)
                {
                    if (object.ReferenceEquals(item.From, value))
                    {
                        return item.To;
                    }
                }
                else if (value is null == false && value.Equals(item.From))
                {
                    return item.To;
                }
                else if (item.From is null == false && item.From.Equals(value))
                {
                    return item.To;
                }
            }
            if (AnyOther is null == false)
            {
                return AnyOther.To;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            foreach (var item in AnyConverts)
            {
                if (object.ReferenceEquals(item.To, value))
                {
                    return item.From;
                }
            }
            return null;
        }
    }
}
