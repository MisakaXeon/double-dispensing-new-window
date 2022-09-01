using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dispensing.ViewModel.Items
{
    public class MedicineOptionItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 加药时间 / 取药时间
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        public string Option { get; set; }
        /// <summary>
        /// 药品名称
        /// </summary>
        public string MedicineName { get; set; }
        /// <summary>
        /// 期效
        /// </summary>
        public DateTime PeriodDate { get; set; }
        /// <summary>
        /// 批号
        /// </summary>
        public string Serial { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 年龄
        /// </summary>
        public string Age { get; set; }
        /// <summary>
        /// 处方号
        /// </summary>
        public string PrescriptionNumber { get; set; }
    }
}
