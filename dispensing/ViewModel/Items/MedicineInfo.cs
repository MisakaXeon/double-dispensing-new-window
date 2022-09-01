using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dispensing.ViewModel.Items
{
    /// <summary>
    /// 药品信息
    /// </summary>
    public class MedicineInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 图片地址
        /// </summary>
        public Uri ImageUri { get; set; }
        /// <summary>
        /// 药品名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 药品数量
        /// </summary>
        public int Count { get; set; }
    }
}
