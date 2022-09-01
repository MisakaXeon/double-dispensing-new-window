using dispensing.Expends;
using dispensing.ViewModel.Items;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace dispensing.CustomConverters
{
    public class BottomSelectedItemConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            var layerContent = value[0] as LayerContent;
            var item = value[1] as LayerMedicineInfo;
            if (item == null)
            {
                return new LayerMedicineInfo() { AddEventHandle = layerContent.AddItemEvent,
                    ModifyEventHandle = layerContent.ModifyEvent
                };
            }
            else
            {
                var info = item.CopyFromOnlyValue();
                info.AddEventHandle = layerContent.AddItemEvent;
                info.ModifyEventHandle = layerContent.ModifyEvent;
                info.DeleteEventHandle = layerContent.DeleteItemEvent;
                return info;
            }
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
